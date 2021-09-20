namespace ArkaApp.Settings
{
    public class Setting
    {
        public static string EncryptKey
        {
            get
            {
                return string.Concat(System.Configuration.ConfigurationManager.AppSettings["enk"], "ArKAcoMPanY");
            }
        }

        public static string EncryptIV
        {
            get
            {
                return string.Concat(System.Configuration.ConfigurationManager.AppSettings["env"], "ARkaCOmpANy");
            }
        }

        public static string HashKey
        {
            get
            {
                return string.Concat(System.Configuration.ConfigurationManager.AppSettings["hsk"], "ARKACOMPANY");
            }
        }
    }
}