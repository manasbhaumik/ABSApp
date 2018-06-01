using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace APS.WebAPI.Models.Authentication
{

    public class AuthenticationRequest
    {

        public string UserName { get; set; }

        public string UserPassword { get; set; }
    }
}