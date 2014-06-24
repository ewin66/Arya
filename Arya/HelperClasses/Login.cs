using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arya.Data;
using System.Windows.Forms;
using Arya.HelperForms;
using System.Deployment.Application;

namespace Arya.HelperClasses
{
    using Framework4.State;

    class Login
    {
        internal static bool IsAuthorized(string ssoId, string emailId, string fullName = null)
        {

            var existingUser = (from user in AryaTools.Instance.InstanceData.Dc.Users
                                where user.OpenIdentity == ssoId && user.EmailAddress == emailId
                                orderby user.SingleSignOnId
                                select user).FirstOrDefault();

            // This piece is to support Arya Web View temporarily
            int ssoIdInt = 0;
            if (existingUser == null)
            {
                if (int.TryParse(ssoId, out ssoIdInt))
                {
                    existingUser = (from user in AryaTools.Instance.InstanceData.Dc.Users
                                    where user.SingleSignOnId == ssoIdInt && user.EmailAddress == emailId
                                    orderby user.SingleSignOnId
                                    select user).FirstOrDefault();
                }
            }

            if (existingUser == null)
                existingUser = (from user in AryaTools.Instance.InstanceData.Dc.Users
                                where user.OpenIdentity == null && user.EmailAddress == emailId
                                orderby user.SingleSignOnId
                                select user).FirstOrDefault();

            if (existingUser == null)
                return false;

            //No need to check if the previous value is different from current value, the Setter method already does it.
            //if (!string.IsNullOrWhiteSpace(fullName))
            //    existingUser.FullName = fullName;
            //existingUser.OpenIdentity = ssoId;

            if (existingUser.FullName != fullName && !string.IsNullOrWhiteSpace(fullName))
                existingUser.FullName = fullName;
            if (existingUser.OpenIdentity != ssoId && !string.IsNullOrWhiteSpace(ssoId) && ssoIdInt == 0)
                existingUser.OpenIdentity = ssoId;

            AryaTools.Instance.InstanceData.CurrentUser = existingUser;
            AryaTools.Instance.SaveChangesIfNecessary(false, false);

            DoLaundry();

            return true;
        }

        private static void DoLaundry()
        {
            // Easter Egg - Time to do your laundry - once every 7 days
            DateTime now = DateTime.Now;
            var quotes = new[]
                                 {
                                     "It's our fundamental guiding principle: provide elegant product data solutions for our clients. Our expertise in data management, data quality, search and navigation drive online revenue and create a customer experience that is intuitive, efficient and rewarding.",
                                     "Some might call us Data Geeks. And we are. But our team of architects and analysts are also journalists, librarians, linguists, IT ninjas, usability experts, MRO aficionados and more. We love data and it shows in our work habits, culture and success with our clients.",
                                     "Nobody will tell you that your website is dysfunctional (they'll just shop somewhere else).",
                                     "Customers don't buy products when they think they have found the right item. They buy when they know they have.",
                                     "Fixing data problems can be a bit like plugging a hole in a leaky bucket; as soon as you cover one issue, another emerges to take its place."
                                 };
            DateTime dtLastLaundry;
            const string lastLaundryDateRegistryKey = "LastLaundry";
            string lastLaundryDate = WindowsRegistry.GetFromRegistry(lastLaundryDateRegistryKey);
            try
            {
                dtLastLaundry = DateTime.Parse(lastLaundryDate);
            }
            catch
            {
                dtLastLaundry = now.Subtract(TimeSpan.FromDays(8));
            }
            if (now - dtLastLaundry > TimeSpan.FromDays(7))
            {
                var randomQuote = quotes[new Random(now.Millisecond).Next(quotes.Length)];
                var result = MessageBox.Show(randomQuote, @"Arya quote");
                WindowsRegistry.SaveToRegistry(lastLaundryDateRegistryKey, now.ToString());
            }
        }
    }
}
