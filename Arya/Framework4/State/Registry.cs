namespace Arya.Framework4.State
{
    using Microsoft.Win32;

    internal class WindowsRegistry
    {
        public const string RegistryKeyCurrentTaxonomy = "CurrentTaxonomy";
        public const string AryaRegistrySubKey = "Arya";
        public const string RegistryKeyProject = "Project";
        private static RegistryKey _baseKey;

        private static void InitRegistryBaseKey(bool autoCreate)
        {
            _baseKey = Registry.CurrentUser.OpenSubKey(AryaRegistrySubKey, true);

            if (_baseKey == null && autoCreate)
                _baseKey = Registry.CurrentUser.CreateSubKey(AryaRegistrySubKey);
        }

        internal static string GetFromRegistry(string key)
        {
            if (_baseKey == null)
                InitRegistryBaseKey(false);

            if (_baseKey != null)
            {
                var value = _baseKey.GetValue(key);
                if (value != null)
                    return value.ToString();
            }

            return null;
        }

        internal static void SaveToRegistry(string key, string value)
        {
            if (_baseKey == null)
                InitRegistryBaseKey(true);


            if (_baseKey != null)
                _baseKey.SetValue(key, value);
        }
    }
}