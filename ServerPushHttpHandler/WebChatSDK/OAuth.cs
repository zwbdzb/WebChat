using System;
using System.Collections.Generic;
using System.Text;
#region 命名空间
using ChatData.PHPLibrary;
#endregion

namespace WebChatSDK
{
    /// <summary>
    /// OAuth签名
    /// </summary>
    public class OAuth
    {

        /// <summary>
        /// SDK
        /// </summary>
        public WebChat SDK { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="sdk"></param>
        public OAuth(WebChat sdk)
        {
            this.SDK = sdk;
        }

        /// <summary>
        /// 其他参数
        /// </summary>
        public PHPArray Array { get; set; }

        /// <summary>
        /// 组装完整参数
        /// </summary>
        /// <returns></returns>
        public  PHPArray GetParameList()
        {
            PHPArray parame = new PHPArray();
            parame.AddObject("UserId", this.SDK.User.UserId);
            parame.AddObject("PassWord", this.SDK.User.PassWord);
          //  parame.Add(new Parameter("UserId", this.SDK.User.UserId));            // 可以深度考虑一下动态添加参数的做法 ！！
          //  parame.Add(new Parameter("PassWord", this.SDK.User.PassWord));

            if (!string.IsNullOrEmpty(this.SDK.CallBackUrl))
            {
                parame.Add(new Parameter("oauth_callback", this.SDK.CallBackUrl));
            }
            if (this.Array != null)
            {
                foreach (KeyValuePair<object, object> p in this.Array)
                {
                    parame.AddObject(p.Key.ToString(), p.Value.ToString());
                }
            }
            //#region  排序
            //parame.Sort(delegate(Parameter x, Parameter y)
            //{
            //    if (x.Name == y.Name)
            //    {
            //        return string.Compare(x.Value, y.Value);
            //    }
            //    else
            //    {
            //        return string.Compare(x.Name, y.Name);
            //    }
            //});
            //#endregion 
            return parame;
        }

        /// <summary>
        /// 返回参数字符串，工程不需要使用
        /// </summary>
        /// <param name="par"></param>
        /// <param name="isEncode"></param>
        /// <returns></returns>
        public static string ParameToString(List<Parameter> par, bool isEncode)
        {
            StringBuilder ParameString = new StringBuilder();
            for (int i = 0; i < par.Count; i++)
            {
                string formatString = i == 0 ? "?{0}={1}" : "&{0}={1}";
                if (isEncode)
                {
                    ParameString.AppendFormat(formatString, Common.UrlEncode(par[i].Name), Common.UrlEncode(par[i].Value));
                }
                else
                {
                    ParameString.AppendFormat(formatString, par[i].Name, par[i].Value);
                }
            }
            return ParameString.ToString();
        }
    }
}
