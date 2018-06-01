using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Data.Entity;
using APS.BusinessEntity.Models.Appointment;
using System.Configuration;

namespace APS.Repository.Appointment
{
    public static class AppointmentBookingExtensions
    {
        public static TimeSlotModel ToTimeSlotModel(this APS_TIMESLOT model)
        {
            if (model == null)
            {
                return null;
            }
            var row = new TimeSlotModel();
            {
               // row.TimeSlotID = model.TIMESLOT_ID;
                row.TimeSlot_Category = model.TIMESLOT_CATEGORY;
                row.TimeSlot_Description = model.TIMESLOT_DESC;
                row.TimeSlot_WeekDay = model.TIMESLOT_WEEKDAY;
                //row.Slot_Start_Time = model.SLOT_START_TIME.ToString("hh:mm");
                //row.Slot_End_Time = model.SLOT_END_TIME.ToString("hh:mm");
                row.CreatedBy = model.CREATED_BY;
                row.CreatedTime = model.CREATED_TIME;
                row.LastUpdatedBy = model.LAST_UPDATED_BY;
                row.LastUpdatedTime = model.LAST_UPDATED_TIME;
            }
            return row;
        }

        public static TimeSlotModel ToTimeSlotDetailsModel(this V_TIMESLOT_DETAILS model)
        {
            int slotAppCount = Convert.ToInt16(ConfigurationManager.AppSettings.Get("AppointSlotCount").ToString());
            if (model == null)
            {
                return null;
            }
            var row = new TimeSlotModel();
            {
                //row.TimeSlotID = model.TIMESLOT_ID;
                row.TimeSlot_Category = model.TIMESLOT_CATEGORY;
                row.TimeSlot_Description = model.TIMESLOT_DESC;
                row.TimeSlot_WeekDay = model.TIMESLOT_WEEKDAY;
                row.IsTimeSlotAvailable = model.App_cnt >= slotAppCount ? false : true;
               // row.Slot_Start_Time = model.SLOT_START_TIME.ToString();
                //row.Slot_End_Time = model.SLOT_END_TIME.ToString();
            }
            return row;
        }


        public static AppointmentModel ToAppointmentModel(this V_APPOINTMENT_DETAILS model)
        {
            if (model == null)
            {
                var row_NULL = new AppointmentModel();
                {
                    row_NULL.IsAppointmentAvailable = false;
                }
                return row_NULL;
            }
            var row = new AppointmentModel();
            {
                row.App_Absent_Count = 0;
                row.App_Change_Count = model.APP_CHANGE_CNT;
                row.App_Date = model.APP_DATE.ToString("dd MMM yyyy");
                row.App_ID = model.APP_ID;
                row.App_Status_CD = model.APP_STATUS;
                row.App_TimeSlot_ID = model.APP_TIMESLOT_ID;
                row.App_TimeSlot_Description = model.TIMESLOT_DESC;
                row.App_UserName = model.APP_USERNAME;
                row.CreatedBy = model.CREATED_BY;
                row.CreatedOn = model.CREATED_ON;
                row.LastUpdatedBy = model.LAST_UPDATED_BY;
                row.LastUpdatedOn = model.LAST_UPDATED_ON;
                row.IsAppointmentAvailable = true;
            }
            return row;
        }
    }
}
