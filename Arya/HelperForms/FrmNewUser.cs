using System;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using Arya.Data;
using Arya.Properties;

namespace Arya.HelperForms
{
    public partial class FrmNewUser : Form
    {
        public FrmNewUser()
        {
            InitializeComponent();
            Icon = Resources.user;
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            epNewUser.Clear();

            var errors = false;

            if (!ValidateEmailAddress())
            {
                epNewUser.SetError(tbEmailAddress, "Valid Email Address required");
                errors = true;
            }

            if (string.IsNullOrWhiteSpace(tbFullName.Text))
            {
                epNewUser.SetError(tbFullName, "Valid Full Name required");
                errors = true;
            }

            if(errors) return;

            using (var db = new SkuDataDbDataContext())
            {
                var ssoID = db.Users.Max(p => p.SingleSignOnId) + 1;
                var newUser = new User
                                  {
                                      ID = Guid.NewGuid(),
                                      EmailAddress = tbEmailAddress.Text.Trim(),
                                      FullName = tbFullName.Text.Trim(),
                                      SingleSignOnId = ssoID,
                                      CreatedOn = DateTime.Now,
                                      Active = true
                                  };
                db.Users.InsertOnSubmit(newUser);
                db.SubmitChanges();

                MessageBox.Show(@"New User Created",@"New User",MessageBoxButtons.OK,MessageBoxIcon.Information);
                btnClear.PerformClick();
            }
        }

        private bool ValidateEmailAddress()
        {
            if(string.IsNullOrWhiteSpace(tbEmailAddress.Text)) return false;
            MailAddress address;

            try {
                address = new MailAddress(tbEmailAddress.Text.Trim());
            } catch(FormatException) {
                //Invalid address
                return false;
            }

            return address.Host.Equals("gmail.com", StringComparison.OrdinalIgnoreCase) ||
                   address.Host.Equals("empiriSense.com", StringComparison.OrdinalIgnoreCase);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbEmailAddress.Clear();
            tbFullName.Clear();
            epNewUser.Clear();
        }
    }
}
