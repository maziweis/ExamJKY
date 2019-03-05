using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 学校通知公告
    /// </summary>
    public class Msg_S
    {
        public ObjectId _id { get; set; }
        public string sid { get; set; }
        public string pt { get; set; }
        public ObjectId pid { get; set; }
        public int ir { get; set; }

    }

}
