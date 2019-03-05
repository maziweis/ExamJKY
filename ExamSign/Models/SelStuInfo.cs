using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 查询学生信息
    /// </summary>
    public class SelStuInfo
    {
        /// <summary>
        /// 学校
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public int Area { get; set; }
        /// <summary>
        /// 学生考号
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 查询条数
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// 跳过条数
        /// </summary>
        public int Skip { get; set; }
    }
    /// <summary>
    /// 导出学生
    /// </summary>
    public class ExportStuInfo
    {
        /// <summary>
        /// 学校
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 学生考号
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
    }
}