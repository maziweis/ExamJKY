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
    /// 通知
    /// </summary>
    public class MessageController : ApiController
    {
        #region 通知
        /// <summary>
        /// 发布通知
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CreateMsg([FromBody] CreateMsg m)
        {
            var users = MongoDbHelper.QueryBy<U_Info>(DbName.U_Info, w => w.tp == 1);
            
            Msg_J j = new Msg_J();
            j._id = ObjectId.GenerateNewId();
            j.tt = m.Title;
            j.pp = m.PublishID;
            j.pt = m.Time;
            j.ct = m.Content;
            j.urls = m.Urls;
            MongoDbHelper.Insert(j, DbName.Msg_J);
            List<Msg_S> ls = new List<Msg_S>();
            for (int i = 0; i < users.Count; i++)
            {
                Msg_S s = new Msg_S();
                s.sid = users[i]._id;
                s.pid = j._id;
                s.ir = 0;
                s.pt = j.pt;
                ls.Add(s);
            }
            MongoDbHelper.Insert(ls, DbName.Msg_S);
            return ResultHelper.OK();

        }
        /// <summary>
        /// 教科院获取所有消息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetJKYMsgs([FromBody] GetJKYMsg m)
        {
            var filter = new BsonDocument();
            int Count = MongoDbHelper.GetCount<Msg_J>(DbName.Msg_J, filter);
            if (Count == 0)
            {
                return ResultHelper.OK(new List<string>());
            }
            var ms = MongoDbHelper.GetPagedList2<Msg_J, string>(DbName.Msg_J, m.Skip, m.Limit, filter, w => w.pt);//分页查找所有消息
            List<CreateMsg> lm = new List<CreateMsg>();
            for (int i = 0; i < ms.Count; i++)
            {
                var u = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == ms[i].pp);//发布者信息
                CreateMsg mg = new CreateMsg();
                mg.ID = ms[i]._id.ToString();
                mg.Title = ms[i].tt;
                mg.PublishName = u.pnm;
                mg.Time = ms[i].pt;
                mg.Content = ms[i].ct;
                mg.Urls = ms[i].urls;
                lm.Add(mg);
            }
            return ResultHelper.OK(new { Data = lm, Count = Count });
        }
        /// <summary>
        /// 获取学校消息
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetSchMsgs([FromBody] GetSchMsg m)
        {
            var filter = new BsonDocument();
            filter.Add("sid", m.SchoolID);
            int Count = MongoDbHelper.GetCount<Msg_S>(DbName.Msg_S, filter);
            if (Count == 0)
            {
                return ResultHelper.OK(new List<string>());
            }
            var ms = MongoDbHelper.GetPagedList2<Msg_S, string>(DbName.Msg_S, m.Skip, m.Limit, filter, w => w.pt);//分页查找所有消息
            List<CreateMsg> lm = new List<CreateMsg>();
            for (int i = 0; i < ms.Count; i++)
            {
                var jm = MongoDbHelper.FindOne<Msg_J>(ms[i].pid.ToString(), DbName.Msg_J);//父消息
                var u = MongoDbHelper.QueryOne<U_Info>(DbName.U_Info, w => w._id == jm.pp);//发布者信息
                CreateMsg mg = new CreateMsg();
                mg.ID = ms[i]._id.ToString();
                mg.Title = jm.tt;
                mg.PublishName = u.pnm;
                mg.Time = ms[i].pt;
                mg.IsRead = ms[i].ir;
                mg.Content = jm.ct;
                mg.Urls = jm.urls;
                lm.Add(mg);
            }
            return ResultHelper.OK(new { Data = lm, Count = Count });
        }
        /// <summary>
        /// 学校消息已读
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ReadMsg([FromBody] ExamID m)
        {
            var mg = MongoDbHelper.FindOne<Msg_S>(m.ID, DbName.Msg_S);
            if (mg == null)
            {
                return ResultHelper.Failed("未找到该消息");
            }
            mg.ir = 1;
            MongoDbHelper.ReplaceOne(m.ID, mg, DbName.Msg_S);
            return ResultHelper.OK();
        }
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UploadFile()
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            DateTime date = DateTime.Now;
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "UploadFiles\\" + date.ToString("yyyy-MM-dd")))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "UploadFiles\\" + date.ToString("yyyy-MM-dd"));
            }
            string path = "UploadFiles\\" + date.ToString("yyyy-MM-dd") + "\\" + date.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
            file.SaveAs(System.AppDomain.CurrentDomain.BaseDirectory + path);
            string url = RequestContext.Url.Request.RequestUri.Authority + "\\" + path;
            return ResultHelper.OK(file.FileName + "," + url);
        }
        #endregion
    }
}
