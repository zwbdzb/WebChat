using System;
using System.Collections.Generic;
using System.Web;
#region 命名空间
using ChatData.ChatBLL;
using ChatData.ChatModel;
#endregion

namespace ChatData
{

    /// <summary>
    /// 枚举定义：处理结果状态码
    /// </summary>
    public enum ResultCode
    {
        Ok = 1,
        Failure=0,
    }

    //通用用户验证类
    public class IPublic
    {
        #region 公共对象
        /// <summary>
        /// 用户处理--业务逻辑类
        /// </summary>
        readonly static UserBLL userBLL = new UserBLL();
        #endregion

        #region 验证用户方法
        /// <summary>
        /// 通用用户验证方法
        //// 参数：
        ///     (用户验证参数)
        /// </summary>
        /// <returns>返回用户信息</returns>
        public static UserInfo VerifyUser(string strUserID, string  strPwd)
        {
            UserInfo user = new UserInfo()
            {
                UserId = strUserID,
                PassWord = strPwd
            };
            user = userBLL.VerifyUser(user);
            if (user != null) return user;
            ResponseResult responseResult = new ResponseResult()
            {
                ResponseDetails = "用户验证失败！",
                ResponseCode = ResultCode.Failure
            };
            return null;
        }
        #endregion
    }
}