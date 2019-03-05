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
    /// 后台
    /// </summary>
    public class AdminController : ApiController
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object _locker = new object();

        #region 登录注册
        /// <summary>
        /// 转移数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage TransUser()
        {
            using (kingsundbNewEntities db = new kingsundbNewEntities())
            {
                var sUer = db.tb_J_School.Where(w => 1 == 1).ToList();
                List<U_Info> ls = new List<U_Info>();
                foreach (var item in sUer)
                {
                    U_Info sc = new U_Info();
                    sc._id = item.SchoolID;
                    sc.em = item.Email;
                    sc.ph = item.Tel;
                    sc.pnm = item.PersonName;
                    sc.pwd = item.Pwd;
                    sc.qq = item.QQ;
                    sc.sfnm = item.SchoolFullName;
                    sc.snm = item.SchoolName;
                    sc.tp = 1;
                    sc.area = 1;
                    ls.Add(sc);
                }
                MongoDbHelper.Insert(ls, DbName.U_Info);
            }
            return ResultHelper.OK();
        }
        /// <summary>
        /// 注册管理员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SignAdmin([FromBody] AdminUser m)
        {
            if (m == null || m.Account == "")
            {
                return ResultHelper.Failed("账号不能为空");
            }
            var u = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.Account);
            if (u != null)
            {
                return ResultHelper.Failed("该用户已存在");
            }
            U_Info j = new U_Info()
            {
                pnm = m.Name,
                pwd = SecurityHelper.EncryptString("666666"),
                _id = m.Account,
                tp = 0
            };
            MongoDbHelper.Insert(j, DbName.U_Info);
            return ResultHelper.OK();
        }

        /// <summary>
        /// 注册学校
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SignSchool([FromBody] User m)
        {
            var sc = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.ID);
            if (sc != null)
            {
                return ResultHelper.Failed("该用户已存在");
            }
            U_Info s = new U_Info()
            {
                _id = m.ID,
                em = m.Email,
                ph = m.Tel,
                pnm = m.PersonName,
                pwd = SecurityHelper.EncryptString("666666"),
                qq = m.QQ,
                sfnm = m.SchoolFullName,
                snm = m.SchoolName,
                tp = 1,
                area = m.AreaID
            };
            MongoDbHelper.Insert(s, DbName.U_Info);
            var exam = MongoDbHelper.QueryOne<E_Info>(DbName.E_Info, w => w.btstate == 0&&w.IsDel==0);
            if (exam != null)
            {
                Sch_Sc sc1 = new Sch_Sc();
                sc1.eid = exam._id;
                sc1.sid = s._id;
                sc1.state = 0;
                sc1.snm = s.snm;
                sc1.area = s.area;
                MongoDbHelper.Insert(sc1, DbName.Sch_Sc);
            }
            return ResultHelper.OK();
        }
        /// <summary>
        /// 注册教研员账号
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SignArea([FromBody] Area m)
        {
            var sc = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.ID);
            if (sc != null)
            {
                return ResultHelper.Failed("该用户已存在");
            }
            U_Info s = new U_Info()
            {
                _id = m.ID,
                em = m.Email,
                ph = m.Tel,
                pnm = m.Name,
                pwd = SecurityHelper.EncryptString("666666"),
                qq = m.QQ,
                tp = 2,
                area = m.AreaID
            };
            MongoDbHelper.Insert(s, DbName.U_Info);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DelUser([FromBody]ExamID m)
        {
            var sch = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.ID);
            if (sch != null)
            {
                var filter = new BsonDocument();
                filter.Add("_id", m.ID);
                MongoDbHelper.DeleteByBson<U_Info>(filter, DbName.U_Info);
                return ResultHelper.OK();
            }
            return ResultHelper.Failed("未找到该学校");
        }
        /// <summary>
        /// 导出学校
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportSchool([FromBody] GetSchool m)
        {
            List<U_Info> sts = new List<U_Info>();
            var filter = new BsonDocument();
            ObjectId objectid = new ObjectId();
            if (ObjectId.TryParse(m.ExamID, out objectid))
            {
                filter.Add("eid", objectid);
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
                if (m.Area != 0)
                {
                    filter.Add("area", m.Area);
                }
                if (m.IsSend != 0)
                {
                    filter.Add("state", m.IsSend);
                }
                var schsc1 = MongoDbHelper.GetPagedList1<Sch_Sc, string>(DbName.Sch_Sc, 0, 10000, filter, w => w._id).ToList();
                var schsc = schsc1.Select(s => s.sid).ToList();
                sts = MongoDbHelper.GetPagedList<U_Info, string>(DbName.U_Info, 0, 10000, w => schsc.Contains(w._id), o => o._id);
            }
            else
            {
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
                sts = MongoDbHelper.GetPagedList1<U_Info, string>(DbName.U_Info, 0, 10000, filter, w => w._id).ToList();
            }
            //var sts = MongoDbHelper.QueryBy<U_Info>(DbName.U_Info, w => w.tp == 1);
            List<string> data = ExcelBLL.GetSchColumn();
            DataTable dt = new DataTable();
            for (int j = 0; j < data.Count; j++)
            {
                dt.Columns.Add(data[j]);
            }
            for (int i = 0; i < sts.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = sts[i]._id;
                dr[1] = sts[i].snm;
                dr[2] = Dict.Area.GetVal(sts[i].area);
                dr[3] = sts[i].pnm;
                dr[4] = sts[i].ph;
                dr[5] = sts[i].em;
                dr[6] = sts[i].qq;
                dt.Rows.Add(dr);
            }
            string file = ExcelBLL.BuildExcel1(data.ToArray(), dt);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }
        /// <summary>
        /// 获取区域教研员
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetArea([FromBody]GetJKYMsg m)
        {
            var filter = new BsonDocument();
            filter.Add("tp", 2);

            int Count = MongoDbHelper.GetCount<U_Info>(DbName.U_Info, filter);
            if (Count == 0)
            {
                return ResultHelper.OK("");
            }
            var schs = MongoDbHelper.GetPagedList2<U_Info, string>(DbName.U_Info, m.Skip, m.Limit, filter, w => w._id);
            List<User> lu = new List<Models.User>();
            for (int i = 0; i < schs.Count; i++)
            {
                Models.User su = new Models.User()
                {
                    ID = schs[i]._id,
                    Email = schs[i].em,
                    PersonName = schs[i].pnm,
                    QQ = schs[i].qq,
                    SchoolName = schs[i].snm,
                    Tel = schs[i].ph,
                    AreaID = schs[i].area,
                    Area = Dict.Area.GetVal(schs[i].area)
                };
                lu.Add(su);
            }
            return ResultHelper.OK(new { Data = lu, Count = Count });
        }
        /// <summary>
        /// 导出区域
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ExportArea()
        {
            var sts = MongoDbHelper.QueryBy<U_Info>(DbName.U_Info, w => w.tp == 2);
            List<string> data = ExcelBLL.GetAreaColumn();
            DataTable dt = new DataTable();
            for (int j = 0; j < data.Count; j++)
            {
                dt.Columns.Add(data[j]);
            }
            for (int i = 0; i < sts.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = sts[i]._id;
                dr[1] = Dict.Area.GetVal(sts[i].area);
                dr[2] = sts[i].pnm;
                dr[3] = sts[i].ph;
                dr[4] = sts[i].em;
                dr[5] = sts[i].qq;
                dt.Rows.Add(dr);
            }
            string file = ExcelBLL.BuildExcel1(data.ToArray(), dt);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + file;
            return ResultHelper.OK(url);
        }
        /// <summary>
        /// 编辑区域教研员信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditArea([FromBody] User m)
        {
            var user = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.ID);
            if (user == null)
            {
                return ResultHelper.Failed("未找到该用户信息");
            }
            user.pnm = m.PersonName;
            user.ph = m.Tel;
            user.em = m.Email;
            user.qq = m.QQ;
            user.area = m.AreaID;
            MongoDbHelper.ReplaceOne1<U_Info>(m.ID, user, DbName.U_Info);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 用户登录 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Login([FromBody] UserLogin m)
        {
            var info = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.Account);
            if (info != null)
            {
                if (m.Password == CommonHelper.SecurityHelper.DecryptString(info.pwd))
                {
                    Models.User su = new Models.User()
                    {
                        ID = info._id,
                        Email = info.em,
                        PersonName = info.pnm,
                        QQ = info.qq,
                        SchoolFullName = info.sfnm,
                        SchoolName = info.snm,
                        Tel = info.ph,
                        AreaID = info.area,
                        Area = info.tp == 0 ? null : Dict.Area.GetVal(info.area),
                        Type = info.tp,
                        SchoolID = info.tp != 1 ? null : info._id
                    };
                    return ResultHelper.OK(su);
                }
                else
                {
                    return ResultHelper.Failed("密码不正确");
                }
            }
            else
            {
                return ResultHelper.Failed("用户不存在");
            }
        }

        /// <summary>
        /// 修改学校信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditSchoolUser([FromBody] User m)
        {
            var user = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.ID);
            if (user == null)
            {
                return ResultHelper.Failed("未找到该用户信息");
            }
            user.snm = m.SchoolName;
            user.pnm = m.PersonName;
            user.ph = m.Tel;
            user.em = m.Email;
            user.qq = m.QQ;
            user.area = m.AreaID;
            MongoDbHelper.ReplaceOne1<U_Info>(m.ID, user, DbName.U_Info);
            return ResultHelper.OK();
        }

        /// <summary>
        /// 修改管理员信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditAdmin([FromBody] AdminUser m)
        {
            if (m == null || m.Account == "")
            {
                return ResultHelper.Failed("账号不能为空");
            }
            var u = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.Account);
            if (u == null)
            {
                return ResultHelper.Failed("该用户不存在");
            }
            u.pnm = m.Name;
            MongoDbHelper.ReplaceOne1<U_Info>(m.Account, u, DbName.U_Info);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ChangePwd([FromBody] ChangePwd m)
        {
            var info = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.Account);
            if (info != null)
            {
                if (m.OldPwd == SecurityHelper.DecryptString(info.pwd))
                {
                    info.pwd = SecurityHelper.EncryptString(m.NewPwd);
                    MongoDbHelper.ReplaceOne1<U_Info>(m.Account, info, DbName.U_Info);
                    return ResultHelper.OK();
                }
                else
                {
                    return ResultHelper.Failed("密码不正确");
                }
            }
            else
            {
                return ResultHelper.Failed("用户不存在");
            }
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ReSetPwd([FromBody] ReSetPwd m)
        {
            var info = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == m.Account);
            if (info != null)
            {
                info.pwd = SecurityHelper.EncryptString("666666");
                MongoDbHelper.ReplaceOne1<U_Info>(m.Account, info, DbName.U_Info);
                return ResultHelper.OK();
            }
            else
            {
                return ResultHelper.Failed("用户不存在");
            }
        }
        #endregion

        /// <summary>
        /// ftp
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage FTP()
        {
            FtpHelper.Download("20181119113849.xls");
            return ResultHelper.OK();
        }
        /// <summary>
        /// 在9位的考号前面补0
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Update0([FromBody] ExamID m)
        {
            ObjectId objectid = new ObjectId();
            if(ObjectId.TryParse(m.ID, out objectid))
            {
                var stus = MongoDbHelper.QueryBy<St_Info>(DbName.St_Info, w => w.eid == objectid);
                for (int i = 0; i < stus.Count; i++)
                {
                    stus[i].stid= stus[i].stid.PadLeft(10, '0');
                    MongoDbHelper.ReplaceOne(stus[i]._id.ToString(), stus[i], DbName.St_Info);
                }
                return ResultHelper.OK();
            }
            return ResultHelper.Failed("考试ID不对");
        }
        /// <summary>
        /// 根据token获取信息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetInfoByToken([FromBody] HYTAuth m)
        {
            if (!string.IsNullOrEmpty(m.token))
            {
                AuthInfo info = new AuthInfo();
                info.account = "admin";
                info.area = "深圳市";
                info.examName = "深圳市2019年普通高中高三年级第一次调研考试考生报名";
                info.role = "市教研员全科";
                info.school = "";
                info.subject = "";
                return ResultHelper.OK(info);
            }
            else
            {
                return ResultHelper.Failed("token无效");
            }
        }
    }
}
