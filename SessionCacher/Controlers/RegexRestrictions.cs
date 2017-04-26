using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SessionCacher.Controlers
{
    public static class Restrictions
    {
        //If true then we should remove record.
        public static bool CheckRestrictions(Process process)
        {
            if (IsMainWindowTitleEmpty(process.MainWindowTitle))
                return true;

            var path = process.MainModule.FileName;
            return IsAppFromWindowsApps(path)
                   || IsAppFromWindowsSystem(path)
                   || IsMyAppName(path);

        }

        private static bool IsAppFromWindowsSystem(string path)
        {
            return Regex.IsMatch(path, ":\\\\WINDOWS");
        }
        private static bool IsAppFromWindowsApps(string path)
        {
            return Regex.IsMatch(path, "WindowsApps");
        }
        private static bool IsMyAppName(string path)
        {
            return Regex.IsMatch(path, System.AppDomain.CurrentDomain.FriendlyName);
        }

        private static bool IsMainWindowTitleEmpty(string title)
        {
            return string.IsNullOrEmpty(title);
        }

        //TODO
        //ADD custom restriction list

    }
}
