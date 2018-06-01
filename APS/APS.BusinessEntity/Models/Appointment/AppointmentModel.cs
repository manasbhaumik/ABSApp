using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using APS.BusinessEntity.Models.Authentication;

namespace APS.BusinessEntity.Models.Appointment
{
    [DataContract]
    
    public class AppointmentModel
    {
        [DataMember]
        public long App_ID { get; set; }

        [DataMember]
        public string App_UserName { get; set; }

        [DataMember]
        public string App_Date { get; set; }

        [DataMember]
        public int App_TimeSlot_ID { get; set; }

        [DataMember]
        public string App_TimeSlot_Description { get; set; }

        [DataMember]
        public int App_Change_Count { get; set; }

        [DataMember]
        public int App_Absent_Count { get; set; }

        [DataMember]
        public int App_Status_CD { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }

        [DataMember]
        public DateTime LastUpdatedOn { get; set; }

        [DataMember]
        public bool IsAppointmentAvailable { get; set; }

        [DataMember]
        public List<TimeSlotDetailsForAppointmentModel> AppointmentTimeSlotDetails { get; set; }

        [DataMember]
        public UserInfoModel UserInfoDetails { get; set; }
    }

    public class Appointment
    {
        public Guid ID { get; set; }
        public string Timeslot_ID { get; set; }
        public Guid? AbsenceReporting_ID { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public short ChangedCount { get; set; }
        public string Status { get; set; }
        public DateTime? ReportTime { get; set; }
        public TimeSpan StrTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }


}
