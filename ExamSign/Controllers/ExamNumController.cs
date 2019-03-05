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
    /// 考试人数
    /// </summary>
    public class ExamNumController : ApiController
    {
        #region 参考实考人数
        /// <summary>
        /// 参考人数录入查询应考人数
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SelStudentNum([FromBody] ExamAndID m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
            ObjectId objectId = new ObjectId();
            if (ObjectId.TryParse(m.ExamID, out objectId))
            {
                var data = MongoDbHelper.QueryOne<Pp_Nm>(DbName.Pp_Nm, w => w.sid == m.ID && w.eid == objectId);
                if (data == null)
                {
                    return ResultHelper.OK(new { Data = new List<PaperNum>(),Count=0 });
                }
                PaperNum t = new PaperNum();
                t.IsSure = data.iss;
                for (int j = 0; j < eInfo.sbs.Count; j++)
                {
                    SubNum sub = new SubNum();
                    var s = data.sbnms.Where(w => w.sbid == eInfo.sbs[j]._id).FirstOrDefault();
                    sub.SubID = s.sbid.ToString();
                    sub.SubName = s.sbnm;
                    sub.SubCount = s.sct;
                    sub.AcCount = s.ac;
                    t.Subs.Add(sub);
                }
                return ResultHelper.OK(t);
            }
            return ResultHelper.Failed("未找到该次考试");
        }
        /// <summary>
        /// 提交实考人数
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CommitStudentNum([FromBody] ExamStuNum m)
        {
            ObjectId objectId = new ObjectId();
            if (ObjectId.TryParse(m.ExamID, out objectId))
            {
                var data = MongoDbHelper.QueryOne<Pp_Nm>(DbName.Pp_Nm, w => w.sid == m.SchoolID && w.eid == objectId);
                if (data == null)
                {
                    return ResultHelper.Failed("未找到该学校");
                }
                PaperNum t = new PaperNum();
                for (int j = 0; j < data.sbnms.Count; j++)
                {
                    var SubNum = m.Subs.Where(w => w.SubID == data.sbnms[j].sbid.ToString()).FirstOrDefault();
                    data.sbnms[j].ac = SubNum.AcCount;
                }
                data.iss = m.IsSure;
                MongoDbHelper.ReplaceOne(data._id.ToString(), data, DbName.Pp_Nm);
                return ResultHelper.OK();
            }
            return ResultHelper.Failed("未找到该次考试");
        }
        /// <summary>
        /// 统计参考人数
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage StaStudentNum([FromBody] SelPaper m)
        {
            var filter = new BsonDocument();
            ObjectId objectId = new ObjectId();
            if (ObjectId.TryParse(m.ExamID, out objectId))
            {
                filter.Add("eid", objectId);
                if (!string.IsNullOrEmpty(m.School))
                {
                    int id = -1;
                    if (int.TryParse(m.School, out id))
                    {
                        filter.Add("sid", m.School);
                    }
                    else
                    {
                        filter.Add("snm", m.School);
                    }
                }
                var data = MongoDbHelper.GetPagedList1<Pp_Nm, string>(DbName.Pp_Nm, m.Skip, m.Limit, filter, w => w.sid);
                int Count = MongoDbHelper.GetCount<Pp_Nm>(DbName.Pp_Nm, filter);
                List<PaperNum> lm = new List<PaperNum>();
                if (Count == 0)
                {
                    var ppp = new List<PaperNum>();
                    PaperNum pn = new PaperNum();
                    ppp.Add(pn);
                    var exam1 = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
                    for (int i = 0; i < exam1.sbs.Count; i++)
                    {
                        SubNum sub = new SubNum();
                        sub.SubName = exam1.sbs[i].sbnm;
                        sub.SubID = exam1.sbs[i]._id.ToString();
                        pn.Subs.Add(sub);
                    }
                    return ResultHelper.OK(new { Data = ppp, Count = Count });
                }
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
              
                for (int i = 0; i < data.Count; i++)
                {
                    PaperNum t = new PaperNum();
                    t.ID = data[i]._id.ToString();
                    t.SchoolID = data[i].sid;
                    t.SchoolName = data[i].snm;
                    t.IsSure = data[i].iss;
                    for (int j = 0; j < exam.sbs.Count; j++)
                    {
                        SubNum sub = new SubNum();
                        sub.SubID = exam.sbs[j]._id.ToString();
                        sub.SubName = exam.sbs[j].sbnm;
                        sub.SubCount = data[i].sbnms.Where(w => w.sbid == exam.sbs[j]._id).FirstOrDefault() == null ? 0 : data[i].sbnms.Where(w => w.sbid == exam.sbs[j]._id).FirstOrDefault().sct;
                        sub.AcCount = data[i].sbnms.Where(w => w.sbid == exam.sbs[j]._id).FirstOrDefault() == null ? 0 : data[i].sbnms.Where(w => w.sbid == exam.sbs[j]._id).FirstOrDefault().ac;
                        t.Subs.Add(sub);
                    }
                    lm.Add(t);
                }
                return ResultHelper.OK(new { Data=lm,Count=Count });
            }
            return ResultHelper.Failed("未找到该次考试");
        }
        /// <summary>
        /// 导出参考人数统计表
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportExamStuExcel([FromBody] ExportPaper m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
            if (eInfo == null)
            {
                return ResultHelper.Failed("未查到当前考试");
            }
            var filter = new BsonDocument();
            filter.Add("eid", eInfo._id);
            if (!string.IsNullOrEmpty(m.School))
            {
                int id = -1;
                if (int.TryParse(m.School, out id))
                {
                    filter.Add("sid", m.School);
                }
                else
                {
                    filter.Add("snm", m.School);
                }
            }
            var pps = MongoDbHelper.GetPagedList1<Pp_Nm, string>(DbName.Pp_Nm, 0, 0, filter, w => w.sid);
            List<string> data = ExcelBLL.GetPaperColumn1(eInfo);
            List<string> data1 = new List<string>();
            for (int i = 0; i < eInfo.sbs.Count; i++)
            {
                data1.Add(eInfo.sbs[i].sbnm);
            }
            DataTable dt = new DataTable();
            for (int j = 0; j < data.Count; j++)
            {
                dt.Columns.Add(data[j]);
            }
            for (int i = 0; i < pps.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = pps[i].sid;
                dr[1] = pps[i].snm;
                for (int j = 0; j < eInfo.sbs.Count; j++)
                {
                    var sbnm = pps[i].sbnms.Where(w => w.sbid == eInfo.sbs[j]._id).FirstOrDefault();
                    dr[2 + j * 3] = sbnm == null ? 0 : sbnm.sct;
                    dr[3 + j * 3] = sbnm == null ? 0 : sbnm.ac;
                    dr[4 + j * 3] = sbnm == null ? 0 : sbnm.sct- sbnm.ac;
                }
                dt.Rows.Add(dr);
            }
            string file = ExcelBLL.BuildPaperExcel(data1.ToArray(), dt);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }
        #endregion
    }
}
