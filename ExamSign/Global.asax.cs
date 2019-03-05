using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ExamSign
{
    /// <summary>
    /// 
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
        /// <summary>
        /// 
        /// </summary>
        protected void Application_BeginRequest()
        {
            try
            {
                string url = Context.Request.Url.ToString();
                if (url.Contains("/page/"))
                {
                    var newUrl = url.Replace("/page/", "?router=");
                    Response.Redirect(newUrl, false);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }


        ///// <summary>
        ///// 让webapi使用session
        ///// </summary>
        //public override void Init()
        //{
        //    this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        //    base.Init();
        //}
    }
}
