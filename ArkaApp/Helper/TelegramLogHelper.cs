using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class TelegramLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public TelegramLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public void SaveLog(int PostID, long Count, string Geo, string GroupTitle, string Regions)
        {
            if (Geo == null)
                Geo = "0";

            _unitOfWork.Repository<Models.EntityModels.TelegramLog>().Insert(new Models.EntityModels.TelegramLog
            {
                PostID = PostID,
                CreatedDate = DateTime.Now,
                Geo = Geo,
                Count = Count,
                GroupTitle = GroupTitle,
                Regions = Regions
            });
            _unitOfWork.SaveChanges();
        }

    }
}