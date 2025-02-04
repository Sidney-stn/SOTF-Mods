using RedLoader;
using Sons.Crafting.Structures;
using SonsSdk;
using UnityEngine;

namespace Banking.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class NewATM : MonoBehaviour
    {
        public bool IsPlaceHolder = false;

        private void Start()
        {
            if (IsPlaceHolder) { Misc.Msg("[NewATM] Returned, no atm controllers added"); return; }
            if (Misc.hostMode != Misc.SimpleSaveGameType.Multiplayer && Misc.hostMode != Misc.SimpleSaveGameType.MultiplayerClient)
            {
                Misc.Msg("[NewATM] Not In Multiplayer!");
                SonsTools.ShowMessage("Not In Multiplayer!", 5);
                DestroyImmediate(gameObject);
                return;
            }

            // This Only Gets Called When A New Completed Sign Has Been Created
            Misc.Msg("[NewATM] Start");
            Misc.Msg("[NewATM] Deleting Bolt And ScrewStructure");
            ScrewStructure scewStructure = gameObject.GetComponent<ScrewStructure>();
            if (scewStructure != null) { 
                DestroyImmediate(scewStructure);
                //Misc.SuperLog("[NewATM] [Start] ScrewStructure Deleted");
            } 
            else
            { 
                //Misc.SuperLog("[NewATM] [Start] ScrewStructure Not Found For Deletion!");
            }
            BoltEntity bolt = gameObject.GetComponent<BoltEntity>();
            if (bolt != null) { 
                if ( Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                {
                    bolt.Entity.Detach();
                }
                DestroyImmediate(bolt); 
                //Misc.SuperLog("[NewATM] [Start] BoltEntity Deleted");
            } else { 
                //Misc.SuperLog("[NewATM] [Start] BoltEntity Not Found For Deletion!");
            }

            if (gameObject != null)
            {
                Misc.Msg("[NewATM] Adding Components");

                Mono.ATMController atmController = gameObject.AddComponent<Mono.ATMController>();
                Mono.DestroyOnC destroyOnC = gameObject.AddComponent<Mono.DestroyOnC>();

                string uniqueId = Guid.NewGuid().ToString();

                atmController.UniqueId = uniqueId;

                Saving.Load.ModdedAtms.Add(gameObject);
                Prefab.ActiveATM.spawnedAtms.Add(atmController.UniqueId, gameObject);

                
                Misc.Msg("[NewATM] Spawning On Multiplayer");
                (ulong steamId, string stringSteamId) = Misc.MySteamId();
                SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnATM
                {
                    Vector3Position = Network.CustomSerializable.Vector3ToString(atmController.GetPos()),
                    QuaternionRotation = Network.CustomSerializable.QuaternionToString(atmController.GetCurrentRotation()),
                    UniqueId = uniqueId,
                    Sender = Misc.MySteamId().Item2,
                    SenderName = Misc.GetLocalPlayerUsername(),
                    ToSteamId = "None"
                });
            }
            else { 
                Misc.Msg("[OnStructureCompleted] signChild == null"); 
                //Misc.SuperLog("[NewATM] [Start] GameObject That NewATM Is Attatched To Is Null!!!!");
            }

            //Misc.SuperLog("[NewATM] [Start] Destroying NewATM Component");
            Destroy(this);
        }
    }
}
