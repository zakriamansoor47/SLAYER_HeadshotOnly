# Donate: If you like my work, you can donate to me via [Steam Trade Offer](https://bit.ly/3qDpgPd)

## Description:
This plugin enables Headshot Only to all players **OR** Enables Headshot Only on Players themselves by using Public Command **!hs**.

## Installation:
**1.** Upload files to your server.

**2.** Edit **configs/plugins/SLAYER_HeadshotOnly/SLAYER_HeadshotOnly.json** if you want to change the settings.

**3.** Change the Map **or** Restart the Server **or** Load the Plugin.

## Features:
**1.** Allow Players to Enable Headshot Only on themselves by the player command `!hs`

**2.** Allow Admin (with specific Flag) to Force Players to Headshot Only.

**3.** Players Can't use the command to Enable/Disable Headshot Only when Admin forces them to Headshot Only.

## Commands:
```
!hs - For Everyone to Enable/Disable Headshot Only on themselves (Headshot Only will always be enabled if "AlwaysEnableHsOnly" is true in the config file)
!headshot - For Admin with Specific Flag to Enable/Disable Headshot Only to all Players (Headshot Only will always be enabled if "AlwaysEnableHsOnly" is true in the config file)
css_headshot - For RCON (Console) to Enable/Disable Headshot Only to all Players (Headshot Only will always be enabled if "AlwaysEnableHsOnly" is true in the config file)
```

## Configuration:
```
{
  "PluginEnabled": true,                    // Enable/Disable plugin. (false = Disabled, true = Enabled)
  "AlwaysEnableHsOnly": false,              // Always Enable Headshot Only in the Server. (false = No, true = Yes)
  "PlayerCanUseHsCmd": true,                // Players Can use Headshot Only Command (!hs) to Enable/Disable Headshot Only on themselves (0 = No, 1 = Yes)
  "AdminFlagtoForceHsOnly": "@css/root",    // Admin flag Which can Enable/Disable Headshot Only to All Players by Command (!headshot)
  "ConfigVersion": 1                        // Don't Change this
}
```

