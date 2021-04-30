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
    }
}
