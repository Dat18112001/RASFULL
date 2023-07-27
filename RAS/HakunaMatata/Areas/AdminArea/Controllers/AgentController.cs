using HakunaMatata.Helpers;
using HakunaMatata.Models.DataModels;
using HakunaMatata.Models.ViewModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HakunaMatata.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize]
    public class AgentController : Controller
    {
        private readonly IAgentServices _services;
        private readonly ILevelServices _levelServices;

        public AgentController(IAgentServices services, ILevelServices levelServices)
        {
            _services = services;
            _levelServices = levelServices;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var agents = await _services.GetListAgent();
            return View(agents);
        }

        public async Task<IActionResult> Details(int id)
        {
            var agent = await _services.GetDetails(id);
            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }

        [HttpGet]
        [Route("thong-tin-ca-nhan")]
        public IActionResult UpdateProfile()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var userInfo = _services.GetUserInfo(Convert.ToInt32(userId));

            ViewBag.UpdatePassStatus = TempData["UpdatePassStatus"];

            return View(userInfo);
        }

        [HttpPost]
        [Route("thong-tin-ca-nhan")]
        public IActionResult UpdateProfile(VM_Agent updateProfile)
        {
            if (ModelState.IsValid)
            {
                var status = _services.UpdateProfile(updateProfile);

                return Json(new { status });
            }

            return View(updateProfile);
        }


        [HttpGet]
        [Route("doi-mat-khau")]
        public IActionResult UpdatePassword()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var viewmodel = new VM_ChangePassword() { Id = Convert.ToInt32(userId) };
            return View(viewmodel);
        }

        [HttpPost]
        [Route("doi-mat-khau")]
        public IActionResult UpdatePassword(VM_ChangePassword data)
        {
            if (ModelState.IsValid)
            {
                int resultCode = _services.UpdatePassword(data);

                if (resultCode == 1)
                {
                    TempData["UpdatePassStatus"] = "Cập nhật mật khẩu thành công!";

                    return RedirectToAction("UpdateProfile");
                }
                else if (resultCode == 0)
                {
                    ViewBag.Message = "Mật khẩu cũ không chính xác!";
                    ViewBag.MesasageCode = 0;
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra, vui lòng thử lại!";
                    ViewBag.MesasageCode = -1;
                }
                return View(data);
            }

            return View(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableConfirm(int id)
        {
            var isSuccess = await _services.Disable(id);
            return Json(new { isSuccess, html = Helper.RenderRazorViewToString(this, "_ViewAllAgents", await _services.GetListAgent()) });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var isSuccess = await _services.Delete(id);
            return Json(new { isSuccess, html = Helper.RenderRazorViewToString(this, "_ViewAllAgents", await _services.GetListAgent()) });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var agent = _levelServices.GetCoin(userId);

            return Json(new
            {
                coin = string.Format("{0:N0}", Convert.ToDecimal(agent.Coin ?? 0)),
                package = agent.Package
            });
        }
    }
}