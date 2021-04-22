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

        private bool checkTheUserInfo(string userId, string password)
        {
            // Any() : 리스트 데이터 유무체크
            return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();
        }

        #endregion

        bool IUser.MatchTheUserInfo(LoginInfoViewModel login)
        {
            return checkTheUserInfo(login.UserId, login.Password);
        }
    }
}
