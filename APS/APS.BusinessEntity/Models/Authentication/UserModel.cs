using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace APS.BusinessEntity.Models.Authentication
{
    public class UserModel
    {
        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserPassword { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsAuthenticated { get; set; }

        [DataMember]
        public int LoginFailAttemptCount { get; set; }

        [DataMember]
        public int PasswordChangeFailAttemptCount { get; set; }

        [DataMember]
        public int DOBFailAttemptCount { get; set; }

        [DataMember]
        public int SecretQuestionID { get; set; }

        //[DataMember]
        //public string SecretQuestion { get; set; }

        [DataMember]
        public string SecretQuestionAnswer { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }
    }
}
