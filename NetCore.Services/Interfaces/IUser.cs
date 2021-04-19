using NetCore.Data.ViewModels;

namespace NetCore.Services.Interfaces
{
    public interface IUser
    {
        bool MatchTheUserInfo(LoginInfoViewModel login);
    }
}
