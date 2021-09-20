using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ArkaApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public AccountController()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        // GET: Account
        public ActionResult Index(string returnUrl, bool isDefault = true)
        {
            var user = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName.ToLower().Equals(User.Identity.Name));

            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(user.UserName,true);
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            var userData = authTicket.UserData.Split(',').ToArray();

            ViewBag.UserID = user.UserID;

            var model = new List<Models.EntityModels.Account>();

            model = _unitOfWork.Repository<Models.EntityModels.Account>().Get(x => x.UserID == user.UserID && x.IsEnabled == true && x.IsDeleted != true).ToList();

            if (isDefault)
            {
                if (model.Count > 0)
                {
                    foreach (var item in model)
                    {
                        if ((bool)item.IsDefault)
                        {
                            var newticket = new FormsAuthenticationTicket(authTicket.Version,
                                                                 authTicket.Name,
                                                                 authTicket.IssueDate,
                                                                 authTicket.Expiration,
                                                                 true,
                                                                 user.UserID + "," + user.UserRoleID + "," + item.AccountID,
                                                                 authTicket.CookiePath);

                            authCookie.Value = FormsAuthentication.Encrypt(newticket);
                            authCookie.Expires = newticket.Expiration.AddHours(24);
                            HttpContext.Response.Cookies.Set(authCookie);

                            if (Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);
                            else
                                return RedirectToAction("Index", "Dashboard");
                        }
                    }
                }
            }

            if (TempData["Done"] != null && TempData["Done"].ToString() == "Done")
                ViewBag.Done = "Done";

            return View(model);
        }

        public ActionResult Add(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.UserID = id;
            return View();
        }

        [HttpPost]
        public ActionResult Add(Models.EntityModels.Account model)
        {
            if (model.IsDefault == null)
                model.IsDefault = false;

            if (ModelState.IsValid)
            {
                if (!_unitOfWork.Repository<Models.EntityModels.Account>().Get().Any(x => x.AccountUserName.Equals(model.AccountUserName)))
                {
                    model.CreatedDate = DateTime.Now;
                    model.IsEnabled = true;

                    if ((bool)model.IsDefault)
                    {
                        var accounts = _unitOfWork.Repository<Models.EntityModels.Account>().Get(x => x.UserID == model.UserID).ToList();
                        if (accounts.Count > 0)
                        {
                            foreach (var item in accounts)
                            {
                                if ((bool)item.IsDefault)
                                {
                                    item.IsDefault = false;
                                }
                            }
                        }
                    }

                    try
                    {
                        model.FollowerCount = 0;
                        model.PostCount = 0;
                        _unitOfWork.Repository<Models.EntityModels.Account>().Insert(model);
                        _unitOfWork.SaveChanges();

                        TempData["Done"] = "Done";
                        return RedirectToAction("Index", new { isDefault = false });
                    }
                    catch
                    {
                        ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");
                        return View(model);
                    }
                }
                else
                    ModelState.AddModelError("AccountUserName", "حسابی با این نام برای شما در سیستم ثبت شده است..");
            }
            else
                ModelState.AddModelError("", "مقادیر ورودی نامعتبر است.");

            return View(model);
        }

        [HttpPost]
        public ActionResult DeactiveAccount(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            try
            {
                var account = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(id);
                account.IsDeleted = true;

                _unitOfWork.Repository<Models.EntityModels.Account>().Update(account);
                _unitOfWork.SaveChanges();

                TempData["Done"] = "Done";
                return RedirectToAction("Index", new { isDefault = false });
            }
            catch
            {
                return Json(new { result = false });
            }
        }

        public ActionResult SetAccountAsDefault(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            try
            {
                var account = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(id);

                var accounts = _unitOfWork.Repository<Models.EntityModels.Account>().Get(x => x.UserID == account.UserID).ToList();
                if (accounts.Count > 0)
                {
                    foreach (var item in accounts)
                    {
                        if ((bool)item.IsDefault)
                        {
                            item.IsDefault = false;
                        }
                    }
                }

                account.IsDefault = true;
                _unitOfWork.Repository<Models.EntityModels.Account>().Update(account);

                _unitOfWork.SaveChanges();

                TempData["Done"] = "Done";
                return RedirectToAction("Index", new { isDefault = false });
            }
            catch
            {
                return Json(new { result = false });
            }
        }

        public ActionResult SelectAccount(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName.ToLower().Equals(User.Identity.Name));

            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(user.UserName, true);
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            var userData = authTicket.UserData.Split(',').ToArray();

            var newticket = new FormsAuthenticationTicket(authTicket.Version,
                                                 authTicket.Name,
                                                 authTicket.IssueDate,
                                                 authTicket.Expiration,
                                                 true,
                                                 user.UserID + "," + user.UserRoleID + "," + id,
                                                 authTicket.CookiePath);

            authCookie.Value = FormsAuthentication.Encrypt(newticket);
            authCookie.Expires = newticket.Expiration.AddHours(24);
            HttpContext.Response.Cookies.Set(authCookie);

            return RedirectToAction("Index", "Dashboard");

        }
    }
}