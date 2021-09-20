using System;

namespace ArkaApp.Helper
{
    public class TelegramContentLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public TelegramContentLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public void SaveLog(int ContentID, long Count, string Geo, string GroupTitle, string Regions)
        {
            if (Geo == null)
                Geo = "0";

            _unitOfWork.Repository<Models.EntityModels.TelegramContentLog>().Insert(new Models.EntityModels.TelegramContentLog
            {
                ContentID = ContentID,
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