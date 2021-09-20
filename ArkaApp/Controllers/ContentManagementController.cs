using ArkaApp.Helper;
using ArkaApp.Helper.Api;
using ArkaApp.Models.ViewModels;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ArkaApp.Controllers
{
    [Authorize]
    public class ContentManagementController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public ContentManagementController()
        {
            ViewBag.ActiveMenu = "content";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        // GET: Content
        public ActionResult Index(ContentSearchViewModel model)
        {
            var accountId = Utility.GetAccountId();

            if (accountId == 0)
                return RedirectToAction("Index", "Account");

            const int RecordsPerPage = 10;

            var Contents = _unitOfWork.Repository<Models.EntityModels.Content>().Get(x => x.AccountID == accountId).ToList();

            if (!string.IsNullOrEmpty(model.SearchButton))
            {
                if (model.Description != null)
                    Contents = Contents.Where(x => x.Description.ToLower().Contains(model.Description.ToLower())).ToList();
            }

            var pageIndex = model.Page ?? 1;
            model.SearchResults = Contents.ToPagedList(pageIndex, RecordsPerPage);

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

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Models.EntityModels.Content model, HttpPostedFileBase File)
        {
            if (string.IsNullOrWhiteSpace(model.Description) && File == null)
            {
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var accountId = Utility.GetAccountId();

                    if (accountId == 0)
                        return RedirectToAction("Index", "Account");

                    if (File != null)
                    {
                        string name = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(File.FileName);
                        File.SaveAs(Server.MapPath("/Uploads/Contents/" + name));
                        model.URL = "/Uploads/Contents/" + name;
                    }

                    model.CreatedDate = DateTime.Now;
                    model.AccountID = accountId;

                    try
                    {
                        _unitOfWork.Repository<Models.EntityModels.Content>().Insert(model);
                        _unitOfWork.SaveChanges();

                        TempData["Done"] = "Done";
                        return RedirectToAction("Index", "ContentManagement");
                    }
                    catch
                    {
                        ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");
                        return View(model);
                    }
                }
                else
                    ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            }

            return View(model);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.Content>().GetById(id);

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.Content>().GetById(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Models.EntityModels.Content model, HttpPostedFileBase File)
        {
            if (ModelState.IsValid)
            {
                Models.EntityModels.Content obj = _unitOfWork.Repository<Models.EntityModels.Content>().GetById(model.ContentID);

                if (File != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(obj.URL)))
                        System.IO.File.Delete(Server.MapPath(obj.URL));

                    string name = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(File.FileName);
                    File.SaveAs(Server.MapPath("/Uploads/Contents/" + name));
                    obj.URL = "/Uploads/Contents/" + name;
                }

                obj.Description = model.Description;

                try
                {
                    _unitOfWork.Repository<Models.EntityModels.Content>().Update(obj);
                    _unitOfWork.SaveChanges();

                    TempData["Done"] = "Done";
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");
                    return View(model);
                }

            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            return View(model);
        }

        public ActionResult DirectRequest(int? id)
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

            ViewBag.ContentID = id;

            DirectContentLogViewModel model = new DirectContentLogViewModel();
            model.Islands = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Island")).ToList();
            model.Seas = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Sea")).ToList();
            model.Lakes = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Lake")).ToList();
            model.Provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Province")).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult DirectRequest(DirectContentLogViewModel model)
        {
            ViewBag.ContentID = model.ContentID;

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
                        DirectContentLogHelper log = new DirectContentLogHelper();

                        if (model.Log.Gender == null)
                            model.Log.Gender = (short)Models.GenderEnumModel.DontCare;

                        log.SaveLog(model.ContentID, (long)model.Log.Count, model.Log.Geo, (short)model.Log.Gender);

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
                    apiLog.SaveLog(Utility.GetAccountId(), (int)model.ContentID, Models.ActionEnumModel.DirectContent + " - " + result.Desc, result.State);
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

            TelegramContentLogViewModel model = new TelegramContentLogViewModel();

            model.Islands = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Island")).ToList();
            model.Seas = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Sea")).ToList();
            model.Lakes = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Lake")).ToList();
            model.Provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Province")).ToList();

            ViewBag.ContentID = id;

            return View(model);
        }

        [HttpPost]
        public ActionResult TelegramRequest(TelegramContentLogViewModel model)
        {
            ViewBag.ContentID = (int)model.Log.ContentID;

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
                        TelegramContentLogHelper log = new TelegramContentLogHelper();

                        if (model.Log.Geo == null)
                            model.Log.Geo = "0";

                        log.SaveLog((int)model.Log.ContentID, (long)model.Log.Count, model.Log.Geo, model.Log.GroupTitle, model.Log.Regions);

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
                    apiLog.SaveLog(Utility.GetAccountId(), (int)model.Log.ContentID, Models.ActionEnumModel.TelegramContent + " - " + result.Desc, result.State);
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

            ViewBag.ContentID = id;

            return View();
        }

        [HttpPost]
        public ActionResult TelegramUserRequest(Models.EntityModels.TelegramUserContentLog model)
        {
            ViewBag.ContentID = (int)model.ContentID;

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
                        TelegramUserContentLogHelper log = new TelegramUserContentLogHelper();
                        log.SaveLog((int)model.ContentID, model.Users);

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
                    apiLog.SaveLog(Utility.GetAccountId(), (int)model.ContentID, Models.ActionEnumModel.TelegramUserContent + " - " + result.Desc, result.State);
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