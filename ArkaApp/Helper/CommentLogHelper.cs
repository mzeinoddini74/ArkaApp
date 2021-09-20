using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class CommentLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public CommentLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public int SaveLog(int PostID, long Count, short GenderID = (short)Models.GenderEnumModel.DontCare, short PriorityID = (short)Models.PriorityEnumModel.DontCare)
        {
            var model = new Models.EntityModels.CommentLog
            {
                PostID = PostID,
                Count = Count,
                CreatedDate = DateTime.Now,
                Gender = GenderID,
            };
            _unitOfWork.Repository<Models.EntityModels.CommentLog>().Insert(model);
            _unitOfWork.SaveChanges();

            return model.CommentLogID;
        }
    }
}