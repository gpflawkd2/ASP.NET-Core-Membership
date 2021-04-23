using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
//using NetCore.Data.DataModels;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {

        private DBFirstDbContext _context;

        public UserService(DBFirstDbContext context)
        {
            _context = context;
        }

        #region private methods

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

        private User GetUserInfo(string userId, string password)
        {
            User user;

            // Lanmda
            user = _context.Users.Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();

            // ** FromSql **
            // 컬럼 모두를 조회해야 Error가 발생되지 않음

            // TABLE -> Where 조건은 Lanmda식으로 사용해야함
            user = _context.Users.FromSql("SELECT UserID, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinUtcDate FROM dbo.[User]")
                                 .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
                                 .FirstOrDefault();

            // VIEW -> Where 조건은 Lanmda식으로 사용해야함
            user = _context.Users.FromSql("SELECT UserID, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinUtcDate FROM dbo.uvwUser")
                                 .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
                                 .FirstOrDefault();

            // FUNCTION
            user = _context.Users.FromSql($"SELECT UserID, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinUtcDate FROM dbo.ufnUser({userId},{password})").FirstOrDefault();


            // STORED PROCEDURE
            // 문자형태로만 파라미터를 전달해야함
            
            user = _context.Users.FromSql("dbo.uspCheckLoginByUserId @p0, @p1", new[] { userId, password }).FirstOrDefault();

            // cf.숫자인 경우 (-> .ToSting())
            //int count = 1;
            //user = _context.Users.FromSql("dbo.uspCheckLoginByUserId @p0, @p1, @p2", new[] { userId, password, count.ToString() }).FirstOrDefault();

            return user;
        }

        private bool checkTheUserInfo(string userId, string password)
        {
            // Any() : 리스트 데이터 유무체크(리스트 형식에서만 가능)
            //return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();

            return GetUserInfo(userId, password) != null ? true : false; 
        }

        #endregion

        bool IUser.MatchTheUserInfo(LoginInfoViewModel login)
        {
            return checkTheUserInfo(login.UserId, login.Password);
        }
    }
}
