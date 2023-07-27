using HakunaMatata.Helpers;
using HakunaMatata.Models.ViewModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Twilio.Rest.Verify.V2.Service;

namespace HakunaMatata.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAccountServices _services;
        private readonly IVerification _phoneServices;
        public AccountController(IAccountServices services, IVerification phoneServices)
        {
            _services = services;
            _phoneServices = phoneServices;
        }


        public IActionResult Register(string returnUrl = "")
        {
            var model = new VM_Register { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(VM_Register registerAccount)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await _services.RegisterUser(registerAccount);
                if (isSuccess)
                {
                    try
                    {
                        var verification = await VerificationResource.CreateAsync(
                            to: registerAccount.PhoneNumber,
                            channel: "sms",
                            pathServiceSid: "XXX"
                        );
                        if (verification.Status == "pending")
                        {
                            return RedirectToAction("ConfirmPhone", "Account");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                        return View(registerAccount);
                    }
                    //await _phoneServices.StartVerificationAsync(registerAccount.PhoneNumber);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(registerAccount);
        }

        public async Task<IActionResult> ConfirmPhone()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmPhone(string ConfirmCode)
        {
            try
            {
                var verification = await VerificationCheckResource.CreateAsync(
                  to: "+84396987327",
                  code: "439294",
                  pathServiceSid: "XXX"
              );

                if (verification.Status == "approved")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(ConfirmCode);
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(ConfirmCode);
            }
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
            scheme: "MyCookieScheme"
            );

            return RedirectToAction("Index", "Home");
        }
    }
}