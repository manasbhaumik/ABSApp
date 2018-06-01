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
    public class AuthenticationModel
    {
        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        [RegularExpression(@"^[STFG]\d{7}[A-Z]$", ErrorMessage = "Incorrect NRIC format")]
        public string UserName { get; set; }


        [DataMember]
        [Required(ErrorMessage = "Password is required")]
        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,16}$", ErrorMessage = "Your password must have at least 8 characters including a number.")]
        //[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&+=#]) [A-Za-z\\d$@$!%*?&+=#]{8,16}$", ErrorMessage = "Your password must have at least 8 characters including 1 lower case letter, 1 upper case letter, 1 numeric character, 1 special character.")]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public int LoginFailAttemptCount { get; set; }

        [DataMember]
        public int SecretQuestionID { get; set; }

        [DataMember]
        public string SecretQuestionAnswer { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Nric No.")]
        public string NricNo { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Contact No.")]
        [DataType(DataType.PhoneNumber)]
        public string ContactNo { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "D.O.B")]
        public DateTime DOB { get; set; }

        [Required]
        [Display(Name = "Center")]
        public string Center { get; set; }

        [Required]
        [Display(Name = "Visiting Frequency")]
        public int VstFrequency { get; set; }

        [Required]
        [Display(Name = "Probation Starting Date :")]
        public DateTime PrbSrtDate { get; set; }

        [Required]
        [Display(Name = "Probation Finishing Date :")]
        public DateTime PrbEndDate { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }
    }

    public class NewUserPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string SecurityQuestion { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string SecurityAnswer { get; set; }
    }

    public class ForgetPasswordbindingModel
    {
        [Required]
        [Display(Name = "Nric No.")]
        public string NricNo { get; set; }

        public string Newpassword { get; set; }

    }

    public class UserSecurityQuestionAnswerModel
    {
        public string UserId { get; set; }

        public string SecurityQuestion { get; set; }

        public string SecurtyAnswer { get; set; }
    }
}
