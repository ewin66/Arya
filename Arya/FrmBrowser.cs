using System.Net;
using LinqKit;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Arya.UserControls;
using System.Threading.Tasks;

namespace Arya
{
    using System;
    using System.Windows.Forms;
    using Framework4.State;

    public partial class FrmBrowser : Form
    {
        #region Fields (2)
        private const string NoImageLocation = "http://dev.empirisense.com/AryaAssets/noimage.jpg";
        private static readonly string BaseUrl = AryaTools.Instance.InstanceData.CurrentProject == null
                                                     ? null
                                                     : AryaTools.Instance.InstanceData.CurrentProject.ProductSearchString;
        public bool ForceClose;
        private JavascriptSkuViewObject Jsvo { get; set; }

        public bool PinCurrentUrl { get; set; }

        public HtmlDocument Html
        {
            get
            {
                return _itemWebBrowser.Document;
            }
        }
        #endregion Fields

        #region Constructors (1)

        public FrmBrowser()
        {
            InitializeComponent();

            DisplayStyle.SetDefaultFont(this);
            Icon = Properties.Resources.AryaLogoIcon;
            Jsvo = new JavascriptSkuViewObject();
            _itemWebBrowser.ObjectForScripting = Jsvo;
            PinCurrentUrl = false;
        }

        #endregion Constructors

        #region Methods (5)

        // Public Methods (4) 

        private delegate void GotoUrlDelegate(string url, string title = null);

        private readonly string _nullImage = Framework.Properties.Resources.HttpBaseUri + "noimage.jpg";
        private Task _currentTask;

        public void DisplayImage(ImageManager imageManager)
        {
            if (!String.IsNullOrEmpty(imageManager.RemoteImageGuid))
            {
                var remoteDest = string.Format("{0}{1}/{2}{3}", Framework.Properties.Resources.HttpBaseUri,
                    AryaTools.Instance.InstanceData.CurrentProject.ID, imageManager.RemoteImageGuid,
                    imageManager.OriginalImageExtension);
                BeginInvoke(new GotoUrlDelegate(AryaTools.Instance.Forms.BrowserForm.GotoUrl), remoteDest,
                    "Enrichment Image");
            }
            else
            {
                BeginInvoke(new GotoUrlDelegate(AryaTools.Instance.Forms.BrowserForm.GotoUrl), _nullImage,
                    "Enrichment Image");
            }
        }

        public void GotoItem(string itemNumber)
        {
            if (string.IsNullOrEmpty(BaseUrl))
                return;

            var parts = BaseUrl.Split('[', ']');
            parts[1] = itemNumber;
            var searchUrl = string.Join(string.Empty, parts);

            //string url = BaseUrl.Contains("%s") ? string.Format(BaseUrl.Replace("%s", "{0}"), itemNumber) : BaseUrl;
            GotoUrl(searchUrl, "Item on the client website");
        }

        public void GotoUrl(string url, string title = null)
        {
            GotoUrl(new Uri(url ?? "about:blank"), title);
        }

        public void GotoUrl(string url, EntityDataGridView parentSkuView, string title = null, bool allowNavigation = true, bool topMost = false)
        {
            Jsvo.ParentSkuView = parentSkuView;
            GotoUrl(new Uri(url ?? "about:blank"), title);
        }

        private async void GotoUrl(Uri url, string title = null)
        {
            if (PinCurrentUrl) return;

            this.InvokeEx(t => t.Text = title ?? @"Arya Browser");

            url = url ?? new Uri("about:blank");

            if (url.AbsoluteUri.Equals("about:blank"))
            {
                _itemWebBrowser.DocumentText = string.Empty;
                this.InvokeEx(t => t.Hide());
            }
            else if (url.AbsoluteUri.StartsWith("https://"))
            {
                var proc = new System.Diagnostics.Process
                           {
                               EnableRaisingEvents = false,
                               StartInfo = { FileName = url.AbsoluteUri }
                           };
                proc.Start();
            }
            else
            {
                if (!Visible)
                {
                    this.InvokeEx(t => t.Show());
                }

                _itemWebBrowser.InvokeEx(wb => wb.Navigate(url));

                //if (_currentTask != null)
                //    _currentTask.ContinueWith(c => TryNavigateAsync(url));
                //else
                //    _currentTask = TryNavigateAsync(url);
                //await _currentTask;
            }
        }

        private async Task TryNavigateAsync(Uri url)
        {
            if (url.AbsoluteUri.StartsWith("https://") || url.AbsoluteUri.StartsWith("http://"))
            {
                if (await WebResourceExist(url))
                    await NavigateAsync(url);
                else
                    await NavigateAsync(new Uri(NoImageLocation));
            }
            else
                await NavigateAsync(url);
        }

        private async Task NavigateAsync(Uri url)
        {
            await Task.Run(() => _itemWebBrowser.Navigate(url));
        }

        private async Task<bool> WebResourceExist(Uri url)
        {
            HttpWebResponse response = null;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            try
            {
                await Task.Run(() => response = (HttpWebResponse)request.GetResponse());
            }
            catch (WebException ex)
            {
                return false;
                /* A WebException will be thrown if the status of the response is not `200 OK` */
            }
            return true;
        }

        public void SetDocumentText(string documentText)
        {
            //GotoUrl((Uri)null);
            _itemWebBrowser.DocumentText = documentText;
            Text = @"Arya Browser";
            if (!Visible)
            {
                Show();
            }
        }

        // Private Methods (1) 

        void LookupItem_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                PinCurrentUrl = false;
                Jsvo.ParentSkuView = null;
                if (Forms.MustCloseForm(e) || ForceClose)
                {
                    System.IO.File.Delete(AryaTools.Instance.ItemImagesTempFile);
                    return;
                }

                {
                    _itemWebBrowser.Url = new Uri("about:blank");
                }

                Hide();
                e.Cancel = true;
            }
            catch (Exception exception)
            {


            }


        }

        #endregion Methods
    }
}