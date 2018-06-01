using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace APS.WebAPI.Models.Appointment
{
    public class AppointmentDetailsRequest
    {
        public string UserName { get; set; }
    }
}