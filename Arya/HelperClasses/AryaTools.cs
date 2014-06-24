using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using LinqKit;
using Arya.Data;
using Arya.Framework.Data;
using Arya.Framework.Utility;
using Arya.Framework4.State;
using Arya.HelperForms;
using Arya.Properties;
using Remark = Arya.Data.Remark;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;

namespace Arya.HelperClasses
{
    public class AryaTools
    {
        #region Delegates

        public delegate void InvokeDelegate();

        #endregion

        #region Constants
        private  const string ErrorFileName = "Error";
        
        #endregion


        #region Private variables
        private readonly string _erroFileLocation = Path.GetTempPath();
        #endregion
        private static AryaTools _instance;
        
        public FillRateWorker AllFillRates = new FillRateWorker();
        public bool AutoLogin;

        public Guid? RemarkID;
        public Remark StickyRemark;
        public object UpdateNodeWorkerSyncRoot = new object();

        private string _currentRemark;
        private Forms _forms;
        private InstanceData _instanceData;
        private string _itemImagesTempFile = string.Empty;

        private AryaTools() { InitAryaTools(); }

        public Forms Forms
        {
            get { return _forms ?? (_forms = new Forms()); }
        }

        public InstanceData InstanceData
        {
            get { return _instanceData ?? (_instanceData = new InstanceData()); }
        }

        public string CurrentRemark
        {
            get
            {
                if (_currentRemark == null || _currentRemark.Equals(FrmRemark.DefaultRemark))
                    return string.Empty;
                return _currentRemark;
            }

            set { _currentRemark = value; }
        }

        public string ItemImagesTempFile
        {
            get
            {
                if (_itemImagesTempFile == string.Empty)
                    _itemImagesTempFile = Path.GetTempFileName();
                return _itemImagesTempFile;
            }
        }

        [DefaultValue(true)]
        public bool AutoSave { get; set; }

        public static AryaTools Instance
        {
            get { return _instance ?? (_instance = new AryaTools()); }
        }

        ~AryaTools()
        {
            if (!string.IsNullOrEmpty(_itemImagesTempFile))
                File.Delete(_itemImagesTempFile);
        }

        //public static void EmailTechTeam(string subject, string messageBody = null)
        //{
        //    const string username = "arya.empirisense@gmail.com";
        //    const string password = "2010sNewPassword";
        //    const string fromAddress = "arya.empirisense@gmail.com";
        //    const string toAddress = "SoftwareDevelopmentPractice@empiriSense.com";
        //    var defaultSubject = string.Format("{0} {1}", Assembly.GetExecutingAssembly().GetName().Name,
        //                                       ApplicationDeployment.IsNetworkDeployed
        //                                           ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
        //                                           : "Not Deployed");

        //    if (String.IsNullOrEmpty(messageBody))
        //        messageBody = File.ReadAllText(Program.ErrorFileName);

        //    var message = new MailMessage
        //                      {
        //                          Subject =
        //                              defaultSubject + (!String.IsNullOrEmpty(subject) ? ": " + subject : String.Empty),
        //                          Body = messageBody,
        //                          From = new MailAddress(fromAddress)
        //                      };

        //    message.To.Add(toAddress);
        //    //// Allow multiple "To" addresses to be separated by a semi-colon
        //    //if (toAddress.Trim().Length > 0)
        //    //{
        //    //    foreach (string addr in toAddress.Split(';'))
        //    //    {
        //    //        message.To.Add(new MailAddress(addr));
        //    //    }
        //    //}

        //    // Set the SMTP server to be used to send the message
        //    var client = new SmtpClient
        //                     {
        //                         Host = "smtp.gmail.com",
        //                         Port = 587,
        //                         EnableSsl = true,
        //                         DeliveryMethod = SmtpDeliveryMethod.Network,
        //                         Credentials = new NetworkCredential(username, password)
        //                     };

        //    // Send the e-mail message
        //    client.Send(message);

           
        //}

        private void InitAryaTools()
        {
            AutoSave = true;
            _forms = null;

            var currentUserId = Guid.Empty;
            var currentProjectId = Guid.Empty;

            if (_instanceData != null)
            {
                if (_instanceData.CurrentUser != null)
                    currentUserId = _instanceData.CurrentUser.ID;

                if (_instanceData.CurrentProject != null)
                    currentProjectId = _instanceData.CurrentProject.ID;
            }


            if (!currentUserId.Equals(Guid.Empty))
            {
                InstanceData.CurrentUser = (from usr in InstanceData.Dc.Users
                                            where usr.ID == currentUserId
                                            select usr).First();
            }

            if (!currentProjectId.Equals(Guid.Empty))
            {
                InstanceData.CurrentProject = (from pro in InstanceData.Dc.Projects
                                               where pro.ID == currentProjectId
                                               select pro).First();
            }

            CopyHourGlassIcon();
        }

        private void CopyHourGlassIcon()
        {
//Copy the hourglass file - once
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Arya";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            var hourGlassFileName = directory + "\\hourglass.png";
            if (!File.Exists(hourGlassFileName))
                Resources.HourGlass.Save(hourGlassFileName);
        }

        private void SaveChangesToDb()
        {
            var waitKey = FrmWaitScreen.ShowMessage("Waiting for workers to finish");

            if (!Monitor.TryEnter(Instance.UpdateNodeWorkerSyncRoot, TimeSpan.FromSeconds(1))) // ||
                //!Monitor.TryEnter(Instance.UpdateDefinitionWorkerSyncRoot, TimeSpan.FromSeconds(1)) ||
                //!Monitor.TryEnter(Instance.UpdateEnrichmentsWorkerSyncRoot, TimeSpan.FromSeconds(1)))
            {
                Forms.TreeForm.taxonomyTree.AbortWorkers();
                Monitor.Enter(Instance.UpdateNodeWorkerSyncRoot);
                //Monitor.Enter(Instance.UpdateDefinitionWorkerSyncRoot);
                //Monitor.Enter(Instance.UpdateEnrichmentsWorkerSyncRoot);
            }
            try
            {
                FrmWaitScreen.UpdateMessage(waitKey, "Saving");
                SubmitAllChanges();
            }
            finally
            {
                Monitor.Exit(Instance.UpdateNodeWorkerSyncRoot);
                //Monitor.Exit(Instance.UpdateDefinitionWorkerSyncRoot);
                //Monitor.Exit(Instance.UpdateEnrichmentsWorkerSyncRoot);
                Forms.TreeForm.taxonomyTree.TryStartWorkers();
            }
            FrmWaitScreen.HideMessage(waitKey);
        }

        private HashSet<string> GetInSchemaAttributes(TaxonomyInfo taxonomy)
        {
            var inSchemaAttributes = new HashSet<string>(from si in taxonomy.SchemaInfos
                                                         let sd = si.SchemaData
                                                         where sd != null && sd.InSchema
                                                         select si.Attribute.AttributeName);
            return inSchemaAttributes;
        }

        internal HashSet<string> GetRankedAttributes(TaxonomyInfo taxonomy)
        {
            var rankedAttributes = new HashSet<string>(from si in taxonomy.SchemaInfos
                                                       let sd = si.SchemaData
                                                       where
                                                           sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                                                       select si.Attribute.AttributeName);

            return rankedAttributes;
        }

        private void InvokeUpdateRemarkSelectionMethod()
        {
            if (Forms.RemarksForm != null)
                Forms.RemarksForm.comboBoxRemarks.SelectedItem = StickyRemark.Remark1;
        }

        internal DialogResult SaveChangesIfNecessary(bool mustSaveToContinue, bool userInitiated)
        {
            if (Forms.RemarksForm != null && StickyRemark != null)
                Forms.RemarksForm.BeginInvoke(new InvokeDelegate(InvokeUpdateRemarkSelectionMethod));

            if (Instance.InstanceData.CurrentUser.IsAryaReadOnly) // && selectedProject)
                return DialogResult.None;

            //if (Dc.HasChanges)
            {
                var result = AutoSave || userInitiated
                                 ? DialogResult.Yes
                                 : mustSaveToContinue
                                       ? MessageBox.Show(
                                           @"All changes must be saved now. Would you like to save your changes?",
                                           @"Unsaved Changes", MessageBoxButtons.YesNoCancel)
                                       : DialogResult.No;
                if (result == DialogResult.Yes)
                    SaveChangesToDb();

                return result;
            }
        }

        internal void SubmitAllChanges()
        {
            try
            {
                //Dc.DeferredLoadingEnabled = false;
                if (InstanceData.CurrentProject != null && InstanceData.CurrentProject.DatabaseName != InstanceData.Dc.Connection.Database)
                {
                    InstanceData.Dc.Connection.ChangeDatabase(InstanceData.CurrentProject.DatabaseName);
                }
                InstanceData.Dc.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                //const string concurrencyErrorFileName = "ConcurrencyErrors.xml";
                //instanciate xml document 
               
                string errorFilepath = CreateXmlFile(InstanceData.Dc.ChangeConflicts);
                //TextWriter sw = new StreamWriter(Path.Combine(System.IO.Path.GetTempPath(),  concurrencyErrors));
                foreach (ObjectChangeConflict occ in InstanceData.Dc.ChangeConflicts)
                {
                    
                   // sw.WriteLine(occ.Object.ToString());
                    occ.Resolve(RefreshMode.KeepChanges, true);
                    //sw.WriteLine(occ.Object.ToString());
                }
                //var concurrencyErrorEmailList = new List<string>()
                //    {
                //        EmailSender.ToAddressDefault,
                //        "TechProjectSupport@empiriSense.com"
                //    };
                //EmailSender.SendEmail(concurrencyErrorEmailList, string.Empty, "Concurrency error has occured", errorFilepath);
                //Automerge database values for members that client has not modified.
                //foreach (var occ in InstanceData.Dc.ChangeConflicts)
                //{
                //    occ.Resolve(RefreshMode.KeepChanges, true);
                //    sw.WriteLine(occ.Object.ToString());
                //}
               // sw.Close();

                Instance.Forms.BrowserForm.GotoUrl(new FileInfo(errorFilepath).FullName, "Concurrency errors");
                MessageBox.Show(String.Format("{1}{0}Concurrency conflict(s) during save. Your changes win.",
                                              Environment.NewLine, ex.Message));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    @"Unable to save to database. You may try to save again after some time. If you are seeing this message too many times, you should restart Arya."
                    + Environment.NewLine + ex.Message, @"Cannot Save", MessageBoxButtons.OK);
            }
        }

        private string CreateXmlFile(ChangeConflictCollection changeConflictCollection)
        {
            var errorFileName = string.Concat(ErrorFileName, "-", Guid.NewGuid(), ".xml");
            var filePath = Path.Combine(_erroFileLocation, errorFileName);
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(dec);// Create the root element
           // doc.Load(filePath);
            XmlElement root = doc.CreateElement("Objects");
            
            foreach (ObjectChangeConflict conflict in changeConflictCollection)
            {
                XmlNode childElement = doc.CreateElement("Object");
                XmlAttribute objectType = doc.CreateAttribute("Type");
                objectType.Value = conflict.Object.GetType().Name;
                
                childElement.Attributes.Append(objectType);
                XmlElement conflictMembers = doc.CreateElement("ConflictedMembers");
                foreach (var member  in conflict.MemberConflicts)
                {
                    XmlNode conflictedMember = doc.CreateElement("ConflictedMember");
                    XmlAttribute memberName = doc.CreateAttribute("Name");
                    memberName.Value = member.Member.Name;
                    conflictedMember.Attributes.Append(memberName);

                    XmlAttribute currentValue = doc.CreateAttribute("CurrentValue");
                    currentValue.Value = member.CurrentValue == null ? "Null" : member.CurrentValue.ToString();
                    conflictedMember.Attributes.Append(currentValue);

                    XmlAttribute originalValue = doc.CreateAttribute("OriginalValue");

                    originalValue.Value = member.OriginalValue == null ? "Null" : member.OriginalValue.ToString();
                    conflictedMember.Attributes.Append(originalValue);

                    XmlAttribute databaseValue = doc.CreateAttribute("DatabaseValue");
                    databaseValue.Value = member.DatabaseValue == null ? "Null" : member.DatabaseValue.ToString();
                    conflictedMember.Attributes.Append(databaseValue);
                    conflictMembers.AppendChild(conflictedMember);

                }
                childElement.AppendChild(conflictMembers);

                PropertyInfo[] properties =
                    conflict.Object.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(pi=>pi.Name,StringComparer.InvariantCultureIgnoreCase).ToArray();
                foreach (var propertyInfo in properties)
                {
                    XmlNode propertyElement = doc.CreateElement("Property");
                    XmlAttribute propertyName = doc.CreateAttribute("Name");
                    propertyName.Value = propertyInfo.Name;
                    propertyElement.Attributes.Append(propertyName);
                    XmlAttribute propertyValue = doc.CreateAttribute("Value");
                    var value = propertyInfo.GetValue(conflict.Object, null);

                    propertyValue.Value = value == null ? string.Empty: value.ToString();
                    propertyElement.Attributes.Append(propertyValue);
                    childElement.AppendChild(propertyElement);
                }
                
                root.AppendChild(childElement);
                doc.AppendChild(root);
                doc.Save(filePath);
                
                //sw.WriteLine(conflict.Object.ToString());
                //conflict.Resolve(RefreshMode.KeepChanges, true);
                //sw.WriteLine(conflict.Object.ToString());
            }

            return filePath;

        }

       

        #region Nested type: StatusType

        internal enum StatusType
        {
            Default,
            Filter
        }

        #endregion
    }
}