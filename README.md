# Plate Up! Mods

A collection of my mods for the game [Plate Up!](https://store.steampowered.com/app/1599600/PlateUp/).  All of my mods can be seen in the [Steam Workshop](https://steamcommunity.com/id/MCGods/myworkshopfiles/?appid=1599600).

Project configured per https://github.com/tpill90/PlateUp-Mods/tree/master

# Mod List

<table>
      <tr>
        <td>Train For Turbo</td>
        <td>Turbo is too hard? Start your kitchen with more customers to hone your skill!! </td>
        <td> <a href="https://github.com/MCGods/PlateUpMods/tree/main/TrainForTurbo/README.md">Readme</a> </td>
        <td> <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=3639508275">Steam Workshop</a> </td>
      </tr>
</table>

# Installation
Mods can be easily installed by clicking `Subscribe` on the mod's workshop page.

# Development

## Dev Environment Setup

How to setup the development enviroment for this project:

1. Install [Visual Studio 2022](https://visualstudio.microsoft.com) and add the `C#` and `Game development with Unity` workloads.  Run the install and allow it to finish.
2. In the repo root directory, update `Environment.props` to point the property `PlateUpInstallDir` to the path of your installed copy of Plate Up!.  The default value is set to point to the default Steam install directory, so if you havent changed where Steam is installed there shouldn't be anything for you to do.
3. In the `_Development` directory is a script `SetupDevEnvironment.ps1` that will install BepInEx and modify PlateUp to be a development build instead of a regular release build.  Run this script with `.\SetupDevEnvironment.ps1`.  This only needs to be done once.  To see if it succeeded launch Plate Up and you should see the text `Development Build` text in the lower-right corner of the screen.

## Building and Debugging

1. Build the project with target `Debug`. The mod will automatically be copied over to the game's mod directory.  You only need to launch the game to test it!
2. Clicking `Start Without Debugging` in Visual Studio will automatically build and launch the game for you.
3. Go to `Debug` -> `Attach Unity debugger`
4. On the `Select Unity Instance` dialog simply double click the only Unity instance listed.
5. The debugger is now attached!  You can place breakpoints anywhere and debug as you normally would.

## Viewing Logs

The script `_TailLogs.ps1` in the repository root can be used to improve the logging experience.  BepInEx annoyingly always puts it's log console underneath the game, requiring you to move the logs to another window each time you launch the game.  Also there is additional logging noise from Plate Up itself that you might not want to be polluting your logs.  This script will filter those logs out, and allows you to keep the logs open in another terminal instance on a different monitor.

## Other Dev Tools

- [Stuff on Demand](https://steamcommunity.com/sharedfiles/filedetails/?id=2956266506) can be used to spawn in appliances for testing.  Open the menu with `F3`.
- [Plate Up Explorer](https://steamcommunity.com/sharedfiles/filedetails/?id=2933828796) can be used to inspect game objects, UI elements, and more.  Open with `Ctrl + Shift + T`.

# Publishing

Update `MOD_VERSION` in `AssemblyInfo.cs` and build a new version in `Release` mode.  Then run the Plate Up! mod uploader that is in the `C:\Program Files (x86)\Steam\steamapps\common\PlateUp\PlateUp\PlateUp_Data` directory.  Point the mod uploader to the `workshop` dir found in each mod's source code folder.
