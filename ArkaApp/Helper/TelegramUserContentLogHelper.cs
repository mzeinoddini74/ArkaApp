using System;

namespace ArkaApp.Helper
{
    public class TelegramUserContentLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public TelegramUserContentLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public void SaveLog(int ContentID, string Users)
        {
            _unitOfWork.Repository<Models.EntityModels.TelegramUserContentLog>().Insert(new Models.EntityModels.TelegramUserContentLog
            {
                ContentID = ContentID,
                CreatedDate = DateTime.Now,
                Users = Users
            });
            _unitOfWork.SaveChanges();
        }

    }

}