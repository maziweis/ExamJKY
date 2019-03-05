using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// 考试名称表
    /// </summary>
    public class E_Info
    {
        public ObjectId _id { get; set; }
        public string enm { get; set; }
        public string ppid { get; set; }
        public long pt { get; set; }
        public long sst { get; set; }
        public long set { get; set; }
        public long tst { get; set; }
        public long tet { get; set; }
        public List<Sb> sbs { get; set; } = new List<Sb>();
        public List<Stp> stps { get; set; } = new List<Stp>();
        public int stcfm { get; set; }
        public int tchcfm { get; set; }
        public int btstate { get; set; }    
        public int IsDel { get; set; }    

    }
    /// <summary>
    /// 考试科目
    /// </summary>
    public class Sb
    {
        public int _id { get; set; }

        public string sbnm { get; set; }
        public int stct { get; set; }
        public int tchct { get; set; }
    }
    /// <summary>
    /// 考生类型
    /// </summary>
    public class Stp
    {
        public ObjectId _id { get; set; }

        public string tp { get; set; }
    }
}
