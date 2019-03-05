using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 查询老师
    /// </summary>
    public class SelTchInfo
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
        /// 科目
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
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
    /// 导出老师
    /// </summary>
    public class ExportTchInfo
    {
        /// <summary>
        /// 学校
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
    }
}