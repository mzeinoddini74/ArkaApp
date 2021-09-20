using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class ApiLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public ApiLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public void SaveLog(int AccountID, int PostID, string Description, int State)
        {

            _unitOfWork.Repository<Models.EntityModels.ApiLog>().Insert(new Models.EntityModels.ApiLog
            {
                AccountID = AccountID,
                CreatedDate = DateTime.Now,
                Description = Description,
                PostID = PostID,
                State = State
            });
            _unitOfWork.SaveChanges();
        }

    }
}