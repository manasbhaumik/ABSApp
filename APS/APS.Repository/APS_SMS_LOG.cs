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
    
    public partial class APS_SMS_LOG
    {
        public long SMS_LOG_ID { get; set; }
        public string SMS_MSG { get; set; }
        public string SMS_FROM_ADDR { get; set; }
        public string SMS_TO_ADDR { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_TIME { get; set; }
        public string LAST_UPDATED_BY { get; set; }
        public System.DateTime LAST_UPDATED_TIME { get; set; }
    }
}
