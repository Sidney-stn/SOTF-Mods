using RedLoader;

namespace StructureDamageViewer
{
    internal class Misc
    {
        internal static void Msg(string msg)
        {
            if (Config.StructureDamageViewerLogging.Value)
            {
                RLog.Msg(msg);
            }
        }
    }
}
