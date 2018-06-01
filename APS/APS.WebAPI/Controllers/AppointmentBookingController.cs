using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APS.BusinessEntity.Models.Appointment;
using APS.BusinessComponent.Appointment;
using APS.WebAPI.Models.Appointment;
using log4net;

namespace APS.WebAPI.Controllers
{
    public class AppointmentBookingController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        public IHttpActionResult GetAppointmentTimeSlots()
        {
            AppointmentTimeSlotResponse response = new AppointmentTimeSlotResponse();
            using (AppointmentBookingBC appointmentBC = new AppointmentBookingBC())
            {
                var model = appointmentBC.GetAppointmentTimeSlots(DateTime.Now.DayOfWeek.ToString());
                if (model != null)
                {
                    response.ResponseCode = 0;
                    response.ResponseMessage = "Success";
                    response.AppointmentTimeSlotDetails = model;
                    return Ok(response);
                }
                else
                {
                    return NotFound();
                }

            }
        }

        [HttpPost]
        public IHttpActionResult GetTimeSlotDetails([FromBody]TimeSlotDetailsRequest request)
        {

            TimeSlotDetailsResponse response = new TimeSlotDetailsResponse();
            using (AppointmentBookingBC appointmentBC = new AppointmentBookingBC())
            {
                var model = appointmentBC.GetTimeSlotDetails(request.TimeSlotID);
                if (model != null)
                {
                    response.ResponseCode = 0;
                    response.ResponseMessage = "Success";
                    response.TimeSlotDetails = model;
                }
                return Ok(response);
            }
        }

        [HttpPost]
        public IHttpActionResult GetAppointmentDetails([FromBody]AppointmentDetailsRequest request)
        {
            AppointmentDetailsResponse response = new AppointmentDetailsResponse();
            using (AppointmentBookingBC appointmentBC = new AppointmentBookingBC())
            {
                var appModel = appointmentBC.GetAppointmentDetails(request.UserName);
                if (appModel != null)
                {
                    response.ResponseCode = 0;
                    response.ResponseMessage = "Success";
                    response.AppointmentDetails = appModel;
                }
                return Ok(response);
            }
        }

        [HttpPost]
        public IHttpActionResult BookOrChangeAppointment([FromBody]BookAppointmentRequest request)
        {
            try
            {
                Log.Error("call BookOrChangeAppointment API");
                using (AppointmentBookingBC appointmentBC = new AppointmentBookingBC())
                {
                    BookAppointmentResponse response = new BookAppointmentResponse();
                    if (request.AppointmentDetails.IsAppointmentAvailable)
                    {
                        request.AppointmentDetails.App_Change_Count = request.AppointmentDetails.App_Change_Count + 1;
                    }
                    if (request.AppointmentDetails.App_Change_Count <= 3)
                    {
                        appointmentBC.BookOrChangeAppointment(request.AppointmentDetails);
                        //if (appModel != null)
                        //{
                        response.ResponseCode = 0;
                        response.ResponseMessage = "Success";
                        //response.AppointmentDetails = appModel;
                        //}
                    }
                    else
                    {
                        appointmentBC.BookOrChangeAppointment(request.AppointmentDetails);
                        response.ResponseCode = 1;
                        response.ResponseMessage = "You have exceeded the number of times allowed for time slot change. Please contact your case officer.";
                    }
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

