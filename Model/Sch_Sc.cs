using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Sch_Sc
    {
        public ObjectId _id { get; set; }
        public ObjectId eid { get; set; }
        public string sid { get; set; }
        public string snm { get; set; }
        public int area { get; set; }
        /// <summary>
        /// 0是未发送，1是已发送
        /// </summary>
        public int state { get; set; }
    }
}
