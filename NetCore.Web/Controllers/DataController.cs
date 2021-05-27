using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCore.Web.Controllers
{
    public class DataController : Controller
    {
        private readonly IDataProtector _protector;
        private HttpContext _context;
        public string _sessionKeyCartName = "_sessionCartKey";

        public DataController(IHttpContextAccessor accessor,IDataProtectionProvider provider)
        {
            _context = accessor.HttpContext;
            _protector = provider.CreateProtector("NetCore.Data.v1");
        }

        #region + private methods

        private List<ItemInfoViewModel> GetCartInfos(ref string message)
        {
            var cartInfos = _context.Session.Get<List<ItemInfoViewModel>>(key: _sessionKeyCartName);

            if(cartInfos == null || cartInfos.Count() < 1)
            {
                message = "장바구니에 담긴 상품이 없습니다.";
            }

            return cartInfos;
        }

        private void SetCartInfos(ItemInfoViewModel item, List<ItemInfoViewModel> cartInfos = null)
        {
            if(cartInfos == null)
            {
                cartInfos = _context.Session.Get<List<ItemInfoViewModel>>(_sessionKeyCartName);

                if(cartInfos == null)
                {
                    cartInfos = new List<ItemInfoViewModel>();
                }
            }

            cartInfos.Add(item);

            _context.Session.Set<List<ItemInfoViewModel>>(_sessionKeyCartName, cartInfos);
        }

        #endregion

        #region + AES

        [HttpGet]
        [Authorize(Roles = "SuperUser, SystemUser")]
        public IActionResult AES()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperUser, SystemUser")]
        public IActionResult AES(AESInfo aes)
        {
            string message = string.Empty;

            if(ModelState.IsValid)
            {
                string userInfo = aes.UserId + aes.Password;
                aes.EncUserInfo = _protector.Protect(userInfo);            // 암호화 정보
                aes.DecUserInfo = _protector.Unprotect(aes.EncUserInfo);   // 복호화 정보

                ViewData["Message"] = "암복호화가 성공적으로 이루어졌습니다."; 

                return View(aes);
            }
            else
            {
                message = "암복호화를 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(aes);
        }

        #endregion

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult AddCart()
        {
            SetCartInfos(new ItemInfoViewModel() { ItemNo = Guid.NewGuid(), ItemName = DateTime.UtcNow.Ticks.ToString() });

            return RedirectToAction("Cart","Data");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult RemoveCart()
        {
            string message = string.Empty;

            var cartInfos = GetCartInfos(ref message);

            if(cartInfos != null && cartInfos.Count() > 0)
            {
                _context.Session.Remove(key:_sessionKeyCartName);
            }

            return RedirectToAction("Cart", "Data");
        }

        public IActionResult Cart()
        {
            string message = string.Empty;

            var cartInfos = GetCartInfos(ref message);

            ViewData["Message"] = message;

            return View(cartInfos);
        }

    }
}