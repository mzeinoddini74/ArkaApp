using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class FollowerLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public FollowerLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }
        public int SaveLog(int AccountID, long Count, string Geo, short GenderID = (short)Models.GenderEnumModel.DontCare, short PriorityID = (short)Models.PriorityEnumModel.DontCare)
        {
            var model = new Models.EntityModels.FollowerLog
            {
                AccountID = AccountID,
                Count = Count,
                CreatedDate = DateTime.Now,
                Gender = GenderID,
                Geo = Geo,
                Priority = PriorityID,
                IsFinished = false
            };
            _unitOfWork.Repository<Models.EntityModels.FollowerLog>().Insert(model);
            _unitOfWork.SaveChanges();

            return model.FollowerLogID;
        }

    }
}