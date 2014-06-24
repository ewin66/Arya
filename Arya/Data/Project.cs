using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.Framework.Settings;

namespace Arya.Data
{
    using Framework.Settings;

    public partial class Project
    {
        #region Properties (1) 

        public string ProjectDescription
        {
            get { return ClientDescription.Trim() + " " + SetName.Trim() + " [" + AryaCodeBaseVersion.Trim() + "]"; }
        }

        #endregion Properties 

        #region Methods (1) 

        // Private Methods (1) 

        private ProjectPreferences _projectPreferences;

        public ProjectPreferences ProjectPreferences
        {
            get
            {
                return _projectPreferences ??
                       (_projectPreferences =
                        Preferences == null
                            ? new ProjectPreferences()
                            : XmlSerializationHelper.DeserializeFromXElement<ProjectPreferences>(Preferences));
            }
            set { Preferences = XmlSerializationHelper.SerializeToXElement(value); }
        }

      
        public List<Url> BrowserUrls
        {
            get
            {

                return ProjectPreferences.AssetUrlSection == null
                           ? new List<Url>()
                           : ProjectPreferences.AssetUrlSection.Urls == null ? new List<Url>() : ProjectPreferences.AssetUrlSection.Urls.Where(
                               p => p.Target == "Browser" && !string.IsNullOrEmpty(p.Value)).ToList();
            }
        }

        public Url PictureBoxUrl
        {
            get
            {
                return ProjectPreferences.AssetUrlSection == null
                           ? null
                           : ProjectPreferences.AssetUrlSection.Urls == null ? null : ProjectPreferences.AssetUrlSection.Urls.FirstOrDefault(
                               p => p.Target == "PictureBox" && !string.IsNullOrEmpty(p.Value));
            }
        }

        //public List<Url> BrowserUrls
        //{
        //    get
        //    {
        //        return ProjectPreferences.AssetUrlSection == null
        //                   ? new List<Url>()
        //                   : ProjectPreferences.AssetUrlSection.Urls.Where(
        //                       p => p.Target == "Browser" && !string.IsNullOrEmpty(p.Value)).ToList();
        //    }
        //}

        //public Url PictureBoxUrl
        //{
        //    get
        //    {
        //        return ProjectPreferences.AssetUrlSection == null
        //                   ? null
        //                   : ProjectPreferences.AssetUrlSection.Urls.FirstOrDefault(
        //                       p => p.Target == "PictureBox" && !string.IsNullOrEmpty(p.Value));
        //    }
        //}

        public string ListSeparator
        {
            get
            {
                return ProjectPreferences.ListSeparator;
            }
            set
            {
                ProjectPreferences.ListSeparator = value;
            }
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

        public void SaveCurrentPreferences()
        {
            Preferences = XmlSerializationHelper.SerializeToXElement(ProjectPreferences);
        }

        #endregion Methods 
    }
}