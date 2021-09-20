using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public class ExploreLogHelper
    {
        Models.Repositories.UnitOfWork _unitOfWork;
        public ExploreLogHelper()
        {
            _unitOfWork = new Models.Repositories.UnitOfWork();
        }

        public int GetLogID(int PostID, bool IsFinished = true)
        {
            return _unitOfWork.Repository<Models.EntityModels.ExploreLog>().GetFirstOrDefault(x => x.PostID == PostID).ExploreLogID;
        }

        public void SaveLog(int PostID, string FromDateFa, string EndDateFa)
        {
            var fromEn = PersianHelper.ToMiladi(FromDateFa);
            var endEn = PersianHelper.ToMiladi(EndDateFa);

            var diffCount = endEn.Date.Subtract(fromEn.Date);

            _unitOfWork.Repository<Models.EntityModels.ExploreLog>().Insert(new Models.EntityModels.ExploreLog
            {
                PostID = PostID,
                DateCount = diffCount.Days,
                CreatedDate = DateTime.Now,
                FromDateEn = fromEn,
                FromDateFa = FromDateFa,
                EndDateEn = endEn,
                EndDateFa = EndDateFa
            });
            _unitOfWork.SaveChanges();
        }
    }
}