using System.Net.Http;
using System.Text;

namespace CommonHelper
{
    public class ResultHelper
    {
        /// <summary>
        /// 验证失败
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static HttpResponseMessage Failed(string msg)
        {
            object obj = new { Success = false, Data = "", Message = "" + msg };

            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(ServiceStack.Text.JsonSerializer.SerializeToString(obj), Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static HttpResponseMessage OK()
        {
            object obj = new { Success = true };
            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(ServiceStack.Text.JsonSerializer.SerializeToString(obj), Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        public static HttpResponseMessage OK(object data)
        {
            object obj = new { Success = true, Data = data, Message = "" };
            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(ServiceStack.Text.JsonSerializer.SerializeToString(obj), Encoding.GetEncoding("UTF-8"), "application/json") };
        }
    }
}
