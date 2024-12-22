using Signs.Mono;
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
                    signController.SetLineText(1, "Press E");
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

                return signCopy;
            }
            return null;
        }

        public static GameObject spawnSignMultiplayer(Vector3 position, Quaternion rotation, string unqueId = null)
        {
            if (signWithComps != null)
            {
                Misc.Msg("Spawning MultiPlayer Player Sign");
                GameObject signCopy = GameObject.Instantiate(signWithComps); // Creating Sign
                if (signCopy == null) { Misc.Msg("[SpawnSignPrefab] signCopy == null!"); }

                signCopy.transform.position = position;  // Set New Positiom
                signCopy.transform.rotation = rotation;  // Set New Rotation

                return signCopy;
            }
            return null;
        }


    }
}
