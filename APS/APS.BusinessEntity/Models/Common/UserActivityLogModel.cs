using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace APS.BusinessEntity.Models.Common
{
    [DataContract]
    public class UserActivityLogModel
    {
        [DataMember]
        public long User_Act_LogID { get; set; }

        [DataMember]
        public string User_Act_Log { get; set; }

        [DataMember]
        public string User_Name { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime CreatedTime { get; set; }
    }


   
}
