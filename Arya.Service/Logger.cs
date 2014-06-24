using System;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Arya.Service
{
    internal static class Logger
    {
        private static readonly ILog LogWriter;

        static Logger()
        {
            LogWriter = LogManager.GetLogger(typeof (Program));
            XmlConfigurator.Configure();
        }

        public static ILog GetLogWriter() { return LogWriter; }
    }
}