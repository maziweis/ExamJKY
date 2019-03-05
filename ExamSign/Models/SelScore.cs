using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 查询成绩
    /// </summary>
    public class SelScore
    {
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 学生ID
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        public string SchoolID { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int OrderBy { get; set; }
        /// <summary>
        /// 顺序逆序 true 顺序 false 逆序
        /// </summary>
        public bool Sort { get; set; }
        /// <summary>
        /// 用户角色 前台传1后台传0
        /// </summary>
        public int Type { get; set; }
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
    /// 导出成绩
    /// </summary>
    public class ExportScore
    {
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 学生ID
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Name { get; set; }
    }
}