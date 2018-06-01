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
    public class ForgetPasswordModel
    {
        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Please enter Username")]
        [RegularExpression(@"^[STFG]\d{7}[A-Z]$", ErrorMessage = "Please enter valid Username.")]
        public string UserName { get; set; }

        [DataMember]
        public string UserPassword { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Please select secret Question")]
        public int PasswordQuestionID { get; set; }

        [DataMember]
        public string PasswordQuestion { get; set; }

        [DataMember]
        public string MobileNumber { get; set; }

        [DataMember]
        //[RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Enter valid email address.")]
        [Required(ErrorMessage = "Mobile No./Email ID is required")]
        public string Email { get; set; }

        [DataMember]
        public int PasswordChangeFailAttempt { get; set; }

        [DataMember]
        public int DateOfBirthFailAttempt { get; set; }

        [DataMember]
        [Required(ErrorMessage = "DateofBirth is required")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfBirth { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Answer is required")]
        public string PasswordQuesAnswer { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public int StatusCode { get; set; }

    }
}
