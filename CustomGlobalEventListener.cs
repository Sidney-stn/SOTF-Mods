using Bolt;
using RedLoader;

namespace SimpleNetworkEvents
{
    [RegisterTypeInIl2Cpp]
    internal class CustomGlobalEventListener : GlobalEventListener
    {
        public void Start()
        {
            BoltNetwork.AddGlobalEventListener(this);
            Misc.Msg("MacheGlobalEventListener Added Into Game (START)");
        }

        public override void OnEvent(debugCommand evnt)
        {
            EventDispatcher.OnReceiveEvent(evnt.input, evnt.input2);
            Misc.Msg("MacheGlobalEventListener OnReciveEvent (OnEvent)");
        }

        public void RemoveGlobalEventListener()
        {
            BoltNetwork.RemoveGlobalEventListener(this);
        }

        public void CleanUpAndDestoy()
        {
            Destroy(this);
        }
    }
}
