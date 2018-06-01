using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

using APS.BusinessEntity.Models.Authentication;
using APS.BusinessEntity.Models.Appointment;
using APS.BusinessEntity.Models.Common;
using System.Net.Http;

using Newtonsoft.Json;
using System.Configuration;
using log4net;


namespace APS.WebApp.Controllers
{
    
    public class ABSController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //string APSWebAPI = ConfigurationManager.AppSettings.Get("APSWebAPI").ToString(); // Own API
        string Trinity_API_URL = ConfigurationManager.AppSettings.Get("TRINITYAPI").ToString();// Common API connect to centralized DB

        public ActionResult ABSLogin()
        {
            ViewBag.ErrorMessage = string.Empty;
                        return View();
        }

        [AllowAnonymous]
        public ActionResult AuthenticateAPS()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ReturnToLogin()
        {
            return RedirectToAction("ABSLogin", "ABS", new { area = "ABS" });
        }


        /// <summary>
        /// Register User for tetsing purpose
        /// </summary>
        private void RegisterTestUser()
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(Trinity_API_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                RegisterBindingModel regmodel = new RegisterBindingModel();
                regmodel.NricNo = "G1234567A";
                regmodel.Password = "Pass@word123";
                regmodel.ConfirmPassword = "Pass@word123";
                regmodel.Email = "sathiyapriyanece@gmail.com";
                regmodel.Name = "Sathiya";
                regmodel.ContactNo = "1234567";
                regmodel.Note = "Test User2";
                regmodel.RoleName = "Supervisee";
                regmodel.Center = "Test";
                regmodel.DOB = Convert.ToDateTime("1969-04-21");
                regmodel.PrbEndDate = Convert.ToDateTime("2018-02-05");
                regmodel.PrbSrtDate = Convert.ToDateTime("2020-02-05");
                regmodel.VstFrequency = 2;

                //HTTP POST
                var response = client.PostAsJsonAsync("http://13.250.140.138:8081/TrinityApiSrvs/api/Account/Register", regmodel).Result;
                //var response = client.PostAsJsonAsync("http://localhost:9484/api/Account/Register", regmodel).Result;
            }
        }

        /// <summary>
        /// Authenticate using Token based authentication
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AuthenticateAPS(AuthenticationModel model)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                string token = GetToken(model);

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var t = JsonConvert.DeserializeObject<Token>(token);

                        if (t.userName != null)
                        {
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);

                            Session["accessToken"] = token;
                            HttpResponseMessage response = client.GetAsync(Trinity_API_URL + "api/Account/FirstTimeUserLogin").Result;
                            if (response != null)
                            {
                                var profile = client.GetAsync(Trinity_API_URL + "api/Account/UserProfile").Result;
                                if (profile.StatusCode == HttpStatusCode.OK)
                                {
                                    var data = profile.Content.ReadAsStringAsync().Result;
                                    var userProf = JsonConvert.DeserializeObject<UserDetailsViewModel>(data);
                                    Session["UserProfile"] = userProf;
                                    Session["LoggedInUserName"] = model.UserName;

                                    HttpResponseMessage rrr = client.GetAsync(Trinity_API_URL + "api/Appointment/GetAbsent").Result;
                                    if (rrr.StatusCode == HttpStatusCode.OK)
                                    {
                                        var dr = rrr.Content.ReadAsStringAsync().Result;
                                    }
                                    //Session["LoggedInUserPassword"] = model.UserPassword;
                                }

                                if (response.StatusCode == HttpStatusCode.BadRequest)
                                {
                                    // First time login user
                                    return View("ABSChangePassword");
                                }
                                else if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    // User logged in already
                                    return RedirectToAction("ViewMessages", "Appointment", new { area = "Appointment" });
                                }
                            }
                            else
                            {
                                Log.Error("API - api/Account/FirstTimeUserLogin returns empty");
                                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
                            }
                        }
                        else
                        {
                            var err = JsonConvert.DeserializeObject<APIError>(token);
                            //show error message from err object
                            ViewBag.ErrorMessage = err.error_description;
                            if (err.error == "locked_out")
                            {
                                Session["FPUserName"] = model.UserName;
                                return RedirectToAction("ABSForgetPassword", "ABS", new { area = "ABS" });
                                //return View("ABSForgetPassword");
                            }
                            else if (err.error == "invalid_grant")
                            {
                                TempData["ErrorMessage"] = err.error_description;
                                ViewBag.ErrorMessage = err.error_description;
                                return View("ABSLogin", model);
                            }
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid Token";
                        ViewBag.ErrorMessage = "Invalid Token";
                        return View("ABSLogin", model);
                    }

                }
                return RedirectToAction("ABSLogin", "ABS", new { area = "ABS" });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public ActionResult ShowErrorPage()
        {
            return View();
        }

        public ActionResult GetUserSecretQuestion(string nricno)
        {
            try
            {
                string ques = string.Empty;
                using (var client = new HttpClient())
                {
                    var profile = client.GetAsync(Trinity_API_URL + "api/Account/GetUserSecurityQA/" + nricno).Result;
                    if (profile.StatusCode == HttpStatusCode.OK)
                    {
                        var data = profile.Content.ReadAsStringAsync().Result;

                        var user_qa = JsonConvert.DeserializeObject<UserSecurityQuestionAnswerModel>(data);
                        ques = user_qa.SecurityQuestion;
                    }
                    else if (profile.StatusCode == HttpStatusCode.NotFound)
                    {
                        ques = string.Empty;
                    }
                }
                return Json(ques, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public ActionResult GetUserRegisteredEmail(string nricno)
        {
            string user_email = string.Empty;
            try
            {
                PersonalProfileModel userProfile = null;
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(Trinity_API_URL + "api/UserProfile/GetUserByNric/" + nricno).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = response.Content.ReadAsStringAsync().Result;
                        userProfile = JsonConvert.DeserializeObject<PersonalProfileModel>(data);
                        user_email = userProfile.email;
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        user_email = string.Empty;
                    }

                }
                return Json(user_email, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public ActionResult UserChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePasswordAfterLogin(ChangePasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                string token = (Session["accessToken"] != null && !string.IsNullOrEmpty(Session["accessToken"].ToString())) ? Session["accessToken"].ToString() : string.Empty;

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var t = JsonConvert.DeserializeObject<Token>(token);

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);
                        ChangePasswordBindingModel regmodel = new ChangePasswordBindingModel();
                        regmodel.OldPassword = model.OldPassword;
                        regmodel.NewPassword = model.NewPassword;
                        regmodel.ConfirmPassword = model.ConfirmPassword;

                        //HTTP POST
                        var response = client.PostAsJsonAsync(Trinity_API_URL + "api/Account/ChangePassword", regmodel).Result;

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ViewMessages", "Appointment", new { area = "Appointment" });
                        }
                        else
                        {
                            return RedirectToAction("ViewMessages", "Appointment", new { area = "Appointment" });
                        }

                    }
                    else { return RedirectToAction("ABSLogin", "ABS", new { area = "ABS" }); }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public ActionResult ABSChangePassword()
        {
            return View("ABSChangePassword");
        }

        [HttpPost]
        public ActionResult ABSChangePassword(ChangePasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid) { return View(); }

                string token = (Session["accessToken"] != null && !string.IsNullOrEmpty(Session["accessToken"].ToString())) ? Session["accessToken"].ToString() : string.Empty;

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var t = JsonConvert.DeserializeObject<Token>(token);

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);
                        List<PasswordQuestionsModel> questions = GetPasswordQuestions();
                        string ques = questions.Where(i => i.PasswordQuestionID == Convert.ToInt32(model.PasswordQuestion)).FirstOrDefault().PasswordQuestion;

                        NewUserPasswordBindingModel regmodel = new NewUserPasswordBindingModel();
                        regmodel.NewPassword = model.UserPassword;
                        regmodel.ConfirmPassword = model.ReTypeUserPassword;
                        regmodel.SecurityQuestion = ques;
                        regmodel.SecurityAnswer = model.PasswordQuesAnswer;

                        //HTTP POST
                        var response = client.PostAsJsonAsync(Trinity_API_URL + "api/Account/NewUserChangePassword", regmodel).Result;

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ViewMessages", "Appointment", new { area = "Appointment" });
                        }
                        else
                        {
                            return View();
                        }

                    }
                    else { return RedirectToAction("ABSLogin", "ABS", new { area = "ABS" }); }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public ActionResult ABSForgetPassword()
        {
            try
            {
                //GetUserSecretQuestion("G1234567G");
                ViewBag.ErrorMessage = string.Empty;
                if (Session["FPUserName"] != null && !string.IsNullOrEmpty(Session["FPUserName"].ToString()))
                {
                    PersonalProfileModel userProfile = null;
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync(Trinity_API_URL + "api/UserProfile/GetUserByNric/" + Session["FPUserName"].ToString()).Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var data = response.Content.ReadAsStringAsync().Result;
                            userProfile = JsonConvert.DeserializeObject<PersonalProfileModel>(data);
                            UserModel userModel = new UserModel();

                            if (userProfile.ForgetPass_FailAttempt <= 3)
                            {
                                return View();
                            }
                            else if (userProfile.ForgetPass_FailAttempt >= 4)
                            {
                                if (userProfile.DOB_FailAttempt <= 3)
                                {
                                    return View("ABSForgetPassword_DOB");
                                }
                                else if (userProfile.DOB_FailAttempt >= 3)
                                {
                                    return RedirectToAction("ABSForgetPasswordMax", "ABS", new { area = "ABS" });
                                }
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

        [HttpPost]
        public ActionResult ABSForgetPassword(ForgetPasswordModel model)
        {

            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return View("ABSForgetPassword");
                //}

                UserSecurityQuestionAnswerModel user_qa = null;
                PersonalProfileModel userProfile = null;

                SendEmailRequest emailRequest = new SendEmailRequest();
                string tempPassword = ConfigurationManager.AppSettings.Get("ResetTempPassword").ToString();
                //Get user secret question and password 
                using (var client = new HttpClient())
                {
                    var profile = client.GetAsync(Trinity_API_URL + "api/Account/GetUserSecurityQA/" + model.UserName).Result;
                    if (profile.StatusCode == HttpStatusCode.OK)
                    {
                        var data = profile.Content.ReadAsStringAsync().Result;
                        user_qa = JsonConvert.DeserializeObject<UserSecurityQuestionAnswerModel>(data);
                    }
                }
                //Get User Profile
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(Trinity_API_URL + "api/UserProfile/GetUserByNric/" + model.UserName).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = response.Content.ReadAsStringAsync().Result;
                        userProfile = JsonConvert.DeserializeObject<PersonalProfileModel>(data);
                        UserModel userModel = new UserModel();
                        if (userProfile.ForgetPass_FailAttempt == null || userProfile.ForgetPass_FailAttempt <= 3)
                        {
                            if (model.PasswordQuestion == user_qa.SecurityQuestion && model.PasswordQuesAnswer == user_qa.SecurtyAnswer)
                            {
                                //RESET PASSWORD
                                //HTTP POST
                                ForgetPasswordbindingModel fpmodel = new ForgetPasswordbindingModel();
                                fpmodel.NricNo = model.UserName;
                                fpmodel.Newpassword = tempPassword;
                                var fpresponse = client.PostAsJsonAsync(Trinity_API_URL + "api/Account/ForgetPassword", fpmodel).Result;
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    Supervisee updateProfileModel = new Supervisee();
                                    updateProfileModel.ForgetPass_FailAttempt = 0;
                                    updateProfileModel.DOB_FailAttempt = 0;
                                    var updateResult = client.PutAsJsonAsync(Trinity_API_URL + "api/UserProfile/updateuserprofile/" + userProfile.UserId, updateProfileModel).Result;
                                    if (updateResult.StatusCode == HttpStatusCode.OK)
                                    {
                                        // SEND EMAIL
                                        MessageModel msg = new MessageModel();
                                        string[] emailrecep = { userProfile.email };
                                        msg.reciepients = emailrecep;
                                        msg.mail = true;
                                        msg.sms = true;
                                        msg.messagebody = "Your password has been changed. New password is: " + tempPassword + " ,Please Change the password after login using the temporary password. Thank You.";
                                        msg.subject = "ABS - Reset Password";
                                        var sendEmail = client.PostAsJsonAsync(Trinity_API_URL + "api/Message/sendemailnotifications", msg).Result;

                                        //LOG ACTIVITY
                                        return RedirectToAction("ABSPasswordSuccess", "ABS", new { area = "ABS", UserEmail = userProfile.email });
                                    }
                                }
                            }
                            else
                            {
                                //Update the PasswordChagneFailAttempt count 
                                Supervisee updateProfileModel = new Supervisee();
                                if (userProfile.ForgetPass_FailAttempt == null || userProfile.ForgetPass_FailAttempt != 0)
                                {
                                    if (userProfile.ForgetPass_FailAttempt == null)
                                    {
                                        updateProfileModel.ForgetPass_FailAttempt = 1;
                                    }
                                    else
                                    {
                                        updateProfileModel.ForgetPass_FailAttempt = userProfile.ForgetPass_FailAttempt + 1;
                                    }
                                    updateProfileModel.DOB_FailAttempt = userProfile.DOB_FailAttempt;
                                }
                                else
                                {
                                    updateProfileModel.ForgetPass_FailAttempt = 1;
                                    updateProfileModel.DOB_FailAttempt = userProfile.DOB_FailAttempt;
                                }
                                var updateResult = client.PutAsJsonAsync(Trinity_API_URL + "api/UserProfile/updateuserprofile/" + userProfile.UserId, updateProfileModel).Result;
                                if (updateResult.StatusCode == HttpStatusCode.OK)
                                {
                                    if (updateProfileModel.ForgetPass_FailAttempt >= 4)
                                    {
                                        Session["ForgotPassModel"] = model;
                                        ViewBag.ErrorMessage = "Invalid entry";

                                        return View("ABSForgetPassword_DOB");
                                    }
                                }
                                Session["ForgotPassModel"] = model;
                                ViewBag.ErrorMessage = "Invalid entry";
                                return View("ABSForgetPassword");
                            }
                        }
                        else
                        {
                            Session["ForgotPassModel"] = model;
                            ViewBag.ErrorMessage = "Invalid entry";
                            return View("ABSForgetPassword_DOB");
                        }
                    }
                    else
                    {
                        return View("ABSForgetPassword");
                    }
                }
                return View("ABSForgetPassword");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public ActionResult ABSDOBForgetPassword(ForgetPasswordModel model)
        {
            try
            {
                if (Session["ForgotPassModel"] != null)
                {
                    var DOB = model.DateOfBirth;
                    model = (ForgetPasswordModel)(Session["ForgotPassModel"]);
                    model.DateOfBirth = DOB;
                }

                PersonalProfileModel userProfile = null;
                string tempPassword = ConfigurationManager.AppSettings.Get("ResetTempPassword").ToString();
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(Trinity_API_URL + "api/UserProfile/GetUserByNric/" + model.UserName).Result;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = response.Content.ReadAsStringAsync().Result;
                        userProfile = JsonConvert.DeserializeObject<PersonalProfileModel>(data);
                        UserModel userModel = new UserModel();

                        if (userProfile.DOB_FailAttempt == null || userProfile.DOB_FailAttempt <= 3)
                        {
                            if (userProfile.DOB == Convert.ToDateTime(model.DateOfBirth))
                            {
                                //     //RESET PASSWORD
                                ForgetPasswordbindingModel fpmodel = new ForgetPasswordbindingModel();
                                fpmodel.NricNo = model.UserName;
                                fpmodel.Newpassword = tempPassword;
                                var fpresponse = client.PostAsJsonAsync(Trinity_API_URL + "api/Account/ForgetPassword", fpmodel).Result;
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    Supervisee updateProfileModel = new Supervisee();
                                    updateProfileModel.ForgetPass_FailAttempt = 0;
                                    updateProfileModel.DOB_FailAttempt = 0;
                                    var updateResult = client.PutAsJsonAsync(Trinity_API_URL + "api/UserProfile/updateuserprofile/" + userProfile.UserId, updateProfileModel).Result;
                                    if (updateResult.StatusCode == HttpStatusCode.OK)
                                    {
                                        // SEND EMAIL
                                        MessageModel msg = new MessageModel();
                                        string[] emailrecep = { userProfile.email };
                                        msg.reciepients = emailrecep;
                                        msg.mail = true;
                                        msg.sms = true;
                                        msg.messagebody = "Your password has been changed. New password is: " + tempPassword + " ,Please Change the password after login using the temporary password. Thank You.";
                                        msg.subject = "ABS - Reset Password";
                                        var sendEmail = client.PostAsJsonAsync(Trinity_API_URL + "api/Message/sendemailnotifications", msg).Result;
                                        //LOG ACTIVITY

                                        return RedirectToAction("ABSPasswordSuccess", "ABS", new { area = "ABS", UserEmail = userProfile.email });
                                    }
                                }
                            }
                            else
                            {
                                Supervisee updateProfileModel = new Supervisee();
                                if (userProfile.DOB_FailAttempt == null || userProfile.DOB_FailAttempt != 0)
                                {
                                    if (userProfile.DOB_FailAttempt == null)
                                    {
                                        updateProfileModel.DOB_FailAttempt = 1;
                                    }
                                    else
                                    {
                                        updateProfileModel.DOB_FailAttempt = userProfile.DOB_FailAttempt + 1;
                                    }
                                    updateProfileModel.ForgetPass_FailAttempt = userProfile.ForgetPass_FailAttempt;
                                }
                                else
                                {
                                    updateProfileModel.DOB_FailAttempt = 1;
                                    updateProfileModel.ForgetPass_FailAttempt = userProfile.ForgetPass_FailAttempt;
                                }
                                var updateResult = client.PutAsJsonAsync(Trinity_API_URL + "api/UserProfile/updateuserprofile/" + userProfile.UserId, updateProfileModel).Result;
                                if (updateResult.StatusCode == HttpStatusCode.OK)
                                {
                                    if (updateProfileModel.DOB_FailAttempt >= 4)
                                    {
                                        Session["ForgotPassModel"] = model;
                                        ViewBag.ErrorMessage = "Invalid entry";
                                        return RedirectToAction("ABSForgetPasswordMax", "ABS", new { area = "" });
                                    }
                                }
                                Session["ForgotPassModel"] = model;
                                ViewBag.ErrorMessage = "Invalid entry";
                                return View("ABSForgetPassword_DOB");
                            }
                        }
                        else
                        {
                            return View("ABSForgetPasswordMax");
                        }
                    }

                }
                return View("ABSForgetPassword_DOB");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        
        public ActionResult ABSDOForgetPassword(ForgetPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("ABSDOForgetPassword");
                }

                UserSecurityQuestionAnswerModel user_qa = null;
                PersonalProfileModel userProfile = null;

                SendEmailRequest emailRequest = new SendEmailRequest();
                string tempPassword = ConfigurationManager.AppSettings.Get("ResetTempPassword").ToString();
                //Get user secret question and password 
                using (var client = new HttpClient())
                {
                    var profile = client.GetAsync(Trinity_API_URL + "api/Account/GetUserSecurityQA/" + model.UserName).Result;
                    if (profile.StatusCode == HttpStatusCode.OK)
                    {
                        var data = profile.Content.ReadAsStringAsync().Result;
                        user_qa = JsonConvert.DeserializeObject<UserSecurityQuestionAnswerModel>(data);
                    }
                }
                //Get User Profile
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(Trinity_API_URL + "api/UserProfile/GetUserByNric/" + model.UserName).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = response.Content.ReadAsStringAsync().Result;
                        userProfile = JsonConvert.DeserializeObject<PersonalProfileModel>(data);
                        UserModel userModel = new UserModel();
                        if (userProfile.ForgetPass_FailAttempt == null || userProfile.ForgetPass_FailAttempt <= 3)
                        {
                            if (model.PasswordQuestion == user_qa.SecurityQuestion && model.PasswordQuesAnswer == user_qa.SecurtyAnswer)
                            {
                                //RESET PASSWORD
                                //HTTP POST
                                ForgetPasswordbindingModel fpmodel = new ForgetPasswordbindingModel();
                                fpmodel.NricNo = model.UserName;
                                fpmodel.Newpassword = tempPassword;
                                var fpresponse = client.PostAsJsonAsync(Trinity_API_URL + "api/Account/ForgetPassword", fpmodel).Result;
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    Supervisee updateProfileModel = new Supervisee();
                                    updateProfileModel.ForgetPass_FailAttempt = 0;
                                    updateProfileModel.DOB_FailAttempt = 0;
                                    var updateResult = client.PutAsJsonAsync(Trinity_API_URL + "api/UserProfile/updateuserprofile/" + userProfile.UserId, updateProfileModel).Result;
                                    if (updateResult.StatusCode == HttpStatusCode.OK)
                                    {
                                        // SEND EMAIL
                                        MessageModel msg = new MessageModel();
                                        string[] emailrecep = { userProfile.email };
                                        msg.reciepients = emailrecep;
                                        msg.mail = true;
                                        msg.sms = true;
                                        msg.messagebody = "Your password has been changed. New password is: " + tempPassword + " ,Please Change the password after login using the temporary password. Thank You.";
                                        msg.subject = "ABS - Reset Password";
                                        var sendEmail = client.PostAsJsonAsync(Trinity_API_URL + "api/Message/sendemailnotifications", msg).Result;

                                        //LOG ACTIVITY
                                        return RedirectToAction("ABSPasswordSuccess", "ABS", new { area = "ABS", UserEmail = userProfile.email });
                                    }
                                }
                            }
                            else
                            {
                                //Update the PasswordChagneFailAttempt count 
                                Supervisee updateProfileModel = new Supervisee();
                                if (userProfile.ForgetPass_FailAttempt == null || userProfile.ForgetPass_FailAttempt != 0)
                                {
                                    if (userProfile.ForgetPass_FailAttempt == null)
                                    {
                                        updateProfileModel.ForgetPass_FailAttempt = 1;
                                    }
                                    else
                                    {
                                        updateProfileModel.ForgetPass_FailAttempt = userProfile.ForgetPass_FailAttempt + 1;
                                    }
                                    updateProfileModel.DOB_FailAttempt = userProfile.DOB_FailAttempt;
                                }
                                else
                                {
                                    updateProfileModel.ForgetPass_FailAttempt = 1;
                                    updateProfileModel.DOB_FailAttempt = userProfile.DOB_FailAttempt;
                                }
                                var updateResult = client.PutAsJsonAsync(Trinity_API_URL + "api/UserProfile/updateuserprofile/" + userProfile.UserId, updateProfileModel).Result;
                                if (updateResult.StatusCode == HttpStatusCode.OK)
                                {
                                    if (updateProfileModel.ForgetPass_FailAttempt >= 4)
                                    {
                                        Session["ForgotPassModel"] = model;
                                        ViewBag.ErrorMessage = "Invalid entry";

                                        return View("ABSForgetPassword_DOB");
                                    }
                                }
                                Session["ForgotPassModel"] = model;
                                ViewBag.ErrorMessage = "Invalid entry";
                                return View("ABSForgetPassword");
                            }
                        }
                        else
                        {
                            Session["ForgotPassModel"] = model;
                            ViewBag.ErrorMessage = "Invalid entry";
                            return View("ABSForgetPassword_DOB");
                        }
                    }
                    else
                    {
                        return View("ABSDOForgetPassword");
                    }
                }
                return View("ABSDOForgetPassword");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.InnerException);
                return RedirectToAction("ShowErrorPage", "ABS", new { area = "ABS" });
            }
        }

        public ActionResult ABSForgetPassword_DOB()
        {
            return View();
        }

        public ActionResult ABSForgetPasswordMax()
        {
            return View();
        }

        public ActionResult ABSChangePasswordSuccess()
        {
            // var model = // build list based on parameter searchText
            return View();
        }

        public ActionResult ABSPasswordSuccess(string UserEmail)
        {
            ViewBag.UserEmail = UserEmail;
            return View();
        }

        public List<PasswordQuestionsModel> GetPasswordQuestions()
        {
            List<PasswordQuestionsModel> passwordQuestions = new List<PasswordQuestionsModel>();
            PasswordQuestionsModel model = null;
            model = new PasswordQuestionsModel();
            model.PasswordQuestionID = 1;
            model.PasswordQuestion = "Who is your favourite teacher in primary school?";
            passwordQuestions.Add(model);
            model = new PasswordQuestionsModel();
            model.PasswordQuestionID = 2;
            model.PasswordQuestion = "In what city or town did your mother and father meet?";
            passwordQuestions.Add(model);
            model = new PasswordQuestionsModel();
            model.PasswordQuestionID = 3;
            model.PasswordQuestion = "What is your favorite movie?";
            passwordQuestions.Add(model);
            model = new PasswordQuestionsModel();
            model.PasswordQuestionID = 4;
            model.PasswordQuestion = "What was your childhood nickname?";
            passwordQuestions.Add(model);
            model = new PasswordQuestionsModel();
            model.PasswordQuestionID = 5;
            model.PasswordQuestion = "What is the name of your favorite childhood friend?";
            passwordQuestions.Add(model);
            model = new PasswordQuestionsModel();
            model.PasswordQuestionID = 6;
            model.PasswordQuestion = "What was the make and model of your first car?";
            passwordQuestions.Add(model);

            return passwordQuestions;

            //using (var client = new HttpClient())
            //{

            //    client.BaseAddress = new Uri(ABSWebAPI);
            //    //HTTP GET
            //    var responseTask = client.GetAsync("Authentication/GetPasswordQuestions");
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<PasswordQuestionsResponse>();
            //        readTask.Wait();

            //        passwordQuestions = readTask.Result.PasswordQuestions;
            //        var questions = passwordQuestions.Select(x => new SelectListItem
            //        {
            //            Value = x.PasswordQuestionID.ToString(),
            //            Text = x.PasswordQuestion.ToString()
            //        });
            //        return new SelectList(questions, "Value", "Text");
            //    }
            //    else
            //    {
            //        throw new Exception();
            //    }
            //}


        }


        #region Old Code
        //if (model != null)
        //{
        //    if (Session["ForgotPassModel"] != null)
        //    {
        //        var DOB = model.DateOfBirth;
        //        model = (ForgetPasswordModel)(Session["ForgotPassModel"]);
        //        model.DateOfBirth = DOB;
        //    }
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(ABSWebAPI + "Authentication/ABSForgotPassword");
        //        //HTTP POST
        //        var postTask = client.PostAsJsonAsync<ForgetPasswordModel>("ABSForgotPassword", model);
        //        postTask.Wait();
        //        var result = postTask.Result;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsAsync<ForgotPasswordResponse>();
        //            readTask.Wait();
        //            if (readTask.Result.ResponseCode == 0)
        //            {
        //                if (readTask.Result.UserForgotInfo.IsSuccess)
        //                {
        //                    return View("_ABSPasswordSuccess");
        //                }
        //                else if (!readTask.Result.UserForgotInfo.IsSuccess)
        //                {
        //                    if (readTask.Result.UserForgotInfo.PasswordChangeFailAttempt > 3)
        //                    {
        //                        if (readTask.Result.UserForgotInfo.DateOfBirthFailAttempt > 3)
        //                        {

        //                            return View("_ABSForgotPasswordUnSuccess");
        //                        }
        //                        else
        //                        {
        //                            if (readTask.Result.UserForgotInfo.DateOfBirthFailAttempt >= 1)
        //                            {
        //                                ViewBag.ErrorMessage = "Invalid entry";
        //                            }
        //                            Session["ForgotPassModel"] = model;
        //                            return View("ABSForgetPasswordMax", model);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ViewBag.ErrorMessage = "Invalid entry";
        //                        return View("ABSForgetPassword");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception();
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception();
        //        }
        //    }
        //    return View();
        //}
        //else
        //{
        //    throw new Exception();
        //} 


        //    AuthenticationRequest request = new AuthenticationRequest();
        //    request.UserName = model.UserName;
        //    request.UserPassword = model.UserPassword;
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(ABSWebAPI + "Authentication/ABSLogin");
        //        //HTTP POST
        //        var postTask = client.PostAsJsonAsync<AuthenticationRequest>("ABSLogin", request);
        //        postTask.Wait();
        //        var result = postTask.Result;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsAsync<AuthenticationResponse>();
        //            readTask.Wait();
        //            if (readTask.Result.ResponseCode == 0)
        //            {
        //                if (readTask.Result.IsLoginFirst)
        //                {
        //                    Session["FirstLoginUserName"] = model.UserName;
        //                    return View("ABSChangePassword");
        //                }
        //                else
        //                {
        //                    // validate authentication and get user profile , messages section
        //                    if (readTask.Result.UserDetails.IsAuthenticated)
        //                    {
        //                        Session["LoginUser"] = model.UserName;
        //                        return View("ABSAppBooking");
        //                    }
        //                    else
        //                    {
        //                        if (readTask.Result.UserDetails.LoginFailAttemptCount > 3)
        //                        {
        //                            return View("ABSForgetPassword");
        //                        }
        //                        ViewBag.ErrorMessage = "Login Error";
        //                        return View("ABSLogin");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // throw to error page or home page http response is null or empty or any error
        //                Log.Error("AuthenticateABS: readTask.Result.ResponseCode != 0 ");
        //                return View(model);
        //            }
        //        }
        //        else
        //        {
        //            // throw to error page or home page http response is null or empty or any error
        //            //throw new Exception();
        //            Log.Error("AuthenticateABS: result.IsSuccessStatusCode: false");
        //            return View(model);
        //        }
        //    }
        //}
        //else
        //{
        //    //throw new Exception();
        //    Log.Error("AuthenticateABS: model is empty");
        //    return null;
        //} 


        //public ActionResult ABSGetBookAppointment()
        //{
        //    AppointmentModel appModel = null;
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
        //                    appModel = readTask.Result.AppointmentDetails;
        //                }
        //            }
        //        }
        //    }
        //    return View(appModel);
        //}


        //if (model != null && Session["FirstLoginUserName"] != null)
        //{
        //    ChangePasswordRequest request = new ChangePasswordRequest();
        //    request.UserName = Session["FirstLoginUserName"].ToString();
        //    request.UserPassword = model.UserPassword;
        //    request.PasswordQuestionID = model.PasswordQuestionID;
        //    request.PasswordQuesAnswer = model.PasswordQuesAnswer;
        //    request.RequestedBy = Session["FirstLoginUserName"].ToString();
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(ABSWebAPI + "Authentication/ABSChangePassword");
        //        //HTTP POST
        //        var postTask = client.PostAsJsonAsync<ChangePasswordRequest>("ABSChangePassword", request);
        //        postTask.Wait();

        //        var result = postTask.Result;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsAsync<AuthenticationResponse>();
        //            readTask.Wait();
        //            if (readTask.Result.ResponseCode == 0)
        //            {
        //                if (readTask.Result.ResponseMessage == "Success")
        //                {
        //                    //return View("ABSChangePasswordSuccess");
        //                    Session["LoginUser"] = Session["FirstLoginUserName"].ToString();
        //                    return View("ABSAppBooking");
        //                }
        //                else
        //                {
        //                    // validate authentication and get user profile , messages section
        //                    return View();
        //                }
        //            }
        //            else
        //            {
        //                // throw to error page or home page http response is null or empty or any error
        //                throw new Exception();
        //            }
        //        }
        //        else
        //        {
        //            // throw to error page or home page http response is null or empty or any error
        //            throw new Exception();
        //        }
        //    }
        //}
        //else
        //{
        //    throw new Exception();
        //} 
        #endregion




    }


}
