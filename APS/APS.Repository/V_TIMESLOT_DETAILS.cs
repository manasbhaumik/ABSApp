//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APS.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class V_TIMESLOT_DETAILS
    {
        public int TIMESLOT_ID { get; set; }
        public string TIMESLOT_CATEGORY { get; set; }
        public string TIMESLOT_DESC { get; set; }
        public Nullable<int> App_cnt { get; set; }
        public string TIMESLOT_WEEKDAY { get; set; }
        public System.TimeSpan SLOT_START_TIME { get; set; }
        public System.TimeSpan SLOT_END_TIME { get; set; }
    }
}
