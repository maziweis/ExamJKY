using MongoDB.Bson;

namespace Model
{
    /// <summary>
    /// 学校信息表
    /// </summary>
    public class U_Info
    {
        public string _id { get; set; }
        public string snm { get; set; }
        public string sfnm { get; set; }
        public string pnm { get; set; }
        public string ph { get; set; }
        public string em { get; set; }
        public string qq { get; set; }
        public string pwd { get; set; }
        public int tp { get; set; }
        public int area { get; set; }
    }

}
