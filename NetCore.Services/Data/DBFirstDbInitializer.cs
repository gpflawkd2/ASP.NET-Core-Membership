using NetCore.Data.Classes;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCore.Services.Data
{
    public class DBFirstDbInitializer
    {
        private DBFirstDbContext _context;
        private IPasswordHasher _hasher;

        public DBFirstDbInitializer(DBFirstDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        /// <summary>
        /// 초기 데이터 설정
        /// </summary>
        public void PlantSeedData()
        {
            string userId = "tester";
            string password = "123456";
            var utcNow = DateTime.UtcNow;

            var passwordInfo = _hasher.SetPasswordInfo(userId, password);

            _context.Database.EnsureCreated();

            if(!_context.Users.Any())
            {
                var users = new List<User>()
                {
                    new User()
                    {
                        UserId = userId.ToLower(),
                        UserName = "Seed 사용자",
                        UserEmail = "tester@gmail.com",
                        GUIDSalt = passwordInfo.GUIDSalt,
                        RNGSalt = passwordInfo.RNGSalt,
                        PasswordHash = passwordInfo.PasswordHash,
                        AccessFailedCount = 0,
                        IsMembershipWithdrawn = false,
                        JoinUtcDate = utcNow
                    }
                };

                _context.Users.AddRange(users);
                _context.SaveChanges();
            }

            if (!_context.UserRoles.Any())
            {
                var userRoles = new List<UserRole>()
                {
                    new UserRole()
                    {
                        RoleId = "AssociateUser",
                        RoleName = "준사용자",
                        RolePriority = 1,
                        ModifiedUtcData = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "GeneralUser",
                        RoleName = "일반사용자",
                        RolePriority = 2,
                        ModifiedUtcData = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "SuperUser",
                        RoleName = "향상된 사용자",
                        RolePriority = 3,
                        ModifiedUtcData = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "SystemUser",
                        RoleName = "시스템 사용자",
                        RolePriority = 4,
                        ModifiedUtcData = utcNow
                    }
                };

                _context.UserRoles.AddRange(userRoles);
                _context.SaveChanges();

            }

            if (!_context.UserRolesByUsers.Any())
            {
                var userRolesByUser = new List<UserRolesByUser>()
                {
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "GeneralUser",
                        OwnedUtcDate = utcNow
                    },
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SuperUser",
                        OwnedUtcDate = utcNow
                    },
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SystemUser",
                        OwnedUtcDate = utcNow
                    }
                };

                _context.UserRolesByUsers.AddRange(userRolesByUser);
                _context.SaveChanges();
            }

        }
    }
}
