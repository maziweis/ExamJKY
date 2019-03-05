using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 海云天认证
    /// </summary>
    public class HYTAuth
    {
        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }
    }
    /// <summary>
    /// 认证返回信息
    /// </summary>
    public class AuthInfo
    {
        /// <summary>
        /// 账户
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string role { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        public string school { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string area { get; set; }
        /// <summary>
        /// 学科
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        public string examName { get; set; }
    }
}