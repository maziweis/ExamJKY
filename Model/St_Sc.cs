using MongoDB.Bson;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// 考生成绩表
    /// </summary>
    public class St_Sc
    {
        public ObjectId _id { get; set; }
        public string stid { get; set; }
        public string sid { get; set; }
        public string cls { get; set; }
        public string nm { get; set; }
        public string idcd { get; set; }
        public ObjectId eid { get; set; }
        //public List<Sbsc> sbs { get; set; } = new List<Sbsc>();
        public double sc { get; set; }
        public double s1 { get; set; }
        public double s2 { get; set; }
        public double s3 { get; set; }
        public double s4 { get; set; }
        public double s5 { get; set; }
        public double s6 { get; set; }
        public double s7 { get; set; }
        public double s8 { get; set; }
        public double s9 { get; set; }
        public double s10 { get; set; }
    }
    public class Sbsc
    {
        public string sbnm { get; set; }
        public int sbsc { get; set; }
        public int sbid { get; set; }
    }
}
