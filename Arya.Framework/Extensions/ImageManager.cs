using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO;
using Arya.Framework.Utility;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.Extensions
{
    public class ImageManager
    {
        #region Fields

        private const string FtpHost = "ftp://dev.empirisense.com";
        private const string HttpDir = "http://dev.empirisense.com/AryaAssets/";
        private const string User = "bmipm";
        private const string Pass = "bmiNovember";
        private const string RemoteRoot = "/PSO/Internal/AryaAssets/";
        private const string NullImage = HttpDir + "noimage.jpg";
        private readonly AryaDbDataContext _db;
        private readonly Guid _projectId;
        private Sku _imageSku;

        #endregion

        #region Properties

        public string RemoteImageGuid { get; set; }
        public string LocalImageName { get; private set; }
        public string LocalDirectory { get; set; }

        public Sku ImageSku
        {
            get
            {
                return _imageSku;
            }
            set
            {
                _imageSku = value;
                RemoteImageGuid = _imageSku == null ? String.Empty : _imageSku.ItemID;
            }
        }

        public string ItemId
        {
            get
            {
                try
                {
                    return ImageSku.ItemID;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            set { ImageSku = _db.Skus.Where(sku => sku.ItemID == value).FirstOrDefault(); }
        }

        public string OriginalFileUri
        {
            get
            {
                return ImageSku != null
                    ? ImageSku.GetValuesForAttribute(_db, Properties.Resources.ImageSkuImageOriginalFileUriAttributeName)
                        .Select(ed => ed.Value)
                        .FirstOrDefault()
                    : string.Empty;
            }
        }

        public string OriginalFileName
        {
            get
            {
                return ImageSku != null
                    ? ImageSku.GetValuesForAttribute(_db,Properties.Resources.ImageSkuImageOriginalFileNameAttributeName)
                        .Select(ed => ed.Value)
                        .FirstOrDefault()
                    : string.Empty;
            }
        }

        public string ImageSkuGuidFileName
        {
            get
            {
                return ImageSku != null
                    ? ImageSku.GetValuesForAttribute(_db,Properties.Resources.ImageSkuGuidFileNameNoExtensionAttributeName)
                        .Select(ed => ed.Value)
                        .FirstOrDefault()
                    : string.Empty;
            }
        }

        public string OriginalImageExtension
        {
            get
            {
                return ImageSku != null
                    ? ImageSku.GetValuesForAttribute(_db, Properties.Resources.ImageSkuImageFileExtensionAttributeName)
                        .Select(ed => ed.Value)
                        .FirstOrDefault()
                    : string.Empty;
            }
        }

        public Image Image
        {
            get
            {
                var remoteDest = String.IsNullOrEmpty(RemoteImageGuid) ? NullImage : RemoteImageGuid;
                var returnImage = StreamImage(remoteDest);

                return returnImage;
            }
        }

        public string RemoteImageUrl
        {
            get
            {
                var remoteDest = String.Empty;

                if (!String.IsNullOrEmpty(RemoteImageGuid))
                    remoteDest = HttpDir + _projectId + "/" + RemoteImageGuid;
                return remoteDest;
            }
        }

        #endregion

        #region Constructors

        public ImageManager(AryaDbDataContext db, Guid projectId)
        {
            _db = db;
            _projectId = projectId;
        }

        public ImageManager(AryaDbDataContext db, Guid projectId, string itemId):this(db,projectId)
        {
            ItemId = itemId;
        }

        #endregion

        #region Public Methods

        public bool UploadImage()
        {
            var success = false;
            if (_db.CurrentUser.IsAryaReadOnly(_projectId))
            {
                MessageBox.Show(@"Enrichment Images cannot be added in read-only mode.");
                return false;
            }

            var imageDialog = new OpenFileDialog
                              {
                                  Filter =
                                      @"Image Files (*.jpg;*.bmp;*.gif;*.png)|*.jpg;*.bmp;*.gif;*.png"
                              };

            var result = imageDialog.ShowDialog();
            if (result == DialogResult.OK)
                success = UploadImage(imageDialog.FileName, false);
            return success;
        }

        public bool UploadImage(string inputSite, bool continueOnFileNotFound = true)
        {
            //var isUri = Uri.IsWellFormedUriString(inputSite, UriKind.Absolute);
            // create temporary file name with the image's extension
            bool success = false;
            var imageGuid = Guid.NewGuid();
            var localImageFileStorePath = Path.GetTempPath() + imageGuid + Path.GetExtension(inputSite);
            //var success = FetchImage(inputSite, localImageFileStorePath);
            // attempt to download then upload the image.
            CreateImageSku(imageGuid, inputSite);
            Uri resultUri;
            if (!Uri.TryCreate(inputSite, UriKind.Absolute, out resultUri))
                return false;
            if (resultUri.Scheme == Uri.UriSchemeFile)
            {
                return StoreImage(inputSite);
            }
            if(FetchImage(inputSite, localImageFileStorePath))
                success = StoreImage(localImageFileStorePath);
            return success;
        }

       

        private void CreateImageSku(Guid guidFileName, string originalFileUri)
        {
            _db.SubmitChanges();

            var newSku = new Sku(_db)
            {
                ID = guidFileName,
                ItemID = guidFileName.ToString(),
                ProjectID = _projectId,
                SkuType = Sku.ItemType.EnrichmentImage.ToString()
            };

            _db.Skus.InsertOnSubmit(newSku);
            _db.SubmitChanges();

            ImageSku = newSku;
            RemoteImageGuid = newSku.ID.ToString();
            var originalFilename = originalFileUri;
            Uri resultUri;
            if (Uri.TryCreate(originalFileUri, UriKind.Absolute, out resultUri))
            {
               // if (resultUri.IsFile)
                    originalFilename = Path.GetFileName(resultUri.AbsolutePath);
            }

            AddAttributeValue(Properties.Resources.ImageSkuGuidFileNameNoExtensionAttributeName, guidFileName.ToString());
            AddAttributeValue(Properties.Resources.ImageSkuImageFileExtensionAttributeName, Path.GetExtension(originalFileUri));
            AddAttributeValue(Properties.Resources.ImageSkuImageOriginalFileUriAttributeName, originalFileUri);
            AddAttributeValue(Properties.Resources.ImageSkuImageOriginalFileNameAttributeName, originalFilename);

            _db.SubmitChanges();
        }

        public string GetImageUrl()
        {
            string remoteDest;

            if (!String.IsNullOrEmpty(RemoteImageGuid))
                remoteDest = HttpDir + _projectId + "/" + RemoteImageGuid + OriginalImageExtension;
            else
                remoteDest = NullImage;
            return remoteDest;
        }

        public bool DownloadImage(Guid taxonomyId, Guid attributeId = default(Guid), Guid lovId=  default(Guid))
        {
            LocalImageName = "";

            var tax = _db.TaxonomyInfos.First(ti => ti.ID == taxonomyId);
            var att = attributeId == default(Guid) ? null : _db.Attributes.First(a => a.ID == attributeId);
            var lov = lovId == default(Guid) ? null : _db.ListOfValues.First(l => l.ID == lovId);

            if (String.IsNullOrEmpty(RemoteImageGuid) || String.IsNullOrEmpty(LocalDirectory)
                || !Directory.Exists(LocalDirectory))
                return false;

            // get fully formed name
            //var localName = GetLocalName(tax, att, lov);
            //LocalImageName = LocalDirectory + "\\" + localName;
            LocalImageName =  LocalDirectory + "\\" +OriginalFileName;
            // download file
            return FetchImage(HttpDir + _projectId + "/" + RemoteImageGuid + OriginalImageExtension, LocalImageName);
        }
        public void AddAttributeValue(string attributeName, string value)
        {
            ImageSku.EntityInfos.Add(new EntityInfo(_db)
            {
                EntityDatas =
                                         {
                                             new EntityData(_db)
                                             {
                                                 Attribute =
                                                     Attribute
                                                     .GetAttributeFromName(
                                                         _db, attributeName,
                                                         true,AttributeTypeEnum.NonMeta,false),
                                                 Value = value
                                             }
                                         }
            });
            _db.SubmitChanges();
        }
        #endregion

        #region Private Methods

        private string GetLocalName(TaxonomyInfo taxonomy, Attribute attribute, ListOfValue lov)
        {
            // this is only a sketch
            var localName = new StringBuilder();

            localName.Append(CleanName(taxonomy.TaxonomyData.NodeName));
            if (attribute != null)
                localName.Append("(" + CleanName(attribute.AttributeName) + ")");

            if (lov != null)
                localName.Append("[" + CleanName(lov.Value) + "]");

            localName.Append(OriginalImageExtension);
            return localName.ToString();
            //return OriginalFileName;

            
        }

        private string CleanName(string name)
        {
            var sb = new StringBuilder();
            var regexAlpha = new Regex("[a-zA-Z0-9 ]");
            var regexSpace = new Regex(" +");

            foreach (var t in name.Where(t => regexAlpha.IsMatch(t.ToString(CultureInfo.InvariantCulture))))
                sb.Append(t);
            var result = regexSpace.Replace(sb.ToString(), "_");

            return result;
        }

        private bool StoreImage(string localFilePath)
        {
            if (String.IsNullOrEmpty(RemoteImageGuid) || !File.Exists(localFilePath))
                return false;

            try
            {
                var remoteDest = RemoteRoot + _projectId + "/" + RemoteImageGuid + Path.GetExtension(localFilePath);
                var ftp = new Ftp(FtpHost, User, Pass);
                ftp.CreateDirectory(RemoteRoot + _projectId);
                ftp.Upload(remoteDest, localFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
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
                    returnImage = Image.FromStream(stream);

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
            var success = false;
            Uri resultUri = null;
            try
            {
                //if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                if (!Uri.TryCreate(url, UriKind.Absolute, out resultUri))
                    return false;

                var request = (HttpWebRequest)WebRequest.Create(resultUri);
                var response = (HttpWebResponse) request.GetResponse();

                // Check that the remote file was found. The ContentType
                // check is performed since a request for a non-existent
                // image file might be redirected to a 404-page, which would
                // yield the StatusCode "OK", even though the image was not
                // found.
                if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Moved
                     || response.StatusCode == HttpStatusCode.Redirect)
                    && response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    // if the remote file was found, download it
                    using (var inputStream = response.GetResponseStream())
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
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

       
    }
}