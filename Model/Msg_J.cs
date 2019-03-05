using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 教科院通知公告 
    /// </summary>
    public class Msg_J
    {
        public ObjectId _id { get; set; }
        public string tt { get; set; }
        public string pp { get; set; }
        public string pt { get; set; }
        public string ct { get; set; }
        public List<Attach> urls { get; set; }
    }
    public class Attach
    {
        public string nm { get; set; }
        public string url { get; set; }
    }
}
