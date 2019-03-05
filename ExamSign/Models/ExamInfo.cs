using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSign.Models
{
    /// <summary>
    /// 学科名称和学生类型
    /// </summary>
    public class ExamInfo
    {
        /// <summary>
        /// 考试ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 发布者ID
        /// </summary>
        public string PublishID { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public long PublishTime { get; set; }
        /// <summary>
        /// 发布者昵称
        /// </summary>
        public string PublishName { get; set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        public string ExamName { get; set; }
        /// <summary>
        /// 学生开始录入时间
        /// </summary>
        public long StuStartTime { get; set; }
        /// <summary>
        /// 学生结束录入时间
        /// </summary>
        public long StuEndTime { get; set; }
        /// <summary>
        /// 老师开始录入时间
        /// </summary>
        public long TchStartTime { get; set; }
        /// <summary>
        /// 老师结束录入时间
        /// </summary>
        public long TchEndTime { get; set; }
        /// <summary>
        /// 学科名称 例： 语文,数学  用逗号分隔
        /// </summary>
        public Dictionary<int, string> subNames { get; set; } = new Dictionary<int, string>();
        /// <summary>
        /// 学生类型 例： 语文,数学  用逗号分隔
        /// </summary>
        public string stuTypes { get; set; }
        /// <summary>
        /// 学生信息是否确认
        /// </summary>
        public int StuConfirm { get; set; }
        /// <summary>
        /// 老师信息是否确认
        /// </summary>
        public int TchConfirm { get; set; }
        /// <summary>
        /// 按钮状态:是否有正在进行的考试 0:有，1:没有
        /// </summary>
        public int ButtonState { get; set; }
        /// <summary>
        /// 新建考试，前台学校是否可以浏览 0不能，1可以
        /// </summary>
        public int IsScan { get; set; }
        /// <summary>
        /// 学生是否开始
        /// </summary>
        public int StuStart { get; set; }
        /// <summary>
        /// 老师是否开始
        /// </summary>
        public int TchStart { get; set; }
        /// <summary>
        /// 学生是否截止
        /// </summary>
        public int StuEnd { get; set; }
        /// <summary>
        /// 老师是否截止
        /// </summary>
        public int TchEnd { get; set; }
    }
}