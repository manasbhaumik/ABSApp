using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APS.BusinessEntity.Models.Authentication;
using APS.BusinessEntity.Models.Common;
using APS.Repository.Authentication;
using APS.Common.Cryptography;
using APS.Common.EmailComponent;
using APS.Common.Logging;
using System.Configuration;
using log4net;

namespace APS.BusinessComponent.Authentication
{
    public class AuthenticationBC : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Dispose()
        {
            //Dispose(true); //I am calling you from Dispose, it's safe
            GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        }

        public bool IsUserLoginFirstTime(UserModel authModel)
        {
            using (AuthenticationRepository repo = new AuthenticationRepository())
            {
                return repo.IsFirstLogin(authModel);
            }
        }

        public IEnumerable<PasswordQuestionsModel> GetPasswordQuestions()
        {
            using (AuthenticationRepository repo = new AuthenticationRepository())
            {
                return repo.GetPasswordQuestions();
            }
        }

        public UserModel ChangePassword(ChangePasswordModel model)
        {
            ActivityLoggingComponent activityLog = new ActivityLoggingComponent();
            UserModel authModel = new UserModel();
            authModel.UserName = model.UserName;
            if (IsUserLoginFirstTime(authModel))
            {
                //first time login create new user with password
                UserModel userModel = new UserModel();
                userModel.UserName = model.UserName;
                userModel.UserPassword = model.UserPassword;
                userModel.SecretQuestionID = model.PasswordQuestionID;
                userModel.SecretQuestionAnswer = model.PasswordQuesAnswer;
                userModel.IsDeleted = false;
                userModel.LoginFailAttemptCount = 0;
                userModel.CreatedBy = model.CreatedBy;
                userModel.LastUpdatedBy = model.LastUpdatedBy;
                return CreateNewUser(userModel);
            }
            else
            {
                //change the user password
                using (AuthenticationRepository repo = new AuthenticationRepository())
                {
                    string cryptoKey = ConfigurationManager.AppSettings.Get("APSEncryptionKey").ToString();
                    ICryptoLibrary cryptLib = new CryptoLibrary();
                    string encrypted_pwd = cryptLib.Encrypt(model.UserPassword, cryptoKey);
                    model.UserPassword = encrypted_pwd;
                    var changePassowrd = repo.ChangeUserPassword(model);
                    if (changePassowrd.UserName != string.Empty)
                    {
                        UserActivityLogModel activityModel = new UserActivityLogModel
                        {
                            User_Name = changePassowrd.UserName,
                            User_Act_Log = "Change password is successful. Activated User Name is: " + changePassowrd.UserName + ".",
                            CreatedBy = changePassowrd.UserName,
                            CreatedTime = DateTime.Now
                        };
                        activityLog.ActivityLog(activityModel);
                    }
                    return changePassowrd;
                }
            }
        }

        public UserModel CreateNewUser(UserModel model)
        {
            ActivityLoggingComponent activityLog = new ActivityLoggingComponent();
            string cryptoKey = ConfigurationManager.AppSettings.Get("APSEncryptionKey").ToString();
            if (!string.IsNullOrEmpty(cryptoKey))
            {
                ICryptoLibrary cryptLib = new CryptoLibrary();
                string encrypted_pwd = cryptLib.Encrypt(model.UserPassword, cryptoKey);
                model.UserPassword = encrypted_pwd;
                using (AuthenticationRepository repo = new AuthenticationRepository())
                {
                    var userCreation = repo.CreateNewUser(model);
                    if (userCreation.UserID > 0)
                    {
                        UserActivityLogModel activityModel = new UserActivityLogModel
                        {
                            User_Name = userCreation.UserName,
                            User_Act_Log = "User creation successful. Newly created User Name is: " + userCreation.UserName + ".",
                            CreatedBy = userCreation.CreatedBy,
                            CreatedTime = DateTime.Now
                        };
                        activityLog.ActivityLog(activityModel);
                    }
                    return userCreation;
                }
            }
            else
            {
                UserActivityLogModel activityModel = new UserActivityLogModel
                {
                    User_Name = model.UserName,
                    User_Act_Log = "Error occurred! User creation was unsuccessful for : " + model.UserName + ".",
                    CreatedBy = model.CreatedBy,
                    CreatedTime = DateTime.Now
                };
                activityLog.ActivityLog(activityModel);
                throw new Exception();
            }
        }

        public UserModel AuthenticateUser(UserModel model)
        {
            string cryptoKey = ConfigurationManager.AppSettings.Get("APSEncryptionKey").ToString();
            if (!string.IsNullOrEmpty(cryptoKey))
            {
                ICryptoLibrary cryptLib = new CryptoLibrary();
                using (AuthenticationRepository repo = new AuthenticationRepository())
                {
                    UserModel userModel = new UserModel();
                    userModel.UserName = model.UserName;
                    userModel = repo.GetUserDetails(userModel);
                    if (userModel != null)
                    {
                        string decrypted_pwd = cryptLib.Decrypt(userModel.UserPassword, cryptoKey);
                        if (model.UserPassword == decrypted_pwd)
                        {
                            //authentication success
                            userModel.IsAuthenticated = true;
                            return userModel;
                        }
                        else
                        { //login attempt failed
                            userModel.IsAuthenticated = false;
                            userModel.LoginFailAttemptCount = userModel.LoginFailAttemptCount + 1;
                            userModel.LastUpdatedBy = userModel.UserName;
                            userModel = repo.UpdateLoginFailedCount(userModel);
                            return userModel;
                        }
                    }
                    else
                    {
                        Log.Error("AuthenticateUser: No User info available in DB for this user: " + userModel.UserName);
                        return null;
                    }
                }
            }
            else
            {
                Log.Error("AuthenticateUser: APSEncryptionKey is empty");
                return null;
            }
        }

        public ForgetPasswordModel ForgetPassword(ForgetPasswordModel model)
        {
            ActivityLoggingComponent activityLog = new ActivityLoggingComponent();
            string cryptoKey = ConfigurationManager.AppSettings.Get("APSEncryptionKey").ToString();
            string tempPassword = ConfigurationManager.AppSettings.Get("ResetTempPassword").ToString();
            if (!string.IsNullOrEmpty(cryptoKey))
            {
                ICryptoLibrary cryptLib = new CryptoLibrary();
                using (AuthenticationRepository repo = new AuthenticationRepository())
                {
                    SendEmailRequest emailRequest = new SendEmailRequest();
                    UserModel userModel = new UserModel();
                    userModel.UserName = model.UserName;
                    userModel = repo.GetUserDetails(userModel);
                    if (userModel != null)
                    {
                        if (userModel.PasswordChangeFailAttemptCount <= 3)
                        {
                            if (model.PasswordQuestionID == userModel.SecretQuestionID && model.PasswordQuesAnswer == userModel.SecretQuestionAnswer)
                            {
                                // Reset the password 
                                string encrypted_pwd = cryptLib.Encrypt(tempPassword, cryptoKey);
                                model.UserPassword = encrypted_pwd;
                                model.PasswordChangeFailAttempt = 0;
                                model.IsSuccess = true;
                                model.LastUpdatedBy = model.UserName;
                                var resetModel = repo.ResetPassword(model);
                                //Then send email or sms the password to the user
                                if (!string.IsNullOrEmpty(model.Email))
                                {
                                    BuildAndSendEmailRequest(emailRequest, model.Email, "APS - Reset Password", "Your password has been changed. New password is: " + tempPassword + "Please Change the password after login using the temporary password. Thank You.");
                                }
                                else if (!string.IsNullOrEmpty(model.MobileNumber))
                                {
                                    // send SMS
                                }
                                if (!string.IsNullOrEmpty(model.UserName))
                                {
                                    UserActivityLogModel activityModel = new UserActivityLogModel
                                    {
                                        User_Name = model.UserName,
                                        User_Act_Log = "Reset password is successful. Activated User Name is: " + model.UserName + ".",
                                        CreatedBy = model.UserName,
                                        CreatedTime = DateTime.Now
                                    };
                                    activityLog.ActivityLog(activityModel);
                                }
                                return resetModel;
                            }
                            else
                            {
                                //Update the PasswordChagneFailAttempt count 
                                if (userModel.PasswordChangeFailAttemptCount != 0)
                                {
                                    model.PasswordChangeFailAttempt = userModel.PasswordChangeFailAttemptCount + 1;
                                }
                                else
                                {
                                    model.PasswordChangeFailAttempt = 1;
                                }
                                model.IsSuccess = false;
                                model.LastUpdatedBy = model.UserName;
                                return repo.UpdateChangePasswordFailedCount(model);
                            }
                        }
                        else
                        {
                            if (userModel.DOBFailAttemptCount <= 3)
                            {
                                UserInfoModel userInfoModel = new UserInfoModel();
                                userInfoModel.UserName = model.UserName;
                                userInfoModel = repo.GetUserInfoDetails(userInfoModel);
                                if (userInfoModel != null)
                                {
                                    string dateOfBirth = userInfoModel.UserDateOfBirth;
                                    if (dateOfBirth == model.DateOfBirth)
                                    {
                                        string encrypted_pwd = cryptLib.Encrypt(tempPassword, cryptoKey);
                                        model.UserPassword = encrypted_pwd;
                                        model.DateOfBirthFailAttempt = 0;
                                        model.IsSuccess = true;
                                        model.LastUpdatedBy = model.UserName;
                                        model.PasswordChangeFailAttempt = userModel.PasswordChangeFailAttemptCount;
                                        var resetModel = repo.ResetPassword(model);
                                        //Then send email or sms the password to the user
                                        if (!string.IsNullOrEmpty(model.Email))
                                        {
                                            BuildAndSendEmailRequest(emailRequest, model.Email, "APS - Reset Password", "Your password has been changed. New password is: " + tempPassword + " Please Change the password after login using the temporary password. Thank You.");
                                        }
                                        else if (!string.IsNullOrEmpty(model.MobileNumber))
                                        {
                                            // send SMS
                                        }
                                        if (!string.IsNullOrEmpty(model.UserName))
                                        {
                                            UserActivityLogModel activityModel = new UserActivityLogModel
                                            {
                                                User_Name = model.UserName,
                                                User_Act_Log = "Reset password is successful using Date of birth option. Activated User Name is: " + model.UserName + ".",
                                                CreatedBy = model.UserName,
                                                CreatedTime = DateTime.Now
                                            };
                                            activityLog.ActivityLog(activityModel);
                                        }
                                        return resetModel;
                                    }
                                    else
                                    {
                                        //Update the PasswordChagneFailAttempt count 
                                        if (userModel.DOBFailAttemptCount != 0)
                                        {
                                            model.DateOfBirthFailAttempt = userModel.DOBFailAttemptCount + 1;
                                        }
                                        else
                                        {
                                            model.DateOfBirthFailAttempt = 1;
                                        }
                                        model.IsSuccess = false;
                                        model.LastUpdatedBy = model.UserName;
                                        model.PasswordChangeFailAttempt = userModel.PasswordChangeFailAttemptCount;
                                        return repo.UpdateDateOfBirthFailCount(model);
                                    }
                                }
                                else
                                {
                                    model.IsSuccess = false;
                                    return model;
                                }
                            }
                            else
                            {
                                //Contact case officer - Exceeded all the possible way
                                model.IsSuccess = false;
                                return model;
                            }
                        }
                    }
                    model.IsSuccess = false;
                    return model;
                }
            }
            else
            {
                throw new Exception();
            }
        }

        private static void BuildAndSendEmailRequest(SendEmailRequest emailRequest, string toEmail, string emailSubject, string emailBody)
        {
            emailRequest.ToEmails = toEmail;
            emailRequest.EmailSubject = emailSubject;
            emailRequest.EmailBody = emailBody;
            EmailComponent objemail = new EmailComponent();
            objemail.SendEmail(emailRequest);
        }
    }
}
