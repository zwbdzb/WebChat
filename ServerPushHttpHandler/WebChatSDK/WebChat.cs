using System;
using System.Collections.Generic;
using System.Text;
#region 命名空间
using System.Net;
using ChatData;
using ChatData.PHPLibrary;
using ChatData.ChatModel;
#endregion

namespace WebChatSDK
{
    public class WebChat
    {
        #region 常用参数
        /// <summary>
        /// 当前用户信息
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// 回调地址：CallBackUrl  ？？？ 没怎么使用
        /// </summary>
        /// <returns></returns>
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 响应结果：ResponseResult
        /// </summary>
        public ResponseResult ResponseResult { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public WebChat() { }
        #endregion

        #region 注册用户
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <returns></returns>
        public string Register_User()
        {
            //除用户验证参数外其他参数
            PHPArray array = new PHPArray();
            array.Add("UserName", User.UserName);
            array.Add("Sex", User.Sex);
            array.Add("Age", User.Age);
            array.Add("Email", User.Email);
            array = GetParam(array);
            return new UserService().Verify_User(array);
        }
        #endregion

        #region 用户上线
        /// <summary>
        /// 用户上线
        /// </summary>
        /// <returns></returns>
        public string Verify_User()
        {
            PHPArray array = GetParam();
            return  new UserService().Verify_User(array);
        } 
        #endregion

        #region 用户下线
        /// <summary>
        /// 用户下线
        /// </summary>
        /// <returns></returns>
        public string Downline_User()
        {
            PHPArray array = GetParam();
            return new UserService().Downline_User(array);
        }
        #endregion

        #region 获取好友
        /// <summary>
        /// 获取好友
        /// </summary>
        /// <returns></returns>
        public string Get_Friends()
        {
            PHPArray array = GetParam();
            return new UserService().Get_Friends(array);
        }
        #endregion

        #region 添加好友
        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="friend">好友信息</param>
        /// <returns></returns>
        public string Add_Friend(UserInfo friend)
        {
            PHPArray array = new PHPArray();
            array.Add("FriendId", friend.UserId);
            array = GetParam(array);
            return new UserService().Add_Friend(array);
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="friend">发送消息</param>
        /// <returns></returns>
        public string Send_Msg(MessageInfo message)
        {
            PHPArray array = new PHPArray();
            array.Add("ReciveUserId", message.ReciveUserId);
            array.Add("Content", message.Content);
            array =GetParam(array);
            return new MessageService().Send_Msg(array);
        }
        #endregion

        #region 私有方法 
        // 基本参数
        private PHPArray GetParam()
        {
            OAuth o = new OAuth(this);
            return o.GetParameList();
        }
        /// <summary>
        /// 组装参数
        /// </summary>
        /// <param name="array">其他参数</param>
        /// <returns></returns>
        private PHPArray GetParam(PHPArray array)
        {
            OAuth o = new OAuth(this);
            o.Array = array;
            return o.GetParameList();
        }
        #endregion
    }
}
