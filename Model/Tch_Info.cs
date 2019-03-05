using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Tch_Info
    {
        public ObjectId _id { get; set; }
        public string sid { get; set; }
        public string snm { get; set; }
        public string sb { get; set; }
        public string nm { get; set; }
        public int sx { get; set; }
        public string zc { get; set; }
        public ObjectId eid { get; set; }
        public long ph { get; set; }
    }
}
