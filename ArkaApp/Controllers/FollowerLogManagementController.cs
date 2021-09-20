using ArkaApp.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ArkaApp.Controllers
{
    [Authorize]
    public class FollowerLogManagementController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public FollowerLogManagementController()
        {
            ViewBag.ActiveMenu = "followers";
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public ActionResult Index(FollowerSearchViewModel model)
        {
            const int RecordsPerPage = 10;

            List<Models.EntityModels.FollowerLog> FollowerLogs = _unitOfWork.Repository<Models.EntityModels.FollowerLog>().Get().ToList();

            if (!string.IsNullOrEmpty(model.SearchButton))
            {
                if (model.Id != 0)
                    FollowerLogs = FollowerLogs.Where(x => x.AccountID == model.Id).ToList();
            }

            var pageIndex = model.Page ?? 1;
            model.SearchResults = FollowerLogs.ToPagedList(pageIndex, RecordsPerPage);

            var Accounts = _unitOfWork.Repository<Models.EntityModels.Account>().Get(x => x.IsEnabled == true)
                .Select(s => new
                {
                    AccountID = s.AccountID,
                    Description = string.Format("{0} - {1}", s.AccountUserName, s.User.FullName)
                });

            model.Accounts = new SelectList(Accounts, "AccountID", "Description");

            return View(model);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.FollowerLog>().GetById(id);

            if (model.Geo != "0")
            {
                var provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get().ToList();
                List<string> GeoTitles = new List<string>();

                foreach (var item in model.Geo.Split(',').ToList())
                {
                    var geoId = Convert.ToInt32(item);
                    GeoTitles.Add(provinces.FirstOrDefault(x => x.ProvinceID == geoId).Title);
                }

                ViewBag.Geo = string.Join(",", GeoTitles);
            }
            else
                ViewBag.Geo = "ایران";

            return View(model);
        }
        public ActionResult LiveDetail(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.FollowerLog>().GetById(id);

            if (model.Geo != "0")
            {
                var provinces = _unitOfWork.Repository<Models.EntityModels.Province>().Get().ToList();
                List<string> GeoTitles = new List<string>();

                foreach (var item in model.Geo.Split(',').ToList())
                {
                    var geoId = Convert.ToInt32(item);
                    GeoTitles.Add(provinces.FirstOrDefault(x => x.ProvinceID == geoId).Title);
                }

                ViewBag.Geo = string.Join(",", GeoTitles);
            }
            else
                ViewBag.Geo = "ایران";

            return View(model);
        }

        public ActionResult GetLineChartData(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.FollowerLog>().GetById(id);

            List<LineChartViewModel> ChartDataList = new List<LineChartViewModel>();

            if (model != null)
            {
                ChartDataList.Add(new LineChartViewModel
                {
                    label = Helper.PersianHelper.GetPersinDate((DateTime)model.CreatedDate) + "(تاریخ ارسال درخواست)",
                    value = (long)model.PreviousCount
                });

                string constring = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                SqlConnection con = new SqlConnection(constring);
                string query = "SELECT CAST(FollowerLogDetail.[Date] AS date) AS Label, MIN(FollowerLogDetail.count) - (SELECT FollowerLog.PreviousCount FROM FollowerLog WHERE FollowerLogID = 1) + (MAX(FollowerLogDetail.Count) - MIN(FollowerLogDetail.count)) AS Value" +
                                " FROM dbo.FollowerLogDetail JOIN FollowerLog ON FollowerLog.FollowerLogID = FollowerLogDetail.FollowerLogID" +
                                " WHERE FollowerLogDetail.FollowerLogID = " + model.FollowerLogID +
                                " GROUP BY CAST(FollowerLogDetail.[Date] AS date)";

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
                            value = (long)model.PreviousCount + Convert.ToInt64(item["Value"])
                        });
                    }
                }
                else
                {
                    var dates = new List<DateTime>();

                    for (var date = model.CreatedDate; date <= model.FinishedDate; date = date.Value.AddDays(1))
                    {
                        dates.Add(date.Value);
                    }

                    long diff = (long)model.Count - (long)model.PreviousCount;
                    long num = diff / dates.Count;
                    long numUp = (long)Math.Ceiling((decimal)num) + 1;
                    long prevNum = (long)model.PreviousCount + numUp;

                    foreach (var date in dates)
                    {
                        ChartDataList.Add(new LineChartViewModel
                        {
                            label = Helper.PersianHelper.GetPersinDate(date),
                            value = prevNum
                        });

                        prevNum += numUp;
                    }
                }
            }

            return Json(ChartDataList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLiveChartData(int? id)
        {
            if (id == null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _unitOfWork.Repository<Models.EntityModels.FollowerLog>().GetById(id);
            var lastCount = model.PreviousCount;

            var details = _unitOfWork.Repository<Models.EntityModels.FollowerLogDetail>().Get(x => x.FollowerLogID == id).ToList();

            if (details.Count > 1)
                lastCount = details.OrderByDescending(x => x.Date).First().Count;

            //call api
            _unitOfWork.Repository<Models.EntityModels.FollowerLogDetail>().Insert(new Models.EntityModels.FollowerLogDetail
            {
                Count = lastCount + 500,
                Date = DateTime.Now,
                FollowerLogID = id
            });
            _unitOfWork.SaveChanges();

            var result = new LiveChartViewModel();
            result.Chart = new List<LineChartViewModel>();
            result.IsActive = true;
            if (model != null)
            {
                result.Chart.Add(new LineChartViewModel
                {
                    label = Helper.PersianHelper.GetPersinDateTime((DateTime)model.CreatedDate, false) + "(تاریخ ارسال درخواست)",
                    value = (long)model.PreviousCount
                });

                string constring = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                SqlConnection con = new SqlConnection(constring);


                string query = "SELECT FollowerLogDetail.[Date] AS Label, FollowerLogDetail.Count AS Value" +
                                " FROM dbo.FollowerLogDetail JOIN FollowerLog ON FollowerLog.FollowerLogID = FollowerLogDetail.FollowerLogID" +
                                " WHERE FollowerLogDetail.FollowerLogID = " + model.FollowerLogID;

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

                        result.Chart.Add(new LineChartViewModel
                        {
                            label = Helper.PersianHelper.GetPersinDateTime(date, false),
                            value = Convert.ToInt64(item["Value"])
                        });
                    }
                }
            }

            //if (apiModel.IsFinished)
            //    result.IsActive = false;

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}