using ArkaApp.Helper;
using ArkaApp.Models.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ArkaApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public DashboardController()
        {
            ViewBag.ActiveMenu = "dashboard";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            var accountId = Utility.GetAccountId();

            if (accountId == 0)
                return RedirectToAction("Index", "Account");

            if (TempData["Done"] != null && TempData["Done"].ToString() == "Done")
                ViewBag.Done = "Done";

            if (TempData["Duplicate"] != null && TempData["Duplicate"].ToString() == "Done")
                ViewBag.Duplicate = "Duplicate";

            //Call api to update account info

            var account = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(accountId);

            DashboadViewModel model = new DashboadViewModel();

            model.AccountID = account.AccountID;
            model.FollowerCount = (long)account.FollowerCount;
            model.PostCount = (long)account.PostCount;
            model.Provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get();

            var followerID = _unitOfWork.Repository<Models.EntityModels.Follower>().Get().OrderByDescending(x => x.Date).First().FollowerID;
            var details = _unitOfWork.Repository<Models.EntityModels.FollowerDetail>().Get(x => x.FollowerID == followerID).ToList();

            model.MapChart = new List<MapChartViewModel>();

            if (details.Count > 1)
            {
                foreach (var item in details)
                {
                    model.MapChart.Add(new MapChartViewModel
                    {
                        Province = item.Province,
                        Count = (long)item.Count
                    });
                }

                foreach (var item in model.Provinces)
                {
                    if (!model.MapChart.Any(x => x.Province.ProvinceID == item.ProvinceID))
                    {
                        model.MapChart.Add(new MapChartViewModel
                        {
                            Province = item,
                            Count = 0
                        });

                    }
                }
            }

            model.Avg = (long)model.MapChart.Average(x => x.Count);

            return View(model);
        }

        public ActionResult GetLineChartData(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.Account>().GetById(id);

            List<LineChartViewModel> ChartDataList = new List<LineChartViewModel>();

            if (model != null)
            {
                string constring = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                SqlConnection con = new SqlConnection(constring);
                string query = "SELECT CAST(Follower.[Date] AS date) AS Label, SUM(Follower.count) AS Value" +
                                " FROM dbo.Follower" +
                                " WHERE Follower.AccountID = " + id +
                                " GROUP BY CAST(Follower.[Date] AS date)";

                DataTable dt = new DataTable();
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.CommandTimeout = 36000;
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 1)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        var date = DateTime.Parse(item["Label"].ToString());

                        ChartDataList.Add(new LineChartViewModel
                        {
                            label = Helper.PersianHelper.GetPersinDate(date),
                            value = Convert.ToInt64(item["Value"])
                        });
                    }
                }

            }

            return Json(ChartDataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserProfile()
        {
            ViewBag.ActiveMenu = "profile";

            var user = _unitOfWork.Repository<Models.EntityModels.User>().GetFirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Models.EntityModels.User, Models.ViewModels.UserEditViewModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Models.ViewModels.UserEditViewModel>(user);

            if (TempData["Done"] != null && TempData["Done"].ToString() == "Done")
                ViewBag.Done = "Done";

            return View(model);
        }

        [HttpPost]
        public ActionResult UserProfile(Models.ViewModels.UserEditViewModel model, HttpPostedFileBase File)
        {

            if (ModelState.IsValid)
            {
                Models.EntityModels.User obj = _unitOfWork.Repository<Models.EntityModels.User>().GetById(model.UserID);

                if (obj.FullName.Equals(model.FullName) && obj.EmailAddress.Equals(model.EmailAddress) && obj.MobileNumber.Equals(model.MobileNumber) && File == null)
                    return RedirectToAction("UserProfile");


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
                obj.MobileNumber = model.MobileNumber;
                obj.EmailAddress = model.EmailAddress;

                try
                {
                    _unitOfWork.Repository<Models.EntityModels.User>().Update(obj);
                    _unitOfWork.SaveChanges();

                    TempData["Done"] = "Done";
                    return RedirectToAction("UserProfile");
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

        [HttpPost]
        public ActionResult DeletePicture(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            try
            {
                var user = _unitOfWork.Repository<Models.EntityModels.User>().GetById(id);

                if (System.IO.File.Exists(Server.MapPath(user.ProfilePicture)))
                    System.IO.File.Delete(Server.MapPath(user.ProfilePicture));

                user.ProfilePicture = null;
                _unitOfWork.Repository<Models.EntityModels.User>().Update(user);
                _unitOfWork.SaveChanges();

                return Json(new { result = true });
            }
            catch
            {
                return Json(new { result = false });
            }
        }

        public ActionResult GetPosts(string conName)
        {
            var accountId = Utility.GetAccountId();

            if (accountId == 0)
                return RedirectToAction("Index", "Account");

            //call api to get post again
            //Refresh databaes
            return RedirectToAction("Index", conName);
        }

        public ActionResult Pricing()
        {
            var model = new List<PricingViewModel>();

            var list = _unitOfWork.Repository<Models.EntityModels.Pricing>().Get().ToList();

            foreach (var item in list)
            {
                var details = _unitOfWork.Repository<Models.EntityModels.PricingDetail>().Get(x => x.PricingID == item.PricingID).ToList();
                model.Add(new PricingViewModel
                {
                    Pricing = item,
                    Details = details,
                    Total = details.Count > 0 ? item.Amount + details.Sum(x => x.Amount) : item.Amount
                });
            }

            return View(model);
        }
    }
}