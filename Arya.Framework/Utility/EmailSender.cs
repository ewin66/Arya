using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Arya.Framework.Utility
{
    public class EmailSender
    {
        #region Constant variables

        //public const string Username = "arya.empirisense@gmail.com";
        //public const string Password = "2010sNewPassword";
        //public const string FromAddress = "arya.empiriSense@gmail.com";
        //public const string ToAddressDefault = "SoftwareDevelopmentPractice@empiriSense.com";
        //#endregion

        #region Private variables

        private static readonly SmtpClient EmailClient;

        #endregion

        #region Constructor

        static EmailSender()
        {
            //EmailClient = new SmtpClient
            //{
            //    Host = "smtp.gmail.com",
            //    Port = 587,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    Credentials = new NetworkCredential(Username, Password)
            //};
        }

        #endregion

        #region Public methods

        public static void SendEmail(string to, string messageBody, string subject, string filePath = null)
        {
            //Attachment data = null;
            //var toEmailAddresses = (string.IsNullOrEmpty(to) ? ToAddressDefault : to).Split(
            //    new[] { ',', ',', '|', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            ////var defaultSubject = string.Format("{0} {1}", Assembly.GetExecutingAssembly().GetName().Name,
            ////                                   ApplicationDeployment.IsNetworkDeployed
            ////                                       ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
            ////                                       : "Not Deployed");

            ////if (String.IsNullOrEmpty(messageBody))
            ////    messageBody = File.ReadAllText(Path.GetTempFileName());
            ////    messageBody = File.ReadAllText(Program.ErrorFileName);
            //var message = new MailMessage
            //{
            //    Subject = (!String.IsNullOrEmpty(subject) ? subject : String.Empty),
            //    Body = messageBody,
            //    From = new MailAddress(FromAddress)
            //};
            //toEmailAddresses.ForEach(message.To.Add);

            //if (filePath != null && File.Exists(Path.Combine(System.IO.Path.GetTempPath(), filePath)))
            //{
            //    data = new Attachment(filePath, MediaTypeNames.Application.Octet);
            //    // Add time stamp information for the file.
            //    ContentDisposition disposition = data.ContentDisposition;
            //    disposition.CreationDate = System.IO.File.GetCreationTime(filePath);
            //    disposition.ModificationDate = System.IO.File.GetLastWriteTime(filePath);
            //    disposition.ReadDate = System.IO.File.GetLastAccessTime(filePath);
            //    // Add the file attachment to this e-mail message.
            //    message.Attachments.Add(data);
            //}


            //EmailClient.Send(message);

            //// Display the values in the ContentDisposition for the attachment.
            //if (data != null)
            //{
            //    data.Dispose();
            //}
        }

        public static void SendEmail(List<string> tos, string body, string subject, string fileName = null)
        {
            var toEmails = tos.First();
            for (var i = 1; i < tos.Count; i++)
                toEmails = toEmails + ',' + tos[i];

            SendEmail(toEmails, body, subject, fileName);
        }

        #endregion
    }
}

#endregion