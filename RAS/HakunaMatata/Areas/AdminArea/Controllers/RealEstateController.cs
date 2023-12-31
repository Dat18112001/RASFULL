﻿using HakunaMatata.Data;
using HakunaMatata.Helpers;
using HakunaMatata.Models.ViewModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HakunaMatata.Areas.AdminArea.Controllers
{
    [Authorize]
    [Area("AdminArea")]
    public class RealEstateController : Controller
    {
        private readonly IRealEstateServices _realEstateServices;
        private readonly IFileServices _fileServices;
        private readonly ILevelServices _levelServices;
        private readonly IAgentServices _agentServices;
        private readonly HakunaMatataContext _context;

        public RealEstateController(HakunaMatataContext context,IRealEstateServices realEstateServices, IFileServices fileServices, ILevelServices levelServices, IAgentServices agentServices)
        {
            _realEstateServices = realEstateServices;
            _fileServices = fileServices;
            _levelServices = levelServices;
            _agentServices = agentServices;
            _context = context;
            
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        /// <summary>
        /// paging in server side
        /// </summary>
        /// <param name="pageIndex">Pagination index</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadData(VM_FilterInfo info)
        {
            var pageIndex = info.pageIndex;
            var searchKey = info.searchKey ?? string.Empty;
            var source = _realEstateServices.GetRealEstates(searchKey);
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * 20).Take(20).ToListAsync();

            return Json(new { data = items, totalRow = count });
        }

        /// <summary>
        /// Client's all post
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("danh-sach-bai-viet")]
        public IActionResult ClientRealEstateList()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var userPosts = _realEstateServices.GetUserAllPosts(Convert.ToInt32(userId));

            if (userPosts == null) return NotFound();

            ViewBag.Message = TempData["Message"];
            ViewBag.MessageType = TempData["MesageType"];
            return View(userPosts);
        }



        /// <summary>
        /// Client's waiting for confirm post
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [Route("danh-sach-cho")]
        public IActionResult CustomerConfirmList()
        {
            var confirmList = _realEstateServices.GetCustomerConFirmList();
            return View(confirmList);
        }


        /// <summary>
        /// Client's waiting for confirm post
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [Route("danh-sach-qua-han")]
        public IActionResult CustomerExpireList()
        {
            var expireList = _realEstateServices.CustomerExpireList();
            return View(expireList);
        }

        [HttpGet]
        [Route("chi-tiet")]
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
                var pictures = await _fileServices.GetPicturesForRealEstate(details.Id);
                details.ImageUrls = Helper.GetImageUrls(pictures);
            }
            ViewData["GOOGLE_MAP_API"] = Constants.GOOGLE_MAP_MARKER_API;
            return View(details);
        }

        public async Task<IActionResult> ClientRealWishList()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value ?? string.Empty;
            var wishListInfo = _realEstateServices.GetWishList(userId);
            if (wishListInfo == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var item in wishListInfo)
                {
                    var pictures = await _fileServices.GetPicturesForRealEstate(item.RealEstateId);
                    var pictureList = pictures.ToList(); // Convert IEnumerable<Picture> to a list

                    if (pictureList.Count > 0)
                    {
                        item.Url = pictureList[0].Url; // Assign the URL of the first picture to the Url property
                    }
                }
            }
            return View(wishListInfo);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("wish-list/{id}")]
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
            return Redirect("/AdminArea/RealEstate/ClientRealWishList");
        }

        [HttpGet]
        [Route("bai-dang-moi")]
        public IActionResult Create()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var user = _levelServices.GetCoin(userId);

            if (((user.Package ?? 0) == 0) && user.LevelId == 3)
            {
                return Redirect("/AdminArea/Payment?userID=" + user.Id);
            }
            
            var realEstateTypeList = _realEstateServices.GetRealEstateTypeList();
            ViewData["RealEstateTypeId"] = new SelectList(realEstateTypeList, "Id", "RealEstateTypeName", realEstateTypeList.First());
            return View();
        }


        [HttpPost]
        [Route("bai-dang-moi")]
        public IActionResult Create(
            [Bind("Title,Address,Price,ContactNumber,Acreage,BeginTime,BackupBeginTime,ExprireTime,BackupExpireTime,RoomNumber,Description,HasPrivateWc,HasMezzanine,AllowCook,FreeTime,SecurityCamera,WaterPrice,ElectronicPrice,WifiPrice,RealEstateTypeId,Latitude,Longtitude,IsFreeWater,IsFreeElectronic,IsFreeWifi,Files")]
            VM_RealEstateDetails details)
        {
            int uploadedFilesCount = 0;
            var realEstateTypeList = _realEstateServices.GetRealEstateTypeList();
            ViewData["RealEstateTypeId"] = new SelectList(realEstateTypeList, "Id", "RealEstateTypeName", details.RealEstateTypeId);

            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value ?? string.Empty;
                if (userId != string.Empty)
                {
                    var realEstateId = _realEstateServices.AddCompleteRealEstate(details, Convert.ToInt32(userId));

                    //tao real estate thanh cong
                    if (realEstateId != -1)
                    {
                        if (details.Files != null && details.Files.Count > 0)
                        {
                            uploadedFilesCount = _fileServices.AddPicture(realEstateId, details.Files);
                        }

                        //use tempdate pass message to index controller
                        TempData["Message"] = string.Format("Thêm phòng trọ thành công, uploaded {0} hình ảnh", uploadedFilesCount);
                        TempData["MesageType"] = 1;
                        return RedirectToAction(nameof(ClientRealEstateList));
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Có lỗi xảy ra, vui lòng thử lại";
                        return View(details);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "User id không hợp lệ";
                    return View(details);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra, vui lòng thử lại";
                return View(details);
            }
        }

        public async Task<IActionResult> Edit(int? id, [FromServices] IAccountServices _accountServices)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            if (!_accountServices.IsValidUser(currentUserId, Convert.ToInt32(id)))
            {
                return RedirectToAction("Denied", "Account");
            }

            var details = await _realEstateServices.GetRealEstateDetails(id);
            if (details == null)
            {
                return NotFound();
            }

            var realEstateTypeList = _realEstateServices.GetRealEstateTypeList();
            ViewData["RealEstateTypeId"] = new SelectList(realEstateTypeList, "Id", "RealEstateTypeName", details.RealEstateTypeId);
            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,
                                  [Bind("Id,Title,Address,Price,Acreage,BeginTime,ExprireTime,RoomNumber,Description,HasPrivateWc,HasMezzanine,AllowCook,FreeTime,SecurityCamera,WaterPrice,ElectronicPrice,WifiPrice,RealEstateTypeId,ContactNumber,Files")] VM_RealEstateDetails details)
        {
            int uploadedFilesCount = 0;

            if (id != details.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var isUpdateSuccess = _realEstateServices.UpdateRealEstate(details);
                    if (isUpdateSuccess)
                    {
                        if (details.Files != null && details.Files.Count > 0)
                        {
                            uploadedFilesCount = _fileServices.AddPicture(details.Id, details.Files);
                            TempData["Message"] = string.Format("Uploaded {0} hình ảnh", uploadedFilesCount);
                            TempData["MesageType"] = 1;
                        }
                        return RedirectToAction(nameof(ClientRealEstateList));
                    }
                    else return View(details);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_realEstateServices.IsExistRealEstate(details.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(details);
        }

        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmRealEstateConfirm(int id, int confirmType, bool isRedirect)
        {
            var isSuccess = _realEstateServices.ConfirmRealEsate(id, confirmType);

            if (isRedirect)
            {
                return Json(new { isSuccess });
            }
            else
            {
                return Json(new { isSuccess, html = Helper.RenderRazorViewToString(this, "_viewConfirmList", _realEstateServices.GetCustomerConFirmList()) });
            }
        }

[HttpPost, ActionName("Booked")]
[ValidateAntiForgeryToken]
public IActionResult BookedRealEsate(int id, int userId, bool isRedirect)
{
    var realEstate = _context.RealEstate.Find(id);

    if (realEstate != null)
    {
        if (realEstate.IsAvaiable)
        {
            // If the real estate is available, book it
            var isSuccess = _realEstateServices.BookedRealEstate(id);
            if (isSuccess)
            {
                // Booking successful
                if (isRedirect)
                {
                    return Json(new { isSuccess = true });
                }
                else
                {
                    return Json(new { isSuccess = true, html = Helper.RenderRazorViewToString(this, "_viewUserAllPosts", _realEstateServices.GetUserAllPosts(userId)) });
                }
            }
        }
        else
        {
            // If the real estate is booked, unbook it
            var isUnbooked = _realEstateServices.UnbookRealEstate(id);
            if (isUnbooked)
            {
                // Unbooking successful
                if (isRedirect)
                {
                    return Json(new { isSuccess = true });
                }
                else
                {
                    return Json(new { isSuccess = true, html = Helper.RenderRazorViewToString(this, "_viewUserAllPosts", _realEstateServices.GetUserAllPosts(userId)) });
                }
            }
        }
    }

    // Booking/unbooking failed
    return Json(new { isSuccess = false });
}


        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public IActionResult DisableRealEsate(int id, int userId, bool isRedirect)
        {
            var isSuccess = _realEstateServices.DisableRealEstate(id);

            if (isRedirect)
            {
                return Json(new { isSuccess });
            }
            else
            {
                return Json(new { isSuccess, html = Helper.RenderRazorViewToString(this, "_viewUserAllPosts", _realEstateServices.GetUserAllPosts(userId)) });
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRealEsate(int id, int userId)
        {
            var returnCode = _realEstateServices.DeleteRealEstate(id, userId);

            if (returnCode < 1)
            {
                return Json(new { isSuccess = false, message = "Xảy ra lỗi hệ thống!" });
            }
            else if (returnCode == 2)
            {
                return Json(new { isSuccess = false, message = "User không hợp lệ!" });
            }

            else if (userId != 1)
            {
                return Json(new { isSuccess = true, html = Helper.RenderRazorViewToString(this, "_viewUserAllPosts", _realEstateServices.GetUserAllPosts(userId)) });
            }
            else
            {
                return Json(new { isSuccess = true, isAdmin = true, html = Helper.RenderRazorViewToString(this, "_viewExpireList", _realEstateServices.CustomerExpireList()) });
            }

        }
        
        [HttpGet]
        public IActionResult PaymentHistory(DateTime? startDate, DateTime? endDate)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            if (userId==0)
            {
                return Redirect("/AdminArea");
            }
            
            var histories = _levelServices.GetHistoryPaymentsByDate(startDate, endDate, userId);
            
            return View(histories);
        }
    }
}