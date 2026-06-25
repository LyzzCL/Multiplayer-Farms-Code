using Microsoft.Xna.Framework;
using MPF_Code.Config;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace MPF_Code.Farms
{
  /// <summary>
  /// Places starter cabins and buildings on enabled additional farms on day 1.
  /// </summary>
  internal static class WorldSetup
  {
    public static void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
      // Only run on the host during the first morning.
      if (!Game1.IsMasterGame) return;

      if (Game1.year != 1 || Game1.season != Season.Spring || Game1.dayOfMonth != 1) return;

      bool placeCabins = ModServices.Config.Starter_Cabins;
      bool placeBuildings = ModServices.Config.Starter_Builds;

      if (!placeCabins && !placeBuildings) return;

      foreach (MpfFarm farm in FarmRegistry.Farms)
      {
        // Skip disabled or custom-type farms.
        if (!ConfigBinder.GetEnabled(farm) || !IsStillDefaultType(farm)) continue;

        GameLocation? location = Game1.getLocationFromName(farm.LocationId);
        if (location is null) continue;

        if (placeCabins) TryPlaceCabin(location, farm);

        if (placeBuildings) TryPlaceStarterBuildings(location, farm);
      }
    }

    /// <summary>
    /// Returns whether the farm still uses its built-in type.
    /// </summary>
    private static bool IsStillDefaultType(MpfFarm farm) =>
      string.Equals(farm.DefaultType, ConfigBinder.GetFarmType(farm), StringComparison.OrdinalIgnoreCase);

    private static void TryPlaceCabin(GameLocation location, MpfFarm farm)
    {
      try
      {
        var tile = new Vector2(farm.CabinTileX, farm.CabinTileY);
        var cabin = new Building("Cabin", tile) { magical = { Value = true }, };

        cabin.skinId.Value = farm.CabinSkinId;
        cabin.daysOfConstructionLeft.Value = 0;
        cabin.load();

        location.buildStructure(cabin, tile, Game1.player, skipSafetyChecks: true);
        cabin.removeOverlappingBushes(location);
      }
      catch (Exception ex)
      {
        ModServices.Monitor.Log($"Failed to place starter cabin on {farm.LocationId}: {ex.Message}", LogLevel.Warn);
      }
    }

    private static void TryPlaceStarterBuildings(GameLocation location, MpfFarm farm)
    {
      foreach (StarterBuildingDef def in farm.StarterBuildings)
      {
        try
        {
          PlaceStarterBuilding(location, def);
        }
        catch (Exception ex)
        {
          ModServices.Monitor.Log($"Failed to place starter {def.BuildingType} on {farm.LocationId}: {ex.Message}",
            LogLevel.Warn);
        }
      }
    }

    private static void PlaceStarterBuilding(GameLocation location, StarterBuildingDef def)
    {
      // Place fences first to match the vanilla layout order.
      if (def.Fences is not null)
      {
        foreach (FenceLineDef fence in def.Fences) PlaceFenceLine(location, fence);
      }

      var building = new Building(def.BuildingType, new Vector2(def.TileX, def.TileY));
      building.FinishConstruction(onGameStart: true);
      building.LoadFromBuildingData(building.GetData(), forUpgrade: false, forConstruction: true);
      building.load();

      if (def.AdoptStarterChickens) AdoptStarterChickens(building);

      ClearBuildingFootprint(location, building);
      location.buildings.Add(building);
    }

    private static void AdoptStarterChickens(Building building)
    {
      try
      {
        var white = new FarmAnimal("White Chicken", Game1.Multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
        var brown = new FarmAnimal("Brown Chicken", Game1.Multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);

        string[] names = Game1.content.LoadString("Strings\\1_6_Strings:StarterChicken_Names").Split('|');
        string picked = names[Game1.random.Next(names.Length)];
        string[] pair = picked.Split(',');

        if (pair.Length >= 2)
        {
          white.Name = pair[0].Trim();
          brown.Name = pair[1].Trim();
        }

        if (building.GetIndoors() is AnimalHouse animalHouse)
        {
          animalHouse.adoptAnimal(white);
          animalHouse.adoptAnimal(brown);
        }
      }
      catch (Exception ex)
      {
        ModServices.Monitor.Log($"Could not adopt starter chickens: {ex.Message}", LogLevel.Warn);
      }
    }

    private static void ClearBuildingFootprint(GameLocation location, Building building)
    {
      for (int x = building.tileX.Value; x < building.tileX.Value + building.tilesWide.Value; x++)
      {
        for (int y = building.tileY.Value; y < building.tileY.Value + building.tilesHigh.Value; y++)
        {
          var tile = new Vector2(x, y);
          location.objects.Remove(tile);
          location.terrainFeatures.Remove(tile);
        }
      }

      building.removeOverlappingBushes(location);
    }

    private static void PlaceFenceLine(GameLocation location, FenceLineDef fence)
    {
      for (int i = fence.Start; i < fence.EndExclusive; i++)
      {
        var tile = fence.Horizontal ? new Vector2(i, fence.Fixed) : new Vector2(fence.Fixed, i);

        bool isGate = i == fence.Gate;
        location.objects.Remove(tile);
        location.objects.Add(tile, new Fence(tile, FarmRegistry.FenceItemId, isGate));
      }
    }
  }
}
