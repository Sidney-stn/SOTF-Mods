using RedLoader;
using SonsSdk;

namespace HotKeyCommands;

public static class Config
{
    public static ConfigCategory HotKeyCommandssCategory { get; private set; }
    public static ConfigCategory Command1 { get; private set; }
    public static ConfigCategory Command2 { get; private set; }
    public static ConfigCategory Command3 { get; private set; }
    public static ConfigCategory Command4 { get; private set; }
    public static ConfigCategory Command5 { get; private set; }
    public static ConfigCategory Command6 { get; private set; }
    public static ConfigCategory Command7 { get; private set; }
    public static ConfigCategory Command8 { get; private set; }
    public static ConfigCategory Command9 { get; private set; }
    public static ConfigCategory Command10 { get; private set; }
    public static KeybindConfigEntry Command1Key { get; private set; }
    public static KeybindConfigEntry Command2Key { get; private set; }
    public static KeybindConfigEntry Command3Key { get; private set; }
    public static KeybindConfigEntry Command4Key { get; private set; }
    public static KeybindConfigEntry Command5Key { get; private set; }
    public static KeybindConfigEntry Command6Key { get; private set; }
    public static KeybindConfigEntry Command7Key { get; private set; }
    public static KeybindConfigEntry Command8Key { get; private set; }
    public static KeybindConfigEntry Command9Key { get; private set; }
    public static KeybindConfigEntry Command10Key { get; private set; }
    public static ConfigEntry<bool> Command1Active { get; private set; }
    public static ConfigEntry<bool> Command2Active { get; private set; }
    public static ConfigEntry<bool> Command3Active { get; private set; }
    public static ConfigEntry<bool> Command4Active { get; private set; }
    public static ConfigEntry<bool> Command5Active { get; private set; }
    public static ConfigEntry<bool> Command6Active { get; private set; }
    public static ConfigEntry<bool> Command7Active { get; private set; }
    public static ConfigEntry<bool> Command8Active { get; private set; }
    public static ConfigEntry<bool> Command9Active { get; private set; }
    public static ConfigEntry<bool> Command10Active { get; private set; }
    public static ConfigEntry<string> Command1Input { get; private set; }
    public static ConfigEntry<string> Command2Input { get; private set; }
    public static ConfigEntry<string> Command3Input { get; private set; }
    public static ConfigEntry<string> Command4Input { get; private set; }
    public static ConfigEntry<string> Command5Input { get; private set; }
    public static ConfigEntry<string> Command6Input { get; private set; }
    public static ConfigEntry<string> Command7Input { get; private set; }
    public static ConfigEntry<string> Command8Input { get; private set; }
    public static ConfigEntry<string> Command9Input { get; private set; }
    public static ConfigEntry<string> Command10Input { get; private set; }

    public static void Init()
    {
        HotKeyCommandssCategory = ConfigSystem.CreateCategory("hotKeyCommandssCategory", "HotKeyCommandssCategory");
        Command1 = ConfigSystem.CreateCategory("command1", "Command1", true);
        Command2 = ConfigSystem.CreateCategory("command2", "Command2", true);
        Command3 = ConfigSystem.CreateCategory("command3", "Command3", true);
        Command4 = ConfigSystem.CreateCategory("command4", "Command4", true);
        Command5 = ConfigSystem.CreateCategory("command5", "Command5", true);
        Command6 = ConfigSystem.CreateCategory("command6", "Command6", true);
        Command7 = ConfigSystem.CreateCategory("command7", "Command7", true);
        Command8 = ConfigSystem.CreateCategory("command8", "Command8", true);
        Command9 = ConfigSystem.CreateCategory("command9", "Command9", true);
        Command10 = ConfigSystem.CreateCategory("command10", "Command10", true);

        Command1Key = Command1.CreateKeybindEntry(
                  "command_1",
                  "numpad0",
                  "Command1",
                  "Command 1");
        Command1Key.Notify(() =>
        {
            CommandActions.Command1();
        });

        Command2Key = Command2.CreateKeybindEntry(
            "command_2",
            "numpad1",
            "Command2",
            "Command 2");
        Command2Key.Notify(() =>
        {
            CommandActions.Command2();
        });

        Command3Key = Command3.CreateKeybindEntry(
            "command_3",
            "numpad2",
            "Command3",
            "Command 3");
        Command3Key.Notify(() =>
        {
            CommandActions.Command3();
        });

        Command4Key = Command4.CreateKeybindEntry(
            "command_4",
            "numpad3",
            "Command4",
            "Command 4");
        Command4Key.Notify(() =>
        {
            CommandActions.Command4();
        });

        Command5Key = Command5.CreateKeybindEntry(
            "command_5",
            "numpad4",
            "Command5",
            "Command 5");
        Command5Key.Notify(() =>
        {
            CommandActions.Command5();
        });

        Command6Key = Command6.CreateKeybindEntry(
            "command_6",
            "numpad5",
            "Command6",
            "Command 6");
        Command6Key.Notify(() =>
        {
            CommandActions.Command6();
        });

        Command7Key = Command7.CreateKeybindEntry(
            "command_7",
            "numpad6",
            "Command7",
            "Command 7");
        Command7Key.Notify(() =>
        {
            CommandActions.Command7();
        });

        Command8Key = Command8.CreateKeybindEntry(
            "command_8",
            "numpad7",
            "Command8",
            "Command 8");
        Command8Key.Notify(() =>
        {
            CommandActions.Command8();
        });

        Command9Key = Command9.CreateKeybindEntry(
            "command_9",
            "numpad8",
            "Command9",
            "Command 9");
        Command9Key.Notify(() =>
        {
            CommandActions.Command9();
        });

        Command10Key = Command10.CreateKeybindEntry(
            "command_10",
            "numpad9",
            "Command10",
            "Command 10");
        Command10Key.Notify(() =>
        {
            CommandActions.Command10();
        });

        Command1Active = Command1.CreateEntry(
            "command_1_active",
            false,
            "Command 1 Active",
            "Command 1 Active");

        Command2Active = Command2.CreateEntry(
            "command_2_active",
            false,
            "Command 2 Active",
            "Command 2 Active");

        Command3Active = Command3.CreateEntry(
            "command_3_active",
            false,
            "Command 3 Active",
            "Command 3 Active");

        Command4Active = Command4.CreateEntry(
            "command_4_active",
            false,
            "Command 4 Active",
            "Command 4 Active");

        Command5Active = Command5.CreateEntry(
            "command_5_active",
            false,
            "Command 5 Active",
            "Command 5 Active");

        Command6Active = Command6.CreateEntry(
            "command_6_active",
            false,
            "Command 6 Active",
            "Command 6 Active");

        Command7Active = Command7.CreateEntry(
            "command_7_active",
            false,
            "Command 7 Active",
            "Command 7 Active");

        Command8Active = Command8.CreateEntry(
            "command_8_active",
            false,
            "Command 8 Active",
            "Command 8 Active");

        Command9Active = Command9.CreateEntry(
            "command_9_active",
            false,
            "Command 9 Active",
            "Command 9 Active");

        Command10Active = Command10.CreateEntry(
            "command_10_active",
            false,
            "Command 10 Active",
            "Command 10 Active");

        Command1Input = Command1.CreateEntry(
            "command_1_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command2Input = Command2.CreateEntry(
            "command_2_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command3Input = Command3.CreateEntry(
            "command_3_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command4Input = Command4.CreateEntry(
            "command_4_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command5Input = Command5.CreateEntry(
            "command_5_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command6Input = Command6.CreateEntry(
            "command_6_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command7Input = Command7.CreateEntry(
            "command_7_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command8Input = Command8.CreateEntry(
            "command_8_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command9Input = Command9.CreateEntry(
            "command_9_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");

        Command10Input = Command10.CreateEntry(
            "command_10_input",
            "",
            "Enter Console Command Here",
            "Enter Console Command Here with or without args");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}