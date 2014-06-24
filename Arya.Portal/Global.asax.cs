using System;
using System.IO;
using System.Linq;

namespace AryaPortal
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            log4net.Config.XmlConfigurator.Configure();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

            var oldDirectories = from directory in new DirectoryInfo(Server.MapPath("~/Temp")).GetDirectories()
                                 where
                                     !directory.GetFiles().Any(
                                         file => file.CreationTime > DateTime.Now.Subtract(new TimeSpan(4, 0, 0, 0)))
                                 select directory;

            foreach (var directory in oldDirectories)
            {
                directory.Delete(true);
            }
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
        }

    }
}
