using System.Collections.Generic;
using System.Linq;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.Framework.Settings;

namespace Arya.Framework.Data.AryaDb
{
    public partial class Project : BaseEntity
    {
        public Project _currentProject;
        public Project Currentproject
        {
            get { return _currentProject; }
            set { _currentProject = value; }
        }

        public Project(AryaDbDataContext parentContext, bool initialize = true)
            : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }


        public string DisplayName
        {
            get
            {
                return ClientDescription + ' ' + SetName;
            }
        }

        #region Properties (1)

        public string ProjectDescription
        {
            get { return ClientDescription.Trim() + " " + SetName.Trim(); }
        }

        public override string ToString()
        {
            return ProjectDescription;
        }

        #endregion Properties

        #region Methods (1)

        // Private Methods (1) 

        private ProjectPreferences _preferencesObj;

        public ProjectPreferences ProjPreferences
        {
            get
            {
                if (_preferencesObj == null)
                {
                    _preferencesObj = Preferences == null
                                         ? new ProjectPreferences()
                                         : XmlSerializationHelper.DeserializeFromXElement<ProjectPreferences>(
                                             Preferences);
                }
                return _preferencesObj;
            }
            //set { this.Preferences = XmlSerializationHelper.SerializeToXElement(value); }
        }

        public IEnumerable<Attribute> GetMetaAttributes(AttributeTypeEnum type)
        {
            var atts = from att in this.Attributes
                where type.ToString() == att.AttributeType
                let displayOrder =
                    (from si in att.SchemaInfos from sd in si.SchemaDatas where sd.Active select sd.DisplayOrder)
                        .FirstOrDefault()
                orderby displayOrder, att.AttributeName
                select att;
            return atts.ToList();
        }


        public List<Url> BrowserUrls
        {
            get
            {
                return ProjPreferences.AssetUrlSection == null
                           ? new List<Url>()
                           : ProjPreferences.AssetUrlSection.Urls == null ? new List<Url>() : ProjPreferences.AssetUrlSection.Urls.Where(
                               p => p.Target == "Browser" && !string.IsNullOrEmpty(p.Value)).ToList();
            }
        }

        public Url PictureBoxUrl
        {
            get
            {
                return ProjPreferences.AssetUrlSection == null
                           ? null
                           : ProjPreferences.AssetUrlSection.Urls == null ? null : ProjPreferences.AssetUrlSection.Urls.FirstOrDefault(
                               p => p.Target == "PictureBox" && !string.IsNullOrEmpty(p.Value));
            }
        }

        public void SaveCurrentPreferences()
        {
            Preferences = XmlSerializationHelper.SerializeToXElement(ProjPreferences);
        }

        #endregion Methods
    }
}