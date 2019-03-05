using MongoDB.Bson;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// 试卷增订统计表
    /// </summary>
    public class Pp_Nm
    {
        public ObjectId _id { get; set; }
        public string sid { get; set; }
        public string snm { get; set; }
        public int ct { get; set; }
        public ObjectId eid { get; set; }
        public List<Sbnm> sbnms { get; set; } = new List<Sbnm>();
        public int iss { get; set; }
    }
    /// <summary>
    /// 学科数量
    /// </summary>
    public class Sbnm
    {
        public string sbnm { get; set; }
        public int sct { get; set; }
        public int ac { get; set; }
        public int sbid { get; set; }
    }
}
