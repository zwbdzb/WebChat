using System;
using System.Collections.Generic;
using System.Text;
#region 命名空间
using System.Web;
using Newtonsoft.Json;
#endregion

namespace ChatData
{
    /// <summary>
    /// 处理信息类
    ///     ResponseData--输出处理数据
    ///     ResponseDetails--处理详细信息
    ///     ResponseStatus--处理状态
    /// </summary>
    public class ResponseResult
    {
        public object ResponseData { get; set; }
        public string ResponseDetails { get; set; }
        public ResultCode ResponseCode { get; set; }

        //  直接返回处理结果JSON字符串
        public string ResultString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        //  将响应内容写入到到Http Web响应流
        public void ResponseWrite()
        {
           HttpContext.Current.Response.Write(ResultString());
        }
    }
}
