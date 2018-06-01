using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APS.BusinessEntity.Models.Authentication;
using APS.BusinessEntity.Models.Common;
using System.Data;
using System.Data.Entity;
using System.Configuration;
using System.Globalization;

namespace APS.Repository.Authentication
{
    public class ActivityLoggingRepository : IDisposable
    {
        public void Dispose()
        {
            //Dispose(true); //I am calling you from Dispose, it's safe
            GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        }
        public void ActivityLogInsert(UserActivityLogModel activityModel)
        {
            APSEntities dbContext = new APSEntities();
            var objUserActivity = new APS_USER_ACT_LOG
            {
                USER_NAME = activityModel.User_Name,
                USER_ACT_LOG = activityModel.User_Act_Log,
                CREATED_BY = activityModel.CreatedBy,
                CREATED_TIME = activityModel.CreatedTime
            };
            dbContext.APS_USER_ACT_LOG.Add(objUserActivity);
            dbContext.SaveChanges();
        }
    }
}
