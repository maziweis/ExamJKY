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
    /// 学生
    /// </summary>
    public class StudentController : ApiController
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object _locker = new object();
        #region 学生
        /// <summary>
        /// 生成学生信息导入表格
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage BuildStuExcel([FromBody] ExamID m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
            if (eInfo == null)
            {
                return ResultHelper.Failed("未查到当前考试");
            }
            List<string> data = ExcelBLL.GetStuColumn(eInfo);
            List<string> col = new List<string>();
            foreach (var item in eInfo.stps)
            {
                col.Add(item.tp);
            }
            string file = BLL.ExcelBLL.BuildStuExcel(data, col);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }

        /// <summary>
        /// 导入学生信息
        /// </summary>
        /// <param name="ExamID"></param>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ImportStuExcel(string ExamID, string SchoolID = "SchoolID")
        {
            lock (_locker)
            {
                try
                {
                    var exam = MongoDbHelper.FindOne<E_Info>(ExamID, DbName.E_Info);
                    if (CommonHelper.Function.ConvertDateI(DateTime.Now) < exam.sst)
                    {
                        return ResultHelper.Failed("还未到录入时间，请等待");
                    }
                    HttpPostedFile file = HttpContext.Current.Request.Files[0];
                    if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "Excel\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\stu-" + DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
                    file.SaveAs(path);
                    string msg = ""; int errnum = 0;

                    msg = ExcelBLL.ImportStuExcel(path, exam, SchoolID, out errnum);
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
        /// 导出学生信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportStuExcel([FromBody] ExportStuInfo m)
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
            if (!string.IsNullOrEmpty(m.StudentID))
            {
                filter.Add("stid", m.StudentID);
            }
            if (!string.IsNullOrEmpty(m.StudentName))
            {
                filter.Add("nm", m.StudentName);
            }
            var sts = MongoDbHelper.GetPagedList1<St_Info, string>(DbName.St_Info, 0, 0, filter, w => w.stid);
            List<string> data = ExcelBLL.GetStuColumn(eInfo);
            DataTable dt = new DataTable();
            for (int j = 0; j < data.Count; j++)
            {
                dt.Columns.Add(data[j]);
            }
            for (int i = 0; i < sts.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = sts[i].sid;
                dr[1] = sts[i].snm;
                dr[2] = sts[i].stid;
                dr[3] = sts[i].cls;
                dr[4] = sts[i].nm;
                for (int j = 0; j < eInfo.sbs.Count; j++)
                {
                    var sb = sts[i].subEs.Where(w => w.sbnm == eInfo.sbs[j].sbnm).FirstOrDefault();
                    if (sb != null)
                    {
                        dr[5 + j * 3] = sb.sbrm;
                        dr[6 + j * 3] = sb.sbst;
                        dr[7 + j * 3] = sb.sbtch;
                    }
                    else
                    {
                        dr[5 + j * 3] = null;
                        dr[6 + j * 3] = null;
                        dr[7 + j * 3] = null;
                    }
                }
                dr[5 + eInfo.sbs.Count * 3] = sts[i].idcd;
                dr[6 + eInfo.sbs.Count * 3] = sts[i].tp;
                dt.Rows.Add(dr);
            }
            string file = ExcelBLL.BuildExcel1(data.ToArray(), dt);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }
        /// <summary>
        /// 查询学生信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SelStudent([FromBody] SelStuInfo m)
        {
            var filter = new BsonDocument();
            ObjectId objectid = new ObjectId();
            if (ObjectId.TryParse(m.ExamID, out objectid))
            {
                filter.Add("eid", objectid);
                int AllCount = MongoDbHelper.GetCount<St_Info>(DbName.St_Info, filter);
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
                if (!string.IsNullOrEmpty(m.StudentID))
                {
                    filter.Add("stid", m.StudentID);
                }
                if (!string.IsNullOrEmpty(m.StudentName))
                {
                    filter.Add("nm", m.StudentName);
                }
                var data = MongoDbHelper.GetPagedList1<St_Info, string>(DbName.St_Info, m.Skip, m.Limit, filter, w => w.stid);

                int Count = MongoDbHelper.GetCount<St_Info>(DbName.St_Info, filter);
                if (Count == 0)
                {
                    return ResultHelper.OK(new List<string>());
                }
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
                List<SubInfo> lsub = new List<SubInfo>();
                if (m.Area != 0)//前台查询
                {
                    var ppnm = MongoDbHelper.QueryOne<Pp_Nm>(DbName.Pp_Nm, w => w.sid == m.School && w.eid == exam._id);
                    AllCount = ppnm.ct;
                    for (int i = 0; i < exam.sbs.Count; i++)
                    {
                        SubInfo sb = new SubInfo();
                        sb.SubName = exam.sbs[i].sbnm;
                        sb.StuCount = ppnm.sbnms.Where(w => w.sbid == exam.sbs[i]._id).FirstOrDefault() == null ? 0 : ppnm.sbnms.Where(w => w.sbid == exam.sbs[i]._id).FirstOrDefault().sct;
                        lsub.Add(sb);
                    }
                }
                else
                {
                    for (int i = 0; i < exam.sbs.Count; i++)
                    {
                        SubInfo sb = new SubInfo();
                        sb.SubName = exam.sbs[i].sbnm;
                        sb.StuCount = exam.sbs[i].stct;
                        lsub.Add(sb);
                    }
                }
                List<StudentInfo> lst = new List<StudentInfo>();
                for (int i = 0; i < data.Count; i++)
                {
                    StudentInfo s = new StudentInfo();
                    s.ID = data[i]._id.ToString();
                    s.ExamID = data[i].eid.ToString();
                    s.Class = data[i].cls;
                    s.IdCard = data[i].idcd;
                    s.Name = data[i].nm;
                    s.SchoolID = data[i].sid;
                    s.SchoolName = data[i].snm;
                    s.StudentID = data[i].stid;
                    s.Type = data[i].tp;
                    for (int j = 0; j < data[i].subEs.Count; j++)
                    {
                        SubInfo sub = new SubInfo();
                        sub.SubID = data[i].subEs[j].sbid.ToString();
                        sub.SubName = data[i].subEs[j].sbnm;
                        sub.SubRoom = data[i].subEs[j].sbrm;
                        sub.SubSite = data[i].subEs[j].sbst;
                        sub.SubTeacher = data[i].subEs[j].sbtch;
                        s.Subs.Add(sub);
                    }
                    lst.Add(s);
                }
                return ResultHelper.OK(new { Data = lst, Count = Count, SubCount = lsub, AllCount = AllCount });
            }
            return ResultHelper.Failed("未找到该次考试信息");
        }
        /// <summary>
        /// 确认学生信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ConfirmStudent([FromBody] ExamID m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
            eInfo.stcfm = 1;
            MongoDbHelper.ReplaceOne(m.ID, eInfo, DbName.E_Info);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 编辑学生
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditStudent([FromBody]StudentInfo m)
        {
            St_Info st = MongoDbHelper.FindOne<St_Info>(m.ID, DbName.St_Info);
            if (st == null)
            {
                return ResultHelper.Failed("未找到该学生信息");
            }
            if (m.StudentID != st.stid)
            {
                St_Info st1 = MongoDbHelper.QueryOne<St_Info>(DbName.St_Info, w => w.stid == m.StudentID);
                if (st1 != null)
                {
                    return ResultHelper.Failed("考号已存在");
                }
            }
            if (!Function.MathIdCard(m.IdCard))
            {
                return ResultHelper.Failed("身份证号不正确");
            }
            if (m.IdCard != st.idcd)
            {
                St_Info st1 = MongoDbHelper.QueryOne<St_Info>(DbName.St_Info, w => w.idcd == m.IdCard);
                if (st1 != null)
                {
                    return ResultHelper.Failed("身份证号已存在");
                }
            }
            st.cls = m.Class;
            st.idcd = m.IdCard;
            st.nm = m.Name;
            st.stid = m.StudentID;
            //st.subEs = new List<SubE>();
            //for (int i = 0; i < m.Subs.Count; i++)
            //{
            //    ObjectId objectid1 = new ObjectId();
            //    if (ObjectId.TryParse(m.Subs[i].SubID, out objectid1))
            //    {
            //        SubE sb = new SubE();
            //        sb.sbid = objectid1;
            //        sb.sbnm = m.Subs[i].SubName;
            //        sb.sbrm = m.Subs[i].SubRoom;
            //        sb.sbst = m.Subs[i].SubSite;
            //        sb.sbtch = m.Subs[i].SubTeacher;
            //        st.subEs.Add(sb);
            //    }
            //}
            MongoDbHelper.ReplaceOne(m.ID, st, DbName.St_Info);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DelStudent([FromBody]DelStuOrTch m)
        {
            lock (_locker)
            {
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);//找到这次考试
                if (exam == null)
                {
                    return ResultHelper.Failed("未找到该次考试");
                }
                St_Info st = MongoDbHelper.FindOne<St_Info>(m.UserID, DbName.St_Info);//找到这个学生
                if (st == null)
                {
                    return ResultHelper.Failed("未找到该考生信息");
                }
                var ppnm = MongoDbHelper.QueryOne<Pp_Nm>(DbName.Pp_Nm, w => w.sid == st.sid && w.eid == exam._id);//找到这个学校试卷数量
                ppnm.ct--;
                for (int i = 0; i < st.subEs.Count; i++)
                {
                    var sb = exam.sbs.Where(w => w._id == st.subEs[i].sbid).FirstOrDefault();
                    var pp = ppnm.sbnms.Where(w => w.sbid == st.subEs[i].sbid).FirstOrDefault();
                    sb.stct--;
                    pp.sct--;
                }
                MongoDbHelper.ReplaceOne(ppnm._id.ToString(), ppnm, DbName.Pp_Nm);
                MongoDbHelper.ReplaceOne(m.ExamID, exam, DbName.E_Info);
                MongoDbHelper.DeleteOne<St_Info>(m.UserID, DbName.St_Info);
                return ResultHelper.OK();
            }
        }
        /// <summary>
        /// 删除学校所有学生
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public HttpResponseMessage DelSchStudent([FromBody]DelStuOrTch m)
        {
            lock (_locker)
            {
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);//找到这次考试
                if (exam == null)
                {
                    return ResultHelper.Failed("未找到该次考试");
                }
                if (exam.set < Function.ConvertDateI(DateTime.Now))
                {
                    return ResultHelper.Failed("已经导入结束，不能清空学生");
                }
                var sts = MongoDbHelper.QueryBy<St_Info>(DbName.St_Info, w => w.sid == m.UserID && w.eid == exam._id);//找到这个学校学生           
                var ppnm = MongoDbHelper.QueryOne<Pp_Nm>(DbName.Pp_Nm, w => w.sid == m.UserID && w.eid == exam._id);//找到这个学校试卷数量
                if (ppnm != null)
                {
                    MongoDbHelper.DeleteOne<Pp_Nm>(ppnm._id.ToString(), DbName.Pp_Nm);
                }
                foreach (var st in sts)
                {
                    for (int i = 0; i < st.subEs.Count; i++)
                    {
                        var sb = exam.sbs.Where(w => w._id == st.subEs[i].sbid).FirstOrDefault();
                        sb.stct--;
                    }
                    MongoDbHelper.DeleteOne<St_Info>(st._id.ToString(), DbName.St_Info);
                }
                var filter = new BsonDocument();
                filter.Add("sid", m.UserID);
                filter.Add("eid", exam._id);
                MongoDbHelper.DeleteByBson<St_Info>(filter, DbName.St_Info);
                MongoDbHelper.ReplaceOne(m.ExamID, exam, DbName.E_Info);
                return ResultHelper.OK();
            }
        }
        /// <summary>
        /// 删除学校所有学生导入结束也可以删
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public HttpResponseMessage DelSchStudent1([FromBody]DelStuOrTch m)
        {
            lock (_locker)
            {
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);//找到这次考试
                if (exam == null)
                {
                    return ResultHelper.Failed("未找到该次考试");
                }                
                var sts = MongoDbHelper.QueryBy<St_Info>(DbName.St_Info, w => w.sid == m.UserID && w.eid == exam._id);//找到这个学校学生           
                var ppnm = MongoDbHelper.QueryOne<Pp_Nm>(DbName.Pp_Nm, w => w.sid == m.UserID && w.eid == exam._id);//找到这个学校试卷数量
                if (ppnm != null)
                {
                    MongoDbHelper.DeleteOne<Pp_Nm>(ppnm._id.ToString(), DbName.Pp_Nm);
                }
                foreach (var st in sts)
                {
                    for (int i = 0; i < st.subEs.Count; i++)
                    {
                        var sb = exam.sbs.Where(w => w._id == st.subEs[i].sbid).FirstOrDefault();
                        sb.stct--;
                    }
                    MongoDbHelper.DeleteOne<St_Info>(st._id.ToString(), DbName.St_Info);
                }
                var filter = new BsonDocument();
                filter.Add("sid", m.UserID);
                filter.Add("eid", exam._id);
                MongoDbHelper.DeleteByBson<St_Info>(filter, DbName.St_Info);
                MongoDbHelper.ReplaceOne(m.ExamID, exam, DbName.E_Info);
                return ResultHelper.OK();
            }
        }
        /// <summary>
        /// 清除学生
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ClearStudent([FromBody]ExamID m)
        {
            var exam = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);//找到这次考试
            if (exam == null)
            {
                return ResultHelper.Failed("未找到该次考试");
            }
            if (exam.set < Function.ConvertDateI(DateTime.Now))
            {
                return ResultHelper.Failed("已经导入结束，不能清空学生");
            }
            var filter = new BsonDocument();
            filter.Add("eid", exam._id);
            MongoDbHelper.DeleteByBson<Pp_Nm>(filter, DbName.Pp_Nm);
            MongoDbHelper.DeleteByBson<St_Info>(filter, DbName.St_Info);
            exam.sbs.ForEach(f => f.stct = 0);
            MongoDbHelper.ReplaceOne(m.ID, exam, DbName.E_Info);
            return ResultHelper.OK();
        }
        #endregion
    }
}
