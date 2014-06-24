using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.Framework.Settings;

namespace Arya.Data
{
    public partial class UserProject
    {
        public string GroupName
        {
            get { return Group.Name; }
        }

        public string ProjectDescription
        {
            get { return Project.ProjectDescription; }
        }

        public string UserName
        {
            get { return User.FullName; }
        }
        
        public UserProjectsPreferences UserProjectPreferences
        {
            get
            {
                return  Preferences == null
                            ? new UserProjectsPreferences()
                            : XmlSerializationHelper.DeserializeFromXElement<UserProjectsPreferences>(Preferences);
            }
            set { this.Preferences = XmlSerializationHelper.SerializeToXElement(value); }
        }

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
            GroupID = Group.DefaultGroupID;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} {2}", User.FullName, Project.ClientDescription, Project.SetName);
        }
    }
}