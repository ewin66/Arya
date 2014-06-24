using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Arya.UserControls;
using System.Windows.Forms;
using Arya.HelperForms;

namespace Arya.HelperClasses
{
    [ComVisible(true)]
    public class JavascriptSkuViewObject
    {
        public EntityDataGridView ParentSkuView { get; set; }

        public JavascriptSkuViewObject()
        {

        }

        public JavascriptSkuViewObject(EntityDataGridView parentSkuView)
        {
            ParentSkuView = parentSkuView;
        }

        public void SelectSku(string skuID, string columnIndex)
        {
            if (ParentSkuView != null)
            {
                int cIndex;
                if (!Int32.TryParse(columnIndex, out cIndex))
                {
                    cIndex = 0;
                }
                ParentSkuView.SelectSku(Guid.Parse(skuID), cIndex);
            }
        }

        public void PinCurrentImages()
        {
            AryaTools.Instance.Forms.BrowserForm.PinCurrentUrl = true;
        }

        public void UnPinCurrentImages()
        {
            AryaTools.Instance.Forms.BrowserForm.PinCurrentUrl = false;
        }

        public void DoLogin(string ssoId, string fullName, string emailId)
        {
            //if (Login.IsAuthorized(ssoId,emailId,fullName))
            //{
            //    AryaTools.Instance.Forms.BrowserForm.Close();
            //    AryaTools.Instance.Forms.StartupForm.LoginSuccess();
            //}
            //else
            //{
            //    var logout = MessageBox.Show("You don't seem to have access to any Arya Projects." +
            //        "\n\n Would you like to logout off your current address and try again with different credentials?", "No Access", MessageBoxButtons.YesNo);

            //    if (logout == DialogResult.Yes)
            //        AryaTools.Instance.Forms.BrowserForm.GotoUrl("https://accounts.google.com/Logout?&continue=" + FrmStartup.LoginUrl);
            //    else
            //        Application.Exit();
            //}
        }
    }
}
