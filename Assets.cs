
using RedLoader;
using SonsSdk;
using UnityEngine;
using static Interop;

namespace SimpleElevator
{
    internal class Assets
    {
        // Singleton instance
        private static Assets _instance;
        // Public accessor for the singleton instance
        public static Assets Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Assets();
                }
                return _instance;
            }
        }
        public GameObject Elevator { get; private set; }
        public GameObject ElevatorControlPanel { get; private set; }

        public Texture2D ElevatorBookPage { get; private set; }
        public Texture2D ElevatorControlPanelBookPage { get; private set; }
        public Texture2D LinkUiIcon { get; private set; }

        private bool _loaded;

        /// <summary>
        /// Load The Assets, Called Once And Early, so OnGameStart can be skipped (Makes it work on dedicated server)
        /// </summary>
        public void LoadAssets()
        {
            if (_loaded) { return; }

            string dataPath = Application.dataPath;
            // sotfPath Are 1 Level Up From The DataPath
            string sotfPath = Directory.GetParent(dataPath).FullName;
            // Mods
            string modsPath = Path.Combine(sotfPath, "Mods");
            // SimpleElevator
            string modPath = Path.Combine(modsPath, "SimpleElevator");
            // Assets
            string assetsPath = Path.Combine(modPath, "ele");
            // Check If The File Exists
            if (!File.Exists(assetsPath))
            {
                RLog.Error("[SimpleElevator] Assets File Not Found");
                return;
            }
            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetsPath);
            if (assetBundle == null)
            {
                RLog.Error("[SimpleElevator] AssetBundle Not Found");
                return;
            }

            // Load the GameObject
            Elevator = assetBundle.LoadAsset<GameObject>("MainElevatorV4");
            if (Elevator == null)
            {
                RLog.Error("[SimpleElevator] StoneGateTool Asset Not Found");
                return;
            }
            Tools.MoveScene.MoveToScene(Elevator);
            

            ElevatorControlPanel = assetBundle.LoadAsset<GameObject>("EControlPanel");
            if (ElevatorControlPanel == null)
            {
                RLog.Error("[SimpleElevator] ElevatorControlPanel Asset Not Found");
                return;
            }
            //ElevatorControlPanel.HideAndDontSave().DontDestroyOnLoad();
            //ElevatorControlPanel.SetActive(false);  // Hide The UI By Default
            Tools.MoveScene.MoveToScene(ElevatorControlPanel);


            // Unload the bundle, but keep loaded assets
            assetBundle.Unload(false);

            bool failedToLoadTexture = false;
            string imagePath = GetImagePath("BookPageElevator");
            if (string.IsNullOrEmpty(imagePath))
            {
                RLog.Error("[SimpleElevator] ElevatorBookPage Image Path Not Found");
                failedToLoadTexture = true;
            } else
            {
                ElevatorBookPage = AssetLoaders.LoadTexture(GetImagePath("BookPageElevator"));
                ElevatorBookPage.hideFlags = HideFlags.HideAndDontSave;
                if (ElevatorBookPage == null)
                {
                    RLog.Error("[SimpleElevator] ElevatorBookPage Asset Not Found");
                    failedToLoadTexture = true;
                }
            }

            string imagePath2 = GetImagePath("BookPageElevatorPanel");
            if (string.IsNullOrEmpty(imagePath2))
            {
                RLog.Error("[SimpleElevator] ElevatorControlPanelBookPage Image Path Not Found");
                failedToLoadTexture = true;
            }
            else
            {
                ElevatorControlPanelBookPage = AssetLoaders.LoadTexture(GetImagePath("BookPageElevatorPanel"));
                ElevatorControlPanelBookPage.hideFlags = HideFlags.HideAndDontSave;
                if (ElevatorControlPanelBookPage == null)
                {
                    RLog.Error("[SimpleElevator] ElevatorControlPanelBookPage Asset Not Found");
                    failedToLoadTexture = true;
                }
            }

            if (failedToLoadTexture)
            {
                Misc.Msg("[SimpleElevator] All Assets Loaded Execpt for 1 or more textures", true);
                RLog.Error("[SimpleElevator] Failed To Load All Textures");
                _loaded = true;
                return;
            }

            RLog.Msg("[SimpleElevator] All Assets Loaded Successfully");
            _loaded = true;
        }

        public bool IsLoaded()
        {
            return _loaded;
        }

        public string GetImagePath(string imageName, string imageExtention = ".png")
        {
            if (string.IsNullOrEmpty(imageName))
            {
                RLog.Error("[SimpleElevator] GetImagePath: imageName is null or empty");
                return null;
            }
            string dataPath = Application.dataPath;
            // sotfPath Are 1 Level Up From The DataPath
            string sotfPath = Directory.GetParent(dataPath).FullName;
            // Mods
            string modsPath = Path.Combine(sotfPath, "Mods");
            // SimpleElevator
            string modPath = Path.Combine(modsPath, "SimpleElevator");
            // Check If The File Exists
            string assetsPath = Path.Combine(modPath, $"{imageName}{imageExtention}");
            if (!File.Exists(assetsPath))
            {
                RLog.Error($"[SimpleElevator] GetImagePath: {imageName} Not Found");
                return null;
            }
            return assetsPath;
        }
    }
}
