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
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;

namespace ExamSign.Controllers
{
    /// <summary>
    /// 成绩
    /// </summary>
    public class ScoreController : ApiController
    {
        /// <summary>
        /// 查询成绩
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SelScore([FromBody]SelScore m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
            var filter = new BsonDocument();
            if (eInfo != null)
            {
                if (eInfo.btstate == 0)
                {
                    return ResultHelper.OK("");
                }
                if (m.Type == 1)
                {
                    var sch = MongoDbHelper.QueryOne<Sch_Sc>(DbName.Sch_Sc, w => w.eid == eInfo._id && w.sid == m.SchoolID);
                    if (sch != null && sch.state == 0)
                    {
                        return ResultHelper.OK("");
                    }
                    if(sch==null)
                    return ResultHelper.OK("未找到该学校");
                }
                filter.Add("eid", eInfo._id);
                if (!string.IsNullOrEmpty(m.StudentID))
                {
                    filter.Add("stid", m.StudentID);
                }
                if (!string.IsNullOrEmpty(m.SchoolID))
                {
                    filter.Add("sid", m.SchoolID);
                }
                if (!string.IsNullOrEmpty(m.Name))
                {
                    filter.Add("nm", m.Name);
                }
                List<St_Sc> data;
                Expression<Func<St_Sc, object>> orderBy = w => w.sc;
                if (m.OrderBy == 1)
                {
                    orderBy = w => w.s1;
                }
                if (m.OrderBy == 2)
                {
                    orderBy = w => w.s2;
                }
                if (m.OrderBy == 3)
                {
                    orderBy = w => w.s3;
                }
                if (m.OrderBy == 4)
                {
                    orderBy = w => w.s4;
                }
                if (m.OrderBy == 5)
                {
                    orderBy = w => w.s5;
                }
                if (m.OrderBy == 6)
                {
                    orderBy = w => w.s6;
                }
                if (m.OrderBy == 7)
                {
                    orderBy = w => w.s7;
                }
                if (m.OrderBy == 8)
                {
                    orderBy = w => w.s8;
                }
                if (m.OrderBy == 9)
                {
                    orderBy = w => w.s9;
                }
                if (m.OrderBy == 10)
                {
                    orderBy = w => w.s10;
                }
                if (m.Sort)
                {
                    data = MongoDbHelper.GetPagedList1<St_Sc, string>(DbName.St_Sc, m.Skip, m.Limit, filter, orderBy);
                }
                else
                {
                    data = MongoDbHelper.GetPagedList2<St_Sc, string>(DbName.St_Sc, m.Skip, m.Limit, filter, orderBy);
                }
                int Count = MongoDbHelper.GetCount<St_Sc>(DbName.St_Sc, filter);
                if (Count == 0)
                {
                    return ResultHelper.OK("");
                }
                var exam = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
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
                    s.SchoolName = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == data[i].sid).snm;
                    s.StudentID = data[i].stid;
                    s.Score = data[i].sc;
                    s.Chinese = data[i].s1;
                    s.Math = data[i].s2;
                    s.Math1 = data[i].s3;
                    s.English = data[i].s4;
                    s.Physical = data[i].s5;
                    s.Chemical = data[i].s6;
                    s.Biological = data[i].s7;
                    s.Geographic = data[i].s8;
                    s.History = data[i].s9;
                    s.Political = data[i].s10;
                    lst.Add(s);
                }
                return ResultHelper.OK(new { Data = lst, Count = Count });
            }
            return ResultHelper.Failed("未找到该次考试信息");
        }
        /// <summary>
        /// 导出成绩
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportScore([FromBody]ExportScore m)
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
                filter.Add("sid", m.School);
            }
            if (!string.IsNullOrEmpty(m.StudentID))
            {
                filter.Add("stid", m.StudentID);
            }
            if (!string.IsNullOrEmpty(m.Name))
            {
                filter.Add("nm", m.Name);
            }
            var stSc = MongoDbHelper.GetPagedList1<St_Sc, string>(DbName.St_Sc, 0, 0, filter, w => w.nm);
            List<string> data = ExcelBLL.GetScoreColumn(eInfo);
            DataTable dt = new DataTable();
            for (int j = 0; j < data.Count; j++)
            {
                dt.Columns.Add(data[j]);
            }
            Type t = typeof(St_Sc);
            List<PropertyInfo> p = t.GetProperties().ToList();
            Dictionary<string, PropertyInfo> dic = new Dictionary<string, PropertyInfo>();
            List<string> sbs = eInfo.sbs.Select(w => "s" + w._id).ToList();
            p.ForEach(w =>
            {
                if (sbs.Contains(w.Name))
                {
                    dic.Add(w.Name, w);
                }
            });
            for (int i = 0; i < stSc.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = stSc[i].stid;
                dr[1] = stSc[i].cls;
                dr[2] = stSc[i].nm;
                dr[3] = stSc[i].idcd;
                for (int j = 0; j < sbs.Count; j++)
                {
                    PropertyInfo pro = dic[sbs[j]];
                    string value = pro.GetValue(stSc[i], null).ToString();
                    dr[4 + j] = value=="-1"?"-":value;
                }
                dr[4 + sbs.Count] = stSc[i].sc;
                dt.Rows.Add(dr);
            }
            string file = ExcelBLL.BuildExcel1(data.ToArray(), dt);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }
        /// <summary>
        /// 获取学校信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetSchool([FromBody]GetSchool m)
        {
            List<U_Info> schs = new List<U_Info>(); int Count = 0;
            var filter = new BsonDocument();
            List<User> lu = new List<Models.User>();
            ObjectId objectid = new ObjectId();
            ObjectId.TryParse(m.ExamID, out objectid);
            List<Sch_Sc> schsc1 = null;
            
            if (m.IsSend == 1)
            {
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
                filter.Add("eid", objectid);
                filter.Add("state", 1);
                Count = MongoDbHelper.GetCount<Sch_Sc>(DbName.Sch_Sc, filter);
                if (Count == 0)
                {
                    return ResultHelper.OK(new List<string>());
                }
                schsc1 = MongoDbHelper.GetPagedList1<Sch_Sc, string>(DbName.Sch_Sc, m.Skip, m.Limit, filter, w => w._id).ToList();
                var schsc = schsc1.Select(s => s.sid).ToList();
                schs = MongoDbHelper.GetPagedList<U_Info, string>(DbName.U_Info, 0, m.Limit, w => schsc.Contains(w._id), o => o._id);

            }
            if (m.IsSend == 2)
            {
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
                filter.Add("eid", objectid);
                filter.Add("state", 0);
                Count = MongoDbHelper.GetCount<Sch_Sc>(DbName.Sch_Sc, filter);
                if (Count == 0)
                {
                    return ResultHelper.OK(new List<string>());
                }
                schsc1 = MongoDbHelper.GetPagedList1<Sch_Sc, string>(DbName.Sch_Sc, m.Skip, m.Limit, filter, w => w._id).ToList();
                var schsc = schsc1.Select(s => s.sid).ToList();
                schs = MongoDbHelper.GetPagedList<U_Info, string>(DbName.U_Info, 0, m.Limit, w => schsc.Contains(w._id), o => o._id);
            }
            if (m.IsSend == 0)
            {
                schsc1 = MongoDbHelper.QueryBy<Sch_Sc>(DbName.Sch_Sc, w => w.eid == objectid);
                filter.Add("tp", 1);
                if (!string.IsNullOrEmpty(m.School))
                {
                    int id = -1;
                    if (int.TryParse(m.School, out id))
                    {
                        filter.Add("_id", m.School);
                    }
                    else
                    {
                        filter.Add("snm", m.School);
                    }
                }
                if (m.Area != 0)
                {
                    filter.Add("area", m.Area);
                }
                Count = MongoDbHelper.GetCount<U_Info>(DbName.U_Info, filter);
                if (Count == 0)
                {
                    return ResultHelper.OK(new List<string>());
                }
                schs = MongoDbHelper.GetPagedList1<U_Info, string>(DbName.U_Info, m.Skip, m.Limit, filter, w => w._id);
            }
            lu = schs.Join(schsc1, u => u._id, o => o.sid, (u, o) => new Models.User
            {
                ID = u._id,
                Email = u.em,
                PersonName = u.pnm,
                QQ = u.qq,
                SchoolFullName = u.sfnm,
                SchoolName = u.snm,
                Tel = u.ph,
                AreaID= u.area,
                Area = Dict.Area.GetVal(u.area),
                Type = o.state
            }).ToList();
            //for (int i = 0; i < schs.Count; i++)
            //{
            //    Models.User su = new Models.User()
            //    {
            //        ID = schs[i]._id,
            //        Email = schs[i].em,
            //        PersonName = schs[i].pnm,
            //        QQ = schs[i].qq,
            //        SchoolFullName = schs[i].sfnm,
            //        SchoolName = schs[i].snm,
            //        Tel = schs[i].ph,
            //        Area = Dict.Area.GetVal(schs[i].area),
            //         Type= m.IsSend
            //    };
            //    lu.Add(su);
            //}
            return ResultHelper.OK(new { Data = lu, Count = Count });
        }
        /// <summary>
        /// 学校管理 查询所有学校
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetAllSchool([FromBody]GetSchool m)
        {
            List<U_Info> schs = new List<U_Info>();
            int Count = 0;
            var filter = new BsonDocument();
            filter.Add("tp", 1);
            if (!string.IsNullOrEmpty(m.School))
            {
                int id = -1;
                if (int.TryParse(m.School, out id))
                {
                    filter.Add("_id", m.School);
                }
                else
                {
                    filter.Add("snm", m.School);
                }
            }
            Count = MongoDbHelper.GetCount<U_Info>(DbName.U_Info, filter);
            schs = MongoDbHelper.GetPagedList1<U_Info, string>(DbName.U_Info, m.Skip, m.Limit, filter, w => w._id);
            List<User> lu = new List<Models.User>();
            lu = schs.Select(u => new Models.User
            {
                ID = u._id,
                Email = u.em,
                PersonName = u.pnm,
                QQ = u.qq,
                SchoolFullName = u.sfnm,
                SchoolName = u.snm,
                Tel = u.ph,
                AreaID= u.area,
                Area = Dict.Area.GetVal(u.area)
            }).ToList();           
            return ResultHelper.OK(new { Data = lu, Count = Count });
        }
        /// <summary>
        /// 发送成绩
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SendScore([FromBody]SendScore m)
        {
            var eInfo = MongoDbHelper.FindOne<E_Info>(m.ExamID, DbName.E_Info);
            if (eInfo == null)
            {
                return ResultHelper.Failed("未查到当前考试");
            }
            if (m.School.ToLower() == "all")
            {
                var allsch = MongoDbHelper.QueryBy<Sch_Sc>(DbName.Sch_Sc, w => w.eid == eInfo._id);
                for (int i = 0; i < allsch.Count; i++)
                {
                    allsch[i].state = 1;
                    MongoDbHelper.ReplaceOne(allsch[i]._id.ToString(), allsch[i], DbName.Sch_Sc);
                }
                return ResultHelper.OK();
            }
            var sch = MongoDbHelper.QueryOne<Sch_Sc>(DbName.Sch_Sc, w => w.eid == eInfo._id && w.sid == m.School);
            if (sch != null)
            {
                sch.state = 1;
                MongoDbHelper.ReplaceOne(sch._id.ToString(), sch, DbName.Sch_Sc);
                return ResultHelper.OK();
            }
            return ResultHelper.Failed("未找到该学校");
        }
        /// <summary>
        /// 导入分数
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ImportScore([FromBody] ExamID m)
        {
            try
            {
                var exam = MongoDbHelper.FindOne<E_Info>(m.ID, DbName.E_Info);
                if (exam == null)
                {
                    return ResultHelper.Failed("未找到考试信息");
                }
                if (exam.btstate == 1)
                {
                    var exam2 = MongoDbHelper.QueryOne<E_Info>(DbName.E_Info, w => w.btstate == 0 && w.IsDel == 0);
                    if (exam2 != null)
                    {
                        return ResultHelper.Failed("考试已结束，无法再次导入成绩");
                    }
                }
                var filter = new BsonDocument();
                filter.Add("eid", exam._id);

                string path = AppDomain.CurrentDomain.BaseDirectory + "ftp\\";
                string path1 = AppDomain.CurrentDomain.BaseDirectory + "ScoreBackUp\\";
                string zipfile = path + "StudentScore.zip";
                //Function.UnZipFile(zipfile, path); 
                if (!File.Exists(zipfile))
                {
                    return ResultHelper.Failed("暂未发布成绩");
                }
                if (!Function.UnRarOrZip(zipfile, path, null))
                {
                    return ResultHelper.Failed("成绩获取失败，请联系管理员");
                }
                //File.Delete(zipfile);
                string filepath = path + "StudentScore.xls";
                string filepath1 = path1 + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                if (!System.IO.Directory.Exists(path1))
                {
                    // 目录不存在，建立目录
                    System.IO.Directory.CreateDirectory(path1);
                }
                System.IO.File.Copy(filepath, filepath1, true);
                MongoDbHelper.DeleteByBson<St_Sc>(filter, DbName.St_Sc);
                exam.btstate = 1;
                MongoDbHelper.ReplaceOne(exam._id.ToString(), exam, DbName.E_Info);
                int errnum = 0;
                string msg = BLL.ExcelBLL.ImportScoreExcel(filepath, exam, out errnum);
                if (msg != "")
                {
                    string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + msg;
                    return ResultHelper.Failed("导入失败" + errnum + "条--" + url);
                }
                return ResultHelper.OK();
            }
            catch (Exception e)
            {
                return ResultHelper.Failed("导入失败"+e.Message);
            }
        }
        /// <summary>
        /// 导入成绩
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage StudentScore()
        {
            //var exam = MongoDbHelper.QueryOne<E_Info>(DbName.E_Info, w => w.btstate == 0);
            //if (exam == null)
            //{
            //    return ResultHelper.Failed("未找到考试信息");
            //}
            //var filter = new BsonDocument();
            //filter.Add("eid", exam._id);
            //MongoDbHelper.DeleteByBson<St_Sc>(filter, DbName.St_Sc);
            //string path = AppDomain.CurrentDomain.BaseDirectory + "ftp\\";
            //string path1 = AppDomain.CurrentDomain.BaseDirectory + "ScoreBackUp\\";
            //string zipfile = path + "StudentScore.zip";
            ////Function.UnZipFile(zipfile, path); 
            //Function.UnRarOrZip(zipfile, path, null);
            //string filepath = path + "StudentScore.xls";
            //string filepath1 = path1 + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            //if (!System.IO.Directory.Exists(path1))
            //{
            //    // 目录不存在，建立目录
            //    System.IO.Directory.CreateDirectory(path1);
            //}
            //System.IO.File.Copy(filepath, filepath1, true);
            //System.Threading.Tasks.Task.Run(() =>
            //{
            //    BLL.ExcelBLL.ImportScoreExcel(filepath, exam);                         
            //});            
            //exam.btstate = 1;
            //MongoDbHelper.ReplaceOne(exam._id.ToString(), exam, DbName.E_Info);
            return ResultHelper.OK();
        }
    }
}
