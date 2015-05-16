using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
#region 命名空间
using ChatData.ChatBLL;
using ChatData.ChatModel;
using ChatData.PHPLibrary;
#endregion
namespace ChatData
{
    public  class UserService
    {
        #region 参数说明
        /*(用户验证参数)
         *  UserId--用户唯一标识
         *  PassWord--用户密码验证
         */
        #endregion

        #region 公共对象
        /// <summary>
        /// 返回处理结果
        /// </summary>
        ResponseResult responseResult = new ResponseResult();

        /// <summary>
        /// 用户处理--业务逻辑类
        /// </summary>
        readonly UserBLL userBLL = new UserBLL();

        /// <summary>
        /// 用户关系处理--业务逻辑类
        /// </summary>
        readonly UserRelationBLL userRelationBLL = new UserRelationBLL();

        #endregion

        #region 注册用户
        /// <summary>
        /// 注册用户信息
        /// </summary>
        public void Register_User(PHPArray phpArray)
        {
            UserInfo user = new UserInfo()
            {
                UserId =phpArray.Get("UserId").ToString(),
                UserName = phpArray.Get("UserName").ToString(),
                PassWord = phpArray.Get("PassWord").ToString(),
                Sex = phpArray.Get("Sex").ToString(),
                Age = Convert.ToInt32(phpArray.Get("Age").ToString()),
                Email = phpArray.Get("Email").ToString(),
                OnlineTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = StatusEnum.在线.ToString()
            };
            if (!userBLL.AddUser(user))
            {
                responseResult.ResponseDetails = "注册用户失败！";
                responseResult.ResponseCode = ResultCode.Failure;
            }
            else
            {
                responseResult.ResponseData = user;
                responseResult.ResponseDetails = "注册用户成功！";
                responseResult.ResponseCode = ResultCode.Ok;
            }
            responseResult.ResultString();
        }
        #endregion

        #region 用户上线
        /// <summary>
        /// 解析参数，获取用户信息
        /// </summary>
        public string  Verify_User(PHPArray  phpArray)
        {
            string strUserID = phpArray.Get("UserId").Value.ToString();
            string strPwd = phpArray.Get("PassWord").Value.ToString();

            UserInfo user = IPublic.VerifyUser(strUserID,strPwd);
            if (user == null)
            {
                return null;
            }

            user.OnlineTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            user.Status = StatusEnum.在线.ToString();
            if (!userBLL.UpdateUser(user))
            {
                responseResult.ResponseDetails = "修改状态失败！";
                responseResult.ResponseCode = ResultCode.Failure; ;

            }
            else
            {
                responseResult.ResponseData = user;
                responseResult.ResponseDetails = "用户上线成功！";
                responseResult.ResponseCode = ResultCode.Ok;
            }
           return  responseResult.ResultString();
        }
        #endregion

        #region 用户离线
        /// <summary>
        /// 用户离线
        /// </summary>
        public string  Downline_User(PHPArray phpArray)
        {
            string strUserID = phpArray.Get("UserId").Value.ToString();
            string strPwd = phpArray.Get("PassWord").Value.ToString();

            UserInfo user = IPublic.VerifyUser(strUserID,strPwd);
            // 验证失败，立即返回
            if (user == null) return "";
            user.OfflineTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            user.Status = StatusEnum.离线.ToString();
            if (!userBLL.UpdateUser(user))
            {
                responseResult.ResponseDetails = "修改状态失败！";
                responseResult.ResponseCode = ResultCode.Failure;
                responseResult.ResultString();
            }
            responseResult.ResponseData = user;
            responseResult.ResponseDetails = "用户下线成功！";
            responseResult.ResponseCode = ResultCode.Ok; 
           return responseResult.ResultString();
        }
        #endregion

        #region 获取好友
        /// <summary>
        /// 获取好友
        /// </summary>
        public string  Get_Friends(PHPArray phpArray)
        {
            string strUserID = phpArray.Get("UserId").Value.ToString();
            string strPwd = phpArray.Get("PassWord").Value.ToString();

            UserInfo user = IPublic.VerifyUser(strUserID,strPwd);
            if (user == null) 
            {
                return null ;
            }
            List<UserInfo> friends = userRelationBLL.GetFriends(user);
            if (friends == null)
            {
                responseResult.ResponseDetails = "没有好友！";
                responseResult.ResponseCode = 0;
            }
            else
            {
                List<UserInfo> OnlineFriends = new List<UserInfo>();
                List<UserInfo> OfflineFriends = new List<UserInfo>();
                foreach (UserInfo friend in friends)
                {
                    if (friend.Status == StatusEnum.在线.ToString())
                    {
                        OnlineFriends.Add(friend);
                    }
                    else
                    {
                        OfflineFriends.Add(friend);
                    }
                }
                responseResult.ResponseData = new Friends()
                {
                    OnlineFriends = OnlineFriends,
                    OfflineFriends = OfflineFriends
                };
                responseResult.ResponseDetails = "获取好友成功！";
                responseResult.ResponseCode = ResultCode.Ok;
            }
           return   responseResult.ResultString();
        }

        class Friends
        {
            public List<UserInfo> OnlineFriends { get; set; }
            public List<UserInfo> OfflineFriends { get; set; }
        }
        #endregion

        #region 添加好友
        /// <summary>
        /// 添加好友
        /// </summary>
        public string  Add_Friend(PHPArray phpArray)
        {
            string strUserID = phpArray.Get("UserId").Value.ToString();
            string strPwd = phpArray.Get("PassWord").Value.ToString();
            string strFriendId = phpArray.Get("FriendId").Value.ToString();

            UserInfo user = IPublic.VerifyUser(strUserID, strPwd);
            if (user == null)
            {
                return null;
            }
            UserRelation user_relation = new UserRelation()
            {
                UserId = strUserID,
                FriendId = strFriendId
            };
            if (!userRelationBLL.AddUserRelation(user_relation))
            {
                responseResult.ResponseDetails = "添加好友失败！";
                responseResult.ResponseCode = ResultCode.Failure;
            }
            else
            {
                responseResult.ResponseData = user_relation;
                responseResult.ResponseDetails = "添加好友成功！";
                responseResult.ResponseCode = ResultCode.Ok;
            }
           return   responseResult.ResultString();
        }
        #endregion
    }
}
