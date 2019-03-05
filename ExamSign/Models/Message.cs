using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 发通知
    /// </summary>
    public class CreateMsg
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 发布人ID
        /// </summary>
        public string PublishID { get; set; }
        /// <summary>
        /// 发布者姓名(创建消息时不用传)
        /// </summary>
        public string PublishName { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string  Content { get; set; }
        /// <summary>
        /// 附件地址:文件名,地址
        /// </summary>
        public List<Model.Attach> Urls { get; set; }
        /// <summary>
        /// 是否已读
        /// </summary>
        public int IsRead { get; set; }
    }
    
    /// <summary>
    /// 教科院消息
    /// </summary>
    public class GetJKYMsg
    {
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
    /// 学校消息
    /// </summary>
    public class GetSchMsg
    {
        /// <summary>
        /// 学校ID
        /// </summary>
        public string SchoolID { get; set; }
        /// <summary>
        /// 查询条数
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// 跳过条数
        /// </summary>
        public int Skip { get; set; }
    }
}