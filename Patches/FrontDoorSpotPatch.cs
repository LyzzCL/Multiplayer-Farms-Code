using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace MPF_Code.Patches
{
  internal static class FrontDoorSpotPatch
  {
    public static bool Prefix(FarmHouse __instance, ref Point __result)
    {
      string? targetLocation = Utility.getHomeOfFarmer(Game1.player)?.GetParentLocation()?.name?.Value;

      foreach (Warp warp in __instance.warps)
      {
        if (warp.TargetName != targetLocation)
          continue;

        if (__instance is Cabin)
        {
          __result = new Point(warp.TargetX, warp.TargetY);
          return false;
        }

        __result = new Point(warp.TargetX, warp.TargetY);
        return false;
      }

      __result = Game1.getFarm().GetMainFarmHouseEntry();
      return false;
    }
  }
}
