using Bolt;
using Sons.Gui;
using SonsSdk;
using SonsSdk.Attributes;
using UnityEngine;
using TheForest.Utils;
using SUI;
using Sons.World;
using static BroadcastMessage.DiscordBot;

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
    }

    protected override void OnGameStart()
    {
        // This is called once the player spawns in the world and gains control.
        if (Config.EnableFiveSecondPrinting.Value) { BroadcastInfo.GenerateObjectWithMono(); }

        // Start Discord Bot
        GameObject discord_server_go = new GameObject();
        discord_server_go.AddComponent<DiscordBot.DiscordBotManager>();

        DiscordBotManager discordBotManager = discord_server_go.GetComponent<DiscordBotManager>();

        ulong exampleChannelId = 1116004344370827466; // Replace with your actual channel ID
        discordBotManager.SendMessageToChannel(exampleChannelId, "Hello from Unity!");

    }

    private void Quitting()
    {

    }
    protected void OnUpdate()
    {

    }
}