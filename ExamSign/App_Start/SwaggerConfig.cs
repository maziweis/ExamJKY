using System.Web.Http;
using WebActivatorEx;
using ExamSign;
using Swashbuckle.Application;
using System;
using System.Xml.XPath;
using System.Reflection;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace ExamSign
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {                       
                        c.SingleApiVersion("v1", "ExamSign");
                        c.IncludeXmlComments(GetXml());
                       
                    })
                .EnableSwaggerUi(c =>
                    {
                        //c.InjectJavaScript(Assembly.GetExecutingAssembly(), "ExamSign.scripts.swagger.js");
                    });
        }

        private static string GetXml()
        {
            return String.Format(@"{0}bin/ExamSign.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
