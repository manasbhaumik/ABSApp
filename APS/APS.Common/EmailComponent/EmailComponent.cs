using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mail;
using APS.BusinessEntity.Models.Common;
using System.Configuration;

namespace APS.Common.EmailComponent
{
    public class EmailComponent
    {
        public void SendEmail(SendEmailRequest request)
        {
            try
            {
                string smtpUsername = ConfigurationManager.AppSettings.Get("APS_SMTP_USERNAME").ToString(); //get from config
                string smtpPassword = ConfigurationManager.AppSettings.Get("APS_SMTP_PASSWORD").ToString(); //get from config
                string smtpHost = ConfigurationManager.AppSettings.Get("APS_SMTP_HOST").ToString(); //get from config
                int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings.Get("APS_SMTP_PORT"));//get from config
                if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword) && !string.IsNullOrEmpty(smtpHost))
                {
                    using (MailMessage mm = new MailMessage(smtpUsername, request.ToEmails))
                    {
                        mm.Subject = request.EmailSubject;
                        mm.Body = request.EmailBody;
                        //no attachment
                        //if (fuAttachment.HasFile)
                        //{
                        //    string FileName = Path.GetFileName(fuAttachment.PostedFile.FileName);
                        //    mm.Attachments.Add(new Attachment(fuAttachment.PostedFile.InputStream, FileName));
                        //}
                        mm.IsBodyHtml = false;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = smtpHost;
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(smtpUsername, smtpPassword);
                        smtp.UseDefaultCredentials = true;
                        smtp.EnableSsl = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = smtpPort;
                        smtp.Send(mm);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
