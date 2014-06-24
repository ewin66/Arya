using System;
using System.Collections;
using System.Configuration;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Arya.Framework.Utility;
using Arya.HelperClasses;
using System.Collections.Generic;

namespace Arya
{
    internal static class Program
    {
		#region Fields (1) 

        //one unique temp file for the entire session
        public readonly static string ErrorFileName = Path.GetTempFileName();
        public static bool _overRideCodeVersion;

		#endregion Fields 

		#region Methods (8) 

		// Public Methods (2) 

        public static bool IsFileInUse(string fileName)
        {
            var flag = false;
            try
            {
                using (Stream stream = new FileStream(fileName, FileMode.Open))
                {
                }
            }
            catch
            {
                flag = true;
            }
            return flag;
        }

        public static void WriteToErrorFile(string message, bool append)
        {
            TextWriter tw = new StreamWriter(ErrorFileName, append);
            tw.WriteLine(message);
            tw.Close();
        }
		// Private Methods (6) 

        private static void HandleAllExceptions(Exception exception)
        {
            try
            {
                WriteExceptionToErrorFile(exception);
                var messageBody = File.ReadAllText(ErrorFileName);
                var subject = string.Format("{0} {1} {2}", Assembly.GetExecutingAssembly().GetName().Name,
                                                  ApplicationDeployment.IsNetworkDeployed
                                                      ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                                                      : "Not Deployed", exception.Message);
                //EmailSender.SendEmail(EmailSender.ToAddressDefault, messageBody, subject);
                if (
                    //MessageBox.Show(
                    //    exception.Message +
                    //    "\n\nWould you like to try to close and reopen your current window/tab and see if it works?",
                    //    "There was a problem", MessageBoxButtons.YesNo) == DialogResult.No)
                     MessageBox.Show(
                        exception.Message +
                        "\n\nYes: Continue with Arya." +
                        "\nNo: Close Arya.",
                        "There was a problem", MessageBoxButtons.YesNo) == DialogResult.No)
                    Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to save error details:\n" + ex.Message + "\n\nActual Error:\n" + exception.Message);
                Application.Exit();
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            //Application.ThreadException += UiThreadExceptionHandler;
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            WriteToErrorFile("Started Arya at " + DateTime.Now, false);


            //This commented out line is temp only. Will have to uncomment as soon as the DataconfigProvider bug get fixed
            //ToggleConfigEncryption(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var inputArgs = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
                if (inputArgs != null && inputArgs.Length > 0)
                {
                    args = inputArgs;
                }
            }
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.ActivationUri != null)
            {
                string queryString = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;
                var nameValueTable = HttpUtility.ParseQueryString(queryString);
                if (nameValueTable["overrideCodeVersion"] != null)
                    Boolean.TryParse(nameValueTable["overrideCodeVersion"], out _overRideCodeVersion);
                // AryaTools.Instance.StartupForm.AutoOpenOldExport = true;

            }
            else
            {
                if (args.Length > 0)
                    _overRideCodeVersion = args[0].ToLower() == "-overridecodeversion";
            }
            //Login bypass
            //if (args != null && args.Length == 2 && Login.IsAuthorized(args[0], args[1]))
            //{
            //    AryaTools.Instance.Forms.StartupForm.LoginSuccess();
            //    AryaTools.Instance.AutoLogin = true;
            //}

            Application.Run(AryaTools.Instance.Forms.StartupForm);
        }

        static void ToggleConfigEncryption(string exeConfigName, bool protect = true)
        {
            var config = ConfigurationManager.
                OpenExeConfiguration(exeConfigName);

            var section =
                config.GetSection("connectionStrings")
                as ConnectionStringsSection;

            if (section != null && (protect == false && section.SectionInformation.IsProtected))
            {
                // Remove encryption.
                section.SectionInformation.UnprotectSection();
                config.Save();
            }
            else if (section != null && (protect && section.SectionInformation.IsProtected == false))
            {
                // Encrypt the section.
                section.SectionInformation.ProtectSection(
                    "DataProtectionConfigurationProvider");
                config.Save();
            }
        }

        private static void UiThreadExceptionHandler(object sender, ThreadExceptionEventArgs t)
        {
            HandleAllExceptions(t.Exception);
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            HandleAllExceptions((Exception)e.ExceptionObject);
        }

        private static void WriteExceptionToErrorFile(Exception exception)
        {
            string message = string.Empty;
            Exception ex = exception;
            string version;
            if (ApplicationDeployment.IsNetworkDeployed)
                version = "Arya " + ApplicationDeployment.CurrentDeployment.CurrentVersion;
            else
            {
                var v = new Version(Application.ProductVersion);
                version = String.Format(
                    "Arya v{0}.{1:00}.{2}.{3} Not Deployed", v.Major, v.Minor, v.Build, v.Revision);
            }

            message += version + Environment.NewLine + DateTime.Now + Environment.NewLine;

            while (ex != null)
            {
                message += "-----" + Environment.NewLine;
                message += ex.Message + Environment.NewLine;
                message += ex.StackTrace + Environment.NewLine;
                message = ex.Data.Cast<DictionaryEntry>().Aggregate(message, (current, data) => current + string.Format("{0} = {1}{2}", data.Key, data.Value, Environment.NewLine));

                ex = ex.InnerException;
            }

            WriteToErrorFile(message, true);
            
        }
        public static Version GetCurrrentCodeVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            else
            {
                // var v = new Version(Application.ProductVersion);
                return new Version(Application.ProductVersion);
                //String.Format("{0}.{1:0}.{2}.{3} Not Deployed", v.Major, v.Minor, v.Build, v.Revision);
            }
        }
        public static IEnumerable AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }
		#endregion Methods 
    }
}