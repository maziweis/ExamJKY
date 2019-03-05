using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 老师信息
    /// </summary>
    public class TeacherInfo
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
        /// 学科
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 职称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public long Phone { get; set; }
    }
    
}