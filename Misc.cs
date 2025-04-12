using RedLoader;


namespace SimpleElevator
{
    internal class Misc
    {
        internal static void Msg(string msg, bool network = false)
        {
            if (!Config.LoggingToConsole.Value) { return; }
            RLog.Msg($"[SimpleElevator] {msg}");
        }
    }
}
