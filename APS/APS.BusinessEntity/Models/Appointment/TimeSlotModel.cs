using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace APS.BusinessEntity.Models.Appointment
{
    [DataContract]
    public class TimeSlotModel
    {
        [DataMember]
        public string TimeSlotID { get; set; }

        [DataMember]
        public string TimeSlot_Category { get; set; }

        [DataMember]
        public string TimeSlot_Description { get; set; }

        [DataMember]
        public string TimeSlot_WeekDay { get; set; }

        [DataMember]
        public TimeSpan? Slot_Start_Time { get; set; }

        [DataMember]
        public TimeSpan? Slot_End_Time { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime CreatedTime { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }

        [DataMember]
        public DateTime LastUpdatedTime { get; set; }

        [DataMember]
        public bool IsTimeSlotAvailable { get; set; }
    }

    public class TimeSlotDetailsForAppointmentModel
    {
        //[DataMember]
        //public string TimeSlot_WeekDay { get; set; }

        [DataMember]
        public string TimeSlot_Category { get; set; }

        [DataMember]
        public List<TimeSlotForAppointmentModel> TimeSlot_Appointment { get; set; }

    }

    public class TimeSlotForAppointmentModel
    {

        [DataMember]
        public string TimeSlotID { get; set; }

        [DataMember]
        public string TimeSlot_Description { get; set; }

        [DataMember]
        public bool IsTimeSlotAvailable { get; set; }

        [DataMember]
        public string Slot_Start_Time { get; set; }

        [DataMember]
        public string Slot_End_Time { get; set; }
    }


    public class TrinityTimeSlotModel
    {
        public string Timeslot_ID { get; set; }
        public string Category { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public int? MaximumSupervisee { get; set; }
        public int TimeSlotIDRef { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? App_cnt { get; set; }
    }
}
