using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APS.BusinessEntity.Models.Appointment;
using APS.BusinessEntity.Models.Authentication;
using APS.BusinessEntity.Models.Common;
using APS.Repository.Appointment;
using APS.Repository.Authentication;
using APS.Common.Cryptography;
using APS.Common.EmailComponent;
using APS.Common.Logging;
using System.Configuration;
using log4net;

namespace APS.BusinessComponent.Appointment
{
    public class AppointmentBookingBC : IDisposable
    {
        public void Dispose()
        {
            //Dispose(true); //I am calling you from Dispose, it's safe
            GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        }

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TimeSlotModel GetTimeSlotDetails(int timeSlotID)
        {
            using (AppointmentBookingRepository repo = new AppointmentBookingRepository())
            {
                return repo.GetTimeSlotDetails(timeSlotID);
            }
        }

        public APS.BusinessEntity.Models.Appointment.AppointmentModel GetAppointmentDetails(string UserName)
        {
            using (AppointmentBookingRepository repo = new AppointmentBookingRepository())
            {
                return repo.GetAppointmentDetails(UserName);
            }
        }

        public IEnumerable<TimeSlotDetailsForAppointmentModel> GetAppointmentTimeSlots(string dayOfWeek)
        {
            using (AppointmentBookingRepository repo = new AppointmentBookingRepository())
            {
                IEnumerable<TimeSlotModel> timeslot = repo.GetAppointmentTimeSlots(dayOfWeek);

                List<TimeSlotDetailsForAppointmentModel> appTimeSlot = new List<TimeSlotDetailsForAppointmentModel>();

                TimeSlotDetailsForAppointmentModel TimeSlotDetailsForAppointment = new TimeSlotDetailsForAppointmentModel();

                TimeSlotForAppointmentModel TimeSlotForAppointment = new TimeSlotForAppointmentModel();
                List<TimeSlotForAppointmentModel> lstTimeSlotForAppointment = new List<TimeSlotForAppointmentModel>();

                var grpList = timeslot
                    .GroupBy(t => t.TimeSlot_Category)
                    .Select(grp => grp.ToList())
                    .ToList();

                foreach (var item in grpList)
                {
                    TimeSlotDetailsForAppointment = new TimeSlotDetailsForAppointmentModel();
                    TimeSlotDetailsForAppointment.TimeSlot_Category = item[0].TimeSlot_Category;
                    //TimeSlotDetailsForAppointment.TimeSlot_WeekDay = item[0].TimeSlot_WeekDay;
                    lstTimeSlotForAppointment = new List<TimeSlotForAppointmentModel>();
                    foreach (var tm in item)
                    {
                        TimeSlotForAppointment = new TimeSlotForAppointmentModel();
                        TimeSlotForAppointment.TimeSlotID = tm.TimeSlotID;
                        TimeSlotForAppointment.TimeSlot_Description = tm.TimeSlot_Description;
                        //TimeSlotForAppointment.Slot_Start_Time = tm.Slot_Start_Time;
                        //TimeSlotForAppointment.Slot_End_Time = tm.Slot_End_Time;
                        TimeSlotForAppointment.IsTimeSlotAvailable = tm.IsTimeSlotAvailable;
                        lstTimeSlotForAppointment.Add(TimeSlotForAppointment);
                    }
                    TimeSlotDetailsForAppointment.TimeSlot_Appointment = lstTimeSlotForAppointment;
                    appTimeSlot.Add(TimeSlotDetailsForAppointment);
                }
                return appTimeSlot;
            }
        }

        public APS.BusinessEntity.Models.Appointment.AppointmentModel BookOrChangeAppointment(APS.BusinessEntity.Models.Appointment.AppointmentModel appModel)
        {
            try
            {
                Log.Error("call BookOrChangeAppointment BC");
                APS.BusinessEntity.Models.Appointment.AppointmentModel appointModel = new APS.BusinessEntity.Models.Appointment.AppointmentModel();
                using (AppointmentBookingRepository repo = new AppointmentBookingRepository())
                {
                    appointModel = repo.BookOrChangeAppointment(appModel);
                    if (appointModel.App_Change_Count < 4)
                    {
                        SendEmailOnBookOrChangeAppointment(appointModel);
                    }
                }
                return appointModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendEmailOnBookOrChangeAppointment(APS.BusinessEntity.Models.Appointment.AppointmentModel appointModel)
        {
            Log.Error("call BookOrChangeAppointment BC");
            SendEmailRequest emailRequest = new SendEmailRequest();
            UserInfoModel userModel = new UserInfoModel();
            userModel.UserName = appointModel.App_UserName;
            using (AuthenticationRepository authRepo = new AuthenticationRepository())
            {

                userModel = authRepo.GetUserInfoDetails(userModel);
            }
            BuildAndSendEmailRequest(emailRequest, userModel.UserEmail, "APS - Book/Change Appointment", "Your appointment has booked. Next appointment will be: " + appointModel.App_Date + " On " + appointModel.App_TimeSlot_Description);
        }

        private static void BuildAndSendEmailRequest(SendEmailRequest emailRequest, string toEmail, string emailSubject, string emailBody)
        {
            Log.Error("call BookOrChangeAppointment BC");
            emailRequest.ToEmails = toEmail;
            emailRequest.EmailSubject = emailSubject;
            emailRequest.EmailBody = emailBody;
            EmailComponent objemail = new EmailComponent();
            objemail.SendEmail(emailRequest);
        }
    }
}
