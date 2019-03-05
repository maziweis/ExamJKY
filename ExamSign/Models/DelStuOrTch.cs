using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 删除学生或老师
    /// </summary>
    public class DelStuOrTch
    {
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 用户ID或者学校ID
        /// </summary>
        public string UserID { get; set; }
    }
}