using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ArkaApp.Controllers
{
    [Authorize]
    public class AccountManagementController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public AccountManagementController()
        {
            ViewBag.ActiveMenu = "accounts";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public ActionResult Index(Models.ViewModels.AccountSearchViewModel model)
        {
            const int RecordsPerPage = 10;

            List<Models.EntityModels.Account> Accounts = _unitOfWork.Repository<Models.EntityModels.Account>().Get(x => x.User.IsEnabled == true).ToList();

            if (!string.IsNullOrEmpty(model.SearchButton))
            {
                if (model.UserID != 0)
                    Accounts = Accounts.Where(x => x.UserID == model.UserID).ToList();

                if (model.UserName != null)
                    Accounts = Accounts.Where(x => x.AccountUserName.ToLower().Contains(model.UserName.ToLower())).ToList();

                if (model.IsEnabled == (int)Models.IsEnabledEnumModel.DeActive)
                    Accounts = Accounts.Where(x => x.IsEnabled == false).ToList();

                else if (model.IsEnabled == (int)Models.IsEnabledEnumModel.Active)
                    Accounts = Accounts.Where(x => x.IsEnabled == true).ToList();
            }

            var pageIndex = model.Page ?? 1;
            model.SearchResults = Accounts.ToPagedList(pageIndex, RecordsPerPage);

            var Users = _unitOfWork.Repository<Models.EntityModels.User>().Get(x => x.IsEnabled == true);
            model.Users = new SelectList(Users, "UserID", "FullName");

            List<SelectListItem> IsEnabled = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "انتخاب وضعیت", Value = "2", Selected = true
                },
                new SelectListItem {
                    Text = "فعال", Value = "1"
                },
                new SelectListItem {
                    Text = "غیرفعال", Value = "0"
                }
            };
            ViewBag.IsEnabled = IsEnabled;

            if (TempData["Done"] != null && TempData["Done"].ToString() == "Done")
                ViewBag.Done = "Done";

            return View(model);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(id);

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var Users = _unitOfWork.Repository<Models.EntityModels.User>().Get(x => x.IsEnabled == true);
            ViewBag.Users = new SelectList(Users, "UserID", "FullName");

            var model = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Models.EntityModels.Account model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Repository<Models.EntityModels.Account>().Update(model);
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

        public ActionResult Add()
        {
            var Users = _unitOfWork.Repository<Models.EntityModels.User>().Get(x => x.IsEnabled == true);
            ViewBag.Users = new SelectList(Users, "UserID", "FullName");

            return View();
        }

        [HttpPost]
        public ActionResult Add(Models.EntityModels.Account model)
        {
            var Users = _unitOfWork.Repository<Models.EntityModels.User>().Get(x => x.IsEnabled == true);
            ViewBag.Users = new SelectList(Users, "UserID", "FullName");

            if (ModelState.IsValid)
            {
                if (!_unitOfWork.Repository<Models.EntityModels.Account>().Get().Any(x => x.AccountUserName.Equals(model.AccountUserName) && x.UserID == model.UserID))
                {
                    var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name).UserID;

                    model.CreatedDate = DateTime.Now;

                    try
                    {
                        _unitOfWork.Repository<Models.EntityModels.Account>().Insert(model);
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
                    ModelState.AddModelError("AccountUserName", "حسابی با این نام کاربری برای این کاربر در سیستم ثبت شده است.");
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            return View(model);
        }

    }
}