

using Endnight.Extensions;
using TheForest;
using TheForest.Utils;
using static PlayerArmBlood;

namespace HotKeyCommands
{
    internal class CommandActions
    {
        private static void RunConsoleCommand(string command)
        {
            if (!LocalPlayer.IsInWorld || HotKeyCommands.debugConsole == null || command.IsNullOrWhitespace()) { return; }
            HotKeyCommands.debugConsole.SendCommand(command);
            //string cmd;
            //string args;
            //int firstSpaceIndex = command.IndexOf(' ');
            //if (firstSpaceIndex == -1)
            //{
            //    // No space found, the entire command is a single word
            //    cmd = command;
            //    args = string.Empty;
            //}
            //else
            //{
            //    // Split the command into cmd and args
            //    cmd = command.Substring(0, firstSpaceIndex);
            //    args = command.Substring(firstSpaceIndex + 1).Trim();
            //}
            //if (!cmd.IsNullOrWhitespace() && args == string.Empty)
            //{
            //    HotKeyCommands.debugConsole.SendCommand(cmd);
            //} else if (!cmd.IsNullOrWhitespace() && !args.IsNullOrWhitespace())
            //{
            //    DebugConsole.TryRunDynamicCommand(command, args, HotKeyCommands.debugConsole);
            //}

        }
        internal static void Command1()
        {
            if (!Config.Command1Active.Value) { return; }
            RunConsoleCommand(Config.Command1Input.Value);
        }

        internal static void Command2()
        {
            if (!Config.Command2Active.Value) { return; }
            RunConsoleCommand(Config.Command2Input.Value);
        }

        internal static void Command3()
        {
            if (!Config.Command3Active.Value) { return; }
            RunConsoleCommand(Config.Command3Input.Value);
        }

        internal static void Command4()
        {
            if (!Config.Command4Active.Value) { return; }
            RunConsoleCommand(Config.Command4Input.Value);
        }

        internal static void Command5()
        {
            if (!Config.Command5Active.Value) { return; }
            RunConsoleCommand(Config.Command5Input.Value);
        }

        internal static void Command6()
        {
            if (!Config.Command6Active.Value) { return; }
            RunConsoleCommand(Config.Command6Input.Value);
        }

        internal static void Command7()
        {
            if (!Config.Command7Active.Value) { return; }
            RunConsoleCommand(Config.Command7Input.Value);
        }

        internal static void Command8()
        {
            if (!Config.Command8Active.Value) { return; }
            RunConsoleCommand(Config.Command8Input.Value);
        }

        internal static void Command9()
        {
            if (!Config.Command9Active.Value) { return; }
            RunConsoleCommand(Config.Command9Input.Value);
        }

        internal static void Command10()
        {
            if (!Config.Command10Active.Value) { return; }
            RunConsoleCommand(Config.Command10Input.Value);
        }
    }
}
