namespace Arya.HelperForms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Framework4.State;

    using Arya.Framework.Common.Extensions;
    using Arya.Framework.Extensions;
    using Arya.HelperClasses;

    public partial class FrmWaitScreen : Form
    {
        #region Fields

        static readonly List<KeyValuePair<Guid, DateTime>> AutoHideMessages = new List<KeyValuePair<Guid, DateTime>>();
        static readonly FrmWaitScreen Instance = new FrmWaitScreen();
        static readonly List<KeyValuePair<Guid, Thread>> KillThreads = new List<KeyValuePair<Guid, Thread>>();
        static readonly List<KeyValuePair<Guid, string>> Messages = new List<KeyValuePair<Guid, string>>();

        #endregion Fields

        #region Constructors

        FrmWaitScreen()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Properties.Resources.AryaLogoIcon;
            Left = Screen.FromControl(this).WorkingArea.Width - Width;
            Top = Screen.FromControl(this).WorkingArea.Height - Height;
            lblWait.Parent = pictBackgroundImage;
        }

        #endregion Constructors

        #region Methods

        // Public Methods (3)
        public static void HideMessage(Guid messageId)
        {
            KillThreads.RemoveAll(kvp => kvp.Key.Equals(messageId));
            AutoHideMessages.RemoveAll(kvp => kvp.Key.Equals(messageId));
            Messages.RemoveAll(kvp => kvp.Key.Equals(messageId));
            if (Messages.Count == 0)
            {
                Instance.lblWait.InvokeEx(d => d.Text = string.Empty);
                Instance.InvokeEx(frm => Instance.Hide());
            }
            else
            {
                //Instance.lblWait.InvokeEx(d => d.Text = Messages[Messages.Count - 1].Value);
                var newMessage = Messages[Messages.Count - 1].Value;
                Instance.InvokeEx(frm => Instance.lblWait.Text = newMessage);
            }
        }

        public static Guid ShowMessage(string message, bool autoHide = false)
        {
            Guid newGuid = Guid.NewGuid();
            Messages.Add(new KeyValuePair<Guid, string>(newGuid, message));

            if (autoHide)
                AutoHideMessages.Add(new KeyValuePair<Guid, DateTime>(newGuid, DateTime.Now.AddSeconds(5)));

            Instance.RefreshMessage();

            return newGuid;
        }

        public static void UpdateMessage(Guid messageId, string messageString)
        {
            try
            {
                KeyValuePair<Guid, string> message = Messages.FirstOrDefault(msg => msg.Key.Equals(messageId));
                if (message.Equals(default(KeyValuePair<Guid, string>)) || !Messages.Contains(message))
                    return;

                Messages[Messages.IndexOf(message)] = new KeyValuePair<Guid, string>(messageId, messageString);
            }
            catch
            {

            }
            finally
            {
                Instance.RefreshMessage();
            }
        }

        private void autoHideTimer_Tick(object sender, EventArgs e)
        {
            var guids = AutoHideMessages.Where(kvp => kvp.Value < DateTime.Now).Select(kvp=>kvp.Key).ToList();
            foreach (var item in guids)
            {
                 AutoHideMessages.RemoveAll(kvp => kvp.Key == item);
                 Messages.RemoveAll(kvp => kvp.Key == item); 
            }
            RefreshMessage();
        }

        // Private Methods (3)
        //private void autoHideTimer_Tick(object sender, EventArgs e)
        //{
        //    //var toHide = AutoHideMessages.Where(kvp => kvp.Value.CompareTo(DateTime.Now) < 0).ToList();
        //    //toHide.ForEach(
        //    //    kvp =>
        //    //    {
        //    //        HideMessage(kvp.Key);
        //    //        AutoHideMessages.Remove(kvp);
        //    //    });
        //}
        void frmPleaseWait_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Forms.MustCloseForm(e))
                return;

            Hide();
            e.Cancel = true;
        }

        private void RefreshMessage()
        {
            if (lblWait.InvokeRequired)
                BeginInvoke(new MethodInvoker(RefreshMessage));
            else
            {
                try
                {
                    var message = Messages[Messages.Count - 1];
                    string labelText = lblWait.Text;

                    if (labelText.Equals(message.Value))
                        return;

                    lblWait.Text = message.Value;

                    BringToFront();
                    Show();

                    Refresh();
                }
                catch (ArgumentOutOfRangeException)
                {
                    if (Messages.Count == 0)
                        Instance.Hide();
                }
            }
        }

        #endregion Methods
    }
}