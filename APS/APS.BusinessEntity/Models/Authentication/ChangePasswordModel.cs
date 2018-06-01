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
    public class ChangePasswordModel
    {
        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        public string UserName { get; set; }

        /*Passwords must be at least min. 8 and max. 16 characters in length, 
minimum of 1 lower case letter [a-z] and 
a minimum of 1 upper case letter [A-Z] and
a minimum of 1 numeric character [0-9] and
a minimum of 1 special character: $ @ $ ! % * ? & + = # 
PASSWORD EXAMPLE : @Password1 
*/
        [DataMember]
        [Required(ErrorMessage = "New Password is required")]
        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,16}$", ErrorMessage = "Your password must have at least 8 characters including a number.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,16}", ErrorMessage = "Your password must have at least 8 characters including 1 lower case letter, 1 upper case letter, 1 numeric character, 1 special character.")]
        public string UserPassword { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("UserPassword", ErrorMessage = "Password and confirm password must be same")]
        public string ReTypeUserPassword { get; set; }

        [DataMember]
        public string EncryptedUserPassword { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Question is required")]
        [Display(Name = "Secret Question")]
        public int PasswordQuestionID { get; set; }

        [DataMember]
        public string PasswordQuestion { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Answer is required")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string PasswordQuesAnswer { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,16}", ErrorMessage = "Your password must have at least 8 characters including 1 lower case letter, 1 upper case letter, 1 numeric character, 1 special character.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
