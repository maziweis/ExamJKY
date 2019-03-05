using ExamSign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;
using MongoDB.Bson;
using KingsunDB;
using BLL;
using System.IO;
using System.Web;
using CommonHelper;
using System.Data;
using MongoDB.Driver;

namespace ExamSign.Controllers
{
    /// <summary>
    /// 考试
    /// </summary>
    public class ExamController : ApiController
    {
        #region 考试
        /// <summary>
        /// 新增考试
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddExam([FromBody] ExamInfo m)
        {
            var hisExam= MongoDbHelper.QueryOne<E_Info>(DbName.E_Info, w => w.btstate == 0&&w.IsDel==0);
            if(hisExam!=null)
            {
                return ResultHelper.Failed("已存在进行中的考试");
            }
            if (m == null)
            {
                return ResultHelper.Failed("类容不能为空");
            }
            E_Info eInfo = new E_Info()
            {
                _id = ObjectId.GenerateNewId(),
                ppid = m.PublishID,
                pt = Function.ConvertDateI(DateTime.Now),
                enm = m.ExamName,
                sst = m.StuStartTime,
                set = m.StuEndTime,
                tst = m.TchStartTime,
                tet = m.TchEndTime,
                stcfm = 0,
                tchcfm = 0,
                btstate=0,
                IsDel=0
            };
            foreach (var item in m.subNames)
            {
                Sb sb = new Sb();
                sb._id = item.Key;
                sb.sbnm = item.Value;
                sb.stct = 0;
                sb.tchct = 0;
                eInfo.sbs.Add(sb);
            }
            string[] stuType = m.stuTypes.Split(',');
            foreach (var item in stuType)
            {
                Stp sTp = new Stp() { _id = ObjectId.GenerateNewId(), tp = item };
                eInfo.stps.Add(sTp);
            }
            MongoDbHelper.Insert(eInfo, DbName.E_Info);
            var allsch = MongoDbHelper.QueryBy<U_Info>(DbName.U_Info, w => w.tp == 1);
            List<Sch_Sc> ls = new List<Sch_Sc>();
            for (int i = 0; i < allsch.Count; i++)
            {
                Sch_Sc sc = new Sch_Sc();
                sc.eid = eInfo._id;
                sc.sid = allsch[i]._id;
                sc.state = 0;
                sc.snm = allsch[i].snm;
                sc.area = allsch[i].area;
                ls.Add(sc);
            }
            MongoDbHelper.Insert(ls, DbName.Sch_Sc);
            return ResultHelper.OK();
        }
        ///// <summary>
        ///// 查询考试
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public HttpResponseMessage SelectExam([FromBody] ExamID m)
        //{
        //    if (m == null)
        //    {
        //        return ResultHelper.Failed("类容不能为空");
        //    }
        //    var eInfo = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
        //    if (eInfo == null)
        //    {
        //        return ResultHelper.Failed("未找到该次考试信息");
        //    }
        //    ExamInfo e = new ExamInfo();
        //    e.ID = eInfo._id.ToString();
        //    e.PublishID = eInfo.ppid;
        //    e.PublishTime = eInfo.pt;
        //    e.ExamName = eInfo.enm;
        //    e.StuStartTime = eInfo.sst;
        //    e.StuEndTime = eInfo.set;
        //    e.TchStartTime = eInfo.tst;
        //    e.TchEndTime = eInfo.tet;
        //    e.StuConfirm = eInfo.stcfm;
        //    e.TchConfirm = eInfo.tchcfm;
        //    e.ButtonState = eInfo.btstate;
        //    foreach (var item in eInfo.sbs)
        //    {
        //        e.subNames.Add(item._id, item.sbnm);
        //    }
        //    foreach (var item in eInfo.stps)
        //    {
        //        e.stuTypes += item.tp + ",";
        //    }
        //    e.stuTypes = e.stuTypes.Remove(e.stuTypes.Length - 1);
        //    return ResultHelper.OK(e);
        //}
        /// <summary>
        /// 获取所有考试
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetExam()
        {
            var eInfos = MongoDbHelper.QueryBy<E_Info>(DbName.E_Info,w=>w.IsDel==0);
            eInfos = eInfos.OrderByDescending(o => o.pt).ToList();
            if (eInfos.Count == 0)
            {
                return ResultHelper.OK(new List<string>());
            }
            List<ExamInfo> es = new List<ExamInfo>();
            foreach (var eInfo in eInfos)
            {
                ExamInfo e = new ExamInfo();
                e.ID = eInfo._id.ToString();
                e.PublishID = eInfo.ppid;
                e.PublishTime = eInfo.pt;
                e.PublishName = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == eInfo.ppid) == null ? "未知" : MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == eInfo.ppid).pnm;
                e.ExamName = eInfo.enm;
                e.StuStartTime = eInfo.sst;
                e.StuEndTime = eInfo.set;
                e.TchStartTime = eInfo.tst;
                e.TchEndTime = eInfo.tet;
                e.StuConfirm = eInfo.stcfm;
                e.TchConfirm = eInfo.tchcfm;
                e.ButtonState = eInfo.btstate;
                e.StuStart = Function.ConvertDateI(DateTime.Now) > eInfo.sst ? 1 : 0;
                e.TchStart = Function.ConvertDateI(DateTime.Now) > eInfo.tst ? 1 : 0;
                e.StuEnd = Function.ConvertDateI(DateTime.Now) > eInfo.set ? 1 : 0;
                e.TchEnd = Function.ConvertDateI(DateTime.Now) > eInfo.tet ? 1 : 0;
                if (Function.ConvertDateI(DateTime.Now)< eInfo.sst&& Function.ConvertDateI(DateTime.Now) < eInfo.tst)
                {
                    e.IsScan = 0;
                }
                else
                {
                    e.IsScan = 1;
                }
                foreach (var item in eInfo.sbs)
                {
                    e.subNames.Add(item._id, item.sbnm);
                }
                foreach (var item in eInfo.stps)
                {
                    e.stuTypes += item.tp + ",";
                }
                e.stuTypes = e.stuTypes.Remove(e.stuTypes.Length - 1);
                es.Add(e);
            }
            return ResultHelper.OK(es);
        }
        /// <summary>
        /// 查询考试
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SelectExam([FromBody] ExamID m)
        {
            if (m == null)
            {
                return ResultHelper.Failed("类容不能为空");
            }
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
            if (eInfo == null)
            {
                return ResultHelper.Failed("未找到该次考试信息");
            }
            ExamInfo e = new ExamInfo();
            e.ID = eInfo._id.ToString();
            e.PublishID = eInfo.ppid;
            e.PublishTime = eInfo.pt;
            e.PublishName = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == eInfo.ppid) == null ? "未知" : MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == eInfo.ppid).pnm;
            e.ExamName = eInfo.enm;
            e.StuStartTime = eInfo.sst;
            e.StuEndTime = eInfo.set;
            e.TchStartTime = eInfo.tst;
            e.TchEndTime = eInfo.tet;
            e.StuConfirm = eInfo.stcfm;
            e.TchConfirm = eInfo.tchcfm;
            e.ButtonState = eInfo.btstate;
            e.StuStart = Function.ConvertDateI(DateTime.Now) > eInfo.sst ? 1 : 0;
            e.TchStart = Function.ConvertDateI(DateTime.Now) > eInfo.tst ? 1 : 0;
            e.StuEnd = Function.ConvertDateI(DateTime.Now) > eInfo.set ? 1 : 0;
            e.TchEnd = Function.ConvertDateI(DateTime.Now) > eInfo.tet ? 1 : 0;
            if (Function.ConvertDateI(DateTime.Now) < eInfo.sst && Function.ConvertDateI(DateTime.Now) < eInfo.tst)
            {
                e.IsScan = 0;
            }
            else
            {
                e.IsScan = 1;
            }
            foreach (var item in eInfo.sbs)
            {
                e.subNames.Add(item._id, item.sbnm);
            }
            foreach (var item in eInfo.stps)
            {
                e.stuTypes += item.tp + ",";
            }
            e.stuTypes = e.stuTypes.Remove(e.stuTypes.Length - 1);
            return ResultHelper.OK(e);
        }
        /// <summary>
        /// 编辑考试
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditExam([FromBody] ExamInfo m)
        {
            if (m == null)
            {
                return ResultHelper.Failed("类容不能为空");
            }
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
            if (eInfo == null)
            {
                return ResultHelper.Failed("未找到该次考试信息");
            }
            eInfo.tet = m.TchEndTime;
            eInfo.set = m.StuEndTime;
            eInfo.enm = m.ExamName;
            if (Function.ConvertDateI(DateTime.Now) < eInfo.sst && Function.ConvertDateI(DateTime.Now) < eInfo.tst)//考试还没开始导入
            {
                eInfo.sst = m.StuStartTime;
                eInfo.tst = m.TchStartTime;
                eInfo.stcfm = 0;
                eInfo.tchcfm = 0;
                eInfo.sbs = new List<Sb>();
                eInfo.stps = new List<Stp>();
                foreach (var item in m.subNames)
                {
                    Sb sb = new Sb();
                    sb._id = item.Key;
                    sb.sbnm = item.Value;
                    sb.stct = 0;
                    sb.tchct = 0;
                    eInfo.sbs.Add(sb);
                }
                string[] stuType = m.stuTypes.Split(',');
                foreach (var item in stuType)
                {
                    Stp sTp = new Stp() { _id = ObjectId.GenerateNewId(), tp = item };
                    eInfo.stps.Add(sTp);
                }
            }           
            MongoDbHelper.ReplaceOne(m.ID, eInfo, DbName.E_Info);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 删除考试
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DelExam([FromBody] ExamID m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
            if (eInfo == null)
            {
                return ResultHelper.Failed("未找到该次考试信息");
            }
            eInfo.IsDel = 1;
            MongoDbHelper.ReplaceOne<E_Info>(m.ID, eInfo, DbName.E_Info);
            return ResultHelper.OK();
        }
        #endregion
    }
}
