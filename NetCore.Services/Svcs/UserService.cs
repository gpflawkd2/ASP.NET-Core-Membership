using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
//using NetCore.Data.DataModels;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {

        private DBFirstDbContext _context;
        private IPasswordHasher _hasher;

        public UserService(DBFirstDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        #region private methods

        /// <summary>
        /// 사용자 리스트 조회
        /// </summary>
        /// <returns></returns>
        private IEnumerable<User> GetUserInfos()
        {
            return _context.Users.ToList();
            //return new List<User>()
            //{
            //    new User()
            //    {
            //        UserId = "adminss",
            //        UserName ="관리자",
            //        UserEmail = "hyerim.park@barunn.net",
            //        Password = "123456"
            //    }
            //};
        }

        /// <summary>
        /// 로그인을 위한 사용자 정보 조회
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private User GetUserInfo(string userId, string password)
        {
            User user;

            // Lanmda
            //user = _context.Users.Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();

            // ** FromSql **
            // 컬럼 모두를 조회해야 Error가 발생되지 않음

            // TABLE -> Where 조건은 Lanmda식으로 사용해야함
            //user = _context.Users.FromSql("SELECT UserID, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinUtcDate FROM dbo.[User]")
            //                     .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //                     .FirstOrDefault();

            // VIEW -> Where 조건은 Lanmda식으로 사용해야함
            //user = _context.Users.FromSql("SELECT UserID, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinUtcDate FROM dbo.uvwUser")
            //                     .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //                     .FirstOrDefault();

            // FUNCTION
            //user = _context.Users.FromSql($"SELECT UserID, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinUtcDate FROM dbo.ufnUser({userId},{password})").FirstOrDefault();


            // STORED PROCEDURE
            // 문자형태로만 파라미터를 전달해야함
            
            user = _context.Users.FromSql("dbo.uspCheckLoginByUserId @p0, @p1", new[] { userId, password }).FirstOrDefault();

            // cf.숫자인 경우 (-> .ToSting())
            //int count = 1;
            //user = _context.Users.FromSql("dbo.uspCheckLoginByUserId @p0, @p1, @p2", new[] { userId, password, count.ToString() }).FirstOrDefault();

            if (user == null)
            {
                // ** ExecuteSqlCommand **
                // int 값을 return
                // Database의 Insert, Update, Delete 작업 후 SELECT 구문을 추가해도 그 값을 return 할 수 없음


                // 접속실패횟수에 대한 증가
                int rowAffected;

                // SQL문 직접 작성
                //rowAffected = _context.Database.ExecuteSqlCommand($"UPDATE dbo.[User] SET AccessFailedCount += 1 WHERE UserId={userId}");

                // STORED PROCEDURE
                rowAffected = _context.Database.ExecuteSqlCommand("dbo.uspFaildLoginByUserId @p0", parameters:new[] { userId });

            }

            return user;
        }

        /// <summary>
        /// 사용자 데이터 유무 체크
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool checkTheUserInfo(string userId, string password)
        {
            // Any() : 리스트 데이터 유무체크(리스트 형식에서만 가능)
            //return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();

            return GetUserInfo(userId, password) != null ? true : false; 
        }


        /// <summary>
        /// 사용자 상세정보 조회
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private User GetUserInfo(string userId)
        {
            return _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault();
        }

        /// <summary>
        /// 로그인한 사용자 권한 정보 조회
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private IEnumerable<UserRolesByUser> GetUserRolesByUserInfos(string userId)
        {
            var userRolesByUserInfos = _context.UserRolesByUsers.Where(uru => uru.UserId.Equals(userId)).ToList();

            foreach(var role in userRolesByUserInfos)
            {
                role.UserRole = GetUserRole(role.RoleId);
            }

            return userRolesByUserInfos.OrderByDescending(urn => urn.UserRole.RolePriority);
        }

        /// <summary>
        /// 권한 상세 정보 조회
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private UserRole GetUserRole(string roleId)
        {
            return _context.UserRoles.Where(ur => ur.RoleId.Equals(roleId)).FirstOrDefault();
        }

        // ToLower() => 아이디 대소문자 처리
        private int RegisterUser(RegisterInfoViewModel register)
        {
            var utcNow = DateTime.UtcNow;
            var passwordInfo = _hasher.SetPasswordInfo(register.UserId, register.Password);

            var user = new User()
            {
                UserId = register.UserId.ToLower(),
                UserName = register.UserName,
                UserEmail = register.UserEmail,
                GUIDSalt = passwordInfo.GUIDSalt,
                RNGSalt = passwordInfo.RNGSalt,
                PasswordHash = passwordInfo.PasswordHash,
                AccessFailedCount = 0,
                IsMembershipWithdrawn = false,
                JoinUtcDate = utcNow
            };

            var userRolesByUser = new UserRolesByUser()
            {
                UserId = register.UserId.ToLower(),
                RoleId = "AssociateUser",
                OwnedUtcDate = utcNow
            };

            _context.Add(user);
            _context.Add(userRolesByUser);

            return _context.SaveChanges();
        }

        private UserInfoViewModel GetUserInfoForUpdate(string userId)
        {
            var user = GetUserInfo(userId);
            var userInfo = new UserInfoViewModel()
            {
                UserId = null,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                ChangeInfo = new ChangeInfoViewModel()
                {
                    UserName = user.UserName,
                    UserEmail = user.UserEmail
                }
            };

            return userInfo;
        }

        private int UpdateUser(UserInfoViewModel user)
        {
            // 파라미터에 class 생성하여 전달시 참고
            //bool check = MatchTheUserInfo(new LoginInfoViewModel() { UserId = user.UserId, Password = user.Password});

            int rowAffected = 0;

            var userInfo = _context.Users.Where(u => u.UserId.Equals(user.UserId)).FirstOrDefault();

            if (userInfo == null)
            {
                return 0;
            }

            bool check = _hasher.CheckThePasswordInfo(user.UserId, user.Password, userInfo.GUIDSalt, userInfo.RNGSalt, userInfo.PasswordHash);

            if (check)
            {
                _context.Update(userInfo);

                // 업데이트 대상 컬럼 설정
                userInfo.UserName = user.UserName;
                userInfo.UserEmail = user.UserEmail;

                rowAffected = _context.SaveChanges();
            }

            return rowAffected;
        }

        private bool CompareInfo(UserInfoViewModel user)
        {
            return user.ChangeInfo.Equals(user);
        }

        private bool MatchTheUserInfo(LoginInfoViewModel login)
        {
            var user = _context.Users.Where(u => u.UserId.Equals(login.UserId)).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            return _hasher.CheckThePasswordInfo(login.UserId, login.Password, user.GUIDSalt, user.RNGSalt, user.PasswordHash);
        }

        #endregion

        // ----------------------------------------------------
        // 명시적 인터페이스 구현
        // ----------------------------------------------------

        bool IUser.MatchTheUserInfo(LoginInfoViewModel login)
        {
            return MatchTheUserInfo(login);
            //return checkTheUserInfo(login.UserId, login.Password);
        }

        User IUser.GetUserInfo(string userId)
        {
            return GetUserInfo(userId);
        }

        public IEnumerable<UserRolesByUser> GetRolesOwnedByUser(string userId)
        {
            return GetUserRolesByUserInfos(userId);
        }

        IEnumerable<UserRolesByUser> IUser.GetRolesOwnedByUser(string userId)
        {
            return GetUserRolesByUserInfos(userId);
        }

        int IUser.RegisterUser(RegisterInfoViewModel register)
        {
            return RegisterUser(register);
        }

        UserInfoViewModel IUser.GetUserInfoForUpdate(string userId)
        {
            return GetUserInfoForUpdate(userId);
        }

        int IUser.UpdateUser(UserInfoViewModel user)
        {
            return UpdateUser(user);
        }

        bool IUser.CompareInfo(UserInfoViewModel user)
        {
            return CompareInfo(user);
        }
    }
}
