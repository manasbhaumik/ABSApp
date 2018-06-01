using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using APS.WebApp.Models.APSDataModel;
using APS.BusinessEntity.Models.Authentication;
using APS.BusinessEntity.Models.Appointment;
using APS.BusinessEntity.Models.Common;
using System.Net.Http;
//using APS.WebAPI.Models.Authentication;
//using APS.WebAPI.Models.Appointment;
using Newtonsoft.Json;
using System.Configuration;
using log4net;
using System.Globalization;

namespace APS.WebApp.Controllers
{
    public class AppointmentController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string APSWebAPI = ConfigurationManager.AppSettings.Get("APSWebAPI").ToString();
        string Trinity_API_URL = ConfigurationManager.AppSettings.Get("TRINITYAPI").ToString();// Common API connect to centralized DB

        public ActionResult ViewMessages()
        {
            try
            {
                if (Session["UserProfile"] != null)
                {
                    var data = (UserDetailsViewModel)Session["UserProfile"];
                    ViewBag.Name = data.Name;
                    ViewBag.NRIC = data.NricNo;
                    ViewBag.SupervisionOrder = data.ProbationStartDate + " - " + data.ProbationEndDate;
                    ViewBag.ReportingCenter = data.Center;
                    ViewBag.SupervisionOfficer = data.CaseOfficer;
                    ViewBag.photo = data.photo;

                    using (var client = new HttpClient())
                    {
                        var token = Session["accessToken"];
                        //AuthenticationModel model = new AuthenticationModel();
                        //model.UserPassword = Session["LoggedInUserPassword"].ToString();
                        //model.UserName = Session["LoggedInUserName"].ToString();
                        //string token = GetToken(model);
                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        var t = JsonConvert.DeserializeObject<Token>(token.ToString());
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

                        HttpResponseMessage resp = client.GetAsync(Trinity_API_URL + "api/Message/sentmessagelistbysupervisee/" + data.id).Result;
                        if (resp.StatusCode == HttpStatusCode.OK)
                        {
                            var messages = resp.Content.ReadAsStringAsync().Result;
                            var superviseeMsgs = JsonConvert.DeserializeObject<List<MessageModel>>(messages);
                            if (superviseeMsgs != null)
                            {
                                var filteredMessages = superviseeMsgs.Where(m => DateTime.ParseExact(m.deldate, "dd/MM/yyyy", CultureInfo.InstalledUICulture) >= DateTime.Now.AddDays(-14)).ToList();
                                ViewData["SuperviseeMessages"] = filteredMessages;
                            }
                        }
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public string GetToken(AuthenticationModel model)
        {

            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ), 
                        new KeyValuePair<string, string>( "username", model.UserName ), 
                        new KeyValuePair<string, string> ( "Password", model.UserPassword )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(Trinity_API_URL + "Token", content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public ActionResult ChangeAppointment()
        {
            try
            {
                if (Session["UserProfile"] != null)
                {
                    var data = (UserDetailsViewModel)Session["UserProfile"];
                    ViewBag.Name = data.Name;
                    ViewBag.NRIC = data.NricNo;
                    ViewBag.SupervisionOrder = data.ProbationStartDate + " - " + data.ProbationEndDate;
                    ViewBag.ReportingCenter = data.Center;
                    ViewBag.SupervisionOfficer = data.CaseOfficer;
                    ViewBag.photo = data.photo;
                    using (var client = new HttpClient())
                    {
                        var token = Session["accessToken"];
                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        var t = JsonConvert.DeserializeObject<Token>(token.ToString());
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

                        HttpResponseMessage resp = client.GetAsync(Trinity_API_URL + "api/Appointment/GetAppointmentTimeSlots/" + DateTime.Now.ToString("yyyy-MM-dd")).Result;
                        if (resp.StatusCode == HttpStatusCode.OK)
                        {
                            var ts = resp.Content.ReadAsStringAsync().Result;
                            var userProf = JsonConvert.DeserializeObject<List<TrinityTimeSlotModel>>(ts);
                            if (userProf != null)
                            {
                                ViewData["AppointmentTimeSlotDetails"] = GetTimeSlotToBind(userProf);
                            }
                        }

                        HttpResponseMessage responsedates = client.GetAsync(Trinity_API_URL + "api/Appointment/SuperviseeAppointmentDates").Result;
                        if (responsedates.StatusCode == HttpStatusCode.OK)
                        {
                            var appDates = responsedates.Content.ReadAsStringAsync().Result;
                            var appoints = JsonConvert.DeserializeObject<List<APS.BusinessEntity.Models.Authentication.AppointmentModel>>(appDates);
                            ViewData["SupAppointments"] = appoints.Where(a => Convert.ToDateTime(a.appdate) >= DateTime.Now.Date).ToList();
                            //ViewData["SupAppointments"] = appoints;
                        }
                    }
                }
                else
                {
                    //Redirect to Login page
                }

                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }
        public ActionResult AppointmentHistory()
        {
            try
            {
                if (Session["UserProfile"] != null)
                {
                    var data = (UserDetailsViewModel)Session["UserProfile"];
                    ViewBag.Name = data.Name;
                    ViewBag.NRIC = data.NricNo;
                    ViewBag.SupervisionOrder = data.ProbationStartDate + " - " + data.ProbationEndDate;
                    ViewBag.ReportingCenter = data.Center;
                    ViewBag.SupervisionOfficer = data.CaseOfficer;
                    ViewBag.photo = data.photo;
                    using (var client = new HttpClient())
                    {
                        var token = Session["accessToken"];
                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        var t = JsonConvert.DeserializeObject<Token>(token.ToString());
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);                        

                        HttpResponseMessage responsedates = client.GetAsync(Trinity_API_URL + "api/Appointment/SuperviseeAppointmentDates").Result;
                        if (responsedates.StatusCode == HttpStatusCode.OK)
                        {
                            var appDates = responsedates.Content.ReadAsStringAsync().Result;
                            var appoints = JsonConvert.DeserializeObject<List<APS.BusinessEntity.Models.Authentication.AppointmentModel>>(appDates);
                                                   
                            var supapplist = appoints.OrderBy(a=>a.appdate).ToList();
                            List<Appointment> indapplist = new List<Appointment>();
                            foreach (var supapps in supapplist)
                            {
                                Appointment appointment = new Appointment();
                                appointment.Date = Convert.ToDateTime(supapps.appdate).Date;
                                appointment.StrTime = supapps.starttime;
                                appointment.EndTime = supapps.endtime;
                                appointment.Status = supapps.appstatus;

                                indapplist.Add(appointment);
                            }
                            ViewData["SupAppointments"] = indapplist;

                            //if (appoints != null && appoints.Count > 0)
                            //{
                            //    ViewBag.CanChangeTimeSlot = IsChangeTimeSlot(appoints[0].appID);
                            //}
                        }
                    }
                }
                else
                {
                    return RedirectToAction("ABSLogin", "ABS", new { area = "ABS" });
                }

                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }
        public ActionResult ViewAppointment()
        {
            try
            {
                if (Session["UserProfile"] != null)
                {
                    var data = (UserDetailsViewModel)Session["UserProfile"];
                    ViewBag.Name = data.Name;
                    ViewBag.NRIC = data.NricNo;
                    ViewBag.SupervisionOrder = data.ProbationStartDate + " - " + data.ProbationEndDate;
                    ViewBag.ReportingCenter = data.Center;
                    ViewBag.SupervisionOfficer = data.CaseOfficer;
                    ViewBag.photo = data.photo;
                    using (var client = new HttpClient())
                    {
                        var token = Session["accessToken"];
                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        var t = JsonConvert.DeserializeObject<Token>(token.ToString());
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

                        HttpResponseMessage resp = client.GetAsync(Trinity_API_URL + "api/Appointment/GetAppointmentTimeSlots/" + DateTime.Now.ToString("yyyy-MM-dd")).Result;
                        if (resp.StatusCode == HttpStatusCode.OK)
                        {
                            var ts = resp.Content.ReadAsStringAsync().Result;
                            var userProf = JsonConvert.DeserializeObject<List<TrinityTimeSlotModel>>(ts);
                            if (userProf != null)
                            {
                                ViewData["AppointmentTimeSlotDetails"] = GetTimeSlotToBind(userProf);
                            }
                        }

                        HttpResponseMessage responsedates = client.GetAsync(Trinity_API_URL + "api/Appointment/SuperviseeAppointmentDates").Result;
                        if (responsedates.StatusCode == HttpStatusCode.OK)
                        {
                            var appDates = responsedates.Content.ReadAsStringAsync().Result;
                            var appoints = JsonConvert.DeserializeObject<List<APS.BusinessEntity.Models.Authentication.AppointmentModel>>(appDates);
                            ViewData["SupAppointments"] = appoints.Where(a => a.appdate >= DateTime.Now.Date && a.appstatus != "Completed").OrderBy(d=>d.appdate).ToList();

                            if (appoints != null && appoints.Count > 0)
                            {
                                ViewBag.CanChangeTimeSlot = IsChangeTimeSlot(appoints[0].appID);
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("ABSLogin", "ABS", new { area = "ABS" });
                }

                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public List<TimeSlotDetailsForAppointmentModel> GetTimeSlotToBind(List<TrinityTimeSlotModel> tsModel)
        {
            try
            {
                tsModel.Where(c => c.Category == "Morning").Select(c => { c.CategoryID = 1; return c; }).ToList();

                tsModel.Where(c => c.Category == "Afternoon").Select(c => { c.CategoryID = 2; return c; }).ToList();

                tsModel.Where(c => c.Category == "Evening").Select(c => { c.CategoryID = 3; return c; }).ToList();

                var grpList = tsModel.OrderBy(x => x.CategoryID).ToList().OrderBy(st => st.StartTime).ToList()
            .GroupBy(t => t.Category)
            .Select(grp => grp.ToList())
            .ToList();
                TimeSlotDetailsForAppointmentModel TimeSlotDetailsForAppointment = new TimeSlotDetailsForAppointmentModel();
                List<TimeSlotForAppointmentModel> lstTimeSlotForAppointment = new List<TimeSlotForAppointmentModel>();
                TimeSlotForAppointmentModel TimeSlotForAppointment = new TimeSlotForAppointmentModel();
                List<TimeSlotDetailsForAppointmentModel> appTimeSlot = new List<TimeSlotDetailsForAppointmentModel>();
                foreach (var item in grpList)
                {
                    TimeSlotDetailsForAppointment = new TimeSlotDetailsForAppointmentModel();
                    TimeSlotDetailsForAppointment.TimeSlot_Category = item[0].Category;
                    lstTimeSlotForAppointment = new List<TimeSlotForAppointmentModel>();
                    foreach (var tm in item)
                    {
                        TimeSlotForAppointment = new TimeSlotForAppointmentModel();
                        TimeSlotForAppointment.TimeSlotID = tm.Timeslot_ID;
                        TimeSlotForAppointment.TimeSlot_Description = tm.Description;
                        TimeSlotForAppointment.Slot_Start_Time = tm.StartTime.Value.Hours.ToString("00") + ":" + tm.StartTime.Value.Minutes.ToString("00");
                        TimeSlotForAppointment.Slot_End_Time = tm.EndTime.Value.Hours.ToString("00") + ":" + tm.EndTime.Value.Minutes.ToString("00");

                        if (tm.MaximumSupervisee > tm.App_cnt)
                        {
                            TimeSlotForAppointment.IsTimeSlotAvailable = true;
                        }
                        else
                        {
                            TimeSlotForAppointment.IsTimeSlotAvailable = false;
                        }
                        lstTimeSlotForAppointment.Add(TimeSlotForAppointment);
                    }
                    TimeSlotDetailsForAppointment.TimeSlot_Appointment = lstTimeSlotForAppointment;
                    appTimeSlot.Add(TimeSlotDetailsForAppointment);
                }

                return appTimeSlot;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return null;
            }
        }

        public ActionResult BookChangeAppointmentDetails(string timeSlotID, string appointmentID)
        {
            string timeSlotDesc = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var token = Session["accessToken"];
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    var t = JsonConvert.DeserializeObject<Token>(token.ToString());
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

                    HttpResponseMessage resp = client.GetAsync(Trinity_API_URL + "api/Appointment/GetAppointmentTimeSlots/" + DateTime.Now.ToString("yyyy-MM-dd")).Result;
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        var ts = resp.Content.ReadAsStringAsync().Result;
                        var timeslotDetails = JsonConvert.DeserializeObject<List<TrinityTimeSlotModel>>(ts);
                        if (timeslotDetails != null)
                        {
                            List<TimeSlotDetailsForAppointmentModel> tsDetails = GetTimeSlotToBind(timeslotDetails);
                            if (tsDetails != null && tsDetails.Count > 0)
                            {
                                var ss = tsDetails.SelectMany(c => c.TimeSlot_Appointment).ToList();
                                var timeslotItem = ss.Where(m => m.TimeSlotID == timeSlotID).SingleOrDefault();
                                timeSlotDesc = timeslotItem.Slot_Start_Time.ToString() + " - " + timeslotItem.Slot_End_Time.ToString();
                            }
                        }
                    }



                    return RedirectToAction("ConfirmAppointment", "Appointment", new { TimeSlotID = timeSlotID, AppointmentID = appointmentID, TimeSlotDesc = timeSlotDesc });

                }

                //HttpResponseMessage responsedates = client.GetAsync(Trinity_API_URL + "api/Appointment/SuperviseeAppointmentDates").Result;
                //if (responsedates.StatusCode == HttpStatusCode.OK)
                //{
                //    var appDates = responsedates.Content.ReadAsStringAsync().Result;
                //    var appoints = JsonConvert.DeserializeObject<List<ABS.BusinessEntity.Models.Authentication.AppointmentModel>>(appDates);
                //    HttpResponseMessage updateTimeSlot = client.PutAsJsonAsync(Trinity_API_URL + "api/Appointment/SetTimeSlot/" + appoints[0].appID, timeSlotID).Result;
                //    if (updateTimeSlot.StatusCode == HttpStatusCode.OK)
                //    {
                //    }

                //Appointment app = new Appointment();
                //app.ID = Guid.Parse("AF972B0C-962A-442C-A92C-291B4F798E34");
                //app.Timeslot_ID = "0272aacb-e442-4320-8b53-de09aa9b6495";

                //HttpResponseMessage updateTimeSlot = client.PostAsJsonAsync(Trinity_API_URL + "api/Appointment/SetTimeSlot", app).Result;
                //if (updateTimeSlot.StatusCode == HttpStatusCode.OK)
                //{
                //}
                //}



            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public ActionResult ConfirmAppointment(string TimeSlotID, string AppointmentID, string TimeSlotDesc)
        {
            ViewBag.TimeSlotID = TimeSlotID;
            ViewBag.TimeSlotDesc = TimeSlotDesc;
            ViewBag.AppointmentID = AppointmentID;
            return View();
        }

        public ActionResult UpdateTimeSlot(string timeSlotID, string appointmentID)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var token = Session["accessToken"];
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    var t = JsonConvert.DeserializeObject<Token>(token.ToString());
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

                    Appointment app = new Appointment();
                    app.ID = Guid.Parse(appointmentID);
                    app.Timeslot_ID = timeSlotID;

                    HttpResponseMessage updateTimeSlot = client.PostAsJsonAsync(Trinity_API_URL + "api/Appointment/SetTimeSlot", app).Result;
                    if (updateTimeSlot.StatusCode == HttpStatusCode.OK)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return RedirectToAction("ViewAppointment", "Appointment", new { area = "" });
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex); Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        private bool IsChangeTimeSlot(string appointmentID)
        {
            bool result = false;
            using (var client = new HttpClient())
            {
                var token = Session["accessToken"];
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                var t = JsonConvert.DeserializeObject<Token>(token.ToString());
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

                HttpResponseMessage appDetails = client.GetAsync(Trinity_API_URL + "api/Appointment/AppointmentDetails/" + appointmentID).Result;
                if (appDetails.StatusCode == HttpStatusCode.OK)
                {
                    var appointDetails = appDetails.Content.ReadAsStringAsync().Result;
                    //var appointDetailsModel = JsonConvert.DeserializeObject<APS.BusinessEntity.Models.Appointment.Appointment>(appointDetails);
                    var appointDetailsModel = JsonConvert.DeserializeObject<APS.BusinessEntity.Models.Authentication.AppointmentModel>(appointDetails);
                    if (appointDetailsModel.rescheduled <= 2)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public ActionResult GetSearchMessages(string searchText)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var data = (UserDetailsViewModel)Session["UserProfile"];
                    var token = Session["accessToken"];
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    var t = JsonConvert.DeserializeObject<Token>(token.ToString());
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

                    HttpResponseMessage resp = client.GetAsync(Trinity_API_URL + "api/Message/sentmessagelistbysupervisee/" + data.id).Result;
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        var messages = resp.Content.ReadAsStringAsync().Result;
                        var superviseeMsgs = JsonConvert.DeserializeObject<List<MessageModel>>(messages);
                        if (superviseeMsgs != null)
                        {
                            var searchmsg = superviseeMsgs.Where(m => m.messagebody.Contains(searchText) || m.subject.Contains(searchText)).ToList();
                            return Json(searchmsg, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex); Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        // GET: Appointment
        //public ActionResult ViewAppointmentBooking()
        //{
        //    try
        //    {
        //        ABS.BusinessEntity.Models.Appointment.AppointmentModel appointModel = null;
        //        // check is any existing appointment available
        //        AppointmentDetailsRequest request = new AppointmentDetailsRequest();
        //        string userName = Session["LoginUser"].ToString();
        //        request.UserName = userName;
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(ABSWebAPI + "AppointmentBooking/GetAppointmentDetails");
        //            var postTask = client.PostAsJsonAsync<AppointmentDetailsRequest>("GetAppointmentDetails", request);
        //            postTask.Wait();

        //            var result = postTask.Result;
        //            if (result.IsSuccessStatusCode)
        //            {
        //                var readTask = result.Content.ReadAsAsync<AppointmentDetailsResponse>();
        //                readTask.Wait();
        //                if (readTask.Result.ResponseCode == 0)
        //                {
        //                    if (readTask.Result.ResponseMessage == "Success")
        //                    {
        //                        appointModel = readTask.Result.AppointmentDetails;
        //                        ViewBag.ChangeTimeSlotCount = appointModel.App_Change_Count;
        //                    }
        //                }
        //            }
        //        }
        //        if (appointModel != null)
        //        {
        //            if (appointModel.IsAppointmentAvailable)
        //            {
        //                //appointment available
        //                ViewBag.IsAppointmentAvailable = true;
        //            }
        //            else
        //            {
        //                //appointment not available
        //                ViewBag.IsAppointmentAvailable = false;
        //                GetAppointmentTimeSlotsDetails(appointModel);
        //                // Get Timeslot details
        //            }
        //        }
        //        return View(appointModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //        throw;
        //    }
        //}

        //public ActionResult GetAppointmentDetails()
        //{
        //    ABS.BusinessEntity.Models.Appointment.AppointmentModel appointModel = null;
        //    // check is any existing appointment available
        //    AppointmentDetailsRequest request = new AppointmentDetailsRequest();
        //    string userName = Session["LoginUser"].ToString();
        //    request.UserName = userName;
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(ABSWebAPI + "AppointmentBooking/GetAppointmentDetails");
        //        var postTask = client.PostAsJsonAsync<AppointmentDetailsRequest>("GetAppointmentDetails", request);
        //        postTask.Wait();

        //        var result = postTask.Result;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsAsync<AppointmentDetailsResponse>();
        //            readTask.Wait();
        //            if (readTask.Result.ResponseCode == 0)
        //            {
        //                if (readTask.Result.ResponseMessage == "Success")
        //                {
        //                    appointModel = readTask.Result.AppointmentDetails;
        //                }
        //            }
        //        }
        //    }
        //    if (appointModel != null)
        //    {
        //        if (appointModel.IsAppointmentAvailable)
        //        {
        //            //appointment available
        //            ViewBag.IsAppointmentAvailable = true;
        //        }
        //        else
        //        {
        //            //appointment not available
        //            ViewBag.IsAppointmentAvailable = false;
        //            GetAppointmentTimeSlotsDetails(appointModel);
        //            // Get Timeslot details
        //        }
        //    }

        //    //return Json(appointModel, JsonRequestBehavior.AllowGet);
        //    return View("ViewAppointmentBooking", appointModel);
        //}

        //public ActionResult BookorChangeAppointment()
        //{
        //    try
        //    {
        //        ABS.BusinessEntity.Models.Appointment.AppointmentModel appointModel = new ABS.BusinessEntity.Models.Appointment.AppointmentModel();
        //        AppointmentDetailsRequest request = new AppointmentDetailsRequest();
        //        string userName = Session["LoginUser"].ToString();
        //        request.UserName = userName;
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(ABSWebAPI + "AppointmentBooking/GetAppointmentDetails");
        //            var postTask = client.PostAsJsonAsync<AppointmentDetailsRequest>("GetAppointmentDetails", request);
        //            postTask.Wait();

        //            var result = postTask.Result;
        //            if (result.IsSuccessStatusCode)
        //            {
        //                var readTask = result.Content.ReadAsAsync<AppointmentDetailsResponse>();
        //                readTask.Wait();
        //                if (readTask.Result.ResponseCode == 0)
        //                {
        //                    if (readTask.Result.ResponseMessage == "Success")
        //                    {
        //                        appointModel = readTask.Result.AppointmentDetails;
        //                        ViewBag.ChangeTimeSlotCount = appointModel.App_Change_Count;
        //                    }
        //                }
        //            }
        //        }
        //        GetAppointmentTimeSlotsDetails(appointModel);
        //        if (appointModel != null)
        //        {
        //            if (appointModel.IsAppointmentAvailable)
        //            {
        //                //appointment available
        //                ViewBag.IsAppointmentAvailable = true;
        //            }
        //            else
        //            {
        //                //appointment not available
        //                ViewBag.IsAppointmentAvailable = false;
        //                // Get Timeslot details
        //            }
        //        }
        //        return View("BookORChangeAppointment", appointModel);
        //    }

        //    catch (Exception ex)
        //    {
        //        Log.Error(ex); throw;
        //    }
        //}

        //private void GetAppointmentTimeSlotsDetails(ABS.BusinessEntity.Models.Appointment.AppointmentModel appointModel)
        //{
        //    try
        //    {
        //        List<TimeSlotDetailsForAppointmentModel> appointmentTimeSlotDetails = null;
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(ABSWebAPI);
        //            var responseTask = client.GetAsync("AppointmentBooking/GetAppointmentTimeSlots");
        //            responseTask.Wait();

        //            var result = responseTask.Result;
        //            if (result.IsSuccessStatusCode)
        //            {
        //                var readTask = result.Content.ReadAsAsync<AppointmentTimeSlotResponse>();
        //                readTask.Wait();
        //                if (readTask.Result.ResponseCode == 0)
        //                {
        //                    if (readTask.Result.ResponseMessage == "Success")
        //                    {
        //                        appointmentTimeSlotDetails = readTask.Result.AppointmentTimeSlotDetails.ToList();
        //                        appointModel.AppointmentTimeSlotDetails = appointmentTimeSlotDetails;
        //                        ViewData["AppointmentTimeSlotDetails"] = appointmentTimeSlotDetails;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex); throw;
        //    }
        //}



        //public ActionResult ConfirmBookOrChangeAppointment(int timeSlotID)
        //{
        //    try
        //    {
        //        Log.Error("call ConfirmBookOrChangeAppointment Action ");
        //        ABS.BusinessEntity.Models.Appointment.AppointmentModel appModel = new ABS.BusinessEntity.Models.Appointment.AppointmentModel();
        //        AppointmentDetailsRequest request = new AppointmentDetailsRequest();
        //        string userName = Session["LoginUser"].ToString();
        //        request.UserName = userName;
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(ABSWebAPI + "AppointmentBooking/GetAppointmentDetails");
        //            var postTask = client.PostAsJsonAsync<AppointmentDetailsRequest>("GetAppointmentDetails", request);
        //            postTask.Wait();

        //            var result = postTask.Result;
        //            Log.Error("result.IsSuccessStatusCode: " + result.IsSuccessStatusCode);
        //            if (result.IsSuccessStatusCode)
        //            {
        //                var readTask = result.Content.ReadAsAsync<AppointmentDetailsResponse>();
        //                readTask.Wait();
        //                if (readTask.Result.ResponseCode == 0)
        //                {
        //                    if (readTask.Result.ResponseMessage == "Success")
        //                    {
        //                        appModel = readTask.Result.AppointmentDetails;
        //                    }
        //                }
        //            }
        //        }

        //        BookAppointmentRequest bookAppRequest = new BookAppointmentRequest();
        //        appModel.App_TimeSlot_ID = timeSlotID;
        //        appModel.App_UserName = Session["LoginUser"].ToString();
        //        appModel.App_Date = DateTime.Now.ToString("dd/MMM/yyyy");
        //        bookAppRequest.AppointmentDetails = appModel;
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(ABSWebAPI + "AppointmentBooking/BookOrChangeAppointment");
        //            var postTask = client.PostAsJsonAsync<BookAppointmentRequest>("BookOrChangeAppointment", bookAppRequest);
        //            postTask.Wait();

        //            var result = postTask.Result;
        //            if (result.IsSuccessStatusCode)
        //            {
        //                var readTask = result.Content.ReadAsAsync<BookAppointmentResponse>();
        //                readTask.Wait();
        //                if (readTask.Result.ResponseCode == 0)
        //                {
        //                    if (readTask.Result.ResponseMessage == "Success")
        //                    {
        //                        //appModel = readTask.Result.AppointmentDetails;
        //                    }
        //                }
        //                else if (readTask.Result.ResponseCode == 1)
        //                {
        //                    string message = readTask.Result.ResponseMessage;
        //                    // redirect to another view just show this message
        //                    ViewBag.ExceedTSMsg = message;
        //                    return Json(new { IsTimeSlotExceed = true }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        }
        //        return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult TimeSlotExceed()
        //{
        //    try
        //    {
        //        ViewBag.ExceedTSMsg = "You have exceeded the number of times allowed for time slot change. Please contact your case officer.";
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex); throw;
        //    }
        //}
    }
}