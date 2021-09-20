using ArkaApp.Helper;
using ArkaApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mail;
using System.Web.Mvc;
using System.Web.Security;

namespace ArkaApp.Controllers
{
    public class HomeController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public HomeController()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        // GET: Home
        [AllowAnonymous]
        public ActionResult Index(string returnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            if (TempData["EmailSent"] != null && TempData["EmailSent"].ToString() == "Done")
                ViewBag.EmailSent = "Done";

            if (TempData["Done"] != null && TempData["Done"].ToString() == "Done")
                ViewBag.Done = "Done";

            if (TempData["Failed"] != null && TempData["Failed"].ToString() == "Failed")
                ViewBag.Failed = "Failed";

            if (TempData["LinkExpired"] != null && TempData["LinkExpired"].ToString() == "Done")
                ViewBag.LinkExpired = "Done";


            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(LoginViewModel model, string returnUrl = "")
        {
            List<ErrorViewModel> error = new List<ErrorViewModel>();
            if (ModelState.IsValid)
            {
                var query = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(u => u.UserName.ToLower() == model.UserName.Trim().ToLower());
                if (query != null)
                {
                    if (query.IsEnabled == true)
                    {
                        if (query.Password == Utility.GetHashed(model.Password, Settings.Setting.HashKey, Utility.GetSaltPass(query.UserName, query.SaltPassword)))
                        {
                            string userData = query.UserID + "," + query.UserRoleID;

                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                              query.UserName,
                              DateTime.Now,
                              DateTime.Now.AddMinutes(30),
                              model.Rememeber,
                              userData,
                              FormsAuthentication.FormsCookiePath);

                            string encTicket = FormsAuthentication.Encrypt(ticket);

                            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                            string decodedUrl = "";
                            if (!string.IsNullOrEmpty(returnUrl))
                                decodedUrl = Server.UrlDecode(returnUrl);

                            if (query.UserRoleID == (int)Models.UserRolesEnumModel.Admin || query.UserRoleID == (int)Models.UserRolesEnumModel.SuperAdmin)
                                return Json(new { url = Url.Action("Index", "Dashboard") });
                            else
                                return Json(new { url = Url.Action("Index", "Account", new { returnUrl = decodedUrl }) });
                        }
                        else
                            error.Add(new ErrorViewModel { Message = "نام کاربری یا رمز عبور شما نامتعبر است." });
                    }
                    else
                        error.Add(new ErrorViewModel { Message = "حساب کاربری شما غیر فعال است." });
                }
                else
                    error.Add(new ErrorViewModel { Message = "نام کاربری یا رمز عبور شما نامتعبر است." });
            }
            else
                error.Add(new ErrorViewModel { Message = "مقادیر ورودی نامعتبر است." });

            return PartialView("_ErrorPartialView", error);
        }

        [Authorize]
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
                ModelState.AddModelError("UserName", "لطفا نام کاربری را وارد نمایید.");
            else
            {
                var user = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName.Equals(UserName));

                if (user == null)
                    ModelState.AddModelError("", "کاربری یافت نشد.");
                else
                {
                    if (user.EmailAddress == null)
                        ModelState.AddModelError("", "آدرس ایمیلی برای شما یافت نشد. لطفا با تیم پشتیبانی تماس بگیرید.");
                    else
                    {
                        try
                        {
                            user.AuthToken = Guid.NewGuid().ToString().Replace("-", "");
                            user.AuthTokenExpiration = DateTime.Now.AddHours(24);
                            _unitOfWork.Repository<Models.EntityModels.User>().Update(user);
                            _unitOfWork.SaveChanges();

                            string body = EmailManager.RenderPartialView("Email", "ForgotPassword", user);
                            EmailManager.Send(user.EmailAddress, "بازیابی رمز عبور", body);
                            TempData["EmailSent"] = "Done";
                            return RedirectToAction("Index");
                        }
                        catch
                        {
                            ModelState.AddModelError("", "در ارسال ایمیل مشکلی پیش آمده است. لطفا با تیم پشتیبانی تماس بگیرید.");
                        }
                    }
                }
            }

            return View();
        }

        public ActionResult ChangePassword(string token)
        {
            if (string.IsNullOrEmpty(token))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.AuthToken.Equals(token));

            if (user.AuthTokenExpiration < DateTime.Now)
            {
                TempData["LinkExpired"] = "Done";
                return RedirectToAction("Index");
            }

            UserChangePasswordViewModel model = new UserChangePasswordViewModel();
            model.UserID = user.UserID;
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(UserChangePasswordViewModel model)
        {
            var user = _unitOfWork.Repository<Models.EntityModels.User>().GetById(model.UserID);

            try
            {
                user.AuthToken = null;
                user.AuthTokenExpiration = null;
                user.SaltPassword = Utility.MakeSaltPassword();
                user.Password = Utility.GetHashed(model.Password, Settings.Setting.HashKey, Utility.GetSaltPass(user.UserName, user.SaltPassword));

                _unitOfWork.Repository<Models.EntityModels.User>().Update(user);
                _unitOfWork.SaveChanges();
            }
            catch
            {
                TempData["Failed"] = "Failed";
                return RedirectToAction("Index");
            }
            TempData["Done"] = "Done";
            return RedirectToAction("Index");
        }
    }
}