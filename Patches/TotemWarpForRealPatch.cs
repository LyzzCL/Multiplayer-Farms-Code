using Microsoft.Xna.Framework;
using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Farm Totem: warps the player to their own cabin's front door instead of the main farmhouse.
  /// Other totem types fall through to vanilla.
  /// </summary>
  internal static class TotemWarpForRealPatch
  {
    public static bool Prefix(StardewValley.Object __instance)
    {
      try
      {
        if (__instance.QualifiedItemId != FarmRegistry.FarmTotemQualifiedId)
          return true; // mountain/beach/desert/island totems: unchanged from vanilla

        FarmHouse? home = Utility.getHomeOfFarmer(Game1.player);
        if (home is null) return true; // vanilla's WarpTotemEntry / whichFarm fallback (identical outcome)

        Point frontDoorSpot = home.getFrontDoorSpot();
        Game1.warpFarmer(home.GetParentLocation().Name, frontDoorSpot.X, frontDoorSpot.Y, flip: false);

        Game1.fadeToBlackAlpha = 0.99f;
        Game1.screenGlow = false;
        Game1.player.temporarilyInvincible = false;
        Game1.player.temporaryInvincibilityTimer = 0;
        Game1.displayFarmer = true;
        return false;
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"Farm Totem warp patch failed: {ex.Message}", LogLevel.Error);
        return true;
      }
    }
  }
}
