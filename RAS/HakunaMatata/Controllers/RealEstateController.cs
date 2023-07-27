using Commom;
using HakunaMatata.Data;
using HakunaMatata.Helpers;
using HakunaMatata.Models.DataModels;
using HakunaMatata.Models.ViewModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HakunaMatata.Controllers
{
    public class RealEstateController : Controller
    {
        private readonly IRealEstateServices _realEstateServices;
        private readonly IFileServices _fileServices;
        private readonly ICommonServices _commonServices;
        public RealEstateController(IRealEstateServices realEstateServices,
                                    IFileServices fileServices,
                                    ICommonServices commonServices)
        {
            _realEstateServices = realEstateServices;
            _fileServices = fileServices;
            _commonServices = commonServices;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? page,
                                               int? type,
                                               int? city,
                                               int? district,
                                               int? priceRange,
                                               int? acreageRange,
                                               string searchString)
        {
            int pageSize = 18;
            var condition = new Condition()
            {
                Type = type ?? 0,
                City = city ?? 0,
                District = district ?? 0,
                PriceRange = priceRange ?? 0,
                AcreageRange = acreageRange ?? 0,
                SearchString = searchString ?? string.Empty
            };

            var source = _realEstateServices.Filter(condition);

            var types = _realEstateServices.GetRealEstateTypeList();
            types = types.Concat(new[] { new RealEstateType { Id = 0, RealEstateTypeName = "Tất cả" } });
            types = types.OrderBy(t => t.Id);

            var cities = _realEstateServices.GetCityList();
            cities = cities.Concat(new[] { new City { Id = 0, CityName = "Tất cả" } });
            cities = cities.OrderBy(c => c.Id);


            var districts = await _commonServices.GetDistrictsByCity(city);
            districts = districts.Concat(new[] { new District { Id = 0, DistrictName = "Tất cả" } });
            districts = districts.OrderBy(d => d.Id);

            var priceRanges = Helper.GetPriceRangeForView();
            var acreagaRange = Helper.GetAcreageRangeForView();

            ViewData["Types"] = new SelectList(types, "Id", "RealEstateTypeName", condition.Type);
            ViewData["Cities"] = new SelectList(cities, "Id", "CityName", condition.City);
            ViewData["Districts"] = new SelectList(districts, "Id", "DistrictName", condition.District);
            ViewData["PriceRanges"] = new SelectList(priceRanges, "Value", "Key", condition.PriceRange);
            ViewData["AcreagaRanges"] = new SelectList(acreagaRange, "Value", "Key", condition.AcreageRange);

            return View(await CustomPagination.CreateAsync(source, condition, page ?? 1, pageSize));
        }

        [AllowAnonymous]
        [Route("thong-tin-chi-tiet")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var details = await _realEstateServices.GetRealEstateDetails(id);
            if (details == null)
            {
                return NotFound();
            }
            else
            {
                //get pictures
                var pictures = await _fileServices.GetPicturesForRealEstate(details.Id);
                details.ImageUrls = Helper.GetImageUrls(pictures);

                //get recommend list
                var recommendList = _realEstateServices.GetRecommendList(id);
                details.RecommmendList = recommendList;
            }
            ViewData["GOOGLE_MAP_API"] = Constants.GOOGLE_MAP_MARKER_API;

            var type = _realEstateServices.GetRealEstateTypeList();
            ViewBag.type = type;

            var city = _realEstateServices.GetCityList();
            ViewBag.city = city;
            return View(details);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("add-to-wishlist")]
        public async Task<IActionResult> AddToWishlist(int? id, int? userID)
        {
            if (userID == null)
            {
                // Nếu userID không tồn tại, bạn có thể xử lý lỗi ở đây
                return BadRequest();
            }

            // Create a new Wishlist item
            var newWishlistItem = new Wishlist
            {
                RealEstateId = id,
                CreatedAt = DateTime.Now,
                AgentId = userID.Value // Gán giá trị userID cho AgentId
            };

            // Save the new Wishlist item to the database using _realEstateServices.AddNewWishList method
            int wishlistId = _realEstateServices.AddNewWishList(newWishlistItem);

            // Redirect to the RealEstate controller's Index action
            return RedirectToAction("Index", "RealEstate");
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("Home/wishlist/{id}")]
        public IActionResult DeleteFromWishlist(int id)
        {
            int result = _realEstateServices.DeleteWishlist(id);

            if (result == 1)
            {
                TempData["SuccessMessage"] = "Item removed from wishlist successfully!";
            }
            else if (result == 0)
            {
                TempData["ErrorMessage"] = "Item not found in the wishlist!";
            }
            else if (result == -1)
            {
                TempData["ErrorMessage"] = "System error occurred while removing item from wishlist!";
            }

            // Redirect to the RealEstate controller's Index action
            return RedirectToAction("wishlist", "Home");
        }




        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetDistricts(int? cityId, int? currentDistrictId)
        {
            if (cityId == null)
            {
                return Json(new { status = false, messsage = "Không tìm thấy thành phố!" });
            }
            else
            {
                var districts = await _commonServices.GetDistrictsByCity(cityId);
                districts = districts.Concat(new[] { new District { Id = 0, DistrictName = "Tất cả" } });
                districts = districts.OrderBy(d => d.Id);

                return Json(new { status = true, data = districts, current = currentDistrictId ?? 0, message = string.Format("Get district list by city id {0}", cityId) });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAllActiveLocation()
        {
            var data = _realEstateServices.GetAllActiveLocation();
            return Json(new { data });
        }
    }
}