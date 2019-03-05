using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 考试ID和其他ID
    /// </summary>
    public class ExamAndID
    {
        /// <summary>
        /// ExamID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
    }
}