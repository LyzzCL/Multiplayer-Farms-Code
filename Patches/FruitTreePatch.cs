using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace MPF_Code.Patches
{
  internal static class FruitTreePatch
  {
    private static readonly HashSet<string> ValidLocationNames = new(StringComparer.OrdinalIgnoreCase)
    {
      "Farm",
      "Custom_MPF_Wilderness",
      "Custom_MPF_Forest",
      "Custom_MPF_Beach",
      "Custom_MPF_Riverland",
      "Custom_MPF_Meadowlands",
      "Custom_MPF_Hilltop",
      "Custom_MPF_4Corners"
    };

    //Copied vanilla placement logic for fruit tree saplings (and bushes), skipping if it is a sapling. 
    public static bool Prefix(StardewValley.Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer? who = null)
    {

      if (__instance.IsFruitTreeSapling() || __instance.IsTeaSapling())
      {

        Vector2 vector = new Vector2(x / 64, y / 64);
        if (__instance.IsFruitTreeSapling())
        {
          if (FruitTree.IsTooCloseToAnotherTree(new Vector2(x / 64, y / 64), location))
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
            __result = false;

            return false;
          }

          if (FruitTree.IsGrowthBlocked(new Vector2(x / 64, y / 64), location))
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", __instance.DisplayName));
            __result = false;
            return false;
          }
        }

        if (location.terrainFeatures.TryGetValue(vector, out var value7))
        {
          if (!(value7 is HoeDirt { crop: null }))
          {
            __result = false;

            return false;
          }

          location.terrainFeatures.Remove(vector);
        }

        string deniedMessage2 = null;
        bool flag = location.doesTileHaveProperty((int) vector.X, (int) vector.Y, "Diggable", "Back") != null;
        string text3 = location.doesTileHaveProperty((int) vector.X, (int) vector.Y, "Type", "Back");
        bool flag2 = location.doesEitherTileOrTileIndexPropertyEqual((int) vector.X, (int) vector.Y, "CanPlantTrees", "Back", "T");
        bool isValidLocation = ValidLocationNames.Contains(location.NameOrUniqueName) || location is Farm;
        if ((isValidLocation && (flag || text3 == "Grass" || text3 == "Dirt" || flag2) && (!location.IsNoSpawnTile(vector, "Tree") || flag2)) || ((flag || text3 == "Stone") && location.CanPlantTreesHere(__instance.ItemId, (int) vector.X, (int) vector.Y, out deniedMessage2)))
        {
          location.playSound("dirtyHit");
          DelayedAction.playSoundAfterDelay("coin", 100);
          if (__instance.IsTeaSapling())
          {
            location.terrainFeatures.Add(vector, new Bush(vector, 3, location));
            __result = true;
            return false;
          }

          FruitTree fruitTree = new FruitTree(__instance.ItemId)
          {
            GreenHouseTileTree = (location.IsGreenhouse && text3 == "Stone")
          };
          fruitTree.growthRate.Value = Math.Max(1, __instance.Quality + 1);
          location.terrainFeatures.Add(vector, fruitTree);
          __result = true;
          return false;
        }

        if (deniedMessage2 == null)
        {
          deniedMessage2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068");
        }

        Game1.showRedMessage(deniedMessage2);
        __result = false;
        return false;
      }
      return true;


    }
  }
}
