using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APS.BusinessEntity.Models.Common
{
    public class SendEmailRequest
    {
        public string FromEmail { get; set; }
        public string ToEmails { get; set; }
        public string CCEmails { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
