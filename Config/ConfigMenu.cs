using GenericModConfigMenu;
using MPF_Code.Farms;
using StardewModdingAPI;

namespace MPF_Code.Config
{
  /// <summary>
  /// Registers the GMCM config menu from the farm registry.
  /// </summary>
  internal static class ConfigMenu
  {
    private const string GenericModConfigMenuId = "spacechase0.GenericModConfigMenu";

    private const string GeneralPageId = "general";
    private const string FarmsPageId = "farms";
    private const string HarmonyPageId = "harmony";
    private const string RootPageId = "";

    private static readonly string[] WarpModeValues =
    {
      "HorizontalRight", "HorizontalLeft", "VerticalUp", "VerticalDown",
    };

    public static void Register()
    {
      IGenericModConfigMenuApi? menu =
        ModServices.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(GenericModConfigMenuId);
      if (menu is null) return;

      IManifest mod = ModServices.Manifest;
      string[] customFarms = GetCustomFarms();

      menu.Register(mod: mod, reset: () => ModEntry.Instance.Config = new ModConfig(),
        save: () => ModServices.Helper.WriteConfig(ModServices.Config));

      // Root page: links to the three sections.
      menu.AddPageLink(mod, GeneralPageId, () => T("menu.page.general.title"), () => T("menu.page.general.tooltip"));
      menu.AddPageLink(mod, FarmsPageId, () => T("menu.page.farms.title"), () => T("menu.page.farms.tooltip"));
      menu.AddPageLink(mod, HarmonyPageId, () => T("menu.page.harmony.title"), () => T("menu.page.harmony.tooltip"));

      // General section.
      menu.AddPage(mod, GeneralPageId, () => T("menu.page.general.header"));
      AddGeneralOptions(menu, mod);

      // Farms page: links to each farm sub-page.
      menu.AddPage(mod, FarmsPageId, () => T("menu.page.farms.header"));
      menu.AddSectionTitle(mod, () => T("menu.farms.section.title"), () => T("menu.farms.section.tooltip"));

      for (int i = 0; i < FarmRegistry.Farms.Count; i++)
      {
        MpfFarm farm = FarmRegistry.Farms[i];
        string pageId = $"Farm{i}";
        menu.AddPageLink(mod, pageId, () => T("menu.farms.link.text", new { number = farm.DefaultNumber }),
          () => T("menu.farms.link.tooltip", new { name = farm.DefaultName }));
      }

      menu.AddPageLink(mod, RootPageId, () => T("menu.back"));

      for (int i = 0; i < FarmRegistry.Farms.Count; i++)
      {
        MpfFarm farm = FarmRegistry.Farms[i];
        string pageId = $"Farm{i}";

        menu.AddPage(mod, pageId, () => farm.DefaultName);
        AddFarmSection(menu, mod, farm, customFarms);
        menu.AddPageLink(mod, FarmsPageId, () => T("menu.back"));
      }

      // Harmony section.
      menu.AddPage(mod, HarmonyPageId, () => T("menu.page.harmony.header"));
      AddHarmonyOptions(menu, mod);
    }

    private static void AddGeneralOptions(IGenericModConfigMenuApi menu, IManifest mod)
    {
      menu.AddSectionTitle(mod, () => T("menu.general.section.title"), () => T("menu.general.section.tooltip"));

      menu.AddBoolOption(mod, name: () => T("option.freeCabins.name"), tooltip: () => T("option.freeCabins.tooltip"),
        getValue: () => ModServices.Config.Free_Cabins, setValue: value => ModServices.Config.Free_Cabins = value);

      menu.AddBoolOption(mod, name: () => T("option.starterCabins.name"),
        tooltip: () => T("option.starterCabins.tooltip"), getValue: () => ModServices.Config.Starter_Cabins,
        setValue: value => ModServices.Config.Starter_Cabins = value);

      menu.AddBoolOption(mod, name: () => T("option.starterItems.name"),
        tooltip: () => T("option.starterItems.tooltip"), getValue: () => ModServices.Config.Starter_Items,
        setValue: value => ModServices.Config.Starter_Items = value);

      menu.AddBoolOption(mod, name: () => T("option.starterBuilds.name"),
        tooltip: () => T("option.starterBuilds.tooltip"), getValue: () => ModServices.Config.Starter_Builds,
        setValue: value => ModServices.Config.Starter_Builds = value);

      menu.AddPageLink(mod, RootPageId, () => T("menu.back"));
    }

    private static void AddHarmonyOptions(IGenericModConfigMenuApi menu, IManifest mod)
    {
      menu.AddSectionTitle(mod, () => T("menu.harmony.section.title"), () => T("menu.harmony.section.tooltip"));

      menu.AddBoolOption(mod, name: () => T("option.returnScepter.name"),
        tooltip: () => T("option.returnScepter.tooltip"), getValue: () => ModServices.Config.Return_Scepter,
        setValue: value => ModServices.Config.Return_Scepter = value);

      menu.AddBoolOption(mod, name: () => T("option.farmTotem.name"), tooltip: () => T("option.farmTotem.tooltip"),
        getValue: () => ModServices.Config.Farm_Totem, setValue: value => ModServices.Config.Farm_Totem = value);

      menu.AddBoolOption(mod, name: () => T("option.lightningRod.name"),
        tooltip: () => T("option.lightningRod.tooltip"), getValue: () => ModServices.Config.Lightning_Rod,
        setValue: value => ModServices.Config.Lightning_Rod = value);

      menu.AddBoolOption(mod, name: () => T("option.miniObelisks.name"),
        tooltip: () => T("option.miniObelisks.tooltip"), getValue: () => ModServices.Config.Mini_Obelisks,
        setValue: value => ModServices.Config.Mini_Obelisks = value);

      menu.AddBoolOption(mod, name: () => T("option.fixTrees.name"), tooltip: () => T("option.fixTrees.tooltip"),
        getValue: () => ModServices.Config.Fix_Trees, setValue: value => ModServices.Config.Fix_Trees = value);

      menu.AddBoolOption(mod, name: () => T("option.fixEvents.name"), tooltip: () => T("option.fixEvents.tooltip"),
        getValue: () => ModServices.Config.Fix_Events, setValue: value => ModServices.Config.Fix_Events = value);

      menu.AddBoolOption(mod, name: () => T("option.fixFestivals.name"),
        tooltip: () => T("option.fixFestivals.tooltip"), getValue: () => ModServices.Config.Fix_Festivals,
        setValue: value => ModServices.Config.Fix_Festivals = value);

      menu.AddBoolOption(mod, name: () => T("option.editableFarmName.name"),
        tooltip: () => T("option.editableFarmName.tooltip"), getValue: () => ModServices.Config.Editable_Farm_Name,
        setValue: value => ModServices.Config.Editable_Farm_Name = value);

      menu.AddSectionTitle(mod, () => T("menu.harmony.section.title.extra"), () => T("menu.harmony.section.tooltip.extra"));
      
      menu.AddBoolOption(mod, name: () => T("option.fixPlacements.name"),
        tooltip: () => T("option.fixPlacements.tooltip"), getValue: () => ModServices.Config.Fix_Placements,
        setValue: value => ModServices.Config.Fix_Placements = value);

      menu.AddPageLink(mod, RootPageId, () => T("menu.back"));
    }

    private static void AddFarmSection(IGenericModConfigMenuApi menu, IManifest mod, MpfFarm farm, string[] customFarms)
    {
      menu.AddSectionTitle(mod, () => T(farm.SectionTitleKey));

      menu.AddBoolOption(mod, name: () => T("option.enable.name"), tooltip: () => string.Empty,
        getValue: () => ConfigBinder.GetEnabled(farm), setValue: value => ConfigBinder.SetEnabled(farm, value));

      menu.AddTextOption(mod, name: () => T("option.farmName.name"),
        tooltip: () => T("option.farmName.tooltip", new { name = farm.DefaultName }),
        getValue: () => ConfigBinder.GetName(farm), setValue: value => ConfigBinder.SetName(farm, value));

      for (int i = 0; i < farm.Warps.Length; i++)
      {
        int warpIndex = i;
        WarpDef warp = farm.Warps[i];

        menu.AddTextOption(mod, name: () => T(warp.LabelKey), tooltip: () => T(warp.TooltipKey),
          getValue: () => ConfigBinder.GetCoords(farm, warpIndex), setValue: value =>
          {
            var (x, y) = ConfigBinder.ParseCoords(value);
            ConfigBinder.SetCoords(farm, warpIndex, $"{x}, {y}");
          });

        menu.AddTextOption(mod, name: () => T("option.direction.name"),
          tooltip: () => T("option.direction.tooltip", new { label = T(warp.LabelKey) }),
          getValue: () => ConfigBinder.GetWarpMode(farm, warpIndex),
          setValue: value => ConfigBinder.SetWarpMode(farm, warpIndex, value), allowedValues: WarpModeValues,
          formatAllowedValue: FormatWarpMode);
      }

      // Fall back to free text when no farm maps are found.
      if (customFarms.Length == 0)
      {
        menu.AddTextOption(mod, name: () => T("option.farmType.name"), tooltip: () => T("option.farmType.tooltip"),
          getValue: () => ConfigBinder.GetFarmType(farm), setValue: value => ConfigBinder.SetFarmType(farm, value));
      }
      else
      {
        menu.AddTextOption(mod, name: () => T("option.farmType.name"), tooltip: () => T("option.farmType.tooltip"),
          getValue: () => ConfigBinder.GetFarmType(farm), setValue: value => ConfigBinder.SetFarmType(farm, value),
          allowedValues: customFarms);
      }
    }

    private static string FormatWarpMode(string value) =>
      value switch
      {
        "HorizontalRight" => "-->",
        "HorizontalLeft" => "@--",
        "VerticalUp" => " ^ ",
        "VerticalDown" => " v ",
        _ => value,
      };

    /// <summary>
    /// Finds available farm type names from the bundled Content Patcher pack.
    /// </summary>
    private static string[] GetCustomFarms()
    {
      try
      {
        string currentModDir = ModServices.Helper.DirectoryPath;
        string? packageDir = Directory.GetParent(currentModDir)?.FullName;

        if (packageDir is null) return Array.Empty<string>();

        string farmsPath = Path.Combine(packageDir, "[CP] Multiplayer Farms", "assets", "Farms");

        if (!Directory.Exists(farmsPath)) return Array.Empty<string>();

        return Directory.GetFiles(farmsPath, "*.tmx", SearchOption.TopDirectoryOnly)
          .Select(Path.GetFileNameWithoutExtension)
          .Where(name => !string.IsNullOrWhiteSpace(name))
          .OrderBy(name => name)
          .ToArray()!;
      }
      catch (Exception ex)
      {
        ModServices.Monitor.Log($"Could not scan custom farm types: {ex.Message}", LogLevel.Warn);
        return Array.Empty<string>();
      }
    }

    private static string T(string key) => ModServices.Helper.Translation.Get(key);

    private static string T(string key, object tokens) => ModServices.Helper.Translation.Get(key, tokens);
  }
}
