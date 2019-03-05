using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 用户登录
    /// </summary>
    public class UserLogin
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
    /// <summary>
    /// 修改密码
    /// </summary>
    public class ChangePwd
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 原始密码
        /// </summary>
        public string OldPwd { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPwd { get; set; }
    }
    /// <summary>
    /// 重置密码
    /// </summary>
    public class ReSetPwd
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
    }
    /// <summary>
    /// 学校用户
    /// </summary>
    public class User
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 学校简称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaID { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 联系人名称
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 学校全称
        /// </summary>
        public string SchoolFullName { get; set; }     
        /// <summary>
        /// 类型 admin : 0,学校：1，教研员：2
        /// </summary>
        public int Type { get; set; }
        public string SchoolID { get; set; }
    }
    /// <summary>
    /// 区域
    /// </summary>
    public class Area
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public int AreaID { get; set; }
        /// <summary>
        /// 教研员名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }
    }
    /// <summary>
    /// 教科院用户
    /// </summary>
    public class AdminUser
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        ///// <summary>
        ///// 密码
        ///// </summary>
        //public string Pwd { get; set; }
        ///// <summary>
        ///// 电话
        ///// </summary>
        //public string Phone { get; set; }
    }
    /// <summary>
    /// 获取学校信息
    /// </summary>
    public class GetSchool
    {
        /// <summary>
        /// 是否发送成绩 0全部，1已发送，2未发送
        /// </summary>
        public int IsSend { get; set; }
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public int Area { get; set; }
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
    /// 发送成绩
    /// </summary>
    public class SendScore
    {
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ExamID { get; set; }
        /// <summary>
        /// 学校 传all代表所有，传学校账号代表单个
        /// </summary>
        public string School { get; set; }
    }
}