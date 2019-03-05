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
    /// 老师
    /// </summary>
    public class TeacherController : ApiController
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object _locker = new object();
        #region 老师
        /// <summary>
        /// 生成老师导入表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage BuildTchExcel([FromBody] ExamID m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
            if (eInfo == null)
            {
                return ResultHelper.Failed("未查到当前考试");
            }
            List<string> data = ExcelBLL.GetTchColumn();
            List<string> col = new List<string>();
            foreach (var item in eInfo.sbs)
            {
                col.Add(item.sbnm);
            }
            string file = BLL.ExcelBLL.BuildTchExcel(data, col);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }
        /// <summary>
        /// 导入老师信息
        /// </summary>
        /// <param name="ExamID"></param>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ImportTchExcel(string ExamID, string SchoolID = "SchoolID")
        {
            lock (_locker)
            {
                try
                {
                    var exam = MongoDbHelper.FindOne<E_Info>(ExamID, DbName.E_Info);
                    if (Function.ConvertDateI(DateTime.Now) < exam.tst)
                    {
                        return ResultHelper.Failed("还未到录入时间，请等待");
                    }
                    HttpPostedFile file = HttpContext.Current.Request.Files[0];
                    if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\tch-" + DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
                    file.SaveAs(path);
                    string msg = ""; int errnum = 0;

                    msg = ExcelBLL.ImportTchExcel(path, exam, SchoolID, out errnum);
                    if (msg != "")
                    {
                        if (msg == "表格格式错误，请重新下载模板")
                        {
                            return ResultHelper.Failed(msg);
                        }
                        string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + msg;
                        return ResultHelper.Failed("导入失败" + errnum + "条--" + url);
                    }
                    return ResultHelper.OK();
                }
                catch (Exception)
                {
                    return ResultHelper.Failed("导入失败");
                }
            }
        }
        /// <summary>
        /// 导出老师信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportTchExcel([FromBody] ExportTchInfo m)
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
            if (!string.IsNullOrEmpty(m.Subject))
            {
                filter.Add("sb", m.Subject);
            }
            if (!string.IsNullOrEmpty(m.Name))
            {
                filter.Add("nm", m.Name);
            }
            var tchs = MongoDbHelper.GetPagedList1<Tch_Info, string>(DbName.Tch_Info, 0, 0, filter, w => w.nm);
            List<string> data = ExcelBLL.GetTchColumn();
            DataTable dt = new DataTable();
            for (int j = 0; j < data.Count; j++)
            {
                dt.Columns.Add(data[j]);
            }
            for (int i = 0; i < tchs.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = tchs[i].sid;
                dr[1] = tchs[i].snm;
                dr[2] = tchs[i].sb;
                dr[3] = tchs[i].nm;
                dr[4] = tchs[i].sx == 0 ? "女" : "男";
                dr[5] = tchs[i].zc;
                dr[6] = tchs[i].ph;
                dt.Rows.Add(dr);
            }
            string file = ExcelBLL.BuildExcel1(data.ToArray(), dt);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }
        /// <summary>
        /// 查询老师信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SelTeacher([FromBody] SelTchInfo m)
        {
            var filter = new BsonDocument();
            ObjectId objectid = new ObjectId();
            if (ObjectId.TryParse(m.ExamID, out objectid))
            {
                filter.Add("eid", objectid);
                int AllCount = MongoDbHelper.GetCount<Tch_Info>(DbName.Tch_Info, filter);
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
                if (!string.IsNullOrEmpty(m.Subject))
                {
                    filter.Add("sb", m.Subject);
                }
                if (!string.IsNullOrEmpty(m.Name))
                {
                    filter.Add("nm", m.Name);
                }
                var data = MongoDbHelper.GetPagedList1<Tch_Info, string>(DbName.Tch_Info, m.Skip, m.Limit, filter, w => w.nm);
                int Count = MongoDbHelper.GetCount<Tch_Info>(DbName.Tch_Info, filter);
                if (Count == 0)
                {
                    return ResultHelper.OK(new List<string>());
                }
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
                List<SubInfo> lsub = new List<SubInfo>();
                if (m.Area != 0)//前台查询
                {
                    AllCount = MongoDbHelper.QueryBy<Tch_Info>(DbName.Tch_Info, w => w.eid == exam._id && w.sid == m.School).Count;
                    for (int i = 0; i < exam.sbs.Count; i++)
                    {
                        SubInfo sb = new SubInfo();
                        sb.SubName = exam.sbs[i].sbnm;
                        sb.TchCount = MongoDbHelper.QueryBy<Tch_Info>(DbName.Tch_Info, w => w.eid == exam._id && w.sid == m.School && w.sb == exam.sbs[i].sbnm).Count;
                        lsub.Add(sb);
                    }
                }
                else
                {
                    for (int i = 0; i < exam.sbs.Count; i++)
                    {
                        SubInfo sb = new SubInfo();
                        sb.SubName = exam.sbs[i].sbnm;
                        sb.TchCount = MongoDbHelper.QueryBy<Tch_Info>(DbName.Tch_Info, w => w.eid == exam._id && w.sb == exam.sbs[i].sbnm).Count;
                        lsub.Add(sb);
                    }
                }
                List<TeacherInfo> ltch = new List<TeacherInfo>();
                for (int i = 0; i < data.Count; i++)
                {
                    TeacherInfo t = new TeacherInfo();
                    t.ID = data[i]._id.ToString();
                    t.ExamID = data[i].eid.ToString();
                    t.Name = data[i].nm;
                    t.SchoolID = data[i].sid;
                    t.SchoolName = data[i].snm;
                    t.Subject = data[i].sb;
                    t.Title = data[i].zc;
                    t.Sex = data[i].sx == 0 ? "女" : "男";
                    t.Phone = data[i].ph;
                    ltch.Add(t);
                }
                return ResultHelper.OK(new { Data = ltch, Count = Count, SubCount = lsub, AllCount = AllCount });
            }
            return ResultHelper.Failed("未找到该次考试信息");
        }
        /// <summary>
        /// 确认老师信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ConfirmTeacher([FromBody] ExamID m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
            eInfo.tchcfm = 1;
            MongoDbHelper.ReplaceOne(m.ID, eInfo, DbName.E_Info);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 编辑老师信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditTeacher([FromBody]TeacherInfo m)
        {
            Tch_Info tch = MongoDbHelper.FindOne<Tch_Info>(m.ID, DbName.Tch_Info);
            if (tch == null)
            {
                return ResultHelper.Failed("未找到该老师信息");
            }
            if (!Function.MathPhone(m.Phone.ToString()))
            {
                return ResultHelper.Failed("请输入有效手机号");
            }
            tch.sb = m.Subject;
            tch.nm = m.Name;
            tch.sx = m.Sex == "男" ? 1 : 0;
            tch.zc = m.Title;
            tch.ph = m.Phone;
            MongoDbHelper.ReplaceOne(m.ID, tch, DbName.Tch_Info);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 删除老师
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DelTeacher([FromBody]DelStuOrTch m)
        {
            lock (_locker)
            {
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
                if (exam == null)
                {
                    return ResultHelper.Failed("未找到该次考试");
                }
                Tch_Info tch = MongoDbHelper.FindOne<Tch_Info>(m.UserID, DbName.Tch_Info);
                if (tch == null)
                {
                    return ResultHelper.Failed("未找到该老师信息");
                }
                var sb = exam.sbs.Where(w => w.sbnm == tch.sb).FirstOrDefault();
                sb.tchct--;
                MongoDbHelper.DeleteOne<Tch_Info>(m.UserID, DbName.Tch_Info);
                MongoDbHelper.ReplaceOne(m.ExamID, exam, DbName.E_Info);
                return ResultHelper.OK();
            }
        }
        /// <summary>
        /// 清除老师
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ClearTeacher([FromBody]ExamID m)
        {
            var exam = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);//找到这次考试
            if (exam == null)
            {
                return ResultHelper.Failed("未找到该次考试");
            }
            if (exam.tet < Function.ConvertDateI(DateTime.Now))
            {
                return ResultHelper.Failed("已经导入结束，不能清空老师");
            }
            var filter = new BsonDocument();
            filter.Add("eid", exam._id);
            MongoDbHelper.DeleteByBson<Tch_Info>(filter, DbName.Tch_Info);
            exam.sbs.ForEach(f => f.tchct = 0);
            MongoDbHelper.ReplaceOne(m.ID, exam, DbName.E_Info);
            return ResultHelper.OK();
        }
        #endregion
    }
}
