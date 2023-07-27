using HakunaMatata.Helpers;
using HakunaMatata.Models.DataModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HakunaMatata.Models.ViewModels;

namespace HakunaMatata.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class HistoryPaymentController : Controller
    {
        private readonly ILevelServices _services;
        public HistoryPaymentController(ILevelServices services)
        {
            _services = services;
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var histories = _services.GetHistoryPaymentsByDate(startDate, endDate); ;

            return View(histories);
        }

        public IActionResult Chart()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetListOrderReport([FromQuery] ChartReportSummaryFilterModel request)
        {
            var data = await _services.GenerateOrderReport(request.Year, request.Month);

            return Json(data);
        }
    }
}