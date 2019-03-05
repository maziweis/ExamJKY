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
    /// 试卷
    /// </summary>
    public class PaperController : ApiController
    {
        #region 试卷
        /// <summary>
        /// 查询试卷
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SelPaper([FromBody] SelPaper m)
        {
            var filter = new BsonDocument();
            ObjectId objectid = new ObjectId();
            if (ObjectId.TryParse(m.ExamID, out objectid))
            {
                filter.Add("eid", objectid);
                int AllCount = 0;
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
                    return ResultHelper.OK(new { Data = ppp, Count = 0, SubCount = new List<SubInfo>(), AllCount = 0 });
                }
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
                List<SubInfo> lsub = new List<SubInfo>();
                for (int i = 0; i < exam.sbs.Count; i++)
                {
                    SubInfo sb = new SubInfo();
                    sb.SubName = exam.sbs[i].sbnm;
                    sb.StuCount = exam.sbs[i].stct;
                    AllCount += sb.StuCount;
                    lsub.Add(sb);
                }
                List<PaperNum> lm = new List<PaperNum>();
                for (int i = 0; i < data.Count; i++)
                {
                    PaperNum t = new PaperNum();
                    t.ID = data[i]._id.ToString();
                    t.ExamID = data[i].eid.ToString();
                    t.SchoolID = data[i].sid;
                    t.SchoolName = data[i].snm;
                    for (int j = 0; j < exam.sbs.Count; j++)
                    {
                        SubNum sub = new SubNum();
                        sub.SubID = exam.sbs[j]._id.ToString();
                        sub.SubName = exam.sbs[j].sbnm;
                        sub.SubCount = data[i].sbnms.Where(w => w.sbid == exam.sbs[j]._id).FirstOrDefault() == null ? 0 : data[i].sbnms.Where(w => w.sbid == exam.sbs[j]._id).FirstOrDefault().sct;
                        t.Count += sub.SubCount;
                        t.Subs.Add(sub);
                    }
                    lm.Add(t);
                }
                return ResultHelper.OK(new { Data = lm, Count = Count, SubCount = lsub, AllCount= AllCount });
            }
            return ResultHelper.Failed("未找到该次考试信息");
        }
        /// <summary>
        /// 导出试卷
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportPaperExcel([FromBody] ExportPaper m)
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
            var sts = MongoDbHelper.GetPagedList1<Pp_Nm, string>(DbName.Pp_Nm, 0, 0, filter, w => w.sid);
            List<string> data = ExcelBLL.GetPaperColumn(eInfo);
            DataTable dt = new DataTable();
            for (int j = 0; j < data.Count; j++)
            {
                dt.Columns.Add(data[j]);
            }
            for (int i = 0; i < sts.Count; i++)
            {
                DataRow dr = dt.NewRow();
                var count = 0;
                dr[0] = sts[i].sid;
                dr[1] = sts[i].snm;               
                for (int j = 0; j < eInfo.sbs.Count; j++)
                {
                    var pp = sts[i].sbnms.Where(w => w.sbnm == eInfo.sbs[j].sbnm).FirstOrDefault();
                    if (pp != null)
                    {
                        dr[3 + j] = pp.sct;
                        count+= pp.sct;
                    }
                    else
                    {
                        dr[3 + j] = 0;
                    }
                }
                dr[2] = count;
                dt.Rows.Add(dr);
            }
            string file = ExcelBLL.BuildExcel1(data.ToArray(), dt);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }
        #endregion
    }
}
