using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using APS.BusinessEntity.Models.Appointment;

namespace APS.WebAPI.Models.Appointment
{
    public class AppointmentDetailsResponse
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public AppointmentModel AppointmentDetails { get; set; }
    }
}