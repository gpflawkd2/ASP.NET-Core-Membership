using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Web.Controllers
{
    [Authorize(Roles = "AssociateUser, GeneralUser, SuperUser, SystemUser")]
    public class MembershipController : Controller
    {
        // 의존성 주입 - 생성자
        private IUser _user;
        private IPasswordHasher _hasher;
        private HttpContext _context;

        public MembershipController(IHttpContextAccessor accessor, IPasswordHasher hasher, IUser user)
        {
            _context = accessor.HttpContext;
            _hasher = hasher;
            _user = user;
        }

        #region private methods

        /// <summary>
        /// 로컬 URL or 외부 Url 여부 체크
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if(Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                // nameof: 문자열로 자동 변환
                return RedirectToAction(nameof(MembershipController.Index), "Membership");
            }
        }

        #endregion

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View(new LoginInfoViewModel());
        }

        [HttpPost("/Login")]
        // 위조방지토큰을 통해 View로부터 받은 Post Data가 유효한지 검증
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        // Data => Services => Web
        // Data => Services
        // Data => Web
        public async Task<IActionResult> LoginAsync(LoginInfoViewModel login, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //string guidSalt = _hasher.GetGUIDSalt();
                //string rngSalt = _hasher.GetRNGSalt();
                //string passwordHash = _hasher.GetPasswordHash(login.UserId, login.Password, guidSalt, rngSalt);

                //if (login.UserId.Equals(userId) && login.Password.Equals(password))
                if (_user.MatchTheUserInfo(login))
                {
                    // 신원보증과 승인권한
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();
                    string userDataInfo = userTopRole.UserRole.RoleName + "|" +
                                          userTopRole.UserRole.RolePriority.ToString() + "|" +
                                          userInfo.UserName + "|" +
                                          userInfo.UserEmail;

                    var identity = new ClaimsIdentity(claims:new[] 
                    {
                      new Claim(type:ClaimTypes.Name,
                      value:userInfo.UserId),
                      new Claim(type:ClaimTypes.Role,
                      value:userTopRole.RoleId),
                      new Claim(type:ClaimTypes.UserData,
                      value:userDataInfo)
                    }, authenticationType:CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                        principal: new ClaimsPrincipal(identity),
                        properties: new AuthenticationProperties()
                        {
                            IsPersistent = login.RememberMe,
                            ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                        });

                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";

                    //return RedirectToAction("Index", "Membership");
                    return RedirectToLocal(returnUrl);
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

            return View("Login", login);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterInfoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Register(RegisterInfoViewModel register, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                // 사용자 가입 서비스
                if (_user.RegisterUser(register) > 0)
                {
                    TempData["Message"] = "사용자 가입이 성공적으로 이루어졌습니다.";
                    return RedirectToAction("Login", "Membership");
                }
                else
                {
                    message = "사용자가 가입되지 않았습니다.";
                }
            }
            else
            {
                message = "사용자 가입을 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);

            return View(register);
        }

        [HttpGet]
        public IActionResult UpdateInfo()
        {
            //_context.User.Identity.Name => 사용자 아이디
            UserInfoViewModel user = _user.GetUserInfoForUpdate(_context.User.Identity.Name);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInfo(UserInfoViewModel user)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                // 변경대상 값 비교
                if(_user.CompareInfo(user))
                {
                    message = "하나 이상의 값이 변경되어야 정보수정이 가능합니다.";

                    ModelState.AddModelError(string.Empty, message);
                    return View(user);
                }

                // 정보수정 서비스
                if(_user.UpdateUser(user) > 0)
                {
                    TempData["Message"] = "사용자 정보수정이 성공적으로 이루어졌습니다.";

                    return RedirectToAction("UpdateInfo", "Membership");
                }
                else
                {
                    message = "사용자 정보가 수정되지 않았습니다.";
                }
               
            }
            else
            {
                message = "사용자 정보수정을 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(user);
        }


        [HttpGet("/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "로그아웃이 성공적으로 이루어졌습니다. <br />웹사이트를 원활히 이용하시려면 로그인하세요.";

            return RedirectToAction("Index", "Membership");
        }

        [HttpPost("/Withdrawn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithdrawnAsync(WithdrawnInfo withdrawn)
        {
            string message = string.Empty;

            if(ModelState.IsValid)
            {
                // 탈퇴 서비스
                if(_user.WithdrawnUser(withdrawn) > 0)
                {
                    TempData["Message"] = "사용자 탈퇴가 성공적으로 이루어졌습니다.";
                    await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    return RedirectToAction("Index", "Membership");
                }
                else
                {
                    message = "사용자가 탈퇴처리되지 않았습니다.";
                }
            }
            else
            {
                message = "사용자가 탈퇴하기 위한 정보를 올바르게 입력하세요.";
            }

            ViewData["Message"] = message;
            return View("Index", withdrawn);
        }

        [HttpGet]
        //[FromServices]: 해당 Action 함수에만 의존성 주입 처리
        public IActionResult Forbidden([FromServices] ILogger<MembershipController> logger)
        {
            StringValues paramReturnUrl;

            //paramReturnUrl[0] => /Data/AES

            bool exists = _context.Request.Query.TryGetValue("returnUrl", out paramReturnUrl);
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty;

            // MethodBase.GetCurrentMethod().Name: 현재 함수 정보를 가지고옴
            // logger.LogTrace
            logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}함수 접근 에러 처리.returnUrl : {paramReturnUrl}");

            ViewData["Message"] = $"귀하는 {paramReturnUrl} 경로로 접근하려고 했습니다만,<br />" +
                                   "인증된 사용자도 접근하지 못하는 페이지가 있습니다.<br />" + 
                                   "담당자에게 해당페이지의 접근권한에 대해 문의하세요.";

            return View();
        }

    }

}