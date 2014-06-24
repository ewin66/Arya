using System;
using System.IO;
using System.Linq;
using Arya.Framework.Data.Services;
using Arya.Service.Properties;

namespace Arya.Service
{
    class Schedule
    {
        public void SubmitJobs()
        {
            // get data context
            using (var db = new AryaServicesDbDataContext())
            {


                // get a list of jobs to be executed
                var now = DateTime.Now;
                var jobs = db.AryaSchedules.Where(s => (s.NextExecution == null || s.NextExecution <= now) );

                // add each job to the task queue
                foreach (var job in jobs)
                {

                    string newArgumentDirectoryPath = Path.Combine(job.ProjectID.ToString(), Guid.NewGuid().ToString());
                    DirectoryCopy(Path.Combine(Settings.Default.ArgumentFileBasePath, job.ArgumentDirectoryPath),
                                  Path.Combine(Settings.Default.ArgumentFileBasePath, newArgumentDirectoryPath), true);
                    //File.Copy();
                    var newJob = new AryaTask
                    {
                        ID = Guid.NewGuid(),
                        ScheduleID = job.ID,
                        Description = job.Description,
                        ArgumentDirectoryPath = newArgumentDirectoryPath,
                        Status = "New",
                        SubmittedBy = job.SubmittedBy,
                        SubmittedOn = now,
                        ProjectID = job.ProjectID,
                        LastUpdateOn = now,
                        JobType = job.JobType
                    };
                    db.AryaTasks.InsertOnSubmit(newJob);
                    Logger.GetLogWriter().Info("SchedueJobId:" + job.ID + "was submitted as regular job");
                    // update the date of execution of the selected job
                    job.NextExecution = now.AddMinutes(job.Interval);
                    job.LastExecution = now;

                    // update the database
                    db.SubmitChanges();
                }
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
