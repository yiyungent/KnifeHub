// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DataAnalysisPlugin.Domain.Entities.SimpleTimeTracker;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysisPlugin.Repositories.EFCore.Core
{
    public class EFCoreDbContext : DbContext
    {
        #region Ctor
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options)
        {
            // 让Entity Framework启动不再效验 __MigrationHistory 表
            // 发现每次效验/查询，都要去创建 __MigrationHistory 表，而 此表 的 ContextKey字段varchar(300) 超过限制导致
            // 解决：Specified key was too long; max key length is 767 bytes
            //Database.SetInitializer<EFCoreDbContext>(null);

            //this.Configuration.AutoDetectChangesEnabled = true;//对多对多，一对多进行curd操作时需要为true

            ////this.Configuration.LazyLoadingEnabled = false;

            //// 记录 EF 生成的 SQL
            //Database.Log = (str) =>
            //{
            //    System.Diagnostics.Debug.WriteLine(str);
            //};
        }
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // EF Core 默认行为
            // 创建数据库时，EF 会创建名称与 `DbSet` 属性名称相同的表。集合的属性名称通常为复数形式。
            // 例如， `Students` 而不是 `Student` .开发人员对表名是否应该复数存在分歧。
            // PS: DbSet 属性名称不用复数, 也就不会创建复数名称的表,
            // 若 DbSet 属性名称为复数: public DbSet<Student> Students { get; set; }
            // 则可用此方式覆盖: modelBuilder.Entity<Student>().ToTable("Student");

            // 表名不会自动转换为复数
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //设置多对多的关系 .Map()配置用于存储关系的外键列和表。
            /*
             Employees  HasMany此实体类型配置一对多关系。对应Orders实体               
            WithMany   将关系配置为 many:many，且在关系的另一端有导航属性。
             * MapLeftKey 配置左外键的列名。左外键指向在 HasMany 调用中指定的导航属性的父实体。
             * MapRightKey 配置右外键的列名。右外键指向在 WithMany 调用中指定的导航属性的父实体。
             */
            // https://www.cnblogs.com/wer-ltm/p/4944745.html
            //this.HasMany(x => x.Orders).
            //    WithMany(x => x.InvolvedEmployees).
            //    Map(m => m.ToTable("EmployeeOrder").
            //        MapLeftKey("EmployeeId").
            //        MapRightKey("OrderId"));

            // 两组一对多，形成多对多，并在第三张关系表中附加字段

            #region 参考
            //// 两组一对多，形成多对多，并在第三张关系表中附加字段
            //modelBuilder.Entity<RoleInfo>()
            //    .HasMany(m => m.Role_Users)
            //    //.WithRequired(m => m.RoleInfo)
            //    .WithOne(m => m.RoleInfo)
            //    .HasForeignKey(m => m.RoleInfoId);
            //modelBuilder.Entity<UserInfo>()
            //    .HasMany(m => m.Role_Users)
            //    //.WithRequired(m => m.UserInfo)
            //    .WithOne(m => m.UserInfo)
            //    .HasForeignKey(m => m.UserInfoId);

            //modelBuilder.Entity<RoleInfo>()
            //    .HasMany(m => m.Role_Functions)
            //    //.WithRequired(m => m.RoleInfo)
            //    .WithOne(m => m.RoleInfo)
            //    .HasForeignKey(m => m.RoleInfoId);
            //modelBuilder.Entity<PermissionInfo>()
            //    .HasMany(m => m.Role_Permissions)
            //    //.WithRequired(m => m.FunctionInfo)
            //    .WithOne(m => m.PermissionInfo)
            //    .HasForeignKey(m => m.PermissionInfoId);

            //modelBuilder.Entity<RoleInfo>()
            //    .HasMany(m => m.Role_Menus)
            //    //.WithRequired(m => m.RoleInfo)
            //    .WithOne(m => m.RoleInfo)
            //    .HasForeignKey(m => m.RoleInfoId);
            //modelBuilder.Entity<Sys_Menu>()
            //    .HasMany(m => m.Role_Menus)
            //    //.WithRequired(m => m.Sys_Menu)
            //    .WithOne(m => m.Sys_Menu)
            //    .HasForeignKey(m => m.Sys_MenuId);


            //modelBuilder.Entity<Sys_Menu>()
            //    .HasMany(m => m.Children)
            //    //.WithOptional(m => m.Parent)
            //    .WithOne(m => m.Parent)
            //    .HasForeignKey(m => m.ParentId);

            //modelBuilder.Entity<Comment>()
            //    .HasMany(m => m.Children)
            //    //.WithOptional(m => m.Parent)
            //    .WithOne(m => m.Parent)
            //    .HasForeignKey(m => m.ParentId); 
            #endregion

            // 其它普通设置
        }
        #endregion

        #region Tables

        public virtual DbSet<SimpleTimeTrackerCsvEntity> SimpleTimeTrackerCsv { get; set; }

        #endregion
    }
}
