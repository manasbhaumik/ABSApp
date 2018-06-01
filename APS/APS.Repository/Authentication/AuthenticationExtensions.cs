using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APS.BusinessEntity.Models.Authentication;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace APS.Repository.Authentication
{
    public static class AuthenticationExtensions
    {
        public static PasswordQuestionsModel ToPasswordQuestionsModel(this APS_PWD_QUESTIONS model)
        {
            if (model == null)
            {
                return null;
            }
            var row = new PasswordQuestionsModel();
            {
                row.PasswordQuestionID = model.PWD_QUES_ID;
                row.PasswordQuestion = model.PWD_QUES;
            }
            return row;
        }

        public static UserModel ToUserModel(this APS_USERS model)
        {
            if (model == null)
            {
                return null;
            }
            var row = new UserModel();
            {
                row.UserID = model.USER_ID;
                row.UserName = model.USER_NAME;
                row.UserPassword = model.USER_PASSWORD;
                row.IsDeleted = model.IS_DELETED == "0" ? true : false;
                row.LoginFailAttemptCount = model.LOGIN_FAIL_ATTEMPT;
                row.SecretQuestionID = model.PWD_QUES_ID;
                row.SecretQuestionAnswer = model.PWD_QUES_ANSWER;
                row.PasswordChangeFailAttemptCount = model.PWD_CHANGE_FAIL_ATTMPT;
                row.DOBFailAttemptCount = model.DOB_FAIL_ATTEMPT;
            }
            return row;
        }

        public static UserInfoModel ToUserInfoModel(this APS_USER_INFO model)
        {
            if (model == null)
            {
                return null;
            }
            var row = new UserInfoModel();
            {
                row.UserID = model.USER_ID;
                row.UserName = model.USER_NAME;
                row.UserFirstName = model.USER_FIRST_NAME;
                row.UserLastName = model.USER_LAST_NAME;
                row.UserMobileNo = model.USER_MOBILE_NO;
                row.UserEmail = model.USER_EMAIL_ADDR;
                row.UserDateOfBirth = Convert.ToDateTime(model.USER_DATE_OF_BIRTH).ToString("dd-MM-yyyy");
            }
            return row;
        }
    }
}
