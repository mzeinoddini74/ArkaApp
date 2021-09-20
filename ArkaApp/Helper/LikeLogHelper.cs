using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class LikeLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public LikeLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public int GetLogID(int PostID, bool IsFinished = true)
        {
            return _unitOfWork.Repository<Models.EntityModels.LikeLog>().GetFirstOrDefault(x => x.PostID == PostID && x.IsFinished == IsFinished).LikeLogID;
        }

        public void SaveLog(int PostID,long PreviousCount, long Count, short GenderID = (short)Models.GenderEnumModel.DontCare, short PriorityID = (short)Models.PriorityEnumModel.DontCare)
        {
            _unitOfWork.Repository<Models.EntityModels.LikeLog>().Insert(new Models.EntityModels.LikeLog
            {
                PostID = PostID,
                Count = Count,
                CreatedDate = DateTime.Now,
                Gender = GenderID,
                Priority = PriorityID,
                PreviousCount = PreviousCount
            });
            _unitOfWork.SaveChanges();
        }

        public void UpdateLog(int LikeLogID, int PreviousCount, int NewCount, bool IsFinished = true)
        {
            var log = _unitOfWork.Repository<Models.EntityModels.LikeLog>().GetById(LikeLogID);

            log.PreviousCount = PreviousCount;
            log.IsFinished = IsFinished;

            _unitOfWork.Repository<Models.EntityModels.LikeLog>().Update(log);
            _unitOfWork.SaveChanges();
        }

    }
}