using MPF_Code.Config;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace MPF_Code.Farms
{
  /// <summary>
  /// Host-side, each second: stamps each farmhand's cabin location name into its modData (for the join
  /// menu, see <see cref="Patches.FarmhandSlotNamePatch"/>), applies farm-rename requests, and propagates
  /// each MPF farm's name to every machine for the in-world display name. Both rides on net-synced modData
  /// (<c>Farmer.modData</c> serializes with the farmhand list; <c>GameLocation.modData</c> syncs live).
  /// </summary>
  internal static class FarmhandNameStamper
  {
    /// <summary>
    /// Entry point. Only writes on change (no per-second net churn) and picks up GMCM renames and cabins
    /// built mid-session automatically.
    /// </summary>
    public static void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
      try
      {
        if (!Context.IsWorldReady) return;

        if (Game1.IsMasterGame)
          StampFarmhandHomeNames(); // Feature: join-menu slot name (always on).

        // The editable farm-name feature (rename on creation + live in-world propagation) is toggleable.
        if (!ModServices.Config.Editable_Farm_Name) return;

        if (Game1.IsMasterGame)
        {
          ApplyRenameRequests();
          StampLocationDisplayNames();
        }

        // Every machine mirrors the net-synced location modData onto its local DisplayName cache.
        ApplyLocationDisplayNames();
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"FarmhandNameStamper tick failed: {ex.Message}", LogLevel.Error);
      }
    }

    /// <summary>Host: stamp each farmhand's cabin location name into its modData for the join menu.</summary>
    private static void StampFarmhandHomeNames()
    {
      var farmhandData = Game1.netWorldState?.Value?.farmhandData;
      if (farmhandData is null) return;

      foreach (Farmer farmhand in farmhandData.Values)
      {
        if (farmhand is null) continue;

        GameLocation? parent = FindCabinParent(farmhand.homeLocation.Value);
        MpfFarm? farm = FarmRegistry.FindByLocation(parent?.Name);

        if (farm is null)
        {
          // Cabin on the main Farm, or unresolved: leave it to vanilla (host's world name).
          if (farmhand.modData.ContainsKey(FarmRegistry.HomeFarmNameModDataKey))
            farmhand.modData.Remove(FarmRegistry.HomeFarmNameModDataKey);
          continue;
        }

        string name = ConfigBinder.GetName(farm);
        if (string.IsNullOrWhiteSpace(name)) name = farm.DefaultName;

        if (!farmhand.modData.TryGetValue(FarmRegistry.HomeFarmNameModDataKey, out string current) ||
            current != name)
        {
          farmhand.modData[FarmRegistry.HomeFarmNameModDataKey] = name;
        }
      }
    }

    /// <summary>
    /// Host: stamp each MPF farm's name onto its location's net-synced modData. On change, invalidates
    /// <c>Data/Locations</c> so Content Patcher re-applies its <c>DisplayName</c> edit from the live config.
    /// </summary>
    private static void StampLocationDisplayNames()
    {
      bool changed = false;

      foreach (MpfFarm farm in FarmRegistry.Farms)
      {
        GameLocation? location = Game1.getLocationFromName(farm.LocationId);
        if (location is null) continue;

        string name = ConfigBinder.GetName(farm);
        if (string.IsNullOrWhiteSpace(name)) name = farm.DefaultName;

        if (!location.modData.TryGetValue(FarmRegistry.LocationDisplayNameModDataKey, out string current) ||
            current != name)
        {
          location.modData[FarmRegistry.LocationDisplayNameModDataKey] = name;
          changed = true;
        }
      }

      if (changed)
        ModServices.Helper.GameContent.InvalidateCache("Data/Locations");
    }

    /// <summary>
    /// Every machine: mirror the net-synced <c>location.modData</c> name onto the location's local-only
    /// <c>DisplayName</c> (whose <c>_displayName</c> cache wouldn't otherwise refresh without a reload).
    /// </summary>
    private static void ApplyLocationDisplayNames()
    {
      foreach (MpfFarm farm in FarmRegistry.Farms)
      {
        GameLocation? location = Game1.getLocationFromName(farm.LocationId);
        if (location is null) continue;

        if (location.modData.TryGetValue(FarmRegistry.LocationDisplayNameModDataKey, out string name) &&
            !string.IsNullOrWhiteSpace(name) && location.DisplayName != name)
        {
          location.DisplayName = name;
        }
      }
    }

    /// <summary>
    /// Applies a client's pending rename request (<see cref="FarmRegistry.RequestedFarmNameModDataKey"/>):
    /// resolves its cabin to an MPF farm, writes the config, then clears the request so it runs once.
    /// </summary>
    private static void ApplyRenameRequests()
    {
      foreach (Farmer farmer in Game1.getOnlineFarmers())
      {
        if (farmer is null) continue;
        if (!farmer.modData.TryGetValue(FarmRegistry.RequestedFarmNameModDataKey, out string requested))
          continue;

        GameLocation? parent = FindCabinParent(farmer.homeLocation.Value);
        MpfFarm? farm = FarmRegistry.FindByLocation(parent?.Name);

        if (farm is not null && !string.IsNullOrWhiteSpace(requested))
        {
          string trimmed = requested.Trim();
          // Idempotent: only write to disk when the name actually changes.
          if (trimmed.Length > 0 && ConfigBinder.GetName(farm) != trimmed)
          {
            ConfigBinder.SetName(farm, trimmed);
            ModServices.Helper.WriteConfig(ModServices.Config);
            // StampLocationDisplayNames() (same tick) propagates the new name to every machine.
            ModServices.Monitor.Log(
              $"Renamed MPF farm {farm.ConfigKey} ({farm.LocationId}) to \"{trimmed}\" " +
              $"(requested by {farmer.Name}).", LogLevel.Info);
          }
        }

        // Applied, or cabin not on an MPF farm / blank request: clear so it only runs once.
        farmer.modData.Remove(FarmRegistry.RequestedFarmNameModDataKey);
      }
    }

    /// <summary>Resolves a cabin's parent location from its interior name by scanning all locations.</summary>
    internal static GameLocation? FindCabinParent(string? cabinInteriorName)
    {
      if (string.IsNullOrWhiteSpace(cabinInteriorName)) return null;

      foreach (GameLocation location in Game1.locations)
      {
        foreach (Building building in location.buildings)
        {
          if (building.isCabin && building.HasIndoorsName(cabinInteriorName))
            return location;
        }
      }

      return null;
    }
  }
}
