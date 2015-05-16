using System;
using System.Collections.Generic;
using System.Text;
#region 命名空间
using System.Web;
using System.Threading;
using System.Collections;
using WebChatSDK;
#endregion

namespace ServerPushHttpHandler
{
   // 继承自服务器异步操作的状态信息，包括请求上下文Contxt、异步完成时的回调m_callback、处理异步操作的其他数据
    public class ServerPushResult : IAsyncResult
    {
        #region Field

        HttpContext m_Context;
        AsyncCallback m_Callback;
        object m_ExtraData;
        bool m_IsCompleted = false;
        public string Result { get; set; }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cb"></param>
        /// <param name="extraData"></param>
        public ServerPushResult(HttpContext context, AsyncCallback cb, object extraData)
        {
            m_Context = context;
            m_Callback = cb;
            m_ExtraData = extraData;
        }
        #endregion

        #region 服务器有数据要传递，可以显示调用Send（）返回
        /// <summary>
        /// 向客户端响应消息
        /// </summary>
        /// <param name="result">结果信息</param>
        public void Send()
        {
            try
            {
                m_Context.Response.Write(Result);           // 1"  服务器有数据
                if (m_Callback != null)
                {
                    m_Callback(this);
                }
            }
            catch { }
            finally
            {
                m_IsCompleted = true;
            }
        }
        #endregion

        #region IAsyncResult 成员
        //获取用户定义的对象，它限定或包含关于异步操作的信息。
        public object AsyncState
        {
            get { return null; }
        }

        //获取用于等待异步操作完成的 WaitHandle。
        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        //获取异步操作是否同步完成的指示。
        public bool CompletedSynchronously
        {
            get { return false; }
        }

        //获取异步操作是否已完成的指示。
        public bool IsCompleted
        {
            get { return m_IsCompleted; }
        }
        #endregion
    }
}


/*
 
 1'' 这里会出现基于Ajax long-polling 技术所说的第三点，客户端还没再次连接时，服务器先保存好收到的数据，等到成功连接，一次性发送数据，
   这里我想 AsynccallBack 应该有等待客户端连接上的机制。
     AsynccallBack 引用相应异步操作完成时调用的方法，这是一个指针，也就是说Response里面有数据或者超时，
 
 */