using RedLoader;

namespace SimpleNetworkEvents
{
    internal class Misc
    {
        internal static void Msg(string msg)
        {
            //if (Config.DebugLoggingBoltPrefabTesting.Value != true) { return; }
            RLog.Msg(msg);
        }
    }
}
