/*服务器推送入口类，必须实现IHttpAsyncHandler接口*/
using System;
using System.Collections.Generic;
using System.Text;
#region 命名空间
using System.Web;
using ChatData.ChatModel;
#endregion

namespace ServerPushHttpHandler
{
    public class ServerPush : IHttpAsyncHandler
    {
        #region IHttpAsyncHandler 成员

        //启用对HTTP处理程序的异步调用
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            // 把异步请求context交给服务器推送处理程序 ServerPushHandler来执行ExecAction处理
            return new ServerPushHandler(context, new ServerPushResult(context, cb, extraData)).ExecAction();
        }

        //进程结束时提供异步处理 End 方法。
        public void EndProcessRequest(IAsyncResult result)
        {

        }
        #endregion

        #region IHttpHandler成员(IHttpAsyncHandler同时继承于IHttpHandler)，目前不用关注
        //获取一个值，该值指示其他请求是否可以使用 IHttpHandler 实例。 （继承自 IHttpHandler。）
        public bool IsReusable
        {
            get { return false; ; }
        }

        //通过实现 IHttpHandler 接口的自定义 HttpHandler 启用 HTTP Web 请求的处理。 （继承自 IHttpHandler。）
        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
