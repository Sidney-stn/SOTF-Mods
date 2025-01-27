using RedLoader;
using Sons.Crafting.Structures;
using UnityEngine;

namespace Signs.Mono
{
    [RegisterTypeInIl2Cpp]
    internal class NewSign : MonoBehaviour
    {
        public bool IsPlaceHolder = false;

        private void Start()
        {
            if (IsPlaceHolder) { return; }
            // This Only Gets Called When A New Completed Sign Has Been Created
            Misc.Msg("[NewSign] Start");
            Misc.Msg("[NewSign] Deleting Bolt And ScrewStructure");
            ScrewStructure scewStructure = gameObject.GetComponent<ScrewStructure>();
            if (scewStructure != null) { DestroyImmediate(scewStructure); }
            BoltEntity bolt = gameObject.GetComponent<BoltEntity>();
            if (bolt != null) { DestroyImmediate(bolt); }

            if (gameObject != null)
            {
                Misc.Msg("[NewSign] Adding Components");
                Mono.SignController signController = gameObject.AddComponent<Mono.SignController>();
                Mono.DestroyOnC destroyOnC = gameObject.AddComponent<Mono.DestroyOnC>();

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
            else { Misc.Msg("[OnStructureCompleted] signChild == null"); }
        }
    }
}
