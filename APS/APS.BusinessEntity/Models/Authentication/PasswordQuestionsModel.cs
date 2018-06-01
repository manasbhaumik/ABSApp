using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace APS.BusinessEntity.Models.Authentication
{
    [DataContract]
    public class PasswordQuestionsModel
    {
        [DataMember]
        public int PasswordQuestionID { get; set; }
        [DataMember]
        public string PasswordQuestion { get; set; }
    }
}
