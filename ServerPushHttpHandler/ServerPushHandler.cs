/* 服务器推送处理程序基础类：ServerPushHandler */
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
#region 命名空间
using WebChatSDK;
using ChatData.ChatModel;
#endregion

namespace ServerPushHttpHandler
{
    public class ServerPushHandler
    {
        #region Field
        HttpContext m_Context;
        ServerPushResult _IAsyncResult;
        static Dictionary<string, ServerPushResult> dict = new Dictionary<string, ServerPushResult>();
        WebChat sdk = new WebChat();
        #endregion

        #region 构造函数
      /// <summary>
        /// 使用请求上下文和异步操作的状态信息来构造异步操作处理对象ServerPushHandler
      /// </summary>
      /// <param name="context">请求上下文</param>
      /// <param name="_IAsyncResult">推送结果</param>
        public ServerPushHandler(HttpContext context, ServerPushResult _IAsyncResult)
        {
            this.m_Context = context;
            this._IAsyncResult = _IAsyncResult;
        }
        #endregion

        #region 执行操作
        /// <summary>
        /// 根据Action判断执行方法
        /// </summary>
        /// <returns></returns>
        public ServerPushResult ExecAction()
        {
            sdk.User = new UserInfo()
            {
                UserId = m_Context.Request["UserId"],
                PassWord = m_Context.Request["PassWord"]
            };      // 实例化并对属性赋值
            switch (m_Context.Request["Action"])
            {
                case "Register":
                    Register();
                    break;
                case "Online":
                    Online();
                    break;
                case "Offline":
                    Offline();
                    break;
                case "Keepline":
                    Keepline();
                    break;
                case "GetFriends":
                    GetFriends();
                    break;
                case "SendMsg":
                    SendMsg();
                    break;
                default:
                    break;
            }
            return _IAsyncResult;
        }
        #endregion

        #region 注册用户
        private void Register()
        {
            sdk.User.UserName = m_Context.Request["UserName"];
            sdk.User.Sex = m_Context.Request["Sex"];
            sdk.User.Age = Convert.ToInt32(m_Context.Request["Age"]);
            sdk.User.Email = m_Context.Request["Email"];
            _IAsyncResult.Result = sdk.Register_User();
            if (!dict.ContainsKey(sdk.User.UserId))
            {
                dict.Add(sdk.User.UserId, _IAsyncResult);
            }
            _IAsyncResult.Send();
        }
        #endregion

        #region 用户上线
        private void Online()
        {
            _IAsyncResult.Result = sdk.Verify_User();
            if (!dict.ContainsKey(sdk.User.UserId))
            {
                dict.Add(sdk.User.UserId, _IAsyncResult);
            }
            _IAsyncResult.Send();
        }
        #endregion

        #region 用户下线
        private void Offline()
        {
            _IAsyncResult.Result = sdk.Downline_User();
            if (dict.ContainsKey(sdk.User.UserId))
            {
                dict.Remove(sdk.User.UserId);
            }
            _IAsyncResult.Send();
        }
        #endregion

        #region 保持长连接,Comet 技术的关键，通过Ajax（long-polling）保持长连接
        private void Keepline()
        {
            // 完全占用Asp.net 工作线程，阻塞服务器返回，直到其他Action要返回数据，没有超时的可能，下一步就是要利用自定义线程解决 占用asp.net工资线程的问题。
            if (!dict.ContainsKey(sdk.User.UserId))
                dict.Add(sdk.User.UserId, _IAsyncResult);
            else            
                dict[sdk.User.UserId] = _IAsyncResult;
        }
        #endregion

        #region 获取好友
        private void GetFriends()
        {
            _IAsyncResult.Result = sdk.Get_Friends();
            _IAsyncResult.Send();
        }
        #endregion

        #region 添加好友
        private void AddFriend()
        {
            UserInfo friend = new UserInfo()
            {
                UserId = m_Context.Request["FriendId"]
            };
            _IAsyncResult.Result = sdk.Add_Friend(friend);
            _IAsyncResult.Send();
        }
        #endregion

        #region 发送消息
        private void SendMsg()
        {
            MessageInfo message = new MessageInfo()
            {
                SendUserId = m_Context.Request["UserId"],
                ReciveUserId = m_Context.Request["ReciveUserId"],
                Content = m_Context.Request["Content"]
            };
            string result = sdk.Send_Msg(message);
            //  下面部分是Comet推送给对方页面显示的，这里最好做成发布事件
            if (dict.ContainsKey(message.ReciveUserId))
            {
                dict[message.ReciveUserId].Result = result;
                dict[message.ReciveUserId].Send();
            }
            //  下面是Ajax回发给自己页面显示的
            _IAsyncResult.Result = result;
            _IAsyncResult.Send();
        }
        #endregion

        #region 全站广播
        public void SendMsg(string Action, string strContent)
        {
            MessageInfo message = new MessageInfo()
            {
                SendUserId = m_Context.Request["UserId"],
                ReciveUserId = m_Context.Request["ReciveUserId"],
                Content = m_Context.Request["Content"]
            };
            foreach (ServerPushResult IAsyncResult in dict.Values)
            {
                //IAsyncResult.ResultXml = strContent;
                //IAsyncResult.Send(null);
            }
        }
        #endregion
    }
}

/*
 基于Ajax的长轮询方式实现的Comet：
 Ajax 的出现使得Javascript可以调用XMLHttpRequest 对象发出HTTP请求，Javascript响应处理函数根据服务器返回的信息对HTML页面的显示进行更新，
  使得Ajax实现“服务器推”与传统的Ajax应用不同之处在于：
 1. 服务器会阻塞请求直到有数据传递或者超时才会返回，经过我另外测试，服务器阻塞超时的时间还是蛮长。
 2. 客户端Javascript响应处理函数会在处理完服务器返回的信息后，再次发出请求，形成长连接。
 3. 当客户端处理接收的数据，重新建立连接时，服务器可能有新的数据到达； 这些信息会被服务器保存直到客户端重新建立
 连接，客户端会一次性把当前服务器端所有信息取回。
    第三点说的略微有点模糊，要理解第三点，除非 AsyncCallback cb 有阻塞机制，不对，不是AsyncCallback有阻塞机制。
 服务器有数据到达，cb应该还是执行了，数据已经被写入Response响应流里，但是因为没有连接，响应流没有管道到达客户端，
 之后等待客户端连上之后，要么因为没有数据，超时将响应流中数据送回客户端，要么因为又有数据来了，又写入响应流，
 将两个响应结果一起送到了客户端。
 所以这里有个难点，客户端在没有连接的情况下，服务器的Response虽然有响应流，但是由于没有连接，此处的响应流相当于阻塞，
 等待时机响应流又可以发送了。
  
 
 
 
 
 */
