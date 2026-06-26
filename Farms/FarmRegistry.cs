using System.Globalization;
using Microsoft.Xna.Framework;

namespace MPF_Code.Farms
{
  /// <summary>
  /// Central registry for MPF farm data and derived token/location lists.
  /// Frozen token, location and prefix values must match the CP/FTM packs exactly.
  /// </summary>
  internal static class FarmRegistry
  {
    // Vanilla item IDs used by patches.
    public const string LightningRodQualifiedId = "(BC)9";
    public const string BatteryQualifiedId = "(O)787";
    public const string MiniObeliskQualifiedId = "(BC)238";
    public const string FarmTotemQualifiedId = "(O)688";
    public const string FenceItemId = "322";

    /// <summary>
    /// modData key (SMAPI-namespaced) the host stamps on each farmhand Farmer with the display
    /// name of its cabin's MPF location. Read by the client to label the farmhand-join menu slot.
    /// </summary>
    public const string HomeFarmNameModDataKey = "LyzzCL.MPF_Core/HomeFarmName";

    /// <summary>
    /// modData key (SMAPI-namespaced) a client sets on its own Farmer during farmhand creation to
    /// request renaming its cabin's MPF farm. The host reads it, writes the config, then clears it.
    /// </summary>
    public const string RequestedFarmNameModDataKey = "LyzzCL.MPF_Core/RequestedFarmName";

    /// <summary>
    /// modData key the host stamps on each MPF farm's <c>GameLocation</c> with its current name. Net-syncs
    /// to every client, which mirrors it onto the location's <c>DisplayName</c> so renames show in-world
    /// without a reload.
    /// </summary>
    public const string LocationDisplayNameModDataKey = "LyzzCL.MPF_Core/LocationDisplayName";

    // Main Farm is included for event/lightning tokens, but is not an MpfFarm.
    public const string MainFarmLocation = "Farm";
    public const string MainFarmEventPrefix = "Farm";

    /// <summary>Fallback cabin door tile used before the cabin exists.</summary>
    public static readonly Point CabinFallback = new(64, 15);

    // Event-token camera/farmer/npc offsets. Uniform across all locations.
    public static readonly EventOffsets NormalEventOffsets = new(2, 3, 2, 2, 2, 3);

    // Expanded offsets are used when jenf1.cabinstofarmhouses is installed.
    public static readonly EventOffsets ExpandedEventOffsets = new(2, 3, 5, 3, 5, 4);

    public static EventOffsets ChooseEventOffsets(bool hasExpandedCabins) =>
      hasExpandedCabins ? ExpandedEventOffsets : NormalEventOffsets;

    // Additional farm definitions.
    // Token names, location IDs and event prefixes are frozen CP/FTM data.
    // Some names are intentionally inconsistent; do not rename or derive them.
    public static readonly IReadOnlyList<MpfFarm> Farms = new[]
    {
      new MpfFarm(
        Slot: 1,
        LocationId: "Custom_MPF_Wilderness",
        EventPrefix: "Wilderness",
        ConfigKey: "F1",
        EnabledToken: "FarmOne",
        NameToken: "FOneName",
        TypeToken: "FOneType",
        DefaultName: "Wilderness",
        DefaultType: "Wilderness",
        SectionTitleKey: "farm.1.title",
        CabinTileX: 68,
        CabinTileY: 11,
        CabinSkinId: "Stone Cabin",
        Warps: new[]
        {
          new WarpDef("FOne",
            "farm.1.warp.1.label",
            "farm.1.warp.1.tooltip"),
        },
        StarterBuildings: new[]
        {
          new StarterBuildingDef("Slime Hutch",
            60,
            13),
        }),

      new MpfFarm(
        Slot: 2,
        LocationId: "Custom_MPF_Forest",
        EventPrefix: "Forest",
        ConfigKey: "F2",
        EnabledToken: "FarmTwo",
        NameToken: "FTwoName",
        TypeToken: "FTwoType",
        DefaultName: "Forest",
        DefaultType: "Forest",
        SectionTitleKey: "farm.2.title",
        CabinTileX: 22,
        CabinTileY: 8,
        CabinSkinId: "Log Cabin",
        Warps: new[]
        {
          new WarpDef("FTwo",
            "farm.2.warp.1.label",
            "farm.2.warp.1.tooltip"),
          new WarpDef("FTwoTwo",
            "farm.2.warp.2.label",
            "farm.2.warp.2.tooltip"),
        },
        StarterBuildings: Array.Empty<StarterBuildingDef>()),

      new MpfFarm(
        Slot: 3,
        LocationId: "Custom_MPF_Beach",
        EventPrefix: "Beach",
        ConfigKey: "F3",
        EnabledToken: "FarmThree",
        NameToken: "FTreName",
        TypeToken: "FTreType",
        DefaultName: "Beach",
        DefaultType: "Beach",
        SectionTitleKey: "farm.3.title",
        CabinTileX: 37,
        CabinTileY: 4,
        CabinSkinId: "Neighbor Cabin",
        Warps: new[]
        {
          new WarpDef("FTre",
            "farm.3.warp.1.label",
            "farm.3.warp.1.tooltip"),
          new WarpDef("FTreTwo",
            "farm.3.warp.2.label",
            "farm.3.warp.2.tooltip"),
        },
        StarterBuildings: Array.Empty<StarterBuildingDef>()),

      new MpfFarm(
        Slot: 4,
        LocationId: "Custom_MPF_Riverland",
        EventPrefix: "Riverland",
        ConfigKey: "F4",
        EnabledToken: "FarmFour",
        NameToken: "FourName",
        TypeToken: "FourType",
        DefaultName: "Riverland",
        DefaultType: "Riverland",
        SectionTitleKey: "farm.4.title",
        CabinTileX: 59,
        CabinTileY: 8,
        CabinSkinId: "Plank Cabin",
        Warps: new[]
        {
          new WarpDef("Four",
            "farm.4.warp.1.label",
            "farm.4.warp.1.tooltip"),
          new WarpDef("FourTwo",
            "farm.4.warp.2.label",
            "farm.4.warp.2.tooltip"),
        },
        StarterBuildings: Array.Empty<StarterBuildingDef>()),

      new MpfFarm(
        Slot: 5,
        LocationId: "Custom_MPF_Meadowlands",
        EventPrefix: "Meadowlands",
        ConfigKey: "F5",
        EnabledToken: "FarmFive",
        NameToken: "FiveName",
        TypeToken: "FiveType",
        DefaultName: "Meadowlands",
        DefaultType: "Meadowlands",
        SectionTitleKey: "farm.5.title",
        CabinTileX: 76,
        CabinTileY: 16,
        CabinSkinId: "Rustic Cabin",
        Warps: new[]
        {
          new WarpDef("Five",
            "farm.5.warp.1.label",
            "farm.5.warp.1.tooltip"),
        },
        StarterBuildings: new[]
        {
          new StarterBuildingDef("Coop",
            54,
            9,
            AdoptStarterChickens: true,
            Fences: new[]
            {
              new FenceLineDef(Horizontal: true,
                Fixed: 20,
                Start: 47,
                EndExclusive: 63,
                Gate: -1),
              new FenceLineDef(Horizontal: false,
                Fixed: 47,
                Start: 16,
                EndExclusive: 20,
                Gate: -1),
              new FenceLineDef(Horizontal: false,
                Fixed: 62,
                Start: 7,
                EndExclusive: 20,
                Gate: 13),
            }),
        }),

      new MpfFarm(
        Slot: 6,
        LocationId: "Custom_MPF_Hilltop",
        EventPrefix: "Hilltop",
        ConfigKey: "F6",
        EnabledToken: "FarmSix",
        NameToken: "FSixName",
        TypeToken: "FSixType",
        DefaultName: "Hilltop",
        DefaultType: "Hilltop",
        SectionTitleKey: "farm.6.title",
        CabinTileX: 26,
        CabinTileY: 10,
        CabinSkinId: "Beach Cabin",
        Warps: new[]
        {
          new WarpDef("FSix",
            "farm.6.warp.1.label",
            "farm.6.warp.1.tooltip"),
          new WarpDef("FSixTwo",
            "farm.6.warp.2.label",
            "farm.6.warp.2.tooltip"),
        },
        StarterBuildings: Array.Empty<StarterBuildingDef>()),

      new MpfFarm(
        Slot: 7,
        LocationId: "Custom_MPF_4Corners",
        EventPrefix: "FourCorners",
        ConfigKey: "F7",
        EnabledToken: "FarmSeven",
        NameToken: "FSvnName",
        TypeToken: "FSvnType",
        DefaultName: "Four Corners",
        DefaultType: "4Corners",
        SectionTitleKey: "farm.7.title",
        CabinTileX: 60,
        CabinTileY: 12,
        CabinSkinId: "Trailer Cabin",
        Warps: new[]
        {
          new WarpDef("FSvn",
            "farm.7.warp.1.label",
            "farm.7.warp.1.tooltip"),
          new WarpDef("FSvnTwo",
            "farm.7.warp.2.label",
            "farm.7.warp.2.tooltip"),
        },
        StarterBuildings: Array.Empty<StarterBuildingDef>()),
    };

    /// <summary>Main farm and additional farms that expose event/lightning tokens.</summary>
    public static readonly IReadOnlyList<(string LocationId, string EventPrefix)> EventLocations =
      new[] { (LocationId: MainFarmLocation, EventPrefix: MainFarmEventPrefix) }
        .Concat(Farms.Select(f => (LocationId: f.LocationId, EventPrefix: f.EventPrefix)))
        .ToArray();

    /// <summary>Locations scanned for lightning rods.</summary>
    public static readonly IReadOnlyList<string> LightningRodLocationIds =
      EventLocations.Select(e => e.LocationId).ToArray();

    // Additional farm IDs only; used by mini-obelisks, front doors and farm totem redirects.
    private static readonly HashSet<string> CustomFarmLocationSet =
      new(Farms.Select(f => f.LocationId), StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyCollection<string> CustomFarmLocationIds => CustomFarmLocationSet;

    /// <summary>Returns whether the location is an MPF additional farm.</summary>
    public static bool IsCustomFarmLocation(string? locationName) =>
      locationName is not null && CustomFarmLocationSet.Contains(locationName);

    /// <summary>Finds an MPF farm by location ID.</summary>
    public static MpfFarm? FindByLocation(string? locationName) =>
      locationName is null
        ? null
        : Farms.FirstOrDefault(f => string.Equals(f.LocationId, locationName, StringComparison.OrdinalIgnoreCase));
  }

  // Supporting records.

  /// <summary>Offsets from a cabin door used for event tokens.</summary>
  internal sealed record EventOffsets(int CameraDx, int CameraDy, int FarmerDx, int FarmerDy, int NpcDx, int NpcDy);

  /// <summary>Warp config entry. TokenPrefix is frozen and creates the CP warp tokens.</summary>
  internal sealed record WarpDef(string TokenPrefix, string LabelKey, string TooltipKey);

  /// <summary>Day-1 fence line definition, with an optional gate position.</summary>
  internal sealed record FenceLineDef(bool Horizontal, int Fixed, int Start, int EndExclusive, int Gate);

  /// <summary>Day-1 starter building definition.</summary>
  internal sealed record StarterBuildingDef(
    string BuildingType,
    int TileX,
    int TileY,
    bool AdoptStarterChickens = false,
    FenceLineDef[]? Fences = null);

  /// <summary>Additional farm definition.</summary>
  internal sealed record MpfFarm(
    int Slot,
    string LocationId,
    string EventPrefix,
    string ConfigKey,
    string EnabledToken,
    string NameToken,
    string TypeToken,
    string DefaultName,
    string DefaultType,
    string SectionTitleKey,
    int CabinTileX,
    int CabinTileY,
    string CabinSkinId,
    WarpDef[] Warps,
    StarterBuildingDef[] StarterBuildings)
  {
    /// <summary>Display slot number used by GMCM.</summary>
    public string DefaultNumber => Slot.ToString(CultureInfo.InvariantCulture);
  }
}
