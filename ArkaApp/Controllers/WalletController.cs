using ArkaApp.Helper;
using ArkaApp.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArkaApp.Controllers
{
    public class WalletController : Controller
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public WalletController()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        //Charge
        public ActionResult Index(ChargeSearchViewModel model)
        {
            var accountId = Utility.GetAccountId();

            if (accountId == 0)
                return RedirectToAction("Index", "Account");

            const int RecordsPerPage = 10;

            var charges = _unitOfWork.Repository<Models.EntityModels.WalletChargeLog>().Get(x => x.AccountID == accountId).OrderByDescending(x => x.CreatedDate).ToList();

            var pageIndex = model.Page ?? 1;
            model.SearchResults = charges.ToPagedList(pageIndex, RecordsPerPage);

            return View(model);
        }

        public ActionResult Payment(PaymentSearchViewModel model)
        {
            var accountId = Utility.GetAccountId();

            if (accountId == 0)
                return RedirectToAction("Index", "Account");

            const int RecordsPerPage = 10;

            var charges = _unitOfWork.Repository<Models.EntityModels.WalletPaymentLog>().Get(x => x.AccountID == accountId).OrderByDescending(x => x.CreatedDate).ToList();

            var pageIndex = model.Page ?? 1;
            model.SearchResults = charges.ToPagedList(pageIndex, RecordsPerPage);

            return View(model);
        }

        public ActionResult Charge()
        {
            List<SelectListItem> Amounts = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "100,000 تومان", Value = "100000", Selected = true
                },
                new SelectListItem {
                    Text = "300,000 تومان", Value = "300000"
                },
                new SelectListItem {
                    Text = "500,000 تومان", Value = "500000"
                },
                new SelectListItem {
                    Text = "1,000,000 تومان", Value = "1000000"
                },
            };
            ViewBag.Amounts = Amounts;

            return View();
        }

        [HttpPost]
        public ActionResult Charge(ChargeWalletViewModel model)
        {
            decimal final = 0;
            Decimal.TryParse(model.Amount, out final);

            
            return View();
        }
    }
}