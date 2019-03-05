using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 学生信息
    /// </summary>
    public class StudentInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        public string SchoolID { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学生ID
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 学生类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        public double Score { get; set; }
        /// <summary>
        /// 语文
        /// </summary>
        public double Chinese { get; set; }
        /// <summary>
        /// 文科数学
        /// </summary>
        public double Math { get; set; }
        /// <summary>
        /// 理科数学
        /// </summary>
        public double Math1 { get; set; }
        /// <summary>
        /// 英语
        /// </summary>
        public double English { get; set; }
        /// <summary>
        /// 物理
        /// </summary>
        public double Physical { get; set; }
        /// <summary>
        /// 化学
        /// </summary>
        public double Chemical { get; set; }
        /// <summary>
        /// 生物
        /// </summary>
        public double Biological { get; set; }
        /// <summary>
        /// 地理
        /// </summary>
        public double Geographic { get; set; }
        /// <summary>
        /// 历史
        /// </summary>
        public double History { get; set; }
        /// <summary>
        /// 政治
        /// </summary>
        public double Political { get; set; }
        /// <summary>
        /// 学科信息
        /// </summary>
        public List<SubInfo> Subs { get; set; } = new List<SubInfo>();
    }
    /// <summary>
    /// 学科信息
    /// </summary>
    public class SubInfo
    {
        /// <summary>
        /// 学科名
        /// </summary>
        public string SubName { get; set; }
        /// <summary>
        /// 试室号
        /// </summary>
        public string SubRoom { get; set; }
        /// <summary>
        /// 座位号
        /// </summary>
        public string SubSite { get; set; }
        /// <summary>
        /// 老师
        /// </summary>
        public string SubTeacher { get; set; }
        /// <summary>
        /// 学科ID
        /// </summary>
        public string  SubID { get; set; }
        /// <summary>
        /// 学生数量
        /// </summary>
        public int StuCount { get; set; }
        /// <summary>
        /// 老师数量
        /// </summary>
        public int TchCount { get; set; }
    }
}