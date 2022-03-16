using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Native_Messaging_Proxy
{
    public class Util
    {
        public static string? GetApplicationRoot()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        public static bool IsDirectoryWritable(string dirPath)
        {
            try
            {
                using FileStream fs = File.Create(
                Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
