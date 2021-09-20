using ArkaApp.Helper;
using ArkaApp.Helper.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ArkaApp.Controllers
{
    [Authorize]
    public class FollowerIncreasementController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public FollowerIncreasementController()
        {
            ViewBag.ActiveMenu = "follower";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        // GET: FollowerIncreasement
        public ActionResult Index()
        {
            //if (!_unitOfWork.Repository<Models.EntityModels.FollowerLog>().Get().Any(x => x.IsFinished == false))
            //{
            Models.ViewModels.FollowerLogViewModel model = new Models.ViewModels.FollowerLogViewModel();

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


            if (TempData["Done"] != null && TempData["Done"].ToString() == "Done")
                ViewBag.Done = "Done";

            if (TempData["400"] != null && TempData["400"].ToString() == "400")
                ViewBag.Error = "400";

            if (TempData["401"] != null && TempData["401"].ToString() == "401")
                ViewBag.Error = "401";

            if (TempData["Error"] != null && TempData["Error"].ToString() == "Error")
                ViewBag.Error = "Error";

            return View(model);
            //}
            //else
            //{
            //    ViewBag.Duplicate= "Duplicate";
            //    return RedirectToAction("Index", "Dashboard");
            //}
        }

        [HttpPost]
        public ActionResult Index(Models.ViewModels.FollowerLogViewModel model)
        {
            var accountId = Utility.GetAccountId();

            if (accountId == 0)
                return RedirectToAction("Index", "Account");

            model.Log.AccountID = accountId;

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
                        FollowerLogHelper log = new FollowerLogHelper();
                        var options = new List<string>();

                        if (model.Log.Gender == null)
                            model.Log.Gender = (short)Models.GenderEnumModel.DontCare;
                        else
                            options.Add("Gender");

                        if (model.Log.Priority == null)
                            model.Log.Priority = (short)Models.PriorityEnumModel.DontCare;
                        else
                            options.Add("Priority");

                        if (model.Log.Geo == null)
                            model.Log.Geo = "0";
                        else
                            options.Add("Geo");

                        var logId = log.SaveLog((int)model.Log.AccountID, (long)model.Log.Count, model.Log.Geo, (short)model.Log.Gender, (short)model.Log.Priority);

                        Utility.SaveWalletPaymentLog((long)model.Log.Count, logId, Models.ActionEnumModel.Follower, options);

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
                    apiLog.SaveLog(Utility.GetAccountId(), 0, Models.ActionEnumModel.Follower + " - " + result.Desc, result.State);
                    return RedirectToAction("Index");
                }
                catch
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

            model.Islands = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Island")).ToList();
            model.Seas = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Sea")).ToList();
            model.Lakes = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Lake")).ToList();
            model.Provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get(x => x.Type.Equals("Province")).ToList();

            return View(model);
        }

    }
}