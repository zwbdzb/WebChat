using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
#region 命名空间
using ChatData.ChatBLL;
using ChatData.ChatModel;
using ChatData.PHPLibrary;
#endregion
namespace ChatData
{
    public class MessageService
    {

        #region 参数说明
        /*(用户验证参数)
         *  UserId--用户唯一标识
         *  PassWord--用户密码验证
         */
        #endregion

        #region 公共对象
        /// <summary>
        /// 返回处理结果类实例
        /// </summary>
        ResponseResult responseResult = new ResponseResult();

        /// <summary>
        /// 消息处理--业务逻辑类
        /// </summary>
        readonly MessageBLL messageBLL = new MessageBLL();
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        public string Send_Msg(PHPArray phpArray)
        {
            string strUserID = phpArray.Get("UserId").Value.ToString();
            string strPwd = phpArray.Get("PassWord").Value.ToString();
            string strReciveUserId = phpArray.Get("ReciveUserId").Value.ToString();
            string strContent = phpArray.Get("Content").Value.ToString();

            UserInfo user = IPublic.VerifyUser(strUserID,strPwd);
            if (user == null)
            {
                responseResult.ResponseCode = ResultCode.Failure;
                return null;
            }
            MessageInfo message = new MessageInfo()         // 公司项目只要发送接口，要在发送时获取双方详细信息
            {
                SendUserId = strUserID,
                ReciveUserId = strReciveUserId,
                Content = strContent,
                SendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            if (!messageBLL.AddMessage(message))
            {
                responseResult.ResponseDetails = "消息发送失败！";
                responseResult.ResponseCode = ResultCode.Failure;
            }
            else
            {
                responseResult.ResponseData = message;
                responseResult.ResponseDetails = "消息发送成功！";
                responseResult.ResponseCode =ResultCode.Ok ;
            }
            return responseResult.ResultString();
        }
        #endregion
    }
}
