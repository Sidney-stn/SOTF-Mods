using RedLoader;
using SonsSdk;
using UnityEngine;

namespace Signs.Saving
{
    internal class Load
    {
        internal static Queue<Saving.Manager.SignsManager> deferredLoadQueue = new Queue<Saving.Manager.SignsManager>();

        internal static void ProcessLoadData(Saving.Manager.SignsManager obj)
        {
            if (BoltNetwork.isClient)
            {
                Misc.Msg("[Loading] Skipped Loading Signs On Multiplayer Client");
                return;
            }
            // Signs Prefab
            Misc.Msg($"[Loading] Signs From Save: {obj.Signs.Count.ToString()}");
            foreach (var signsData in obj.Signs)
            {
                if (signsData.Position == Vector3.zero)
                {
                    Misc.Msg("[Loading] Skipping Sign Load, Position is Zero");
                    continue;
                }
                Misc.Msg("[Loading] Creating New Signs");
                if (Misc.hostMode == Misc.SimpleSaveGameType.SinglePlayer)
                {
                    GameObject sign = GameObject.Instantiate(Structure.Setup.signStructure, signsData.Position, signsData.Rotation);
                    if (sign == null)
                    {
                        RLog.Error("[Load] GameObject Sign Equals Null!");
                    }
                    var signController = sign.GetComponent<Mono.SignController>();
                    if (signController == null)
                    {
                        RLog.Error("[Load] Sign Controller Does Not Exsist When It Should");
                        SonsTools.ShowMessage("Error Loding Sign Text Data, please report this to the mod author or try agian", 5);
                        return;
                    }
                    signController.SetAllText(signsData.Line1Text, signsData.Line2Text, signsData.Line3Text, signsData.Line4Text, false);
                }
                else if (Misc.hostMode == Misc.SimpleSaveGameType.Multiplayer || Misc.hostMode == Misc.SimpleSaveGameType.MultiplayerClient)
                {
                    try
                    {
                        GameObject sign = BoltNetwork.Instantiate(Structure.Setup.signStructure, signsData.Position, signsData.Rotation);
                        if (sign == null)
                        {
                            RLog.Error("[Load] GameObject Sign Equals Null!");
                        }
                        var signController = sign.GetComponent<Mono.SignController>();
                        if (signController == null)
                        {
                            RLog.Error("[Load] Sign Controller Does Not Exsist When It Should");
                            SonsTools.ShowMessage("Error Loding Sign Text Data, please report this to the mod author or try agian", 5);
                            return;
                        }
                        signController.SetAllText(signsData.Line1Text, signsData.Line2Text, signsData.Line3Text, signsData.Line4Text, false);
                    } catch (Exception ex)
                    {
                        RLog.Error($"[Load] Something went wrong when BoltNetwork.Instantiate on new sign! Error: {ex}");
                    }
                    
                }
            }
        }
    }
}
