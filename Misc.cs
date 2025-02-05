using RedLoader;

namespace Shops
{
    internal class Misc
    {
        internal static void Msg(string msg)
        {
            if (Config.DebugLoggingIngameShops.Value)
            {
                RLog.Msg(msg);
            }

        }

        internal static void NetLog(string msg)
        {
            if (Config.NetworkDebugIngameShops.Value)
            {
                Msg($"[NetLog] {msg}");
            }
        }
        internal static void SuperLog(string msg)
        {
            if (Config.ExtremeDebugLogging.Value)
            {
                Msg($"[SuperLog] {msg}");
            }
        }
    }
}
