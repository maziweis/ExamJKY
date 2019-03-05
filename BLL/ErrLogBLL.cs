using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ErrLogBLL
    {
        public static void AddLog(string controllerName, string actionName,string Msg, object Data)
        {
            Err_Log log = new Err_Log();
            log.control = controllerName;
            log.act = actionName;
            log.msg = Msg;
            log.data = Data;
            CommonHelper.MongoDbHelper.Insert(log, "err_log");
        }
    }
}
