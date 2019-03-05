using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 实际参考人数
    /// </summary>
    public class ExamStuNum
    {
        /// <summary>
        /// 学校ID
        /// </summary>
        public string SchoolID { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 学科信息
        /// </summary>
        public List<SubNum> Subs { get; set; } = new List<SubNum>();
        /// <summary>
        /// 保存1，提交2，默认0
        /// </summary>

        public int IsSure { get; set; }
    }
}