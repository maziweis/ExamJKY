﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace KingsunDB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class kingsundbNewEntities : DbContext
    {
        public kingsundbNewEntities()
            : base("name=kingsundbNewEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tb_J_School> tb_J_School { get; set; }
        public virtual DbSet<tb_J_Teacher> tb_J_Teacher { get; set; }
        public virtual DbSet<tb_J_TestOrder> tb_J_TestOrder { get; set; }
        public virtual DbSet<tb_J_ExamStudent> tb_J_ExamStudent { get; set; }
        public virtual DbSet<tb_J_MessageData> tb_J_MessageData { get; set; }
        public virtual DbSet<tb_J_SchoolCourse> tb_J_SchoolCourse { get; set; }
    }
}
