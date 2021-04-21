using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;

namespace NetCore.Web.Controllers
{
    public class MembershipController : Controller
    {
        // 의존성 주입 - 생성자
        private IUser _user;

        public MembershipController(IUser user)
        {
            _user = user;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        // 위조방지토큰을 통해 View로부터 받은 Post Data가 유효한지 검증
        [ValidateAntiForgeryToken]
        // Data => Services => Web
        // Data => Services
        // Data => Web
        public IActionResult Login(LoginInfoViewModel login)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {

                //if (login.UserId.Equals(userId) && login.Password.Equals(password))
                // 뷰모델
                // 서비스 개념
                if(_user.MatchTheUserInfo(login))
                {
                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";
                    return RedirectToAction("Index", "Membership");
                }
                else
                {
                    message = "로그인되지 않았습니다.";
                }
            }
            else
            {
                message = "로그인 정보를 올바르게 입력하세요!!";
            }

            ModelState.AddModelError(string.Empty, message);

            return View(login);
        }
    }

}