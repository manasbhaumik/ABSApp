using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using APS.BusinessEntity.Models.Authentication;

namespace APS.WebAPI.Models.Authentication
{
    public class ChangePasswordRequest
    {
        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public int PasswordQuestionID { get; set; }

        public string PasswordQuestion { get; set; }

        public string PasswordQuesAnswer { get; set; }

        public string RequestedBy { get; set; }
    }
}