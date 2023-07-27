using HakunaMatata.Helpers;
using HakunaMatata.Models.DataModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using HakunaMatata.Models.ViewModels;
using Microsoft.AspNetCore.Http;


namespace HakunaMatata.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class PaymentController : Controller
    {
        private readonly ILevelServices _services;
        private readonly IVNPayService _vnPayService;
        private readonly IMomoService _momoService;

        public PaymentController(ILevelServices services, IVNPayService vnPayService, IMomoService momoService)
        {
            _services = services;
            _vnPayService = vnPayService;
            _momoService = momoService;
        }

        public IActionResult Index(string userID)
        {
            ViewBag.UserID = userID;
            int userId = Convert.ToInt32(userID);
            ViewBag.Coin = _services.GetCoin(userId).Coin;
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Fail()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index([Bind("Id,Coin")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _services.CreateHistoryPayment(agent);
                    _services.UpdateCoinAgent(agent);
                    return RedirectToAction("Success");
                }
                catch (Exception)
                {
                    if (!_services.IsExist(agent.Id))
                    {
                        return NotFound();
                    }
                    else throw;
                }
            }

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "Index") });
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutModel request)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            if (userId==0)
            {
                return Redirect("/AdminArea");
            }
            
            var paymentCode = Guid.NewGuid().ToString().Split("-")[0];
            switch (request.PaymentMethod.ToLower())
            {
                case "vnpay":
                    var responseUriVnPay = _vnPayService.CreatePayment(new PaymentInfoModel()
                    {
                        TotalAmount = request.TotalAmount,
                        PaymentCode = paymentCode
                    }, HttpContext);
                    return Json(responseUriVnPay.Uri);
                case "momo":
                    var responseUriMomo = await _momoService.CreatePayment(new PaymentInfoModel()
                    {
                        TotalAmount = request.TotalAmount,
                        PaymentCode = paymentCode
                    });
                    return Json(responseUriMomo.Uri);
                default:
                    return Json("Invalid payment method");
            }
        }

        public IActionResult PaymentCallback()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            if (userId==0)
            {
                return Redirect("/AdminArea");
            }
            
            var queryCollection = Request.Query;

            if (queryCollection?.Count == 0 || queryCollection == null)
                return Redirect("/AdminArea/Payment");

            var paymentMethod = queryCollection.FirstOrDefault(t => t.Key.Contains("payment_method")).Value
                .ToString().ToLower();
            
            var agent = new Agent()
            {
                Id = userId
            };
            
            switch (paymentMethod)
            {
                case "vnpay":
                    var vnPayResponse = _vnPayService.PaymentExecute(Request.Query);
                    agent.Coin = int.Parse(vnPayResponse.TotalAmount);
                    _services.CreateHistoryPayment(agent);
                    _services.UpdateCoinAgent(agent);
                    return Redirect(vnPayResponse.Success
                        ? "/AdminArea/Payment/Success"
                        : "/AdminArea/Payment/Fail");
                case "momo":
                    var momoResponse = _momoService.PaymentExecute(Request.Query);
                    agent.Coin = int.Parse(momoResponse.TotalAmount);
                    _services.CreateHistoryPayment(agent);
                    _services.UpdateCoinAgent(agent);
                    return Redirect(momoResponse.Success
                        ? "/AdminArea/Payment/Success"
                        : "/AdminArea/Payment/Fail");
                default:
                    return Redirect("/AdminArea/Payment");
            }
        }
    }
}