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

        /// <summary>
        /// [사용자 정보 수정을 위한 검색]
        /// </summary>
        /// <param name="userId">사용자 아이디</param>
        /// <returns></returns>
        UserInfoViewModel GetUserInfoForUpdate(string userId);

        /// <summary>
        /// [사용자 정보 수정]
        /// </summary>
        /// <param name="user">사용자정보 뷰모델</param>
        /// <returns></returns>
        int UpdateUser(UserInfoViewModel user);

        /// <summary>
        /// [사용자 정보수정에서 변경대상 비교] true : 전부 Data가 같은 경우, false: 하나라도 다른 경우
        /// </summary>
        /// <param name="user">사용자정보 뷰모델</param>
        /// <returns></returns>
        bool CompareInfo(UserInfoViewModel user);

        /// <summary>
        /// [사용자 탈퇴]
        /// </summary>
        /// <param name="user">사용자탈퇴정보 뷰모델</param>
        /// <returns></returns>
        int WithdrawnUser(WithdrawnInfo user);
    }
}
