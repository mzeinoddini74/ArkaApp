using System;

namespace ArkaApp.Helper
{
    public class TelegramUserLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public TelegramUserLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public void SaveLog(int PostID, string Users)
        {
            _unitOfWork.Repository<Models.EntityModels.TelegramUserLog>().Insert(new Models.EntityModels.TelegramUserLog
            {
                PostID = PostID,
                CreatedDate = DateTime.Now,
                Users = Users
            });
            _unitOfWork.SaveChanges();
        }

    }

}