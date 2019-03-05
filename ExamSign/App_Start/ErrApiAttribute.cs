using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace ExamSign
{

    /// <summary>
    /// 全局异常捕获
    /// </summary>
    public class ErrApiAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// API全局异常处理
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(HttpActionExecutedContext filterContext)
        {
            //写日志
            string controllerName = filterContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionContext.ActionDescriptor.ActionName;
            string httpMethod = filterContext.Request.Method.ToString();
            string Msg = filterContext.Exception.Message;
            string Data = "";

            System.Web.HttpContextWrapper context = ((System.Web.HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]);
            using (StreamReader sr = new StreamReader(context.Request.InputStream))
            {
                Data = sr.ReadToEnd();
            }
            BLL.ErrLogBLL.AddLog(controllerName, actionName, Msg, Data);     
            //返回错误信息
            Exception e = filterContext.Exception;
            filterContext.Response = CommonHelper.ResultHelper.Failed(Msg);

            base.OnException(filterContext);
        }
    }
}