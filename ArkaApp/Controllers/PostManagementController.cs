using ArkaApp.Helper;
using ArkaApp.Helper.Api;
using ArkaApp.Models.ViewModels;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ArkaApp.Controllers
{
    public class PostManagementController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public PostManagementController()
        {
            ViewBag.ActiveMenu = "message";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }
        public ActionResult Index(PostSearchViewModel model)
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

        public ActionResult DirectRequest(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            DirectLogViewModel model = new DirectLogViewModel();

            model.Islands = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Island")).ToList();
            model.Seas = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Sea")).ToList();
            model.Lakes = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Lake")).ToList();
            model.Provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Province")).ToList();

            List<SelectListItem> Gender = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "زن", Value = "2"
                },
                new SelectListItem {
                    Text = "مرد", Value = "3"
                }
            };
            ViewBag.Gender = Gender;

            ViewBag.PostID = id;

            return View(model);
        }

        [HttpPost]
        public ActionResult DirectRequest(DirectLogViewModel model)
        {
            ViewBag.PostID = (int)model.Log.PostID;

            if (ModelState.IsValid)
            {
                try
                {
                    //Call Api
                    ApiHelper api = new ApiHelper();
                    var account = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(Utility.GetAccountId()).AccountUserName;
                    HttpResponseMessage res = api.IncreaseFollower(account);
                    var response = res.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<Models.ApiModels.IncreasementModel.Root>(response);

                    if (result.State == (int)Models.ResponseEnumModel.Ok)
                    {
                        //Save Log
                        DirectLogHelper log = new DirectLogHelper();

                        if (model.Log.Gender == null)
                            model.Log.Gender = (short)Models.GenderEnumModel.DontCare;

                        log.SaveLog((int)model.Log.PostID, (long)model.Log.Count, model.Log.Geo, (short)model.Log.Gender);

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
                    apiLog.SaveLog(Utility.GetAccountId(), (int)model.Log.PostID, Models.ActionEnumModel.DirectPost + " - " + result.Desc, result.State);
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("", "در ارسال درخواست مشکلی رخ داده است. لطفا مجددا تلاش کنید.");
                }
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            model.Islands = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Island")).ToList();
            model.Seas = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Sea")).ToList();
            model.Lakes = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Lake")).ToList();
            model.Provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Province")).ToList();

            List<SelectListItem> Gender = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "زن", Value = "2"
                },
                new SelectListItem {
                    Text = "مرد", Value = "3"
                }
            };
            ViewBag.Gender = Gender;

            return View(model);
        }


        public ActionResult TelegramRequest(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TelegramLogViewModel model = new TelegramLogViewModel();

            model.Islands = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Island")).ToList();
            model.Seas = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Sea")).ToList();
            model.Lakes = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Lake")).ToList();
            model.Provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Province")).ToList();

            ViewBag.PostID = id;

            return View(model);
        }

        [HttpPost]
        public ActionResult TelegramRequest(TelegramLogViewModel model)
        {
            ViewBag.PostID = (int)model.Log.PostID;

            if (ModelState.IsValid)
            {
                try
                {
                    //Call Api
                    ApiHelper api = new ApiHelper();
                    var account = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(Utility.GetAccountId()).AccountUserName;
                    HttpResponseMessage res = api.IncreaseFollower(account);
                    var response = res.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<Models.ApiModels.IncreasementModel.Root>(response);

                    if (result.State == (int)Models.ResponseEnumModel.Ok)
                    {
                        //Save Log
                        TelegramLogHelper log = new TelegramLogHelper();

                        if (model.Log.Geo == null)
                            model.Log.Geo = "0";

                        log.SaveLog((int)model.Log.PostID, (long)model.Log.Count, model.Log.Geo, model.Log.GroupTitle, model.Log.Regions);

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
                    apiLog.SaveLog(Utility.GetAccountId(), (int)model.Log.PostID, Models.ActionEnumModel.TelegramPost + " - " + result.Desc, result.State);
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("", "در ارسال درخواست مشکلی رخ داده است. لطفا مجددا تلاش کنید.");
                }
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            model.Islands = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Island")).ToList();
            model.Seas = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Sea")).ToList();
            model.Lakes = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Lake")).ToList();
            model.Provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Province")).ToList();

            return View(model);
        }



        public ActionResult TelegramUserRequest(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.PostID = id;

            return View();
        }

        [HttpPost]
        public ActionResult TelegramUserRequest(Models.EntityModels.TelegramUserLog model)
        {
            ViewBag.PostID = (int)model.PostID;

            if (ModelState.IsValid)
            {
                try
                {
                    //Call Api
                    ApiHelper api = new ApiHelper();
                    var account = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(Utility.GetAccountId()).AccountUserName;
                    HttpResponseMessage res = api.IncreaseFollower(account);
                    var response = res.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<Models.ApiModels.IncreasementModel.Root>(response);

                    if (result.State == (int)Models.ResponseEnumModel.Ok)
                    {
                        //Save Log
                        TelegramUserLogHelper log = new TelegramUserLogHelper();
                        log.SaveLog((int)model.PostID, model.Users);

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
                    apiLog.SaveLog(Utility.GetAccountId(), (int)model.PostID, Models.ActionEnumModel.TelegramUserPost + " - " + result.Desc, result.State);
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("", "در ارسال درخواست مشکلی رخ داده است. لطفا مجددا تلاش کنید.");
                }
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            return View(model);
        }
    }
}