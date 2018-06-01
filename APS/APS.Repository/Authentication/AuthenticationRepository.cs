using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APS.BusinessEntity.Models.Authentication;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Configuration;
using System.Globalization;

namespace APS.Repository.Authentication
{
    public class AuthenticationRepository : IDisposable
    {
        public void Dispose()
        {
            //Dispose(true); //I am calling you from Dispose, it's safe
            GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        }

        /// <summary>
        /// Check is user login to system for first time
        /// </summary>
        /// <param name="authModel"></param>
        /// <returns></returns>
        public bool IsFirstLogin(UserModel authModel)
        {
            bool isfirstLogin = false;

            APSEntities dbContext = new APSEntities();
            var objUser = dbContext.APS_USERS.Where(i => i.USER_NAME == authModel.UserName).ToList();
            if (objUser != null && objUser.Count() == 0)
            {
                isfirstLogin = true;
            }
            return isfirstLogin;
        }

        /// <summary>
        /// Get all password questions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PasswordQuestionsModel> GetPasswordQuestions()
        {
            APSEntities dbContext = new APSEntities();
            return dbContext.APS_PWD_QUESTIONS.ToArray().Select(p => p.ToPasswordQuestionsModel());
        }

        public UserModel ChangeUserPassword(ChangePasswordModel model)
        {
            APSEntities dbContext = new APSEntities();
            var selectedEntity = dbContext.APS_USERS.FirstOrDefault(p => p.USER_NAME == model.UserName);
            selectedEntity.USER_NAME = model.UserName;
            selectedEntity.USER_PASSWORD = model.UserPassword;
            selectedEntity.PWD_QUES_ID = model.PasswordQuestionID;
            selectedEntity.PWD_QUES_ANSWER = model.PasswordQuesAnswer;
            selectedEntity.LAST_UPDATED_BY = model.LastUpdatedBy;
            selectedEntity.LAST_UPDATED_TIME = DateTime.Now;

            dbContext.Entry(selectedEntity).State = EntityState.Modified;
            dbContext.SaveChanges();
            UserModel userModel = ToUserModelFromChangePassword(model);
            return userModel;
        }

        public ForgetPasswordModel ResetPassword(ForgetPasswordModel model)
        {
            APSEntities dbContext = new APSEntities();
            var selectedEntity = dbContext.APS_USERS.FirstOrDefault(p => p.USER_NAME == model.UserName);
            selectedEntity.USER_NAME = model.UserName;
            selectedEntity.USER_PASSWORD = model.UserPassword;
            selectedEntity.PWD_CHANGE_FAIL_ATTMPT = model.PasswordChangeFailAttempt;
            selectedEntity.LAST_UPDATED_BY = model.LastUpdatedBy;
            selectedEntity.LOGIN_FAIL_ATTEMPT = 0;
            selectedEntity.PWD_CHANGE_FAIL_ATTMPT = 0;
            selectedEntity.DOB_FAIL_ATTEMPT = 0;
            selectedEntity.LAST_UPDATED_TIME = DateTime.Now;

            dbContext.Entry(selectedEntity).State = EntityState.Modified;
            dbContext.SaveChanges();

            return model;
        }

        public ForgetPasswordModel UpdateChangePasswordFailedCount(ForgetPasswordModel model)
        {
            APSEntities dbContext = new APSEntities();
            var selectedEntity = dbContext.APS_USERS.FirstOrDefault(p => p.USER_NAME == model.UserName);
            selectedEntity.USER_NAME = model.UserName;
            selectedEntity.PWD_CHANGE_FAIL_ATTMPT = model.PasswordChangeFailAttempt;
            selectedEntity.LAST_UPDATED_BY = model.LastUpdatedBy;
            selectedEntity.LAST_UPDATED_TIME = DateTime.Now;
            dbContext.Entry(selectedEntity).State = EntityState.Modified;
            dbContext.SaveChanges();
            return model;
        }

        public UserModel UpdateLoginFailedCount(UserModel model)
        {
            APSEntities dbContext = new APSEntities();
            var selectedEntity = dbContext.APS_USERS.FirstOrDefault(p => p.USER_NAME == model.UserName);
            selectedEntity.USER_NAME = model.UserName;
            selectedEntity.LOGIN_FAIL_ATTEMPT = model.LoginFailAttemptCount;
            selectedEntity.LAST_UPDATED_BY = model.LastUpdatedBy;
            selectedEntity.LAST_UPDATED_TIME = DateTime.Now;
            dbContext.Entry(selectedEntity).State = EntityState.Modified;
            dbContext.SaveChanges();
            return selectedEntity.ToUserModel();
        }

        public ForgetPasswordModel UpdateDateOfBirthFailCount(ForgetPasswordModel model)
        {
            APSEntities dbContext = new APSEntities();
            var selectedEntity = dbContext.APS_USERS.FirstOrDefault(p => p.USER_NAME == model.UserName);
            selectedEntity.USER_NAME = model.UserName;
            selectedEntity.DOB_FAIL_ATTEMPT = model.DateOfBirthFailAttempt;
            selectedEntity.LAST_UPDATED_BY = model.LastUpdatedBy;
            selectedEntity.LAST_UPDATED_TIME = DateTime.Now;
            dbContext.Entry(selectedEntity).State = EntityState.Modified;
            dbContext.SaveChanges();
            return model;
        }

        public UserModel ToUserModelFromChangePassword(ChangePasswordModel model)
        {
            if (model == null)
            {
                return null;
            }
            var row = new UserModel();
            {
                row.UserID = model.UserID;
                row.UserName = model.UserName;
                row.UserPassword = model.UserPassword;
                row.IsDeleted = false;
                row.SecretQuestionID = model.PasswordQuestionID;
                row.SecretQuestionAnswer = model.PasswordQuesAnswer;
                row.LastUpdatedBy = model.LastUpdatedBy;
            }
            return row;
        }

        public UserModel CreateNewUser(UserModel model)
        {
            APSEntities dbContext = new APSEntities();
            var objuser = new APS_USERS
            {
                USER_NAME = model.UserName,
                USER_PASSWORD = model.UserPassword,
                PWD_QUES_ID = model.SecretQuestionID,
                PWD_QUES_ANSWER = model.SecretQuestionAnswer,
                IS_DELETED = model.IsDeleted ? "0" : "1",
                LOGIN_FAIL_ATTEMPT = model.LoginFailAttemptCount,
                CREATED_BY = model.CreatedBy,
                CREATED_TIME = DateTime.Now,
                LAST_UPDATED_BY = model.LastUpdatedBy,
                LAST_UPDATED_TIME = DateTime.Now
            };

            dbContext.APS_USERS.Add(objuser);
            dbContext.SaveChanges();
            model.UserID = objuser.USER_ID;

            // create user dummy info

            var objUser_info = new APS_USER_INFO
            {
                USER_ID = model.UserID,
                USER_NAME = model.UserName,
                USER_FIRST_NAME = "DUMMY",
                USER_LAST_NAME = model.UserName,
                USER_MOBILE_NO = "86346745",
                USER_DATE_OF_BIRTH = DateTime.ParseExact("21-07-1980", "dd-mm-yyyy", CultureInfo.InvariantCulture),
                USER_EMAIL_ADDR = ConfigurationManager.AppSettings.Get("DummyEmailID").ToString(),
                CASE_OFFICER_NAME = "CO" + " " + model.UserName,
                CASE_OFFICER_EMAIL = ConfigurationManager.AppSettings.Get("DummyEmailID").ToString(),
                CASE_OFFICER_MOBILE_NO = "86346745",
                CREATED_BY = model.CreatedBy,
                CREATED_TIME = DateTime.Now,
                LAST_UPDATED_BY = model.LastUpdatedBy,
                LAST_UPDATED_TIME = DateTime.Now
            };
            dbContext.APS_USER_INFO.Add(objUser_info);
            dbContext.SaveChanges();
            return model;
        }

        public bool AuthenticateUser(AuthenticationModel authModel)
        {
            bool isAuthenticate = false;
            APSEntities dbContext = new APSEntities();
            var objUser = dbContext.APS_USERS.Where(i => i.USER_NAME == authModel.UserName && i.USER_PASSWORD == authModel.UserPassword).ToList();
            if (objUser != null && objUser.Count() > 0)
            {
                isAuthenticate = true;
            }
            return isAuthenticate;
        }

        public UserModel GetUserDetails(UserModel model)
        {
            APSEntities dbContext = new APSEntities();
            var objUser = dbContext.APS_USERS.Where(i => i.USER_NAME == model.UserName).ToArray();
            if (objUser != null && objUser.Count() > 0)
            {
                return objUser.Select(u => u.ToUserModel()).FirstOrDefault();
            }
            else
            {
                throw new Exception();
            }
        }

        public UserInfoModel GetUserInfoDetails(UserInfoModel model)
        {
            APSEntities dbContext = new APSEntities();
            var objUser = dbContext.APS_USER_INFO.Where(i => i.USER_NAME == model.UserName).ToArray();
            if (objUser != null && objUser.Count() > 0)
            {
                return objUser.Select(u => u.ToUserInfoModel()).FirstOrDefault();
            }
            else
            {
                throw new Exception();
            }
        }

    }
}
