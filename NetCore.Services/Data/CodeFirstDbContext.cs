using Microsoft.EntityFrameworkCore;
using NetCore.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Services.Data
{
    // Fluent API

    // 상속
    // CodeFirstDbContext : 자식클래스
    // DbContext : 부모클래스
    public class CodeFirstDbContext : DbContext
    {
        // 생성자 상속
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options) : base(options)
        {
        }

        // DB 테이블
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRolesByUser> UserRolesByUsers { get; set; }


        // 메서드 상속, 부모클래스에서 OnModelCreating 메서드가 virtual 키워드로 지정이 되어 있어야만 자식클래스에서 override 키워드를 통해 상속받을 수 있음
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DB 테이블이름 변경
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable("UserRolesByUser");

            // 복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });

            // 컬럼 기본값 지정
            modelBuilder.Entity<User>(e =>
            {
                e.Property(c => c.IsMembershipWithdrawn).HasDefaultValue(value: false);
            });

            // 인덱스 지정
            modelBuilder.Entity<User>().HasIndex(c => new { c.UserEmail }).IsUnique(unique:true);
        }
    }
}
