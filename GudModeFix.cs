using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace gudModeFix;
public class HelloWorldPlugin : BasePlugin
{
    public override string ModuleName => "GudMode Fix";
    public override string ModuleVersion => "0.1.1";
    private bool _is_round_started = false;
    private List<CCSPlayerController> _players = new();

    public override void Load(bool hotReload) {
        Log.console(ConsoleColor.Green, $"Loading '{ModuleName}' version {ModuleVersion}!");
    }

    public override void Unload(bool hotReload) {
        Log.console(ConsoleColor.Green, $"Unloading '{ModuleName}' version {ModuleVersion}.");
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info) {
        _is_round_started = true;

        _players = Utilities.GetPlayers();

        Log.console(ConsoleColor.Green, $"Round started with {@event.Timelimit}, checking {_players.Count} players.");
        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info) {
        _is_round_started = false;

        _players = new();

        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult OnPlayerChangeTeam(EventPlayerTeam @event, GameEventInfo info) {
        // If round is not started we dont care for team changes
        if (!_is_round_started) 
            return HookResult.Continue;

        // Filter out new players, players goint to spectator and players that are not changing teams
        /**
         * 0 - Unassigned
         * 1 - Spectator
         * 2 - Terror
         * 3 - Counter-Terror
        */
        if (
            @event.Team == 0 ||
            @event.Team == 1 ||
            @event.Oldteam == 0 ||
            @event.Team == @event.Oldteam
            ) 
            return HookResult.Continue;

        CCSPlayerController player = @event.Userid;

        // Check if player was when the round started
        if(_players.Find(List => List == player) == null)
            return HookResult.Continue;

        // Filter out invalid players
        if (
            player is null || 
            !player.IsValid || 
            !player.PlayerPawn.IsValid
            )
            return HookResult.Continue;

        Log.console(ConsoleColor.Yellow, $"Player {@event.Userid.PlayerName} tried to change teams from {@event.Oldteam} to {@event.Team}, killing to prevent godmode glich.");

        /*
         * Atempt to kill the player, but didnt stop the godmode glitch as the player seems to be in a stare where he is not alive but spawmed
            CCSPlayerPawn pawn = player.PlayerPawn.Value;
            pawn.CommitSuicide(true, true);
        */
         
        // Setting this as handled will prevent the player from changing teams
        return HookResult.Handled;
    }
}

public static class Log
{
    private static string ModulePrefix = "GudMode Fix";

    public static void consoleLog(string message)
    {
        Console.WriteLine($"[{ModulePrefix}] " + message);
    }

    public static void console(ConsoleColor color, string message)
    {
        Console.ForegroundColor = color;
        consoleLog(message); 
        Console.ResetColor();
    }

    public static void chat(string message) {
        Server.PrintToChatAll(message.chatColorer());
    }

    private static string chatColorer(this string message) {
        message = message.Replace("{DEFAULT}", $"{ChatColors.Default}");
        message = message.Replace("{WHITE}", $"{ChatColors.White}");
        message = message.Replace("{DARKRED}", $"{ChatColors.Darkred}");
        message = message.Replace("{GREEN}", $"{ChatColors.Green}");
        message = message.Replace("{LIGHTYELLOW}", $"{ChatColors.LightYellow}");
        message = message.Replace("{LIGHTBLUE}", $"{ChatColors.LightBlue}");
        message = message.Replace("{OLIVE}", $"{ChatColors.Olive}");
        message = message.Replace("{LIME}", $"{ChatColors.Lime}");
        message = message.Replace("{RED}", $"{ChatColors.Red}");
        message = message.Replace("{PURPLE}", $"{ChatColors.Purple}");
        message = message.Replace("{GREY}", $"{ChatColors.Grey}");
        message = message.Replace("{YELLOW}", $"{ChatColors.Yellow}");
        message = message.Replace("{GOLD}", $"{ChatColors.Gold}");
        message = message.Replace("{SILVER}", $"{ChatColors.Silver}");
        message = message.Replace("{BLUE}", $"{ChatColors.Blue}");
        message = message.Replace("{DARKBLUE}", $"{ChatColors.DarkBlue}");
        message = message.Replace("{BLUEGREY}", $"{ChatColors.BlueGrey}");
        message = message.Replace("{MAGENTA}", $"{ChatColors.Magenta}");
        message = message.Replace("{LIGHTRED}", $"{ChatColors.LightRed}");

        return message;
    }
}