using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;


namespace SLAYER_HeadshotOnly;

// Used these to remove compile warnings
#pragma warning disable CS8602

public class SLAYER_HeadshotOnlyConfig: BasePluginConfig
{
    [JsonPropertyName("PluginEnabled")] public bool PluginEnabled { get; set; } = true;
    [JsonPropertyName("AlwaysEnableHsOnly")] public bool AlwaysEnableHsOnly { get; set; } = false;
    [JsonPropertyName("PlayerCanUseHsCmd")] public bool PlayerCanUseHsCmd { get; set; } = true;
    [JsonPropertyName("HsOnlyWeapons")] public string HsOnlyWeapons { get; set; } = "";
    [JsonPropertyName("AdminFlagtoForceHsOnly")] public string AdminFlagtoForceHsOnly { get; set; } = "@css/root";
}

public class SLAYER_HeadshotOnly : BasePlugin, IPluginConfig<SLAYER_HeadshotOnlyConfig>
{
    public override string ModuleName => "SLAYER_HeadshotOnly";
    public override string ModuleVersion => "1.2.1";
    public override string ModuleAuthor => "SLAYER";
    public override string ModuleDescription => "Enable/Disable Headshot Only. Allow players to Enable/Disable Headshot Only on themselves";


    public required SLAYER_HeadshotOnlyConfig Config {get; set;}

    public bool[] g_Headshot= new bool[64];
    public bool adminHeadshotOnly = false;
    public string[] HsWeapons = new string[50];

    public void OnConfigParsed(SLAYER_HeadshotOnlyConfig config)
    {
        Config = config;
    }
    public override void Load(bool hotReload)
    {
        AddCommand("css_hs", "Enabled/Disabled Headshot only", cmd_hs);
        AddCommand("css_headshot", "Enabled/Disabled Headshot only", cmd_AdminHsOnly);
        HsWeapons = Config.HsOnlyWeapons.Split(";");
        RegisterEventHandler<EventPlayerHurt>((@event, info) => 
        {
            var player = @event.Userid;
            var attacker = @event.Attacker;
            var DmgHealth = @event.DmgHealth;
            var DmgArmor = @event.DmgArmor;
            if(!Config.PluginEnabled || player == null || attacker == null || !player.IsValid || !attacker.IsValid)return HookResult.Continue;
            // Some Checks to validate Attacker
            if (player.TeamNum == attacker.TeamNum && !(DmgHealth > 0 || DmgArmor > 0))return HookResult.Continue;

            if(g_Headshot[attacker.Slot] || adminHeadshotOnly || Config.AlwaysEnableHsOnly)
            {
                if(!string.IsNullOrEmpty(Config.HsOnlyWeapons) && HsWeapons != null && HsWeapons.Count() > 0 && !HsWeapons.Contains($"weapon_{@event.Weapon}")) // Check, Is Headshot Weapons String is Empty or Not and player have weapons or not
                {
                    return HookResult.Continue; 
                }
                if(@event.Hitgroup != 1) // if headshot is enabled and bullet not hitting on Head
                {
                    player.PlayerPawn.Value.Health += DmgHealth; // add the dmg health to Normal health
                    player.PlayerPawn.Value.ArmorValue += DmgArmor; // Update the Armor as well
                }
            }
            return HookResult.Continue;
        });
    }
    
    private void cmd_hs(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(!Config.PluginEnabled || Config.AlwaysEnableHsOnly || !Config.PlayerCanUseHsCmd || player == null || !player.IsValid)return;

        if(g_Headshot[player.Slot] == false)
        {
            player.PrintToChat($"{Localizer["Chat.Prefix"]} {Localizer["Chat.PlayerHS_On"]}");
            Server.PrintToChatAll($"{Localizer["Chat.Prefix"]} {Localizer["Chat.AllHS_On", player.PlayerName]}");
            g_Headshot[player.Slot] = true;
        }
        else
        {
            player.PrintToChat($"{Localizer["Chat.Prefix"]} {Localizer["Chat.PlayerHS_Off"]}");
            Server.PrintToChatAll($"{Localizer["Chat.Prefix"]} {Localizer["Chat.AllHS_Off", player.PlayerName]}");
            g_Headshot[player.Slot] = false;
        }
        
    }
    private void cmd_AdminHsOnly(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(!Config.PluginEnabled || Config.AlwaysEnableHsOnly || player == null || !player.IsValid)return;

        if(!AdminManager.PlayerHasPermissions(player, Config.AdminFlagtoForceHsOnly))
        {
            player.PrintToChat($"{Localizer["Chat.Prefix"]} {ChatColors.DarkRed}You don't have permission to use this command!");
            return;
        }
        if(adminHeadshotOnly == false)
        {
           adminHeadshotOnly = true;
            Server.PrintToChatAll($"{Localizer["Chat.Prefix"]} {Localizer["Chat.AdminHS_On"]}");
        }
        else
        {
            adminHeadshotOnly = false;
            Server.PrintToChatAll($"{Localizer["Chat.Prefix"]} {Localizer["Chat.AdminHS_Off"]}");
        }
    }
}

