using Signs.Mono;
using System.Collections.Generic;
using UnityEngine;

namespace Signs.Prefab
{
    public class SignPrefab
    {
        public static GameObject signWithComps;
        public static Dictionary<string, GameObject> spawnedSigns = new Dictionary<string, GameObject>();

        public static void SetupSignPrefab()
        {
            if (signWithComps == null)
            {
                if (Assets.SignObj == null) { Misc.Msg("Cant Setup Sign Prefab, Sign Asset is null!"); return; }
                signWithComps = GameObject.Instantiate(Assets.SignObj);
                Mono.SignController signMono = signWithComps.AddComponent<Mono.SignController>();
                Mono.DestroyOnC destroyOnC = signWithComps.AddComponent<Mono.DestroyOnC>();
            }
        }

        public static GameObject spawnSignSingePlayer(Vector3 position, Quaternion rotation, bool isNew = true, string line1Text = null, string line2Text = null, string line3Text = null, string line4Text = null, string uniqueId = null)
        {
            if (signWithComps != null)
            {
                Misc.Msg("Spawning Single Player Sign");
                GameObject signCopy = GameObject.Instantiate(signWithComps); // Creating Sign
                if (signCopy == null) { Misc.Msg("[SpawnSignPrefab] signCopy == null!"); }

                signCopy.transform.position = position;  // Set New Positiom
                signCopy.transform.rotation = rotation;  // Set New Rotation

                SignController signController = signCopy.GetComponent<SignController>();
                if (isNew) {
                    signController.SetLineText(1, $"Press {Config.ToggleMenuKey.Value.ToUpper()}");
                    signController.SetLineText(2, "To Edit");
                    signController.SetLineText(3, "Sign");
                    signController.SetLineText(4, "");
                } else
                {
                    if (line1Text != null) { signController.SetLineText(1, line1Text); Misc.Msg($"[SignPrefab] [SpawnSignSingePlayer] Set Line1 Text To: {line1Text}"); }
                    if (line2Text != null) { signController.SetLineText(2, line2Text); Misc.Msg($"[SignPrefab] [SpawnSignSingePlayer] Set Line2 Text To: {line2Text}"); }
                    if (line3Text != null) { signController.SetLineText(3, line3Text); Misc.Msg($"[SignPrefab] [SpawnSignSingePlayer] Set Line3 Text To: {line3Text}"); }
                    if (line4Text != null) { signController.SetLineText(4, line4Text); Misc.Msg($"[SignPrefab] [SpawnSignSingePlayer] Set Line4 Text To: {line4Text}"); }
                }
                if (uniqueId != null) { signController.UniqueId = uniqueId; }
                else { signController.UniqueId = Guid.NewGuid().ToString(); }

                Saving.Load.ModdedSigns.Add(signCopy);
                spawnedSigns.Add(signController.UniqueId, signCopy);

                return signCopy;
            }
            return null;
        }

        public static GameObject spawnSignMultiplayer(Vector3 position, Quaternion rotation, string line1Text = null, string line2Text = null, string line3Text = null, string line4Text = null, string uniqueId = null, bool raiseCreateEvent = false)
        {
            if (uniqueId != null)
            {
                if (DoesShopWithUniqueIdExist(uniqueId)) { Misc.Msg($"Shop with Id: {uniqueId}, does already exsist"); return null; } // Check If Shop Already Exists (Prevent Duplicates
            } else { uniqueId = Guid.NewGuid().ToString(); }  // Creating Sign For First Time
            if (signWithComps != null)
            {
                Misc.Msg("Spawning Multiplayer Player Sign");
                GameObject signCopy = GameObject.Instantiate(signWithComps); // Creating Sign
                if (signCopy == null) { Misc.Msg("[SpawnSignPrefab] signCopy == null!"); }

                signCopy.transform.position = position;  // Set New Positiom
                signCopy.transform.rotation = rotation;  // Set New Rotation

                SignController signController = signCopy.GetComponent<SignController>();
                if (line1Text != null) { signController.SetLineText(1, line1Text); Misc.Msg($"[SignPrefab] [SpawnSignMultiplayer] Set Line1 Text To: {line1Text}"); }
                if (line2Text != null) { signController.SetLineText(2, line2Text); Misc.Msg($"[SignPrefab] [SpawnSignMultiplayer] Set Line2 Text To: {line2Text}"); }
                if (line3Text != null) { signController.SetLineText(3, line3Text); Misc.Msg($"[SignPrefab] [SpawnSignMultiplayer] Set Line3 Text To: {line3Text}"); }
                if (line4Text != null) { signController.SetLineText(4, line4Text); Misc.Msg($"[SignPrefab] [SpawnSignMultiplayer] Set Line4 Text To: {line4Text}"); }

                if (line1Text == null) { signController.SetLineText(1, $"Press {Config.ToggleMenuKey.Value.ToUpper()}"); Misc.Msg($"[SignPrefab] [SpawnSignMultiplayer] Set Line1 Text To: Press {Config.ToggleMenuKey.Value.ToUpper()}"); }
                if (line2Text == null) { signController.SetLineText(2, "To Edit"); Misc.Msg($"[SignPrefab] [SpawnSignMultiplayer] Set Line2 Text To: To Edit"); }
                if (line3Text == null) { signController.SetLineText(3, "Sign"); Misc.Msg($"[SignPrefab] [SpawnSignMultiplayer] Set Line3 Text To: Sign"); }
                if (line4Text == null) { signController.SetLineText(4, ""); Misc.Msg($"[SignPrefab] [SpawnSignMultiplayer] Set Line4 Text To: ''"); }

                if (uniqueId != null) { signController.UniqueId = uniqueId; }
                else { Misc.Msg("[SignPrefab] [SpawnSignMultiplayer] Something Went Wrong Getting unique id!"); }

                Saving.Load.ModdedSigns.Add(signCopy);
                spawnedSigns.Add(signController.UniqueId, signCopy);

                if (raiseCreateEvent)
                {
                    (ulong steamId, string stringSteamId) = Misc.MySteamId();
                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.SpawnSingeSign
                    {
                        Vector3Position = Network.CustomSerializable.Vector3ToString((Vector3)position),
                        QuaternionRotation = Network.CustomSerializable.QuaternionToString((Quaternion)rotation),
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

                return signCopy;
            }
            else
            {
                if (Config.NetworkDebugIngameSign.Value)
                {
                    Misc.Msg("[SignPrefab] [SpawnSignMultiplayer] Sign Prefab is null!");
                }
            }
            return null;
        }

        public static GameObject FindShopByUniqueId(string uniqueId)
        {
            if (spawnedSigns.TryGetValue(uniqueId, out GameObject sign))
            {
                return sign;
            }
            else
            {
                Misc.Msg($"Shop with unique ID {uniqueId} not found.");
                return null;
            }
        }

        public static bool DoesShopWithUniqueIdExist(string uniqueId)
        {
            if (spawnedSigns.ContainsKey(uniqueId))
            {
                return true;
            }
            else
            {
                Misc.Msg($"Shop with unique ID {uniqueId} not found.");
                return false;
            }
        }

    }
}
