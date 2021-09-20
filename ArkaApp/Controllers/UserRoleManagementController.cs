using AutoMapper;
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
    public class UserRoleManagementController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public UserRoleManagementController()
        {
            ViewBag.ActiveMenu = "userRoles";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public ActionResult Index(Models.ViewModels.UserRoleSearchViewModel model)
        {
            const int RecordsPerPage = 10;

            List<Models.ViewModels.UserRoleViewModel> UserRoles = _unitOfWork.Repository<Models.EntityModels.UserRole>()
                .Get()
                .Select(x => new Models.ViewModels.UserRoleViewModel
                {
                    UserRole = x,
                    CreatedBy = _unitOfWork.Repository<Models.EntityModels.User>().GetById(x.CreatedBy).FullName 
                }).ToList();

            var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);

            if (currentUser.UserRoleID != (int)Models.UserRolesEnumModel.SuperAdmin)
                UserRoles.Where(x => x.UserRole.UserRoleID != (int)Models.UserRolesEnumModel.SuperAdmin);

            if (!string.IsNullOrEmpty(model.SearchButton))
            {
                if (model.CreatedBy != 0)
                    UserRoles = UserRoles.Where(x => x.UserRole.CreatedBy == model.CreatedBy).ToList();

                if (model.Title != null)
                    UserRoles = UserRoles.Where(x => x.UserRole.Title.ToLower().Contains(model.Title.ToLower())).ToList();

                if (model.TitleFa != null)
                    UserRoles = UserRoles.Where(x => x.UserRole.TitleFa.ToLower().Contains(model.TitleFa.ToLower())).ToList();

                if (model.IsEnabled == (int)Models.IsEnabledEnumModel.DeActive)
                    UserRoles = UserRoles.Where(x => x.UserRole.IsEnabled == false).ToList();

                else if (model.IsEnabled == (int)Models.IsEnabledEnumModel.Active)
                    UserRoles = UserRoles.Where(x => x.UserRole.IsEnabled == true).ToList();
            }

            var pageIndex = model.Page ?? 1;
            model.SearchResults = UserRoles.ToPagedList(pageIndex, RecordsPerPage);

            var Creators = _unitOfWork.Repository<Models.EntityModels.User>().Get(x => x.IsEnabled == true);
            model.Creators = new SelectList(Creators, "UserID", "FullName");

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

            ViewBag.UserRole = currentUser.UserRole.Title;

            return View(model);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Models.ViewModels.UserRoleViewModel model = new Models.ViewModels.UserRoleViewModel();
            model.UserRole = _unitOfWork.Repository<Models.EntityModels.UserRole>().GetById(id);

            model.CreatedBy = _unitOfWork.Repository<Models.EntityModels.User>().GetById(model.UserRole.CreatedBy).FullName;
            model.UpdatedBy = model.UserRole.UpdatedBy != null ? _unitOfWork.Repository<Models.EntityModels.User>().GetById(model.UserRole.UpdatedBy).FullName : string.Empty;

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.UserRole>().GetById(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Models.EntityModels.UserRole model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name).UserID;

                model.UpdatedBy =currentUser;
                model.UpdatedDate = DateTime.Now;

                try
                {
                    _unitOfWork.Repository<Models.EntityModels.UserRole>().Update(model);
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

        public ActionResult Users(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.User>().Get(x => x.UserRoleID == id).ToList();

            ViewBag.RoleTitle = _unitOfWork.Repository<Models.EntityModels.UserRole>().GetById(id).TitleFa;
            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Models.EntityModels.UserRole model)
        {
            if (ModelState.IsValid)
            {
                if (!_unitOfWork.Repository<Models.EntityModels.UserRole>().Get().Any(x => x.Title.Equals(model.Title)))
                {
                    var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name).UserID;

                    model.CreatedBy = currentUser;
                    model.CreatedDate = DateTime.Now;

                    try
                    {
                        _unitOfWork.Repository<Models.EntityModels.UserRole>().Insert(model);
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
                    ModelState.AddModelError("Title", "کاربری با این نام کاربری در سیستم ثبت شده است..");
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            return View(model);
        }

    }
}