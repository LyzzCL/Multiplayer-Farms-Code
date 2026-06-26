using Microsoft.Xna.Framework;
using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Resolves the front-door spot for cabins on MPF farms (vanilla only handles warps targeting "Farm").
  /// </summary>
  internal static class FrontDoorSpotPatch
  {
    public static void Postfix(FarmHouse __instance, ref Point __result)
    {
      try
      {
        GameLocation? parent = __instance.GetParentLocation();
        if (parent is null || !FarmRegistry.IsCustomFarmLocation(parent.Name))
          return; // vanilla's result is correct for the main farmhouse / main-farm cabins

        foreach (Warp warp in __instance.warps)
        {
          if (warp.TargetName == parent.Name)
          {
            __result = new Point(warp.TargetX, warp.TargetY);
            return;
          }
        }

        // No warp back to the parent farm: leave vanilla's result rather than guess.
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"FrontDoorSpot postfix failed: {ex.Message}", LogLevel.Error);
      }
    }
  }
}
