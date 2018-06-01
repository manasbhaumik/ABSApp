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
    public class UserInfoModel
    {
        [DataMember]
        public long UserID { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }

        [DataMember]
        public string UserMobileNo { get; set; }

        [DataMember]
        public string UserEmail { get; set; }

        [DataMember]
        public string CaseOfficerName { get; set; }

        [DataMember]
        public string CaseOfficerMobileNo { get; set; }

        [DataMember]
        public string CaseOfficerEmail { get; set; }

        [DataMember]
        public string UserInfoCreatedBy { get; set; }

        [DataMember]
        public DateTime UserInfoCreatedOn { get; set; }

        [DataMember]
        public string UserInfoLastUpdatedBy { get; set; }

        [DataMember]
        public DateTime UserInfoLastUpdatedOn { get; set; }

        [DataMember]
        public string UserDateOfBirth { get; set; }
    }

    public class UserDetailsViewModel
    {
        public string id { get; set; }
        public string NricNo { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string SmartCardID { get; set; }
        public string[] rolename { get; set; }
        public string status { get; set; }
        public string CaseOfficer { get; set; }
        public string Center { get; set; }
        public string Vstfrequency { get; set; }
        public string ProbationStartDate { get; set; }
        public string ProbationEndDate { get; set; }
        public byte[] photo { get; set; }
    }
    public class PersonalProfileModel
    {
        public string UserId { get; set; }
        public string nricno { get; set; }
        public string name { get; set; }
        public string contactno { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string cardno { get; set; }
        public string BlkHouse_Number { get; set; }
        public string FlrUnit_Number { get; set; }
        public string Street_Name { get; set; }
        public string Country { get; set; }
        public string Postal_Code { get; set; }
        public string altBlkHouse_Number { get; set; }
        public string altFlrUnit_Number { get; set; }
        public string altStreet_Name { get; set; }
        public string altCountry { get; set; }
        public string altPostal_Code { get; set; }
        public string[] rolename { get; set; }
        public int? ForgetPass_FailAttempt { get; set; }
        public int? DOB_FailAttempt { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Supervisee supervisee { get; set; }
        public string Primary_Email { get; set; }

    }
    public class Supervisee : PersonalProfileModel
    {
        public string caseofficerid { get; set; }
        public string caseofficernric { get; set; }
        public string officername { get; set; }
        public string coffphoneno { get; set; }
        public string lastmessagedate { get; set; }
        public AppointmentModel appointment { get; set; }
        public DocSubmission documents { get; set; }
    }
    public class NotificationModel
    {
        public string NoticeID { get; set; }
        public string FromUsrID { get; set; }
        public string FromName { get; set; }
        public string TousrID { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string NoticeDate { get; set; }
        public string SourceTable { get; set; }
        public string RecId { get; set; }
        public bool isread { get; set; }

    }

    public class AppointmentModel : Supervisee
    {
        public string appID { get; set; }
        public DateTime appdate { get; set; }
        public TimeSpan starttime { get; set; }
        public TimeSpan endtime { get; set; }
        public DateTime? reporttime { get; set; }
        public string appstatus { get; set; }
        public string nxtdate { get; set; }
        public int? rescheduled { get; set; }
        public AbsenceReport AbsenceReport { get; set; }
    }

    public class MessageModel
    {
        public int msgid { get; set; }
        public string sender { get; set; }
        [Required]
        public string subject { get; set; }
        [Required]
        public string messagebody { get; set; }
        public string deldate { get; set; }
        [Required]
        public string[] reciepients { get; set; }
        [Required]
        public bool sms { get; set; }
        [Required]
        public bool mail { get; set; }
    }
    public class OfficerModel : PersonalProfileModel
    {
        public string officerid { get; set; }
        public string officernric { get; set; }
        public string officername { get; set; }
        public string officercontactno { get; set; }

    }
    public class AbsenceReport : AppointmentModel
    {

        public string RtpID { get; set; }
        public DateTime ReportDate { get; set; }
        public string Reason { get; set; }
        public string ReasonDetails { get; set; }
        public byte[] ScannedDoc { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string DocStatus { get; set; }

    }
    public class DocSubmission : Supervisee
    {
        public string DocID { get; set; }
        public string SubmissionDate { get; set; }
        public string Subject { get; set; }
        public byte[] DocFile { get; set; }
        public int DocStatus { get; set; }
        //public HttpPostedFile UploadFile { get; set; }
    }
    public class ChangeDate
    {
        public DateTime appdate { get; set; }
        public string msgbody { get; set; }
    }

}
