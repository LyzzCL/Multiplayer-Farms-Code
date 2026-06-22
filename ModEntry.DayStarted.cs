using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace MPF_Code
{
  internal sealed partial class ModEntry
  {
    private static readonly string[] CabinSkins =
    {
      "Stone Cabin", "Log Cabin", "Plank Cabin", "Rustic Cabin", "Trailer Cabin", "Neighbor Cabin", "Beach Cabin"
    };

    private static readonly CabinPlacement[] CabinPlacements = new[]
    {
      new CabinPlacement("Custom_MPF_Wilderness", new Vector2(68f, 11f), 0),
      new CabinPlacement("Custom_MPF_Forest", new Vector2(22f, 8f), 1),
      new CabinPlacement("Custom_MPF_Beach", new Vector2(37f, 4f), 5),
      new CabinPlacement("Custom_MPF_Riverland", new Vector2(59f, 8f), 2),
      new CabinPlacement("Custom_MPF_Meadowlands", new Vector2(76f, 16f), 3),
      new CabinPlacement("Custom_MPF_Hilltop", new Vector2(26f, 10f), 6),
      new CabinPlacement("Custom_MPF_4Corners", new Vector2(60f, 12f), 4),
    };

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
      if (!Game1.IsMasterGame)
        return;

      if (Game1.year != 1 || Game1.season != Season.Spring || Game1.dayOfMonth != 1)
        return;

      if (Config.Starter_Cabins)
        PlaceStarterCabins();

      if (Config.Starter_Builds)
        PlaceStarterBuildings();
    }

    private void PlaceStarterCabins()
    {
      if (Config.F1_Enabled && IsConfiguredFor("Wilderness", Config.F1_Type))
        PlaceCabinFromLocation("Custom_MPF_Wilderness", new Vector2(68f, 11f), CabinSkins[0]);

      if (Config.F2_Enabled && IsConfiguredFor("Forest", Config.F2_Type))
        PlaceCabinFromLocation("Custom_MPF_Forest", new Vector2(22f, 8f), CabinSkins[1]);

      if (Config.F3_Enabled && IsConfiguredFor("Beach", Config.F3_Type))
        PlaceCabinFromLocation("Custom_MPF_Beach", new Vector2(37f, 4f), CabinSkins[5]);

      if (Config.F4_Enabled && IsConfiguredFor("Riverland", Config.F4_Type))
        PlaceCabinFromLocation("Custom_MPF_Riverland", new Vector2(59f, 8f), CabinSkins[2]);

      if (Config.F5_Enabled && IsConfiguredFor("Meadowlands", Config.F5_Type))
        PlaceCabinFromLocation("Custom_MPF_Meadowlands", new Vector2(76f, 16f), CabinSkins[3]);

      if (Config.F6_Enabled && IsConfiguredFor("Hilltop", Config.F6_Type))
        PlaceCabinFromLocation("Custom_MPF_Hilltop", new Vector2(26f, 10f), CabinSkins[6]);

      if (Config.F7_Enabled && IsConfiguredFor("4Corners", Config.F7_Type))
        PlaceCabinFromLocation("Custom_MPF_4Corners", new Vector2(60f, 12f), CabinSkins[4]);
    }

    private void PlaceCabinFromLocation(string locationName, Vector2 tile, string skinId)
    {
      var location = Game1.getLocationFromName(locationName);
      if (location is null)
        return;

      PlaceCabin(location, tile, skinId);
    }

    private void PlaceCabin(GameLocation location, Vector2 tile, string skinId)
    {
      var cabin = new Building("Cabin", tile)
      {
        magical = { Value = true }
      };

      cabin.skinId.Value = skinId;
      cabin.daysOfConstructionLeft.Value = 0;
      cabin.load();

      location.buildStructure(cabin, tile, Game1.player, skipSafetyChecks: true);
      cabin.removeOverlappingBushes(location);
    }

    private void PlaceStarterBuildings()
    {
      if (Config.F1_Enabled && IsConfiguredFor("Wilderness", Config.F1_Type))
      {
        var wilderness = Game1.getLocationFromName("Custom_MPF_Wilderness");
        if (wilderness is not null)
          AddSlimeHutchStarterBuilding(wilderness);
      }

      if (Config.F5_Enabled && IsConfiguredFor("Meadowlands", Config.F5_Type))
      {
        var meadowlands = Game1.getLocationFromName("Custom_MPF_Meadowlands");
        if (meadowlands is not null)
          AddMeadowlandsStarterBuildings(meadowlands);
      }
    }

    private static bool IsConfiguredFor(string expectedType, string actualType)
    {
      return string.Equals(expectedType, actualType, StringComparison.OrdinalIgnoreCase);
    }

    private static void AddSlimeHutchStarterBuilding(GameLocation location)
    {
      var hutch = new Building("Slime Hutch", new Vector2(60, 13));
      hutch.FinishConstruction(onGameStart: true);
      hutch.LoadFromBuildingData(hutch.GetData(), forUpgrade: false, forConstruction: true);
      hutch.load();

      ClearBuildingFootprint(location, hutch);
      location.buildings.Add(hutch);
    }

    private static void AddMeadowlandsStarterBuildings(GameLocation location)
    {
      AddFenceLineHorizontal(location, 47, 63, 20, gateX: -1);
      AddFenceLineVertical(location, 47, 16, 20, gateY: -1);
      AddFenceLineVertical(location, 62, 7, 20, gateY: 13);

      var building = new Building("Coop", new Vector2(54f, 9f));
      building.FinishConstruction(onGameStart: true);
      building.LoadFromBuildingData(building.GetData(), forUpgrade: false, forConstruction: true);
      building.load();

      var farmAnimal = new FarmAnimal("White Chicken", Game1.Multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
      var farmAnimal2 = new FarmAnimal("Brown Chicken", Game1.Multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);

      string[] names = Game1.content.LoadString("Strings\\1_6_Strings:StarterChicken_Names").Split('|');
      string picked = names[Game1.random.Next(names.Length)];
      farmAnimal.Name = picked.Split(',')[0].Trim();
      farmAnimal2.Name = picked.Split(',')[1].Trim();

      (building.GetIndoors() as AnimalHouse)?.adoptAnimal(farmAnimal);
      (building.GetIndoors() as AnimalHouse)?.adoptAnimal(farmAnimal2);
      ClearBuildingFootprint(location, building);
      location.buildings.Add(building);
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

    private static void AddFenceLineHorizontal(GameLocation location, int startX, int endXExclusive, int y, int gateX)
    {
      for (int x = startX; x < endXExclusive; x++)
      {
        var tile = new Vector2(x, y);
        bool isGate = x == gateX;
        location.objects.Remove(tile);
        location.objects.Add(tile, new Fence(tile, "322", isGate));
      }
    }

    private static void AddFenceLineVertical(GameLocation location, int x, int startY, int endYExclusive, int gateY)
    {
      for (int y = startY; y < endYExclusive; y++)
      {
        var tile = new Vector2(x, y);
        bool isGate = y == gateY;
        location.objects.Remove(tile);
        location.objects.Add(tile, new Fence(tile, "322", isGate));
      }
    }

    private sealed record CabinPlacement(string LocationName, Vector2 Tile, int SkinIndex);
  }
}
