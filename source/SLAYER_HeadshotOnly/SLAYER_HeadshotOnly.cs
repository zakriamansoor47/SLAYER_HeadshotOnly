using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;


namespace SLAYER_HeadshotOnly;

// Used these to remove compile warnings
#pragma warning disable CS8602

public class ConfigSpecials : BasePluginConfig
{
    [JsonPropertyName("PluginEnabled")] public bool PluginEnabled { get; set; } = true;
    [JsonPropertyName("AlwaysEnableHsOnly")] public bool AlwaysEnableHsOnly { get; set; } = false;
    [JsonPropertyName("PlayerCanUseHsCmd")] public bool PlayerCanUseHsCmd { get; set; } = true;
    [JsonPropertyName("HsOnlyWeapons")] public string HsOnlyWeapons { get; set; } = "";
    [JsonPropertyName("AdminFlagtoForceHsOnly")] public string AdminFlagtoForceHsOnly { get; set; } = "@css/root";
}

public class SLAYER_HeadshotOnly : BasePlugin, IPluginConfig<ConfigSpecials>
{
    public override string ModuleName => "SLAYER_HeadshotOnly";
    public override string ModuleVersion => "1.1";
    public override string ModuleAuthor => "SLAYER";
    public override string ModuleDescription => "Enable/Disable Headshot Only. Allow players to Enable/Disable Headshot Only on themselves";


    public required ConfigSpecials Config {get; set;}

    public bool[] g_Headshot= new bool[64];
    public bool adminHeadshotOnly = false;
    public string[] HsWeapons = new string[50];

    public void OnConfigParsed(ConfigSpecials config)
    {
        Config = config;
    }
    public override void Load(bool hotReload)
    {
        AddCommand("css_hs", "Enabled/Disabled Scope", cmd_hs);
        AddCommand("css_headshot", "Enabled/Disabled Scope", cmd_AdminHsOnly);
        HsWeapons = Config.HsOnlyWeapons.Split(";");
        RegisterEventHandler<EventPlayerHurt>((@event, info) => 
        {
            CCSPlayerController attacker = @event.Attacker;
            if(Config.PluginEnabled && g_Headshot[attacker.Slot] || adminHeadshotOnly || Config.AlwaysEnableHsOnly) // if Plugin is Enabled
            {
                var player = @event.Userid;
                if (!player.IsValid || player== null)
                    return HookResult.Continue;

                if (!attacker.IsValid || player.TeamNum == attacker.TeamNum && !(@event.DmgHealth > 0 || @event.DmgArmor > 0))
                    return HookResult.Continue;

                if(Config.HsOnlyWeapons != "" && HsWeapons != null && HsWeapons.Count() > 0 && attacker.PlayerPawn.Value.WeaponServices!.MyWeapons.Count != 0) // Check, Is Headshot Weapons String is Empty or Not and player have weapons or not
                {
                    var ActiveWeaponName = attacker?.PlayerPawn?.Value.WeaponServices?.ActiveWeapon?.Value.DesignerName; // Get Active Weapon Name
                    if(ActiveWeaponName != null && ActiveWeaponName != "" && !HsWeapons.Contains(ActiveWeaponName)) // Check, Is Headshot Only is Active on Current Weapon or not
                    {
                        return HookResult.Continue;
                    }    
                }
                if(@event.Hitgroup != 1) // if bullet not hitting on Head
                {
                    player.PlayerPawn.Value.Health += @event.DmgHealth; // add the dmg health to Normal health
                    player.PlayerPawn.Value.ArmorValue += @event.DmgArmor; // Update the Armor as well
                }
            }
            return HookResult.Continue;
        }, HookMode.Pre);
    }
    
    private void cmd_hs(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(player == null) // if player is server then return
        {
            commandInfo.ReplyToCommand("[Headshot Only] Cannot use command from RCON");
            return;
        }
        if(Config.PluginEnabled == false)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Darkred}Plugin is Disabled!");
            return;
        }
        if(Config.PlayerCanUseHsCmd == false)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Darkred}Command is Disabled!");
            return;
        }
        if(Config.AlwaysEnableHsOnly)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Darkred}Command is Unavailable Right Now!");
            return;
        }
        if(g_Headshot[player.Slot] == false)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Purple}Now you can {ChatColors.Darkred}kill {ChatColors.Purple}with {ChatColors.Lime}Headshot Only!");
            Server.PrintToChatAll($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Green}{player.PlayerName} {ChatColors.Purple}now {ChatColors.Darkred}can kill with {ChatColors.Lime}Headshot Only!");
            g_Headshot[player.Slot] = true;
        }
        else
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Purple}Now you can {ChatColors.Darkred}kill {ChatColors.Lime}Normally");
            Server.PrintToChatAll($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Olive}{player.PlayerName} {ChatColors.Purple}now can {ChatColors.Darkred}kill {ChatColors.Lime}Normally!");
            g_Headshot[player.Slot] = false;
        }
        
    }
    private void cmd_AdminHsOnly(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(Config.PluginEnabled == false)
        {
            if(player != null)player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Darkred}Plugin is Disabled!");
            else commandInfo.ReplyToCommand("[Headshot Only] Plugin is Disabled!");
            return;
        }
        if(player != null && !AdminManager.PlayerHasPermissions(player, Config.AdminFlagtoForceHsOnly))
        {
            if(player != null)player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Darkred}You don't have permission to use this command!");
            return;
        }
        if(player != null && Config.AlwaysEnableHsOnly)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Darkred}Command is Unavailable Right Now!");
            return;
        }
        if(adminHeadshotOnly == false)
        {
           adminHeadshotOnly = true;
            Server.PrintToChatAll($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Purple}Admin {ChatColors.Darkred}Enabled {ChatColors.Lime}Headshot Only!");
        }
        else
        {
            adminHeadshotOnly = false;
            Server.PrintToChatAll($" {ChatColors.Lime}[{ChatColors.Darkred}Headshot {ChatColors.Green}Only{ChatColors.Lime}] {ChatColors.Purple}Admin {ChatColors.Darkred}Disabled {ChatColors.Lime}Headshot Only!");
        }
    }
}

