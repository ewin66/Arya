using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Arya.Framework.Common;

namespace Arya.Framework.Utility
{
    public static class Util
    {
        public static readonly Dictionary<Guid, string> ConnectionStrings = new Dictionary<Guid, string>();
        
        public static string GetAryaDbConnectionString(Guid projectID)
        {
            if (projectID != Guid.Empty && ConnectionStrings.ContainsKey(projectID))
                return ConnectionStrings[projectID];

            string cs;
            try
            {
                cs = ConfigurationManager.ConnectionStrings["AryaDbConnectionString"].ConnectionString;
            }
            catch (Exception)
            {
                cs = Properties.Settings.Default.AryaDbConnectionString;
            }
            return cs;
        }

        public static string GetAryaServicesConnectionString()
        {
            string cs;
            try
            {
                cs = ConfigurationManager.ConnectionStrings["AryaServicesConnectionString"].ConnectionString;
            }
            catch (Exception)
            {
                cs = Properties.Settings.Default.AryaServicesConnectionString;
            }
            return cs;
        }
    }
}
