using SonsSdk;
using UnityEngine;


namespace WirelessSignals.Sound
{
    internal class Lever
    {
        internal static bool isLoaded = false;
        public static void Register()
        {
            if (isLoaded)
            {
                return;
            }
            
            Misc.Msg("Registering Lever Sound");
            string dataPath = Application.dataPath;

            // sotfPath Are 1 Level Up From The DataPath
            string sotfPath = Directory.GetParent(dataPath).FullName;

            // Mods Path
            string modsPath = Path.Combine(sotfPath, "Mods");

            // WirelessSignals Path
            string wirelessSignalsPath = Path.Combine(modsPath, "WirelessSignals");

            // Sound Path MP3
            string soundPath = Path.Combine(wirelessSignalsPath, "LeverSound.wav");
            Misc.Msg($"Sound Path: {soundPath}");

            SoundTools.RegisterSound("LeverSound", soundPath, true);

            isLoaded = true;
            
        }
    }
}
