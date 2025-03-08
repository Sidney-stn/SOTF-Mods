

using UnityEngine;

namespace Signs.Tools
{
    internal class DedicatedServer
    {
        public static bool IsDeticatedServer()
        {
            string dataPath = Application.dataPath;

            // sotfPath Are 1 Level Up From The DataPath
            string sotfPath = Directory.GetParent(dataPath).FullName;

            // SonsOfTheForestDS.exe
            string sotfDs = Path.Combine(sotfPath, "SonsOfTheForestDS.exe");

            // Check If The File Exists
            if (File.Exists(sotfDs))
            {
                return true;
            }
            return false;
        }
    }
}
