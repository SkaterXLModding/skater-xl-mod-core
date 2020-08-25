using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SXLMod
{
    public class SXLConfiguration
    {
        private string _path;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public SXLConfiguration(string configPath = null)
        {
            if (!File.Exists(configPath))
            {
                File.Create(configPath);
            }
            _path = new FileInfo(configPath)?.FullName;
        }

        public string Read(string key, string section = null)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? "default", key, "", retVal, 255, _path);
            return retVal.ToString();
        }

        public string TryGet(string key, string section = null, string defaultValue = null)
        {
            if (KeyExists(key, section))
            {
                return Read(key, section);
            }
            return defaultValue;
        }

        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section.ToLower() ?? "default", key, value, _path);
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? "default");
        }

        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? "default");
        }

        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }

    }
}
