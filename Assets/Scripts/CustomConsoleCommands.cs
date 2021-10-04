using UnityEngine;
using CommandTerminal;

public static class CustomConsoleCommands
{
    [RegisterCommand()]
    public static void CommandPing(CommandArg[] args)
    {
        Terminal.Log("pong");
    }

    [RegisterCommand(MinArgCount = 1)]
    public static void CommandEcho(CommandArg[] args)
    {
        if (Terminal.IssuedError) return;

        Terminal.Log(CommandTerminal.BuiltinCommands.JoinArguments(args));
    }
}
