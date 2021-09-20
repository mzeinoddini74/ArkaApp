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
    public class ExploreController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public ExploreController()
        {
            ViewBag.ActiveMenu = "explore";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        // GET: Explore
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

        public ActionResult ExploreRequest(int? id = 2)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.PostID = id;

            return View();
        }

        [HttpPost]
        public ActionResult ExploreRequest(Models.EntityModels.ExploreLog model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Call Api
                    ApiHelper api = new ApiHelper();
                    var post = _unitOfWork.Repository<Models.EntityModels.Post>().GetById(model.PostID).PostPK;
                    HttpResponseMessage res = api.IncreaseView(post);
                    var response = res.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<Models.ApiModels.IncreasementModel.Root>(response);

                    if (result.State == (int)Models.ResponseEnumModel.Ok)
                    {
                        //Save Log
                        Helper.ExploreLogHelper log = new Helper.ExploreLogHelper();
                        log.SaveLog(model.PostID, model.FromDateFa.ToEnglishNumber(), model.EndDateFa.ToEnglishNumber());

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
                    apiLog.SaveLog(Utility.GetAccountId(), model.PostID, Models.ActionEnumModel.Explore + " - " + result.Desc, result.State);
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "در ارسال درخواست مشکلی رخ داده است. لطفا مجددا تلاش کنید.");
                }
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            ViewBag.PostID = model.PostID;

            return View(model);
        }
    }
}