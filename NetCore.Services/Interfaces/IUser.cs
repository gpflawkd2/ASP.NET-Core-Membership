using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using System.Collections.Generic;

namespace NetCore.Services.Interfaces
{
    public interface IUser
    {
        bool MatchTheUserInfo(LoginInfoViewModel login);

        User GetUserInfo(string userId);

        IEnumerable<UserRolesByUser> GetRolesOwnedByUser(string userId);

        /// <summary>
        /// [사용자 가입]
        /// </summary>
        /// <param name="register">사용자 가입용 뷰모델</param>
        /// <returns></returns>
        int RegisterUser(RegisterInfoViewModel register);
    }
}
