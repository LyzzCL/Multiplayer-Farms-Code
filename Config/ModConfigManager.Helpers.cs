using ContentPatcher;
using GenericModConfigMenu;
using StardewModdingAPI;
using System;
using System.IO;
using System.Linq;

namespace MPF_Code.Config
{
    internal partial class ModConfigManager
    {
        private static readonly string[] WarpModeValues =
        {
            "HorizontalRight",
            "HorizontalLeft",
            "VerticalUp",
            "VerticalDown"
        };

        private void AddGeneralOptions(IGenericModConfigMenuApi configMenu)
        {
          configMenu.AddSectionTitle(_mod.ModManifest,
            text: () => "General Options (Requires World Restart)",
            tooltip: () =>
              "Basic gameplay options.\n" +
              "These options only take effect before creating or joining a new world.");

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Free Cabins",
                tooltip: () => "Allows you to purchase cabins at Robin for 0g.",
                getValue: () => _mod.Config.Free_Cabins,
                setValue: value => _mod.Config.Free_Cabins = value);

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Starter Cabins",
                tooltip: () => "Generates pre-built cabins on all farms. \n" +
                               "Note: using a custom farm will disable starter cabins, please go to Robin.",
                getValue: () => _mod.Config.Starter_Cabins,
                setValue: value => _mod.Config.Starter_Cabins = value);

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Starter Items",
                tooltip: () => "Generates crates on each farm with themed starter items. \n" +
                               "Note: using a custom farm will disable crates.",
                getValue: () => _mod.Config.Starter_Items,
                setValue: value => _mod.Config.Starter_Items = value);

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Starter Buildings",
                tooltip: () => "Enables pre-built buildings on:" +
                               "\n - Wilderness (Slime Hutch)" +
                               "\n - Meadowlands (Coop with 2 chickens)." +
                               "\n Note: using a custom farm will disable starter buildings.",
                getValue: () => _mod.Config.Starter_Builds,
                setValue: value => _mod.Config.Starter_Builds = value);
            
            configMenu.AddPageLink( _mod.ModManifest, "",() => "Back");
        }

        private void AddHarmonyOptions(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(_mod.ModManifest,
                text: () => "Harmony Patches (Requires Restart)",
                tooltip: () =>
                    "The following patches directly change base game code and require restarting the client for them to be enabled/disabled.\n" +
                    "Only need to disable this if they are causing you issues (for example if you are using a mod that modifies Return Scepter behavior).");

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Return Scepter Fix",
                tooltip: () => "Modifies Return Scepter to allow farmhands warp to their cabins built on additional farms.",
                getValue: () => _mod.Config.Return_Scepter,
                setValue: value => _mod.Config.Return_Scepter = value);

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Farm Totem Fix",
                tooltip: () => "Modifies Farm Totem to allow farmhands warp to their cabins built on additional farms.",
                getValue: () => _mod.Config.Farm_Totem,
                setValue: value => _mod.Config.Farm_Totem = value);

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Lightning Rod Fix",
                tooltip: () => "Modifies Lightning Rods to be able to generate batteries on additional farms.",
                getValue: () => _mod.Config.Lightning_Rod,
                setValue: value => _mod.Config.Lightning_Rod = value);

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Mini Obelisks Fix",
                tooltip: () => "Modifies Mini Obelisks to be able to be placed on additional farms.",
                getValue: () => _mod.Config.Mini_Obelisks,
                setValue: value => _mod.Config.Mini_Obelisks = value);

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Fix Trees",
                tooltip: () => "Fixes the issue of trees spawning/growing very often.",
                getValue: () => _mod.Config.Fix_Trees,
                setValue: value => _mod.Config.Fix_Trees = value);
            
            configMenu.AddBoolOption(_mod.ModManifest,
              name: () => "Fix Events [WIP]",
              tooltip: () => "Remove every event tied to the main farm so that it can later be replace with " +
                             "custom events that take into account every additional farm, expect missing translations.",
              getValue: () => _mod.Config.Fix_Events,
              setValue: value => _mod.Config.Fix_Events = value);
            
            configMenu.AddBoolOption(_mod.ModManifest,
              name: () => "Fix Festivals",
              tooltip: () => "Modifies default festival end warp location and coordinates for custom farms.",
              getValue: () => _mod.Config.Fix_Festivals,
              setValue: value => _mod.Config.Fix_Festivals = value);
            
            configMenu.AddPageLink( _mod.ModManifest, "",() => "Back");
        }

        private void AddFarmSection(IGenericModConfigMenuApi configMenu, FarmDefinition farm, string[] customFarms)
        {
            configMenu.AddSectionTitle(_mod.ModManifest, text: () => farm.SectionTitle);

            configMenu.AddBoolOption(_mod.ModManifest,
                name: () => "Enable",
                tooltip: () => string.Empty,
                getValue: farm.GetEnabled,
                setValue: farm.SetEnabled);

            configMenu.AddTextOption(_mod.ModManifest,
                name: () => "Name",
                tooltip: () => $"Default: {farm.DefaultName}",
                getValue: farm.GetName,
                setValue: farm.SetName);

            foreach (var warp in farm.Warps)
            {
                configMenu.AddTextOption(_mod.ModManifest,
                    name: () => warp.Label,
                    tooltip: () => warp.Tooltip,
                    getValue: warp.GetCoords,
                    setValue: value =>
                    {
                        var (x, y) = ParseCoords(value);
                        warp.SetCoords($"{x}, {y}");
                    });

                AddWarpModeOption(
                    configMenu,
                    name: () => $"Direction",
                    tooltip: () => $"Choose the warp direction for {warp.Label}. \n\n[WIP] Feature, only works on Farm 7",
                    getValue: warp.GetWarpMode,
                    setValue: warp.SetWarpMode);
            }

            if (customFarms.Length == 0)
            {
                configMenu.AddTextOption(_mod.ModManifest,
                    name: () => "Type",
                    tooltip: () => "Pick a built-in or custom farm.",
                    getValue: farm.GetFarmType,
                    setValue: farm.SetFarmType);
            }
            else
            {
                configMenu.AddTextOption(_mod.ModManifest,
                    name: () => "Type",
                    tooltip: () => "Pick a built-in or custom farm.",
                    getValue: farm.GetFarmType,
                    setValue: farm.SetFarmType,
                    allowedValues: customFarms);
            }
        }

        private void AddWarpModeOption(
            IGenericModConfigMenuApi configMenu,
            Func<string> name,
            Func<string> tooltip,
            Func<string> getValue,
            Action<string> setValue)
        {
            configMenu.AddTextOption(_mod.ModManifest,
                name: name,
                tooltip: tooltip,
                getValue: getValue,
                setValue: setValue,
                allowedValues: WarpModeValues,
                formatAllowedValue: FormatWarpMode);
        }

        private static string FormatWarpMode(string value) => value switch
        {
            "HorizontalRight" => "-->",
            "HorizontalLeft" => "@--",
            "VerticalUp" => " ^ ",
            "VerticalDown" => " v ",
            _ => value
        };

        private void RegisterFarmTokens(IContentPatcherAPI cpApi, FarmDefinition farm)
        {
            cpApi.RegisterToken(_mod.ModManifest, farm.EnabledTokenName, getValue: () => new[] { BoolText(farm.GetEnabled()) });
            cpApi.RegisterToken(_mod.ModManifest, farm.NameTokenName, getValue: () => new[] { farm.GetName() });
            cpApi.RegisterToken(_mod.ModManifest, farm.TypeTokenName, getValue: () => new[] { farm.GetFarmType() });

            foreach (var warp in farm.Warps)
            {
                RegisterCoordToken(cpApi, warp.TokenPrefix, warp.GetCoords);
                cpApi.RegisterToken(_mod.ModManifest, $"{warp.TokenPrefix}WarpMode", getValue: () => new[] { warp.GetWarpMode() });
            }
        }

        private void RegisterCoordToken(IContentPatcherAPI cpApi, string tokenPrefix, Func<string> getter)
        {
            cpApi.RegisterToken(_mod.ModManifest, $"{tokenPrefix}X", getValue: () =>
            {
                var (x, _) = ParseCoords(getter());
                return new[] { x.ToString() };
            });

            cpApi.RegisterToken(_mod.ModManifest, $"{tokenPrefix}Y", getValue: () =>
            {
                var (_, y) = ParseCoords(getter());
                return new[] { y.ToString() };
            });
        }

        private static string[] GetCustomFarms()
        {
            var farmsPath = Path.Combine(
                Constants.GamePath,
                "Mods",
                "Multiplayer Farms",
                "[CP] Multiplayer Farms",
                "assets",
                "Farms");

            if (!Directory.Exists(farmsPath))
                return Array.Empty<string>();

            return Directory.GetFiles(farmsPath, "*.tmx", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .OrderBy(name => name)
                .ToArray();
        }

        private static (int x, int y) ParseCoords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (0, 0);

            input = input.Replace("(", string.Empty).Replace(")", string.Empty);

            var parts = input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && int.TryParse(parts[0], out var x) && int.TryParse(parts[1], out var y))
                return (x, y);

            return (16, 16);
        }

        private static string BoolText(bool value)
        {
            return value ? "true" : "false";
        }
    }
}
