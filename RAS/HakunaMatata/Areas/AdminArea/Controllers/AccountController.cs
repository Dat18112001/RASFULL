using HakunaMatata.Helpers;
using HakunaMatata.Models.ViewModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using MimeKit;


namespace HakunaMatata.Areas.AdminArea.Controllers
{
    [Area("AdminArea")] 
    public class AccountController : Controller
    {
        private readonly IAccountServices _services;

        public AccountController(IAccountServices services)
        {
            _services = services;
        }
        
        [Route("dang-nhap")]
        public IActionResult Login(string returnUrl = "")
        {
            var model = new VM_Login { ReturnUrl = returnUrl };
            ViewBag.RegisterMessage = TempData["RegisterMessage"];
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap")]
        public async Task<IActionResult> Login(VM_Login account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var member = _services.GetUser(account);
                    if (member != null)
                    {
                        var userPrincipal = Helper.GenerateIdentity(member);

                        var props = new AuthenticationProperties
                        {
                            IsPersistent = account.IsRememberMe
                        };
                        
                        //sign in
                        await HttpContext.SignInAsync(
                            scheme: "MyCookieScheme",
                            principal: userPrincipal,
                            properties: props
                            );

                        if (!string.IsNullOrEmpty(account.ReturnUrl)
                            && Url.IsLocalUrl(account.ReturnUrl))
                            return Redirect(account.ReturnUrl);
                        else
                        {
                            //client
                            if (member.LevelId == 3)
                            {
                                return RedirectToAction("ClientRealWishList", "RealEstate");
                            }
                            return RedirectToAction("CustomerConfirmList", "RealEstate");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Sai tài khoản hoặc mật khẩu!";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View(account);
        }

        [Route("dang-ki")]
        public IActionResult Register(string email, string returnUrl = "")
        {
            var model = new VM_Register { ReturnUrl = returnUrl };

            if (!string.IsNullOrEmpty(email))
            {
                model.Email = email;
            }
            else
            {
                ViewBag.isActiveOtp = false;
            }

            return View(model);
        }


        [Route("send-otp")]
        public IActionResult SendOTP(string email)
        {


            if (!string.IsNullOrEmpty(email))
            {
                // Tạo mã OTP
                string otp = GenerateOTP();
                ViewBag.OTP = otp;
                // Gửi email chứa mã OTP
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Web Quản Lý Nhà Trọ", "devcuongbui1@gmail.com"));
                message.To.Add(new MailboxAddress("Người nhận", email));

                message.Subject = "Xác thực mã OTP";
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"<b>Mã OTP xác minh đăng ký tài khoản của bạn là: {otp}</b>"
                };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    smtp.Authenticate("devcuongbui1@gmail.com", "cfzovzkkzwfwimcz");

                    smtp.Send(message);
                    smtp.Disconnect(true);
                }
            }

            ViewData["Email"] = email; // Store the email in ViewData

            return View();
        }






        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ki")]
        public async Task<IActionResult> Register(VM_Register newUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var isSuccess = await _services.RegisterUser(newUser);
                    if (isSuccess)
                    {
                        TempData["RegisterMessage"] = "Đăng kí thành công, đăng nhập để tiếp tục.";
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Đăng ký không thành công.");
                        return View(newUser);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View(newUser);
                }
            }

            return View(newUser);
        }


        public IActionResult Denied()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckExist(string loginName)
        {
            bool isExisted = _services.CheckExist(loginName);
            return Json(new { isExisted });
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            
            await HttpContext.SignOutAsync(
            scheme: "MyCookieScheme"
            );

            return RedirectToAction("Login");
        }
        public string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }
        public async Task SendEmailOTP(string email, string otp)
        {

        }

        [HttpGet]
        [Route("quen-mat-khau")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Redirect("quen-mat-khau");
            }
            // Tạo mã OTP
            string otp = GenerateOTP();
            ViewBag.OTP = otp;
            // Gửi email chứa mã OTP
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Web Quản Lý Nhà Trọ", "devcuongbui1@gmail.com"));
            message.To.Add(new MailboxAddress("Người nhận", email));

            message.Subject = "Xác thực mã OTP";
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"<b>Mã OTP xác minh đăng ký tài khoản của bạn là: {otp}</b>"
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);

                // Note: only needed if the SMTP server requires authentication
                smtp.Authenticate("devcuongbui1@gmail.com", "cfzovzkkzwfwimcz");

                smtp.Send(message);
                smtp.Disconnect(true);
            }
            
            ViewBag.Email = email;
            
            ViewData["Email"] = email; // Store the email in ViewData

            return View();
        }
        
        [HttpGet]
        [Route("reset-password")]
        public IActionResult SubmitReset(string email, string newPassword)
        {
            var result = _services.ResetPassword(email, newPassword);
            return Redirect(result ? "dang-nhap" : "quen-mat-khau");
        }
    }
}