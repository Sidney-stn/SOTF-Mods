using RedLoader;
using SonsSdk;
using UnityEngine;
using System.IO;

namespace StoneGate
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
        public GameObject StoneGateTool { get; private set; }
        public GameObject StoneGateToolUI { get; private set; }
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
            // StoneGate
            string modPath = Path.Combine(modsPath, "StoneGate");
            // Assets
            string assetsPath = Path.Combine(modPath, "stonegate");
            // Check If The File Exists
            if (!File.Exists(assetsPath))
            {
                RLog.Error("[StoneGate] Assets File Not Found");
                return;
            }
            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetsPath);
            if (assetBundle == null)
            {
                RLog.Error("[StoneGate] AssetBundle Not Found");
                return;
            }

            // Load the GameObject
            StoneGateTool = assetBundle.LoadAsset<GameObject>("StoneGateTool");
            if (StoneGateTool == null)
            {
                RLog.Error("[StoneGate] StoneGateTool Asset Not Found");
                return;
            }
            StoneGateTool.DontDestroyOnLoad().HideAndDontSave();
            Tools.MoveScene.MoveToScene(StoneGateTool);


            // Set Rigidbody To Kinematic True
            StoneGateTool.GetComponent<Rigidbody>().isKinematic = true;
            StoneGateTool.GetComponent<Rigidbody>().useGravity = false;

            // Destroy MeshCollider
            Transform stick = StoneGateTool.transform.FindDeepChild("Stick (392)");
            List<Transform> transforms = stick.GetChildren();
            foreach (Transform transform in transforms)
            {
                if (transform.gameObject.TryGetComponent(out MeshCollider _))
                {
                    GameObject.Destroy(transform.gameObject.GetComponent<MeshCollider>());
                    Misc.Msg($"[StoneGate] Destroyed MeshCollider on {transform.gameObject.name}");
                }
            }

            // Find BoxCollider
            BoxCollider boxCollider = StoneGateTool.GetComponentInChildren<BoxCollider>();
            boxCollider.isTrigger = true;

            // Destory Capsule Colliders
            List<CapsuleCollider> capsuleColliders = StoneGateTool.GetComponentsInChildren<CapsuleCollider>().ToList();
            foreach (CapsuleCollider capsuleCollider in capsuleColliders)
            {
                GameObject.Destroy(capsuleCollider);
            }

            StoneGateToolUI = assetBundle.LoadAsset<GameObject>("StoneGateToolUI");
            if (StoneGateToolUI == null)
            {
                RLog.Error("[StoneGate] StoneGateToolUI Asset Not Found");
                return;
            }
            StoneGateToolUI.HideAndDontSave().DontDestroyOnLoad();
            StoneGateToolUI.SetActive(false);  // Hide The UI By Default


            // Unload the bundle, but keep loaded assets
            assetBundle.Unload(false);

            RLog.Msg("[StoneGate] All Assets Loaded Successfully");
            _loaded = true;
        }

        public bool IsLoaded()
        {
            return _loaded;
        }

        public string GetStoneGateToolPath()
        {
            string dataPath = Application.dataPath;
            // sotfPath Are 1 Level Up From The DataPath
            string sotfPath = Directory.GetParent(dataPath).FullName;
            // Mods
            string modsPath = Path.Combine(sotfPath, "Mods");
            // StoneGate
            string modPath = Path.Combine(modsPath, "StoneGate");
            // StoneGateIcon
            string assetsPath = Path.Combine(modPath, "StoneGateIcon.png");
            return assetsPath;
        }
        public string GetOpenCloseIconPath()
        {
            string dataPath = Application.dataPath;
            // sotfPath Are 1 Level Up From The DataPath
            string sotfPath = Directory.GetParent(dataPath).FullName;
            // Mods
            string modsPath = Path.Combine(sotfPath, "Mods");
            // StoneGate
            string modPath = Path.Combine(modsPath, "StoneGate");
            // StoneGateIcon
            string assetsPath = Path.Combine(modPath, "OpenCloseIcon.png");
            return assetsPath;
        }
    }
}