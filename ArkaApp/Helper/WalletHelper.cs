using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class WalletHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public WalletHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }
        public void SaveChargeLog(int AccountID, decimal Amount)
        {
            _unitOfWork.Repository<Models.EntityModels.WalletChargeLog>().Insert(new Models.EntityModels.WalletChargeLog
            {
                AccountID = AccountID,
                Amount = Amount,
                CreatedDate = DateTime.Now
            });
            _unitOfWork.SaveChanges();
        }

        public void SavePaymentLog(int AccountID, decimal Amount, int LogID, string LogType)
        {
            _unitOfWork.Repository<Models.EntityModels.WalletPaymentLog>().Insert(new Models.EntityModels.WalletPaymentLog
            {
                AccountID = AccountID,
                Amount = Amount,
                CreatedDate = DateTime.Now,
                LogID = LogID,
                LogType = LogType
            });
            _unitOfWork.SaveChanges();
        }

        public decimal GetPackageAmount(int PricingId, long Count, List<string> Options)
        {
            var pricing = _unitOfWork.Repository<Models.EntityModels.Pricing>().GetById(PricingId);
            var details = _unitOfWork.Repository<Models.EntityModels.PricingDetail>().Get(x => x.PricingID == pricing.PricingID).ToList();

            decimal total = 0;

            var diff = Count / pricing.Count;
            total = diff * pricing.Amount;

            if (Options.Count > 0)
            {
                if (details.Count > 0)
                {
                    foreach (var item in details)
                    {
                        if (Options.Exists(x => x == item.Title))
                        {
                            total += diff * item.Amount;
                        }
                    }
                }
            }

            return total;
        }
    }
}