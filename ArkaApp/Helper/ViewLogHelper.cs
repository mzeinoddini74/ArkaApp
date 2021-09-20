using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class ViewLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public ViewLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public int GetLogID(int PostID, bool IsFinished = true)
        {
            return _unitOfWork.Repository<Models.EntityModels.ViewLog>().GetFirstOrDefault(x => x.PostID == PostID && x.IsFinished == IsFinished).ViewLogID;
        }

        public void SaveLog(int PostID, long Count, short PriorityID = (short)Models.PriorityEnumModel.DontCare)
        {

            _unitOfWork.Repository<Models.EntityModels.ViewLog>().Insert(new Models.EntityModels.ViewLog
            {
                PostID = PostID,
                Count = Count,
                CreatedDate = DateTime.Now,
                Priority = PriorityID,
                IsFinished = false
            });
            _unitOfWork.SaveChanges();
        }

        public void UpdateLog(int ViewLogID, int PreviousCount, int NewCount, bool IsFinished = true)
        {
            var log = _unitOfWork.Repository<Models.EntityModels.ViewLog>().GetById(ViewLogID);

            log.PreviousCount = PreviousCount;
            log.IsFinished = IsFinished;

            _unitOfWork.Repository<Models.EntityModels.ViewLog>().Update(log);
            _unitOfWork.SaveChanges();
        }

    }
}