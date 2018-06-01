using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APS.WebApp.Models.APSDataModel
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
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
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
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
        [Display(Name = "Security Question")]
        public string SecQuestion { get; set; }

        [Required]
        [Display(Name = "Security Answer")]
        public string SecAnswer { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "NRIC")]
        public string NRIC { get; set; }

        [Required]
        [Display(Name = "Mobile No")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "Security Question")]
        public string SecQuestion { get; set; }

        [Required]
        [Display(Name = "Security Answer")]
        public string SecAnswer { get; set; }
    }

    public class APSAppointment
    {
        public string[] AppBooking { get; set; }  // new proeprty
        public IEnumerable<APSBookingListModel> ApsBookingList { get; set; }
    }

    public class APSBookingListModel
    {        
        [Display(Name = "Appointment")]
        public string Appointment { get; set; }

        
        [Display(Name = "AppValue")]
        public string AppValue { get; set; }

        [Display(Name = "Slot")]
        public bool slot { get; set; }

        [Display(Name = "UserID")]
        public string UserID { get; set; }

        
        [Display(Name = "AppDate")]
        public string AppDate { get; set; }

        
    }
}
