using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mail;
using APS.BusinessEntity.Models.Common;
using APS.Repository.Authentication;
using System.Configuration;


namespace APS.Common.Logging
{
    public class ActivityLoggingComponent : IDisposable
    {
        public void Dispose()
        {
            //Dispose(true); //I am calling you from Dispose, it's safe
            GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        }
        public void ActivityLog(UserActivityLogModel userActivity)
        {
            try
            {
                using (ActivityLoggingRepository repo = new ActivityLoggingRepository())
                {
                    repo.ActivityLogInsert(userActivity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
