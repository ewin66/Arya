using System;
using System.IO;
using System.ServiceProcess;

namespace Arya.Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            var service = new ServiceBase[] {new JobService()};

            if (args.Length > 0 && args[0] == "-c")
            {
                Console.WriteLine("Starting...");
                ((JobService) service[0]).StartUp();
                Console.WriteLine("System running; press any key to stop");
                Console.Read();
                ((JobService) service[0]).ShutDown();
                Console.WriteLine("System stopped");
            }
            else
            {
                //Run as a Windows Service
                //((JobService)service[0]).StartUp();
                //((JobService)service[0]).ShutDown();
                ServiceBase.Run(service);
            }
        }
    }
}