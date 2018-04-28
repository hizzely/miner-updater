using System.Configuration;

namespace MinerUpdater
{
    class AppConfig
    {
        public static string Get(string _keyName)
        {
            return ConfigurationManager.AppSettings[_keyName];
        }

        public static bool Update(string _keyname, string _value)
        {
            var appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var modifyAppConfig = appConfig.AppSettings.Settings;

            if (modifyAppConfig[_keyname] == null)
                modifyAppConfig.Add(_keyname, _value);
            else
                modifyAppConfig[_keyname].Value = _value;

            appConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(appConfig.AppSettings.SectionInformation.Name);

            return true;
        }
    }
}
