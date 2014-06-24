using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Arya.Data;

namespace Arya.HelperClasses
{
    public class AssetCache
    {
        #region Fields (6) 

        private static readonly Regex RxImageUrl = new Regex("\\[(.+)\\]",RegexOptions.Compiled);
        private static readonly Regex RxOptionalExtension = new Regex("\\((.+)\\)");
        private readonly ConcurrentDictionary<string,string> downloadingFiles = new ConcurrentDictionary<string, string>(); 
        private bool[] _lookup;

        public AssetCache()
        {
            //buffer for faster removal of special characters - benchmark of 13ms :)
            _lookup = new bool[65535];
            for (char c = '0'; c <= '9'; c++) _lookup[c] = true;
            for (char c = 'A'; c <= 'Z'; c++) _lookup[c] = true;
            for (char c = 'a'; c <= 'z'; c++) _lookup[c] = true;
            _lookup['.'] = true;
            _lookup['_'] = true;

        }

        #endregion Fields 

        #region Properties (1) 

        public string RemoveSpecialCharacters(string inputString)
        {
            var buffer = new char[inputString.Length];
            var index = 0;
            foreach (char c in inputString.Where(c => _lookup[c]))
            {
                buffer[index] = c;
                index++;
            }
            return new string(buffer, 0, index);
        }

        public static string GetImageAttributeNameFromUrl(string url)
        {
            string imageAttributeName;

            try
            {
                imageAttributeName =
                    RxImageUrl.Matches(url)[0].Groups[1].Value;
            }
            catch (Exception)
            {
                imageAttributeName = string.Empty;
            }

            return imageAttributeName;
        }

        #endregion Properties 

        #region Methods (2) 

        // Public Methods (1) 

        public List<AssetInfo> GetAssets(Sku sku)
        {
            //if sku is null, return empty list
            if(sku == null)
                return new List<AssetInfo>();

            var imageUrls = AryaTools.Instance.InstanceData.CurrentProject.BrowserUrls;

            return imageUrls.Select(imageUrl => GetAsset(sku, GetImageAttributeNameFromUrl(imageUrl.Value), imageUrl.Value, imageUrl.Type)).Where(asset => asset != null).ToList();
        }

        public AssetInfo GetAsset(Sku sku)
        {
            var imageUrl = AryaTools.Instance.InstanceData.CurrentProject.BrowserUrls.First();

            return GetAsset(sku, GetImageAttributeNameFromUrl(imageUrl.Value), imageUrl.Value,
                            imageUrl.AssetAttributeName);
        }

        public AssetInfo GetAsset(Sku sku, string assetAttributeName, string assetUrl, string assetType)
        {
            if (sku == null)
                return null;

            string image = sku.GetValuesForAttribute(assetAttributeName).Select(ed => ed.Value).FirstOrDefault() ??
                           string.Empty;
            if (string.IsNullOrEmpty(image))
                return null;

            string directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Arya";
            string file = AryaTools.Instance.InstanceData.CurrentProject.ID + "_" + assetType + "_"+ RemoveSpecialCharacters(image);

            //string optionalExtension = RxOptionalExtension.Matches(assetUrl)[0].Groups[1].Value;

            //if (!string.IsNullOrWhiteSpace(optionalExtension))
            //{
            //    imageAttributeName = Path.ChangeExtension(imageAttributeName, optionalExtension);
            //}

            var imageFullFilename = directory + "\\" + file;
            if (File.Exists(imageFullFilename))
                return new AssetInfo(image, imageFullFilename, assetType);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            var hourGlassFileName = directory + "\\hourglass.png";

            string url = RxImageUrl.Replace(assetUrl, image);
            if (downloadingFiles.ContainsKey(url))
                return new AssetInfo(image,hourGlassFileName,assetType);

            if (!downloadingFiles.ContainsKey(url))
            {
                downloadingFiles.TryAdd(url, imageFullFilename);
                System.Threading.Tasks.Task.Factory.StartNew(() => DownloadWorkerMethod(url, imageFullFilename));
            }

            return new AssetInfo(image,hourGlassFileName,assetType);
        }

        private void DownloadWorkerMethod(string url, string imageFullFileName)
        {
            using (var client = new WebClient())
            {
                int attempt = 0;
                var success = false;
                while (!success && attempt < 5)
                    try
                    {
                        client.DownloadFile(url, imageFullFileName);
                        success = true;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(500);
                        attempt++;
                    }

                if (!success)
                {
                    try
                    {
                        Properties.Resources.Error.Save(imageFullFileName);
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show(file.Value, "Bad Image Name?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                string outvalue;
                downloadingFiles.TryRemove(url, out outvalue);
            }
        }

        public class AssetInfo
        {
            public AssetInfo(string assetName,string assetLocation, string assetType)
            {
                this.AssetName = assetName;
                this.AssetType = assetType;
                this.AssetLocation = assetLocation;
            }

            public string AssetType { get; private set; }

            public string AssetLocation { get; private set; }

            public string AssetName { get; private set; }
        }

        #endregion Methods 
    }
}