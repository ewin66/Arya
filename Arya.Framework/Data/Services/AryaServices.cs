using System;
using System.Linq;
using Arya.Framework.Common;
using Arya.Framework.Extensions;
using Arya.Framework.Utility;

namespace Arya.Framework.Data.Services
{
    public static class AryaServices
    {
        public static AryaTask CreateAryaTask(Guid id, Guid projectID, string description, string argumentDirectoryPath, Guid submittedBy, Type jobType)
        {
            using (var db = new AryaServicesDbDataContext())
            {
                var nt = new AryaTask
                         {
                             ID = id,
                             ProjectID = projectID,
                             Description = description,
                             ArgumentDirectoryPath = argumentDirectoryPath,
                             Status = "New",
                             SubmittedBy = submittedBy,
                             SubmittedOn = DateTime.Now,
                             LastUpdateOn = DateTime.Now,
                             JobType = jobType.FullName.Split('.').Last()
                         };

                db.AryaTasks.InsertOnSubmit(nt);
                db.SubmitChanges();

                return nt;
            }
        }

        public static void SendEmail(AryaTask job, bool includeLogUrl, string customStatus, string projectName, string emailAddress, string portalLocation)
        {
            var subject = String.Format("{0}: {1}", job.JobType.Spacify(), job.Description);
            //var to = aryaDb.CurrentUser.EmailAddress;
            var to = emailAddress;
            var messageBody = String.Format(@"
    Project Name: {0}
    Description: {1}
    Status: {2}", projectName, job.Description, customStatus ?? WorkerBase.GetFriendlyWorkerState((WorkerState) Enum.Parse(typeof(WorkerState),job.Status)));

            if (includeLogUrl)
            {
                messageBody += String.Format("{0}{0}Log file can be viewed here: {1}/Log.aspx?ID={2}", Environment.NewLine,
                    portalLocation, job.ID);
            }

            EmailSender.SendEmail(to, messageBody, subject);
        }

        public static void UpdateWorkerStatusInDb(object sender, WorkerStatusUpdateEventArgs data)
        {
            if (data.WorkerId == Guid.Empty)
                return;

            using (var dc = new AryaServicesDbDataContext())
            {
                var task = dc.AryaTasks.FirstOrDefault(nt => nt.ID == data.WorkerId);

                if (task == null)
                    return;

                task.Status = data.CurrentState.ToString();
                task.StatusMessage = data.StatusMessage;
                task.LastUpdateOn = DateTime.Now;

                dc.SubmitChanges();
            }
        }
    }

    public partial class AryaTask
    {
        public string DisplayJobType
        {
            get
            {
                return this.JobType.Spacify().Replace(" Worker", string.Empty);
            }
        }
    }
}