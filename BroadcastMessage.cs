using SonsSdk;


namespace BroadcastMessage;

public class BroadcastMessage : SonsMod
{
    public BroadcastMessage()
    {

        // Uncomment any of these if you need a method to run on a specific update loop.
        //OnUpdateCallback = OnUpdate;
        //OnLateUpdateCallback = MyLateUpdateMethod;
        //OnFixedUpdateCallback = MyFixedUpdateMethod;
        //OnGUICallback = MyGUIMethod;

        // Uncomment this to automatically apply harmony patches in your assembly.
        //HarmonyPatchAll = true;
    }

    protected override void OnInitializeMod()
    {
        // Do your early mod initialization which doesn't involve game or sdk references here
        Config.Init();
    }

    protected override void OnSdkInitialized()
    {
        // Do your mod initialization which involves game or sdk references here
        // This is for stuff like UI creation, event registration etc.
        BroadcastMessageUi.Create();

        // Add in-game settings ui for your mod.
        // SettingsRegistry.CreateSettings(this, null, typeof(Config));
        Misc.Msg($"DLLPath: {DiscordBotManager.dllPath}, Directory: {DiscordBotManager.directory}, FileDirectory: {DiscordBotManager.fileDir}");
        // [BroadcastMessage] DLLPath: C:\Program Files (x86)\Steam\steamapps\common\Sons Of The Forest\Mods\BroadcastMessage.dll, Directory: C:\Program Files (x86)\Steam\steamapps\common\Sons Of The Forest\Mods, FileDirectory: C:\Program Files (x86)\Steam\steamapps\common\Sons Of The Forest\Mods\BroadcastMessage

        // Create the DiscordBotManager instance
        DiscordBotManager botManager = new DiscordBotManager();

        // Start the Discord bot
        botManager.StartBot();

        // Example: Send a command to the bot
        botManager.SendCommand("Hello from the game!");
    }

    protected override void OnGameStart()
    {
        

    }


    private void Quitting()
    {

    }
    protected void OnUpdate()
    {

    }
}