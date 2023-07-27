using HakunaMatata.Helpers;
using HakunaMatata.Models.DataModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HakunaMatata.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ShopController : Controller
    {
        private readonly ILevelServices _services;
        public ShopController(ILevelServices services)
        {
            _services = services;
        }

        public IActionResult Index(string userID)
        {
            ViewBag.UserID = userID;
            int userId = Convert.ToInt32(userID);
            ViewBag.Coin = _services.GetCoin(userId).Coin;
            ViewBag.Package = _services.GetCoin(userId).Package;
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index([Bind("Id,Coin,Package")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _services.UpdateCoinPackageAgent(agent);
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
    }
}