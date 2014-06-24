using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using LinqKit;
using Arya.Framework.Common;
using Arya.Framework.Data.Services;
using Arya.Service.Properties;
using Timer = System.Timers.Timer;

namespace Arya.Service
{
    public partial class JobService : ServiceBase
    {
        #region Fields

        private readonly List<Task> _allTasks = new List<Task>();
        private readonly Timer _timer = new Timer(5000);

        #endregion Fields

        #region Constructors

        public JobService() { InitializeComponent(); }

        #endregion Constructors

        #region Methods

        public void ShutDown()
        {
            _timer.Stop();
            Logger.GetLogWriter().Info("Arya.Service stopped.");
            eventLog.WriteEntry("Arya.Service stopped.");
        }

        public void StartUp()
        {
            Init();

            _timer.Elapsed += DoWork;
            _timer.Start();

            Logger.GetLogWriter().Info("Arya.Service started.");
            eventLog.WriteEntry("Arya.Service started.");
        }

        private void Init()
        {
            using (var db = new AryaServicesDbDataContext())
            {
                //Restart tasks that were "working" in the previous run (if any)
                var workingTasks = db.AryaTasks.Where(t => t.Status == WorkerState.Working.ToString());
                workingTasks.ForEach(t => t.Status = WorkerState.New.ToString());
                db.SubmitChanges();
            }
        }

        protected override void OnStart(string[] args) { new Thread(StartUp).Start(); }

        protected override void OnStop() { ShutDown(); }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            Thread.Sleep(5000);
            var numberOfSimultaneousJobs = FetchUpdatedSettings();

            //First, queue new potential jobs from the scheduled tasks
            new Schedule().SubmitJobs();

            //Start new tasks
            if (_allTasks.Count < numberOfSimultaneousJobs)
            {
                var task = new JobAssigner().ProcessJobs(_allTasks.Count);
                _allTasks.Add(task);
                task.ContinueWith(t => _allTasks.Remove(t));
            }

            _timer.Start();
        }

        private int FetchUpdatedSettings()
        {
            Settings.Default.Reload();
            try
            {
                _timer.Interval = Settings.Default.JobServiceRunIntervalInSeconds * 1000;
            }
            catch (Exception)
            {
                _timer.Interval = 5000;
            }

            int numberOfSimultaneousJobs;
            try
            {
                numberOfSimultaneousJobs = Settings.Default.NumberOfSimultaneousJobs;
            }
            catch (Exception)
            {
                numberOfSimultaneousJobs = 2;
            }
            return numberOfSimultaneousJobs;
        }

        #endregion Methods
    }
}