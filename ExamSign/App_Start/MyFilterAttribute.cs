using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ExamSign
{
    /// <summary>
    /// 自定义Filter
    /// </summary>
    public class MyFilterAttribute : ActionFilterAttribute
    {
        private const string Key = "__action_duration__";

        /// <summary>
        /// 执行开始
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var stopWatch = new Stopwatch();
            filterContext.Request.Properties[Key] = stopWatch;
            stopWatch.Start();

            //base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 执行结束
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            if (filterContext.Request.Properties.ContainsKey(Key))
            {
                var stopWatch = filterContext.Request.Properties[Key] as Stopwatch;
                if (stopWatch != null)
                {
                    stopWatch.Stop();

                    System.Web.HttpContextWrapper context = ((System.Web.HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]);
                    string controllerName = filterContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    string actionName = filterContext.ActionContext.ActionDescriptor.ActionName;
                    string httpMethod = filterContext.Request.Method.ToString();

                    double rt = stopWatch.Elapsed.TotalSeconds;
                    if (rt > 2)
                    {
                        BLL.ErrLogBLL.AddLog(controllerName, actionName, "Api执行时间", rt);
                    }
                }
            }

            //base.OnActionExecuted(filterContext);
        }
    }
}