using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Natalie.Data;
using Natalie.Framework.IO;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Natalie.HelperForms;
using Attribute = Natalie.Data.Attribute;

namespace Natalie.HelperClasses
{
    class ImageManager
    {
        public delegate void GotoUrlDelegate(string url, string title = null);

        #region Fields

        private const string FtpHost = "ftp://iris.bytemanagers.com";
        private const string HttpDir = "http://iris.bytemanagers.com/NatalieAssets/";
        private const string User = "bmipm";
        private const string Pass = "bmiNovember";
        private const string RemoteRoot = "/PSO/Internal/NatalieAssets/";
        private const string NullImage = HttpDir + "noimage.jpg";
        private readonly string _projectId = NatalieTools.Instance.InstanceData.CurrentProject.ID.ToString();
        private Sku _imageSku;

        #endregion

        #region Properties

        public string RemoteImageGuid { get; set; }
        public string LocalImageName { get; private set; }
        public string LocalDirectory { get; set; }

        public Sku ImageSku
        {
            get { return _imageSku; }
            set
            {
                _imageSku = value;
                RemoteImageGuid = _imageSku == null ? String.Empty : _imageSku.ItemID;
            }
        }

        public string OriginalFileUri
        {
            get
            {
                return ImageSku != null
                           ? ImageSku.GetValuesForAttribute(Framework.Data.NatalieDb.Attribute.ImageSkuImageOriginalFileUriAttributeName).Select(ed => ed.Value).FirstOrDefault()
                           : string.Empty;
            }
        }
        public string ImageSkuGuidFileName
        {
            get
            {
                return ImageSku != null
                    ? ImageSku.GetValuesForAttribute(Framework.Data.NatalieDb.Attribute.ImageSkuGuidFileNameAttributeName).Select(ed => ed.Value).FirstOrDefault()
                    : string.Empty;
            }
        }
        public string OriginalImageExtension
        {
            get
            {
                return ImageSku != null
                           ? ImageSku.GetValuesForAttribute(Framework.Data.NatalieDb.Attribute.ImageSkuImageFileExtensionAttributeName).Select(ed => ed.Value).FirstOrDefault()
                           : string.Empty;
            }
        }

        public Image Image
        { 
            get
            {
                string remoteDest = String.IsNullOrEmpty(RemoteImageGuid) ? NullImage : RemoteImageGuid;
                Image returnImage = StreamImage(remoteDest);

                return returnImage;
            }
        }

        public string RemoteImageUrl
        {
            get
            {
                string remoteDest = String.Empty;

                if (!String.IsNullOrEmpty(RemoteImageGuid))
                {
                    remoteDest = HttpDir + _projectId + "/" + RemoteImageGuid;
                }
                return remoteDest;   
            }
        }
     
        #endregion

        #region Public Methods

        public bool UploadImage()
        {
            bool success = false;
            var imageDialog = new OpenFileDialog
                {
                    Filter = "Image Files (*.jpg;*.bmp;*.gif;*.png)|*.jpg;*.bmp;*.gif;*.png"
                };

            var result = imageDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                success = UploadImage(imageDialog.FileName);
            }
            return success;
        }

        public bool UploadImage(Uri inputSite)
        {
            bool success = false;

            string url = inputSite.AbsoluteUri;

            string tempImage = Path.GetTempFileName();
            success = FetchImage(url, tempImage);
            if (success)
            {
                success = UploadImage(tempImage);
            }

            return success;
        }

        public bool UploadImage(string localName)
        {
            bool success = false;
            
            if (File.Exists(localName))
            {
                RemoteImageGuid = Guid.NewGuid().ToString();

                var newSku = new Sku
                {
                    ID = new Guid(RemoteImageGuid),
                    ItemID = RemoteImageGuid,
                    ProjectID = NatalieTools.Instance.InstanceData.CurrentProject.ID,
                    SkuType = Framework.Data.NatalieDb.Sku.ItemType.EnrichmentImage.ToString()
                };
                
                newSku.EntityInfos.Add(new EntityInfo
                {
                    EntityDatas = { new EntityData { Attribute = Attribute.GetAttributeFromName("Image FileName", true), Value = localName } }
                });
                
                newSku.EntityInfos.Add(new EntityInfo
                {
                    EntityDatas = { new EntityData { Attribute = Attribute.GetAttributeFromName("Image FileExtension", true), Value = Path.GetExtension(localName) } }
                });
                ImageSku = newSku;

                StoreImage(localName);

                success = true;
            }
            return success;
        }

        public void DisplayImage()
        {
            if (!String.IsNullOrEmpty(RemoteImageGuid))
            {
                string remoteDest = HttpDir + _projectId + "/" + RemoteImageGuid + OriginalImageExtension;
                NatalieTools.Instance.Forms.BrowserForm.BeginInvoke(
                    new GotoUrlDelegate(NatalieTools.Instance.Forms.BrowserForm.GotoUrl), remoteDest, "Enrichment Image");
            }
            else
            {
                NatalieTools.Instance.Forms.BrowserForm.BeginInvoke(
                    new GotoUrlDelegate(NatalieTools.Instance.Forms.BrowserForm.GotoUrl), NullImage, "Enrichment Image");
            }
        }

        public bool DownloadImage(TaxonomyInfo taxonomy, Attribute attribute = null, ListOfValue lov = null)
        {
            bool success = false;

            if (!(String.IsNullOrEmpty(RemoteImageGuid) || String.IsNullOrEmpty(LocalDirectory)) && Directory.Exists(LocalDirectory))
            {
                // get fully formed name
                string localName = GetLocalName(taxonomy, attribute, lov);
                LocalImageName = LocalDirectory + "\\" + localName;

                // download file
                success = FetchImage(HttpDir + _projectId + "/" + RemoteImageGuid + OriginalImageExtension, LocalImageName);
            }
            else
            {
                LocalImageName = "";
            }
            return success;
        }

        #endregion

        #region Private Methods

        private string GetLocalName(TaxonomyInfo taxonomy, Attribute attribute, ListOfValue lov)
        {
            // this is only a sketch
            var localName = new StringBuilder();

            localName.Append(CleanName(taxonomy.TaxonomyData.NodeName));
            if (attribute != null)
            {
                localName.Append("("+CleanName(attribute.AttributeName)+")");
            }

            if (lov != null)
            {
                localName.Append("["+CleanName(lov.Value)+"]");
            }

            localName.Append(OriginalImageExtension);

            return localName.ToString();
        }

        private string CleanName(string name)
        {
            var sb = new StringBuilder();
            var regexAlpha = new Regex("[a-zA-Z0-9 ]");
            var regexSpace = new Regex(" +");
            
            foreach (char t in name.Where(t => regexAlpha.IsMatch(t.ToString(CultureInfo.InvariantCulture))))
            {
                sb.Append(t);
            }
            string result = regexSpace.Replace(sb.ToString(), "_");    
            
            return result;
        }

        private void StoreImage(string localName)
        {
            if (!String.IsNullOrEmpty(RemoteImageGuid))
            {
                Guid waitkey = FrmWaitScreen.ShowMessage("Uploading image...");
                string remoteDest = RemoteRoot + _projectId + "/" + RemoteImageGuid + OriginalImageExtension;
                var ftp = new Ftp(FtpHost, User, Pass);
                ftp.CreateDirectory(RemoteRoot + _projectId);
                ftp.Upload(remoteDest, localName);
                FrmWaitScreen.HideMessage(waitkey);
            }
        }

        private Image StreamImage(string url)
        {
            Image returnImage = null;

            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.AllowWriteStreamBuffering = true;
                request.Timeout = 20000;

                var response = request.GetResponse();
                var stream = response.GetResponseStream();

                if (stream != null)
                {
                    returnImage = Image.FromStream(stream);
                }

                response.Close();
            }
            catch
            {
                returnImage = null;
            }

            return returnImage;
        }

        private bool FetchImage(string url, string fileName)
        {
            bool success = false;
            
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                // if the remote file was found, download it
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    if (inputStream != null)
                    {
                        var buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);
                        success = true;
                    }
                }
            }
            return success;
        }
        #endregion
    }
}
