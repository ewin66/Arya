using System;
using System.Collections;
using System.Linq;
using System.Xml.Serialization;
using LinqKit;
using Polenter.Serialization;

namespace Arya.Framework.Utility
{
    public static class SharpSerializerSettingsFactory
    {
        public static SharpSerializerXmlSettings GetSharpSerializerXmlSettings(this object obj, string rootName)
        {
            var settings = GetSharpSerializerXmlSettings(obj.GetType(), rootName);

            const string shouldSerializeString = "ShouldSerialize";
            var methods =
                obj.GetType()
                    .GetMethods()
                    .Where(m => m.Name.StartsWith(shouldSerializeString) && m.ReturnType == typeof (bool));

            foreach (var method in methods)
            {
                var shouldSerialize = (bool)method.Invoke(obj, new object[] { });
                if (!shouldSerialize)
                    settings.AdvancedSettings.PropertiesToIgnore.Add(obj.GetType(),
                        method.Name.Replace(shouldSerializeString, string.Empty));
            }

            return settings;
        }

        public static SharpSerializerXmlSettings GetSharpSerializerXmlSettings(this Type type, string rootName)
        {
            var settings = new SharpSerializerXmlSettings
            {
                IncludeAssemblyVersionInTypeName = false,
                IncludeCultureInTypeName = false,
                IncludePublicKeyTokenInTypeName = false,
                AdvancedSettings = { RootName = rootName }
            };
            settings.AdvancedSettings.AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));
            
            //Ignore "Capacity" Property for all List<T> types
            var types =
                type.GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && typeof(IList).IsAssignableFrom(p.PropertyType));
            types.ForEach(t => settings.AdvancedSettings.PropertiesToIgnore.Add(t.PropertyType, "Capacity"));

            return settings;
        }
    }
}