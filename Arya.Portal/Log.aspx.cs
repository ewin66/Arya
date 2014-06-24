using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;
using Arya.Framework.Common;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Data.Services;
using Arya.Framework.Extensions;
using Polenter.Serialization;

namespace Arya.Portal
{
    public partial class Log : Page
    {
        #region Fields

        private string _argumentsPath;
        private Guid _taskId;
        private string _status;

        #endregion Fields

        #region Properties

        private static string Basepath
        {
            get { return ConfigurationManager.AppSettings["BasePath"]; }
        }

        private string ArgumentsPath
        {
            get
            {
                if (_argumentsPath != null)
                    return _argumentsPath;

                using (var db = new AryaServicesDbDataContext())
                {
                    _argumentsPath =
                        (from task in db.AryaTasks where task.ID == TaskId select task.ArgumentDirectoryPath)
                            .FirstOrDefault();
                }
                return _argumentsPath;
            }
        }

        private Guid TaskId
        {
            get
            {
                if (_taskId != Guid.Empty)
                    return _taskId;
                Guid.TryParse(Request.QueryString["ID"] ?? string.Empty, out _taskId);
                return _taskId;
            }
        }

        #endregion Properties

        #region Methods

        protected void blFiles_Click(object sender, BulletedListEventArgs e)
        {
            var selectedItem = blFiles.Items[e.Index];
            FileResponse(selectedItem.Value, Path.Combine(Basepath, _argumentsPath, selectedItem.Value));
        }

        protected void BtnDownloadAllFiles_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.BufferOutput = false; // for large files
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", string.Format("attachment; filename={0}.zip", TaskId));

            using (var zip = new ZipFile())
            {
                zip.AddDirectory(Path.Combine(Basepath, ArgumentsPath));
                zip.Save(Response.OutputStream);
            }
            Response.Close();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            const string timeTaken = "Time taken: ";

            //if (Initialize() &&
            var sw = new Stopwatch();
            sw.Start();
            LoadArguments();
            lblArgumentsProcessTime.Text = timeTaken + sw.Elapsed.ToString("g");

            sw.Restart();
            LoadLog();
            lblLogProcessTime.Text = timeTaken + sw.Elapsed.ToString("g");

            sw.Restart();
            PopulateFileLinks();
            lblFileListProcessTime.Text = timeTaken + sw.Elapsed.ToString("g");

            if (_status == "Working")
            {
                Response.Write("<script>setTimeout(function () { location.reload(1); }, 5000);</script>");
            }
            //return;
            //pnlBadRequest.Visible = true;
        }

        private static Panel ProcessWorkerSummary(WorkerSummary summaryLog)
        {
            var summaryPanel = new Panel();
            var properties = summaryLog.GetType().GetProperties();
            List<WorkerSummary> children = null;

            var name = properties.FirstOrDefault(p => p.Name == "WorkerName");
            if (name != null)
            {
                summaryPanel.Controls.Add(new Label
                                          {
                                              Text =
                                                  string.Format("<h2>{0}<br /></h2>",
                                                      name.GetValue(summaryLog))
                                          });
            }

            foreach (var prop in properties.OrderBy(p => p.Name).Where(prop => prop != name && prop.IsBrowsable()))
            {
                if (prop.PropertyType == typeof(List<WorkerSummary>))
                {
                    children = prop.GetValue(summaryLog) as List<WorkerSummary>;
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType.IsGenericType)
                {
                    var values = (IEnumerable)prop.GetValue(summaryLog);
                    var count = values.Cast<object>().Count();
                    var labelText = count > 25
                        ? count + " instances. Please see log file for details."
                        : values.Cast<object>()
                            .Aggregate(string.Empty,
                                (current, val) =>
                                    current
                                    + ("&nbsp; &nbsp; &nbsp; &nbsp;" + val.ToString().HtmlLineBreakify() + "<br />"));

                    if (string.IsNullOrWhiteSpace(labelText))
                        continue;

                    summaryPanel.Controls.Add(new Label
                                              {
                                                  Text =
                                                      string.Format("{0}:<br />{1}", prop.Name.Spacify(),
                                                          labelText)
                                              });
                }
                else
                {
                    var value = prop.GetValue(summaryLog);
                    if (value == null)
                        continue;

                    var stringValue = value.ToString();
                    if (value is Enum)
                        stringValue = value.ToString().Spacify();

                    summaryPanel.Controls.Add(new Label
                                              {
                                                  Text =
                                                      string.Format("{0}: {1}<br />", prop.Name.Spacify(),
                                                          stringValue.HtmlLineBreakify())
                                              });
                }
            }

            if (children == null)
                return summaryPanel;

            foreach (var child in children)
                summaryPanel.Controls.Add(ProcessWorkerSummary(child));

            return summaryPanel;
        }

        private void FileResponse(string downloadfilename, string filePath)
        {
            // Checking if file exists
            if (File.Exists(filePath))
            {
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("content-disposition", "attachment; filename=" + downloadfilename);

                // Add the file size into the response header
                Response.AddHeader("Content-Length",
                    new FileInfo(filePath).Length.ToString(CultureInfo.InvariantCulture));

                // Write the file into the response
                Response.TransmitFile(filePath);
                Response.End();
            }
            else
            {
                Response.ContentType = "text/html";
                Response.Write("Downloading Error!");
            }
        }

        private bool Initialize() { return TaskId != Guid.Empty && !string.IsNullOrWhiteSpace(ArgumentsPath); }

        private bool LoadArguments()
        {
            try
            {
                using (var taskDc = new AryaServicesDbDataContext())
                using (var aryaDc = new AryaDbDataContext())
                {
                    var task = taskDc.AryaTasks.Single(t => t.ID == TaskId);
                    var taskInfo = (from up in aryaDc.UserProjects
                                    where up.Project.ID == task.ProjectID && up.User.ID == task.SubmittedBy
                                    select
                                        new
                                        {
                                            JobType = task.DisplayJobType,
                                            Project = up.Project.DisplayName,
                                            SubmittedBy = up.User.FullName,
                                            SubmittedOn = task.SubmittedOn.ToString(),
                                            Status = task.Status.Spacify()
                                        }).First();

                    _status = taskInfo.Status;

                    var properties = taskInfo.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(taskInfo).ToString();
                        phArguments.Controls.Add(new Label { Text = prop.Name.Spacify() + ":", CssClass = "PropertyName" });
                        phArguments.Controls.Add(new Label { Text = value, CssClass = "PropertyValue" });
                        phArguments.Controls.Add(new LiteralControl("<br />"));
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            try
            {
                var arguments = new SharpSerializer().Deserialize(Path.Combine(Basepath, ArgumentsPath, "Arguments.xml"));
                var properties = (from prop in arguments.GetType().GetProperties()
                                  let category =
                                      prop.IsDefined(typeof(CategoryAttribute), false)
                                          ? prop.GetCustomAttributes(typeof(CategoryAttribute), false)
                                              .Cast<CategoryAttribute>()
                                              .Single()
                                              .Category
                                          : "Other"
                                  let name =
                                      prop.IsDefined(typeof(DisplayNameAttribute), false)
                                          ? prop.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                              .Cast<DisplayNameAttribute>()
                                              .Single()
                                              .DisplayName
                                          : prop.Name
                                  let displayOrder =
                                      prop.IsDefined(typeof(PropertyOrderAttribute), false)
                                          ? prop.GetCustomAttributes(typeof(PropertyOrderAttribute), false)
                                              .Cast<PropertyOrderAttribute>()
                                              .Single()
                                              .Order
                                          : 999
                                  let shouldSerializeMethod = arguments.GetType().GetMethod("ShouldSerialize" + prop.Name)
                                  let shouldSerialize =
                                      shouldSerializeMethod == null || (bool)shouldSerializeMethod.Invoke(arguments, new object[] { })
                                  where shouldSerialize
                                  orderby displayOrder
                                  select new { Property = prop, Category = category, Name = name, displayOrder }).ToList();

                foreach (var prop in properties)
                {
                    var propertyType = prop.Property.PropertyType;
                    if (propertyType == typeof(Guid) || propertyType == typeof(Guid[]))
                        continue;

                    var basevalue = prop.Property.GetValue(arguments) ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(basevalue.ToString()))
                        continue;

                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        phArguments.Controls.Add(new Label
                                                 {
                                                     Text = prop.Name.Spacify() + ":",
                                                     CssClass = "PropertyName",
                                                 });

                        phArguments.Controls.Add(new LiteralControl("<br />"));

                        var kvps = (IDictionary)basevalue;
                        foreach (DictionaryEntry kvp in kvps)
                        {
                            phArguments.Controls.Add(new Label
                                                     {
                                                         Text = kvp.Key.ToString().Spacify(),
                                                         CssClass = "PropertyValueLeft"
                                                     });
                            phArguments.Controls.Add(new Label
                                                     {
                                                         Text = kvp.Value.ToString().Spacify(),
                                                         CssClass = "PropertyValue"
                                                     });
                            phArguments.Controls.Add(new LiteralControl("<br />"));
                        }
                        continue;
                    }

                    var stringValue = basevalue.ToString();
                    if (propertyType.IsArray)
                    {
                        stringValue = string.Empty;
                        var values = (Array)prop.Property.GetValue(arguments);
                        for (var i = 0; i < values.Length; i++)
                            stringValue += (i > 0 ? ", " : string.Empty) + values.GetValue(i);
                    }

                    if (string.IsNullOrWhiteSpace(stringValue) || stringValue == new DateTime().ToString())
                        continue;

                    if (prop.Name.ToLower().EndsWith("path"))
                    {
                        try
                        {
                            var path = Path.GetFullPath(stringValue);
                            stringValue = Path.GetFileName(stringValue);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    phArguments.Controls.Add(new Label { Text = prop.Name.Spacify() + ":", CssClass = "PropertyName" });
                    phArguments.Controls.Add(new Label { Text = stringValue, CssClass = "PropertyValue" });
                    phArguments.Controls.Add(new LiteralControl("<br />"));
                }

                pnlArguments.Visible = true;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool LoadLog()
        {
            try
            {
                using (var file = new StreamReader(Path.Combine(Basepath, ArgumentsPath, "log.xml")))
                {
                    var summaryLog = file.ReadToEnd().DeserializeObject<WorkerSummary>();
                    phSummaries.Controls.Add(ProcessWorkerSummary(summaryLog));
                }
                pnlLog.Visible = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool PopulateFileLinks()
        {
            try
            {
                var ignoreFiles = new[] { "log.xml", "arguments.xml" };
                var files = Directory.GetFiles(Path.Combine(Basepath, ArgumentsPath));

                BtnDownloadAllFiles.Text = string.Format("Download {0} files", files.Length);
                if (files.Length > 10)
                {
                    blFiles.DataSource = Enumerable.Range(1, 0).Select(r => new { Text = "", Value = "" });
                }
                else
                {
                    var filenames = (from file in files
                                     let nameWithExtension = Path.GetFileName(file) ?? string.Empty
                                     where !ignoreFiles.Contains(nameWithExtension.ToLower())
                                     orderby nameWithExtension
                                     select new { Text = nameWithExtension, Value = nameWithExtension }).ToList();

                    blFiles.DataSource = filenames;
                }
                blFiles.DataTextField = "Text";
                blFiles.DataValueField = "Value";
                blFiles.DataBind();

                pnlFiles.Visible = files.Length > 0;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Methods
    }
}