using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APS.BusinessEntity.Models.Authentication;
using APS.BusinessComponent.Authentication;
using APS.WebAPI.Models;
using APS.WebAPI.Models.Authentication;
using log4net;


namespace APS.WebAPI.Controllers
{
    public class AuthenticationController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // GET api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Log.Debug("Debug");
            Log.Error("Error");
            Log.Fatal("Fatal");
            Log.Info("Info");
            Log.Warn("Warn");
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet]
        public string Get(int id)
        {
            throw new Exception("Nasty exception here");
        }


        /// <summary>
        /// APS Login
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns></returns>
        public IHttpActionResult APSLogin([FromBody]AuthenticationRequest authRequest)
        {
            AuthenticationResponse authResponse = new AuthenticationResponse();
            if (authRequest != null)
            {
                UserModel authModel = new UserModel();
                authModel.UserName = authRequest.UserName;
                authModel.UserPassword = authRequest.UserPassword;
                using (AuthenticationBC authBC = new AuthenticationBC())
                {
                    // check is user first login
                    if (authBC.IsUserLoginFirstTime(authModel))
                    {
                        authResponse.ResponseCode = 0;
                        authResponse.ResponseMessage = "Success";
                        authResponse.IsLoginFirst = true;
                        authResponse.UserDetails = authModel;
                    }
                    else
                    {
                        //do authenticate against DB                        
                        UserModel model = authBC.AuthenticateUser(authModel);
                        if (model != null)
                        {
                            if (model.IsAuthenticated)
                            {
                                //authentication success
                                authResponse.ResponseCode = 0;
                                authResponse.ResponseMessage = "Success";
                                authResponse.UserDetails = model;
                            }
                            else
                            {
                                //authentication failed
                                authResponse.ResponseCode = 0;
                                authResponse.ResponseMessage = "Success";
                                authResponse.UserDetails = model;
                            }
                        }
                    }
                }
            }
            else
            {
                authResponse.ResponseCode = 1;
                authResponse.ResponseMessage = "AuthenticationRequest is empty.";
            }

            return Ok(authResponse);
        }

        /// <summary>
        /// Get Password Questions with ID
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetPasswordQuestions()
        {
            PasswordQuestionsResponse response = new PasswordQuestionsResponse();
            response.ResponseCode = 0;
            response.ResponseMessage = "Success";
            using (AuthenticationBC authBC = new AuthenticationBC())
            {
                response.PasswordQuestions = authBC.GetPasswordQuestions();
            }
            return Ok(response);
        }

        /// <summary>
        /// Change Password 
        /// </summary>
        /// <param name="changepwdRequest"></param>
        /// <returns></returns>
        public IHttpActionResult APSChangePassword(ChangePasswordRequest changepwdRequest)
        {
            ChangePasswordResponse response = new ChangePasswordResponse();

            using (AuthenticationBC authBC = new AuthenticationBC())
            {
                ChangePasswordModel cpModel = new ChangePasswordModel();
                cpModel.UserName = changepwdRequest.UserName;
                cpModel.UserPassword = changepwdRequest.UserPassword;
                cpModel.PasswordQuestionID = changepwdRequest.PasswordQuestionID;
                cpModel.PasswordQuestion = changepwdRequest.PasswordQuestion;
                cpModel.PasswordQuesAnswer = changepwdRequest.PasswordQuesAnswer;
                cpModel.CreatedBy = changepwdRequest.RequestedBy;
                cpModel.LastUpdatedBy = changepwdRequest.RequestedBy;
                UserModel userModel = authBC.ChangePassword(cpModel);
                if (userModel != null)
                {
                    response.ResponseCode = 0;
                    response.ResponseMessage = "Success";
                    response.UserInfo = userModel;
                }
                else
                {
                    response.ResponseCode = 1;
                    response.ResponseMessage = "Failed";
                }
                return Ok(response);
            }
        }

        public IHttpActionResult APSForgotPassword(ForgetPasswordModel forgotPassowrdRequest)
        {
            ForgotPasswordResponse frResponse = new ForgotPasswordResponse();
            using (AuthenticationBC authBC = new AuthenticationBC())
            {
                ForgetPasswordModel response = authBC.ForgetPassword(forgotPassowrdRequest);
                if (response != null)
                {
                    frResponse.ResponseCode = 0;
                    frResponse.ResponseMessage = "Success";
                    frResponse.UserForgotInfo = response;
                }
                else
                {
                    frResponse.ResponseCode = 1;
                    frResponse.ResponseMessage = "Failed";
                }
                return Ok(frResponse);
            }
        }
    }
}
