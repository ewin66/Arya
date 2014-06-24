using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Arya.Framework.Common;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Data.Services;
using Arya.Framework.Extensions;
using Arya.Framework.Utility;
using Arya.Service.Properties;

namespace Arya.Service
{
    public class JobAssigner
    {
        #region Fields

        private ILog _currentLogWriter;
        private Dictionary<string, Type> _jobProcessors;

        #endregion Fields

        #region Properties

        private Dictionary<string, Type> JobProcessors
        {
            get
            {
                if (_jobProcessors == null || _jobProcessors.Count == 0)
                {
                    _jobProcessors = new Dictionary<string, Type>();
                    AppDomain.CurrentDomain.AppendPrivatePath(Settings.Default.JobProcessorDllLocation);
                    var pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        Settings.Default.JobProcessorDllLocation);

                    foreach (var file in Directory.GetFiles(pluginsDirectory, "*.dll"))
                    {
                        var assembly = Assembly.LoadFile(file);
                        //Type workerBaseType = assembly.GetType(WorkerBaseClassToFind);
                        //if (workerBaseType == null)
                        //    continue;
                        foreach (var currentType in assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(WorkerBase))))
                            _jobProcessors.Add(currentType.Name, currentType);
                    }
                }
                return _jobProcessors;
            }
        }

        private ILog CurrentLogWriter
        {
            get { return _currentLogWriter ?? (_currentLogWriter = Logger.GetLogWriter()); }
        }

        #endregion Properties

        #region Methods

        public async Task ProcessJobs(object processBucket)
        {
            //CurrentLogWriter.Debug("Job assigner has started processing jobs.");
            try
            {
                AryaTask job;
                using (var aryaServicesDb = new AryaServicesDbDataContext())
                {
                    job =
                        aryaServicesDb.AryaTasks.FirstOrDefault(
                            s => s.Status.ToLower() == WorkerState.New.ToString().ToLower());

                    if (job == null)
                        return;
                    CurrentLogWriter.InfoFormat(
                            "Job ID: {0} Starting\tDescription:{1}\tSubmitted by:{2}\tSubmitted on:{3}\tJob type:{4}\tProcess bucket:{5}",
                            job.ID, job.Description, job.SubmittedBy, job.SubmittedOn, job.JobType, processBucket);
                    job.JobAssignedOn = DateTime.Now;
                    job.Status = WorkerState.Working.ToString();

                    aryaServicesDb.SubmitChanges();
                }
                string currentJobStatus, emailAddresses = string.Empty;
                try
                {
                    if (JobProcessors.ContainsKey(job.JobType))
                    {
                        //create the job instance
                        var currentJobType = JobProcessors[job.JobType];

                        var jobExecutor =
                            (WorkerBase)
                                Activator.CreateInstance(currentJobType,
                                    new object[]
                                            {
                                                Path.Combine(Settings.Default.ArgumentFileBasePath,
                                                    job.ArgumentDirectoryPath)
                                            });

                        jobExecutor.CurrentLogWriter = CurrentLogWriter;
                        jobExecutor.WorkerStatusChange += AryaServices.UpdateWorkerStatusInDb;
                        emailAddresses = jobExecutor.Arguments.NotificationEmailAddresses;
                        ProcessEmailJobStatus(emailAddresses, job, true, "Started");
                        await Task.Run(() => jobExecutor.Run());
                        jobExecutor.SaveSummary();
                        currentJobStatus = jobExecutor.Summary.State.ToString();
                    }
                    else
                    {
                        currentJobStatus = WorkerState.Abort.ToString();
                        CurrentLogWriter.ErrorFormat("{0}: {1} not found in the dll provided", job.ID, job.JobType);
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Empty;
                    var e = ex;
                    while (e != null)
                    {
                        message = Environment.NewLine + e.Message;
                        e = e.InnerException;
                    }
                    CurrentLogWriter.Error(ex.Source + message + Environment.NewLine + ex.StackTrace);
                    currentJobStatus = WorkerState.Abort.ToString();
                    CurrentLogWriter.Error(ex.SerializeObject());
                }

                using (var aryaServicesDb = new AryaServicesDbDataContext())
                {
                    job = aryaServicesDb.AryaTasks.First(t => t.ID == job.ID);
                    job.Status = currentJobStatus;
                    ProcessEmailJobStatus(emailAddresses, job, true);
                    CurrentLogWriter.InfoFormat("Job Id: {0} Finished processing, Last Status: {1}", job.ID,
                        currentJobStatus);
                    job.LastUpdateOn = DateTime.Now;
                    aryaServicesDb.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                CurrentLogWriter.Error(ex.SerializeObject());
            }

            CurrentLogWriter.Debug("Job assigner is exiting.");
        }

        private void ProcessEmailJobStatus(string emailAddresses, AryaTask job, bool includeLogUrl, string customStatus = null)
        {
            var emailAddress = emailAddresses;
            var projectName = string.Empty;
            try
            {
                using (var aryaDb = new AryaDbDataContext(job.ProjectID, job.SubmittedBy))
                {
                    if (string.IsNullOrWhiteSpace(emailAddress))
                        emailAddress = aryaDb.CurrentUser.EmailAddress;
                    projectName = aryaDb.CurrentProject.ProjectName;
                }
            }
            catch (UnauthorizedAccessException e)
            {
                CurrentLogWriter.Warn(e.Message);
            }

            try
            {
                AryaServices.SendEmail(job, includeLogUrl, customStatus, projectName, emailAddress, Settings.Default.PortalLocation);
            }
            catch (Exception ex)
            {
                CurrentLogWriter.Error(String.Format("Email sending has failed. {0} has occured. Details{1}",
                    ex.GetType(), ex.StackTrace));
            }
        }

        #endregion Methods
    }
}