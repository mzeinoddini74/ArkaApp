using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class DirectContentLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public DirectContentLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public void SaveLog(int ContentID, long Count, string Geo, short GenderID = (short)Models.GenderEnumModel.DontCare)
        {
            if (Geo == null)
                Geo = "0";

            _unitOfWork.Repository<Models.EntityModels.DirectContentLog>().Insert(new Models.EntityModels.DirectContentLog
            {
                ContentID = ContentID,
                CreatedDate = DateTime.Now,
                Geo = Geo,
                Count = Count,
                Gender = GenderID
            });
            _unitOfWork.SaveChanges();
        }

    }
}