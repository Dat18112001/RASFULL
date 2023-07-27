using HakunaMatata.Helpers;
using HakunaMatata.Models.DataModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HakunaMatata.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly ILevelServices _services;
        public NewsController(ILevelServices services)
        {
            _services = services;
        }

        public IActionResult Index()
        {
            var news = _services.GetListNews();
            return View(news);
        }

        [NoDirectAccess]
        public IActionResult CreateOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new News());
            }
            else
            {
                var news = _services.GetNewsDetails(id);
                if (news == null)
                {
                    return NotFound();
                }
                return View(news);
            }

        }

        [NoDirectAccess]
        public IActionResult CreateOrEditImage(int id = 0)
        {
            if (id == 0)
            {
                return View(new Newspicture());
            }
            else
            {
                var newspic = _services.GetNewsDetails(id);
                if (newspic == null)
                {
                    return NotFound();
                }
                return View(newspic);
            }

        }

        [HttpPost]
        public IActionResult CreateOrEdit(int id, [Bind("Id,Title,NewsBody")] News news)
        {
            if (ModelState.IsValid)
            {
                //insert
                if (id == 0)
                {
                    _services.CreateNews(news);
                }

                //update
                else
                {
                    try
                    {
                        _services.UpdateNews(news);
                    }
                    catch (Exception)
                    {
                        if (!_services.IsExistNews(news.Id))
                        {
                            return NotFound();
                        }
                        else throw;
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllNews", _services.GetListNews()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEdit", news) });
        }

        [HttpPost]
        public IActionResult CreateOrEditImage(int id, [Bind("Id,PictureName,NewsID,URL")] Newspicture newspic)
        {
            if (ModelState.IsValid)
            {
                //insert
                if (id == 0)
                {
                    _services.CreateNewsPicture(newspic);
                }

                //update
                else
                {
                    try
                    {
                        _services.UpdateNewsPicture(newspic);
                    }
                    catch (Exception)
                    {
                        if (!_services.IsExistNews(newspic.Id))
                        {
                            return NotFound();
                        }
                        else throw;
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllNews", _services.GetListNews()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEditImage", newspic) });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            _services.DeleteNews(id);
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAllNews", _services.GetListNews()) });
        }
    }
}