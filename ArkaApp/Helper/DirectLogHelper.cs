using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class DirectLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public DirectLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public void SaveLog(int PostID, long Count, string Geo, short GenderID = (short)Models.GenderEnumModel.DontCare)
        {
            if (Geo == null)
                Geo = "0";

            _unitOfWork.Repository<Models.EntityModels.DirectLog>().Insert(new Models.EntityModels.DirectLog
            {
                PostID = PostID,
                CreatedDate = DateTime.Now,
                Geo = Geo,
                Count = Count,
                Gender = GenderID
            });
            _unitOfWork.SaveChanges();
        }

    }
}