using ArkaApp.Helper;
using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ArkaApp.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public UserManagementController()
        {
            ViewBag.ActiveMenu = "users";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        // GET: UserManagement
        public ActionResult Index(Models.ViewModels.UserSearchViewModel model)
        {
            const int RecordsPerPage = 10;

            List<Models.ViewModels.UserViewModel> Users = _unitOfWork.Repository<Models.EntityModels.User>()
                .Get()
                .Select(x => new Models.ViewModels.UserViewModel
                {
                    User = x,
                    CreatedBy = x.CreatedBy != null ? _unitOfWork.Repository<Models.EntityModels.User>().GetById(x.CreatedBy).FullName : "سیستم"
                }).ToList();

            var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);

            if (currentUser.UserRoleID != (int)Models.UserRolesEnumModel.SuperAdmin)
                Users.Where(x => x.User.UserRoleID != (int)Models.UserRolesEnumModel.SuperAdmin);

            if (!string.IsNullOrEmpty(model.SearchButton))
            {
                if (model.CreatedBy != 0)
                    Users = Users.Where(x => x.User.CreatedBy == model.CreatedBy).ToList();

                if (model.UserRoleID != 0)
                    Users = Users.Where(x => x.User.UserRoleID == model.UserRoleID).ToList();

                if (model.UserName != null)
                    Users = Users.Where(x => x.User.UserName.ToLower().Contains(model.UserName.ToLower())).ToList();

                if (model.EmailAddress != null)
                    Users = Users.Where(x => x.User.EmailAddress.ToLower().Contains(model.EmailAddress.ToLower())).ToList();

                if (model.FullName != null)
                    Users = Users.Where(x => x.User.FullName.ToLower().Contains(model.FullName.ToLower())).ToList();

                if (model.IsEnabled == (int)Models.IsEnabledEnumModel.DeActive)
                    Users = Users.Where(x => x.User.IsEnabled == false).ToList();

                else if (model.IsEnabled == (int)Models.IsEnabledEnumModel.Active)
                    Users = Users.Where(x => x.User.IsEnabled == true).ToList();
            }

            var pageIndex = model.Page ?? 1;
            model.SearchResults = Users.ToPagedList(pageIndex, RecordsPerPage);

            var Creators = _unitOfWork.Repository<Models.EntityModels.User>().Get(x => x.IsEnabled == true);
            model.Creators = new SelectList(Creators, "UserID", "FullName");

            var Roles = _unitOfWork.Repository<Models.EntityModels.UserRole>().Get(x => x.IsEnabled == true);
            model.UserRoles = new SelectList(Roles, "UserRoleID", "TitleFa");

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

            Models.ViewModels.UserViewModel model = new Models.ViewModels.UserViewModel();
            model.User = _unitOfWork.Repository<Models.EntityModels.User>().GetById(id);

            var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);

            if (currentUser.UserRoleID != (int)Models.UserRolesEnumModel.Admin && currentUser.UserRoleID == (int)Models.UserRolesEnumModel.Admin)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            model.CreatedBy = model.User.CreatedBy != null ? _unitOfWork.Repository<Models.EntityModels.User>().GetById(model.User.CreatedBy).FullName : "سیستم";
            model.UpdatedBy = model.User.UpdatedBy != null ? _unitOfWork.Repository<Models.EntityModels.User>().GetById(model.User.UpdatedBy).FullName : string.Empty;

            var Roles = _unitOfWork.Repository<Models.EntityModels.UserRole>().Get(x => x.IsEnabled == true);
            ViewBag.UserRoles = new SelectList(Roles, "UserRoleID", "TitleFa");

            ViewBag.UserRole = currentUser.UserRole.Title;

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = _unitOfWork.Repository<Models.EntityModels.User>().GetById(id);

            var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);

            if (currentUser.UserRoleID != (int)Models.UserRolesEnumModel.Admin && user.UserRoleID == (int)Models.UserRolesEnumModel.Admin)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Models.EntityModels.User, Models.ViewModels.UserEditViewModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Models.ViewModels.UserEditViewModel>(user);

            var Roles = _unitOfWork.Repository<Models.EntityModels.UserRole>().Get(x => x.IsEnabled == true);
            ViewBag.UserRoles = new SelectList(Roles, "UserRoleID", "Title");

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Models.ViewModels.UserEditViewModel model, HttpPostedFileBase File)
        {
            var Roles = _unitOfWork.Repository<Models.EntityModels.UserRole>().Get(x => x.IsEnabled == true);
            ViewBag.UserRoles = new SelectList(Roles, "UserRoleID", "TitleFa");

            if (ModelState.IsValid)
            {
                Models.EntityModels.User obj = _unitOfWork.Repository<Models.EntityModels.User>().GetById(model.UserID);

                if (File != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(obj.ProfilePicture)))
                        System.IO.File.Delete(Server.MapPath(obj.ProfilePicture));

                    string name = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(File.FileName);
                    File.SaveAs(Server.MapPath("/Uploads/Users/" + name));
                    obj.ProfilePicture = "/Uploads/Users/" + name;

                }

                var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name).UserID;

                obj.UpdatedBy = currentUser;
                obj.UpdatedDate = DateTime.Now;
                obj.FullName = model.FullName;
                obj.EmailAddress = model.EmailAddress;
                obj.IsEnabled = model.IsEnabled;
                obj.UserRoleID = model.UserRoleID;
                obj.MobileNumber = model.MobileNumber;

                try
                {
                    _unitOfWork.Repository<Models.EntityModels.User>().Update(obj);
                    _unitOfWork.SaveChanges();

                    TempData["Done"] = "Done";
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");
                    return View(model);
                }

            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            try
            {
                var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);
                var user = _unitOfWork.Repository<Models.EntityModels.User>().GetById(id);

                if (currentUser.UserRoleID != (int)Models.UserRolesEnumModel.Admin && user.UserRoleID == (int)Models.UserRolesEnumModel.Admin)
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                var ProfilePicture = _unitOfWork.Repository<Models.EntityModels.User>().GetById(id).ProfilePicture;

                if (System.IO.File.Exists(Server.MapPath("/Uploads/Users/" + ProfilePicture)))
                    System.IO.File.Delete(Server.MapPath("/Uploads/Users/" + ProfilePicture));

                _unitOfWork.Repository<Models.EntityModels.User>().Delete(id);
                _unitOfWork.SaveChanges();

                return Json(new { result = true });
            }
            catch
            {
                return Json(new { result = false });

            }
        }

        public ActionResult Add()
        {
            var Roles = _unitOfWork.Repository<Models.EntityModels.UserRole>().Get(x => x.IsEnabled == true);
            ViewBag.UserRoles = new SelectList(Roles, "UserRoleID", "Title");

            return View();
        }

        [HttpPost]
        public ActionResult Add(Models.EntityModels.User model, HttpPostedFileBase File)
        {
            var Roles = _unitOfWork.Repository<Models.EntityModels.UserRole>().Get(x => x.IsEnabled == true);
            ViewBag.UserRoles = new SelectList(Roles, "UserRoleID", "TitleFa");

            if (ModelState.IsValid)
            {
                if (!_unitOfWork.Repository<Models.EntityModels.User>().Get().Any(x => x.UserName.Equals(model.UserName)))
                {
                    if (File != null)
                    {
                        string name = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(File.FileName);
                        File.SaveAs(Server.MapPath("/Uploads/Users/" + name));
                        model.ProfilePicture = "/Uploads/Users/" + name;
                    }

                    var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name).UserID;

                    model.CreatedBy = currentUser;
                    model.CreatedDate = DateTime.Now;

                    model.SaltPassword = Utility.MakeSaltPassword();
                    model.Password = Utility.GetHashed(model.Password, Settings.Setting.HashKey, Utility.GetSaltPass(model.UserName, model.SaltPassword));

                    try
                    {
                        _unitOfWork.Repository<Models.EntityModels.User>().Insert(model);
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
                    ModelState.AddModelError("UserName", "کاربری با این نام کاربری در سیستم ثبت شده است..");
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            return View(model);
        }

        public ActionResult SuperEdit(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);

            if (currentUser.UserRoleID != (int)Models.UserRolesEnumModel.SuperAdmin)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var user = _unitOfWork.Repository<Models.EntityModels.User>().GetById(id);

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Models.EntityModels.User, Models.ViewModels.UserSuperEditViewModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Models.ViewModels.UserSuperEditViewModel>(user);

            return View(model);
        }

        [HttpPost]
        public ActionResult SuperEdit(Models.ViewModels.UserSuperEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Models.EntityModels.User obj = _unitOfWork.Repository<Models.EntityModels.User>().GetById(model.UserID);

                var currentUser = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name).UserID;

                obj.UpdatedBy = currentUser;
                obj.UpdatedDate = DateTime.Now;
                obj.UserName = model.UserName;

                obj.SaltPassword = Utility.MakeSaltPassword();
                obj.Password = Utility.GetHashed(model.Password, Settings.Setting.HashKey, Utility.GetSaltPass(model.UserName, obj.SaltPassword));

                try
                {
                    _unitOfWork.Repository<Models.EntityModels.User>().Update(obj);
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

    }
}