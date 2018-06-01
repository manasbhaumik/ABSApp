using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using APS.BusinessEntity.Models.Authentication;
namespace APS.WebAPI.Models.Authentication
{
    public class ForgotPasswordResponse
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public ForgetPasswordModel UserForgotInfo { get; set; }
    }
}