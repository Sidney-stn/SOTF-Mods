using RedLoader;
using Sons.Crafting.Structures;
using UnityEngine;

namespace Signs.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class NewSign : MonoBehaviour
    {
        public bool IsPlaceHolder = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Runs On Start Automatically After Script Is Loaded")]
        private void Start()
        {
            if (IsPlaceHolder) { Misc.Msg("[NewSign] Returned, no signcontroller added"); return; }
            // This Only Gets Called When A New Completed Sign Has Been Created
            Misc.Msg("[NewSign] Start");
            Misc.Msg("[NewSign] Deleting Bolt And ScrewStructure");
            ScrewStructure scewStructure = gameObject.GetComponent<ScrewStructure>();
            if (scewStructure != null) { DestroyImmediate(scewStructure); Misc.SuperLog("[NewSign] [Start] ScrewStructure Deleted"); } else { Misc.SuperLog("[NewSign] [Start] ScrewStructure Not Found For Deletion!"); }
            BoltEntity bolt = gameObject.GetComponent<BoltEntity>();
            if (bolt != null) {
                //if ( Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                //{
                //    bolt.Entity.Detach();
                //}
                //DestroyImmediate(bolt); 
                //Misc.SuperLog("[NewSign] [Start] BoltEntity Deleted");

                // Test For Normal And Dedicated Server
                if (BoltNetwork.isRunning)
                {
                    if (BoltNetwork.isServer)
                    {
                        bolt.Entity.Detach();
                        DestroyImmediate(bolt);
                        Misc.SuperLog("[NewSign] [Start] BoltEntity Deleted");
                    }
                    else
                    {
                        Misc.SuperLog("[NewSign] [Start] Not Deleting BoltEntity On Client");
                    }
                }
                else
                {
                    DestroyImmediate(bolt);
                    Misc.SuperLog("[NewSign] [Start] BoltEntity Deleted");
                }
                } else { Misc.SuperLog("[NewSign] [Start] BoltEntity Not Found For Deletion!"); }

            if (gameObject != null)
            {
                Misc.Msg("[NewSign] Adding Components");
                Mono.SignController signController = gameObject.AddComponent<Mono.SignController>();
                Misc.SuperLog("[NewSign] [Start] SignController Added");
                Mono.DestroyOnC destroyOnC = gameObject.AddComponent<Mono.DestroyOnC>();
                Misc.SuperLog("[NewSign] [Start] DestroyOnC Added");

                signController.SetLineText(1, $"Press {Config.ToggleMenuKey.Value.ToUpper()}");
                signController.SetLineText(2, "To Edit");
                signController.SetLineText(3, "Sign");
                signController.SetLineText(4, "");

                string uniqueId = Guid.NewGuid().ToString();

                signController.UniqueId = uniqueId;

                Saving.Load.ModdedSigns.Add(gameObject);
                Prefab.SignPrefab.spawnedSigns.Add(signController.UniqueId, gameObject);

                if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                {
                    Misc.Msg("[NewSign] Spawning On Multiplayer");
                    (ulong steamId, string stringSteamId) = Misc.MySteamId();
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnSingeSign
                    {
                        Vector3Position = Network.CustomSerializable.Vector3ToString((Vector3)signController.GetPos()),
                        QuaternionRotation = Network.CustomSerializable.QuaternionToString((Quaternion)signController.GetCurrentRotation()),
                        UniqueId = uniqueId,
                        Sender = stringSteamId,
                        SenderName = Misc.GetLocalPlayerUsername(),
                        Line1Text = signController.GetLineText(1),
                        Line2Text = signController.GetLineText(2),
                        Line3Text = signController.GetLineText(3),
                        Line4Text = signController.GetLineText(4),
                        ToSteamId = "None"
                    });
                }
            }
            else { Misc.Msg("[OnStructureCompleted] signChild == null"); Misc.SuperLog("[NewSign] [Start] GameObject That NewSign Is Attatched To Is Null!!!!"); }

            Misc.SuperLog("[NewSign] [Start] Destroying NewSign Component");
            Destroy(this);
        }
    }
}
