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
    }
}
