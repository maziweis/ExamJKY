using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 考生信息表
    /// </summary>
   public class St_Info
    {
        public ObjectId _id { get; set; }
        public string stid { get; set; }
        public string sid { get; set; }
        public string snm { get; set; }
        public string cls { get; set; }
        public string nm { get; set; }
        public List<SubE> subEs { get; set; } = new List<SubE>();
        public string idcd { get; set; }
        public ObjectId eid { get; set; }
        public string tp { get; set; }
    }

    public class SubE
    {
        public string sbnm { get; set; }
        public string sbrm { get; set; }
        public string sbst { get; set; }
        public string sbtch { get; set; }
        public int sbid { get; set; }
    }
}
