using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 试卷数量
    /// </summary>
    public class PaperNum
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
        /// 总数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 学科信息
        /// </summary>
        public List<SubNum> Subs { get; set; } = new List<SubNum>();
        /// <summary>
        /// 是否确认考试人数
        /// </summary>
        public int IsSure { get; set; }
    }
    /// <summary>
    /// 学科信息
    /// </summary>
    public class SubNum
    {
        /// <summary>
        /// 学科名
        /// </summary>
        public string SubName { get; set; }
        /// <summary>
        /// 学科ID
        /// </summary>
        public string SubID { get; set; }
        /// <summary>
        /// 应考数量
        /// </summary>
        public int SubCount { get; set; }
        /// <summary>
        /// 实际数量
        /// </summary>
        public int AcCount { get; set; }
    }
}