using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ArkaApp.Helper
{
    public static class Utility
    {
        public static string MakeSaltPassword()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        public static string GetHashed(string value, string key, string saltPassword)
        {
            return Hash.HMAC_SHA256(value + saltPassword, key);
        }
        public static string GetSaltPass(string userName, string saltPassword)
        {
            userName = userName.ToLower();
            var usLengh = userName.Length;

            return userName.Substring(usLengh > 6 ? usLengh - 4 : 2, usLengh > 6 ? 4 : usLengh - 2) + saltPassword + userName.Substring(0, 3);
        }
        public static bool HasUserAccess(string userName, string controllerName)
        {
            using (Models.EntityModels.ArkaAppEntities entities = new Models.EntityModels.ArkaAppEntities())
            {
                int RoleID = entities.User.Where(u => u.UserName == userName).SingleOrDefault().UserRoleID;
                List<int> MenuIDList = entities.MenuRole.Where(m => m.UserRoleID == RoleID).Select(m => m.MenuID).ToList();

                foreach (int menuID in MenuIDList)
                {
                    if (controllerName == entities.Menu.Where(m => m.MenuID == menuID).SingleOrDefault().ControllerName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static int GetAccountId()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var userData = authTicket.UserData.Split(',').ToArray();

            if (userData.Length != 3)
                return 0;
            else
                return Convert.ToInt32(userData[2]);
        }

        public static int GetUserId()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var userData = authTicket.UserData.Split(',').ToArray();
            if (userData.Length > 0)
                return 0;
            else
                return Convert.ToInt32(userData[0]);
        }

        public static void SaveWalletPaymentLog(long Count, int LogId, Models.ActionEnumModel LogType, List<string> Options)
        {
            var accountId = Utility.GetAccountId();

            WalletHelper wallet = new WalletHelper();
            var amount = wallet.GetPackageAmount((int)LogType, Count, Options);

            wallet.SavePaymentLog(accountId, amount, LogId, LogType.ToString());
        }

        public static bool HasImageExtension(this string source)
        {
            return (source.ToLower().EndsWith(".png")
                || source.EndsWith(".jpg")
                || source.EndsWith(".jpeg")
                || source.EndsWith(".tiff")
                || source.EndsWith(".bmp"));
        }

        public static bool HasVideoExtension(this string source)
        {
            return (source.ToLower().EndsWith(".webm")
                || source.EndsWith(".mkv")
                || source.EndsWith(".mp4")
                || source.EndsWith(".gif")
                || source.EndsWith(".avi")
                || source.EndsWith(".wmv"));
        }

        public static bool HasAudioExtension(this string source)
        {
            return (source.ToLower().EndsWith(".3ga")
                || source.EndsWith(".m4a")
                || source.EndsWith(".wav")
                || source.EndsWith(".mp3"));
        }
    }
}