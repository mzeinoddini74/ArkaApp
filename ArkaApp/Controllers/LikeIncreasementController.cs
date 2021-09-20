using ArkaApp.Helper;
using ArkaApp.Helper.Api;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ArkaApp.Controllers
{
    [Authorize]
    public class LikeIncreasementController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public LikeIncreasementController()
        {
            ViewBag.ActiveMenu = "like";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        // GET: ViewIncreasement
        public ActionResult Index(Models.ViewModels.PostSearchViewModel model)
        {
            var accountId = Utility.GetAccountId();

            if (accountId == 0)
                return RedirectToAction("Index", "Account");

            const int RecordsPerPage = 10;

            var Posts = _unitOfWork.Repository<Models.EntityModels.Post>().Get(x => x.AccountID == accountId).ToList();

            if (!string.IsNullOrEmpty(model.SearchButton))
            {
                if (model.Caption != null)
                    Posts = Posts.Where(x => x.Caption.ToLower().Contains(model.Caption.ToLower())).ToList();

            }

            var pageIndex = model.Page ?? 1;
            model.SearchResults = Posts.ToPagedList(pageIndex, RecordsPerPage);

            if (TempData["Done"] != null && TempData["Done"].ToString() == "Done")
                ViewBag.Done = "Done";

            if (TempData["400"] != null && TempData["400"].ToString() == "400")
                ViewBag.Error = "400";

            if (TempData["401"] != null && TempData["401"].ToString() == "401")
                ViewBag.Error = "401";

            if (TempData["Error"] != null && TempData["Error"].ToString() == "Error")
                ViewBag.Error = "Error";

            return View(model);
        }
        public ActionResult Increase(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            List<SelectListItem> Gender = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "زن", Value = "2"
                },
                new SelectListItem {
                    Text = "مرد", Value = "3"
                }
            };
            ViewBag.Gender = Gender;

            List<SelectListItem> Priority = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "متوسط", Value = "2", Selected = true
                },
                new SelectListItem {
                    Text = "بالا", Value = "1"
                },
                new SelectListItem {
                    Text = "کم", Value = "3"
                }
            };
            ViewBag.Priority = Priority;
            ViewBag.PostID = id;

            return View();
        }

        [HttpPost]
        public ActionResult Increase(Models.EntityModels.LikeLog model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Call Api
                    ApiHelper api = new ApiHelper();
                    var post = _unitOfWork.Repository<Models.EntityModels.Post>().GetById(model.PostID).PostPK;
                    HttpResponseMessage res = api.IncreaseLike(post);
                    var response = res.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<Models.ApiModels.IncreasementModel.Root>(response);

                    if (result.State == (int)Models.ResponseEnumModel.Ok)
                    {
                        //Save Log
                        LikeLogHelper log = new LikeLogHelper();

                        if (model.Gender == null)
                            model.Gender = (short)Models.GenderEnumModel.DontCare;

                        if (model.Priority == null)
                            model.Priority = (short)Models.PriorityEnumModel.DontCare;

                        log.SaveLog(model.PostID, (long)result.InitialCount, (long)model.Count, (short)model.Gender, (short)model.Priority);

                        TempData["Done"] = "Done";
                        return RedirectToAction("Index", "Dashboard");

                    }
                    else if (result.State == (int)Models.ResponseEnumModel.Unauthorized)
                        //401
                        TempData["401"] = "401";
                    else if (result.State == (int)Models.ResponseEnumModel.BadRequest)
                        //400
                        TempData["400"] = "400";
                    else
                        TempData["Error"] = "Error";

                    //Save log
                    ApiLogHelper apiLog = new ApiLogHelper();
                    apiLog.SaveLog(Utility.GetAccountId(), model.PostID, Models.ActionEnumModel.Like + " - " + result.Desc, result.State);
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "در ارسال درخواست مشکلی رخ داده است. لطفا مجددا تلاش کنید.");
                }
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            List<SelectListItem> Gender = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "زن", Value = "2"
                },
                new SelectListItem {
                    Text = "مرد", Value = "3"
                }
            };
            ViewBag.Gender = Gender;

            List<SelectListItem> Priority = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "متوسط", Value = "2", Selected = true
                },
                new SelectListItem {
                    Text = "بالا", Value = "1"
                },
                new SelectListItem {
                    Text = "کم", Value = "3"
                }
            };

            ViewBag.Priority = Priority;

            ViewBag.PostID = model.PostID;

            return View(model);
        }
    }
}