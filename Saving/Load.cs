using RedLoader;
using SonsSdk;
using UnityEngine;

namespace StoneGate.Saving
{
    internal class Load
    {
        internal static Queue<Saving.Manager.GatesManager> deferredLoadQueue = new Queue<Saving.Manager.GatesManager>();


        /// <summary>
        /// Process the deferred load queue, this is used to load the gates after the world has been loaded
        /// </summary>
        /// <param name="obj"></param>
        internal static void ProcessLoadData(Saving.Manager.GatesManager obj)
        {
            if (BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                if (Testing.Settings.logSavingSystem)
                    Misc.Msg("[Loading] Skipped Loading StoneGates On Multiplayer Client");
                return;
            }
            if (Testing.Settings.logSavingSystem)
            {
                // Check if the gates data is null
                if (obj == null)
                {
                    Misc.Msg("[Loading] GatesData IS NULL");
                    return;
                }
                Misc.Msg($"[Loading] Gates From Save: {obj.Gates.Count.ToString()}");
            }
                
            foreach (var gatesData in obj.Gates)
            {
                if (Testing.Settings.logSavingSystem)
                    Misc.Msg("[Loading] Creating New Gates");

                if (gatesData == null)
                {
                    Misc.Msg("[Loading] GatesData IS NULL");
                    continue;
                }
                Tools.Gates.LoadIndivudalSaveData(gatesData);
            }
        }
    }
}
