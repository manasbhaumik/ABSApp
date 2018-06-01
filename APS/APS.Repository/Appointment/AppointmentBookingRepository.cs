using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Configuration;
using System.Globalization;
using APS.BusinessEntity.Models.Appointment;
using APS.BusinessEntity.Constants;
using APS.BusinessEntity.Models.Authentication;
using APS.Repository.Authentication;

namespace APS.Repository.Appointment
{
    public class AppointmentBookingRepository : IDisposable
    {
        public void Dispose()
        {
            //Dispose(true); //I am calling you from Dispose, it's safe
            GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        }

        public IEnumerable<TimeSlotModel> GetAppointmentTimeSlots(string dayOfWeek)
        {
            dayOfWeek = DateTime.Now.DayOfWeek.ToString();
            APSEntities dbContext = new APSEntities();
            var objTimeSlots = dbContext.V_TIMESLOT_DETAILS.Where(i => i.TIMESLOT_WEEKDAY == dayOfWeek).ToArray();
            if (objTimeSlots != null && objTimeSlots.Count() > 0)
            {
                return objTimeSlots.Select(u => u.ToTimeSlotDetailsModel());
            }
            else
            {
                return null;
            }
        }

        public APS.BusinessEntity.Models.Appointment.AppointmentModel GetAppointmentDetails(string UserName)
        {
            APSEntities dbContext = new APSEntities();
            UserInfoModel userInfo = new UserInfoModel();
            userInfo.UserName = UserName;
            using (AuthenticationRepository authRepo = new AuthenticationRepository())
            {
                userInfo = authRepo.GetUserInfoDetails(userInfo);
            }
            var appointments = dbContext.V_APPOINTMENT_DETAILS.Where(i => i.APP_USERNAME == UserName).FirstOrDefault();
            if (appointments != null)
            {
                var model = appointments.ToAppointmentModel();
                model.UserInfoDetails = userInfo;
                return model;
            }
            else
            {
                APS.BusinessEntity.Models.Appointment.AppointmentModel model = new APS.BusinessEntity.Models.Appointment.AppointmentModel();
                model.IsAppointmentAvailable = false;
                model.UserInfoDetails = userInfo;
                return model;
            }
        }

        public TimeSlotModel GetTimeSlotDetails(int timeSlotID)
        {
            APSEntities dbContext = new APSEntities();
            var tiemslots = dbContext.APS_TIMESLOT.Where(i => i.TIMESLOT_ID == timeSlotID).FirstOrDefault();
            if (tiemslots != null)
            {
                return tiemslots.ToTimeSlotModel();
            }
            else
            {
                return null;
            }
        }

        public APS.BusinessEntity.Models.Appointment.AppointmentModel BookOrChangeAppointment(APS.BusinessEntity.Models.Appointment.AppointmentModel appModel)
        {
            try
            {
                UserInfoModel userInfo = new UserInfoModel();
                userInfo.UserName = appModel.App_UserName;
                using (AuthenticationRepository authRepo = new AuthenticationRepository())
                {
                    userInfo = authRepo.GetUserInfoDetails(userInfo);
                }

                APS.BusinessEntity.Models.Appointment.AppointmentModel appointModel = new APS.BusinessEntity.Models.Appointment.AppointmentModel();

                APSEntities dbContext = new APSEntities();
                DateTime appDt;
                DateTime.TryParse(appModel.App_Date, out appDt);
                if (!appModel.IsAppointmentAvailable)
                {
                    var objApp = new APS_APPOINTMENT
                    {

                        APP_DATE = appDt,
                        APP_TIMESLOT_ID = appModel.App_TimeSlot_ID,
                        APP_USERNAME = appModel.App_UserName,
                        CREATED_BY = appModel.App_UserName,
                        CREATED_ON = DateTime.Now,
                        LAST_UPDATED_BY = appModel.App_UserName,
                        LAST_UPDATED_ON = DateTime.Now,
                        APP_STATUS = BusinessConstants.AppointmetStatus.Active,
                        APP_CHANGE_CNT = 0
                    };

                    dbContext.APS_APPOINTMENT.Add(objApp);
                    dbContext.SaveChanges();
                    appModel.App_ID = objApp.APP_ID;
                }
                else
                {
                    var selectedEntity = dbContext.APS_APPOINTMENT.FirstOrDefault(p => p.APP_ID == appModel.App_ID);
                    if (appModel.App_Change_Count <= 3)
                    {
                        selectedEntity.APP_CHANGE_CNT = appModel.App_Change_Count;
                        selectedEntity.APP_TIMESLOT_ID = appModel.App_TimeSlot_ID;
                        selectedEntity.LAST_UPDATED_BY = appModel.App_UserName;
                        selectedEntity.LAST_UPDATED_ON = DateTime.Now;
                    }
                    else if (appModel.App_Change_Count > 3)
                    {
                        selectedEntity.APP_CHANGE_CNT = appModel.App_Change_Count;
                        selectedEntity.LAST_UPDATED_BY = appModel.App_UserName;
                        selectedEntity.LAST_UPDATED_ON = DateTime.Now;
                    }

                    dbContext.Entry(selectedEntity).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }

                var appointments = dbContext.V_APPOINTMENT_DETAILS.Where(i => i.APP_USERNAME == appModel.App_UserName).FirstOrDefault();
                if (appointments != null)
                {
                    appointModel = appointments.ToAppointmentModel();
                }
                appointModel.UserInfoDetails = userInfo;
                return appointModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
