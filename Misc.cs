using RedLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadcastMessage
{
    internal class Misc
    {
        internal static void Msg(string msg)
        {
            if (!Config.EnableLogging.Value) { return; }
            RLog.Msg(msg);
        }

        internal static void ErrorMsg(string msg)
        {
            RLog.Msg(msg);
        }
    }
}
