using NHunspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.SpellCheck
{
    public enum AvailableDictionaries
    {
        en_US,
        en_GB
    }

    public  class SpellCheckerFactory
    {
        private static Hunspell _spellChecker;
        private const string FtpHost = "ftp://dev.empirisense.com";
        private const string HttpDir = "http://dev.empirisense.com/AryaAssets/AppAssets";
        private const string User = "bmipm";
        private const string Pass = "bmiNovember";
        private const string AryaAppAssetDir = "AppAssets";
        private static string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AryaAppAssetDir);


        public static Hunspell GetSpellChecker(AvailableDictionaries dictionaryName = AvailableDictionaries.en_US)
        {
            string affFilePath = GetFilePath(Enum.GetName(typeof(AvailableDictionaries), dictionaryName),".aff");
            string dictionaryFilePath = GetFilePath(Enum.GetName(typeof(AvailableDictionaries), dictionaryName),".dic");
            if (_spellChecker == null)
            {
                _spellChecker = new Hunspell(affFilePath, dictionaryFilePath);            
            }
            return _spellChecker;
        }

        private static string GetFilePath(string fileName, string fileExtension)
        {
            if (!File.Exists(BasePath + fileName + fileExtension))//fileExist?
                DownloadFile(fileName + fileExtension);            
            return BasePath + fileName + fileExtension;
        }

        private static bool DownloadFile(string fileName)
        {
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
            var success = false;
            Uri resultUri = null;
            string fileUri = Path.Combine( HttpDir, fileName);
            try
            {
                //if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                if (!Uri.TryCreate(fileUri, UriKind.Absolute, out resultUri))
                    return false;

                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(resultUri);
                var response = (System.Net.HttpWebResponse)request.GetResponse();

                // Check that the remote file was found. The ContentType
                // check is performed since a request for a non-existent
                // image file might be redirected to a 404-page, which would
                // yield the StatusCode "OK", even though the image was not
                // found.
                if ((response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Moved
                     || response.StatusCode == System.Net.HttpStatusCode.Redirect))
                {
                    // if the remote file was found, download it
                    using (var inputStream = response.GetResponseStream())
                    using (Stream outputStream = File.OpenWrite(BasePath + fileName))
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

        
        static private  byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

    }
}
