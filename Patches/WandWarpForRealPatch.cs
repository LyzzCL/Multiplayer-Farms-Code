using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Return Scepter: warps the player to their own cabin's front door; falls through to vanilla otherwise.
  /// </summary>
  internal static class WandWarpForRealPatch
  {
    public static bool Prefix(Wand __instance)
    {
      try
      {
        Farmer lastUser = __instance.lastUser ?? Game1.player;
        FarmHouse? home = Utility.getHomeOfFarmer(lastUser);
        if (home is null) return true; // let vanilla handle it

        Point frontDoorSpot = home.getFrontDoorSpot();
        Game1.warpFarmer(home.GetParentLocation().Name, frontDoorSpot.X, frontDoorSpot.Y, flip: false);

        Game1.fadeToBlackAlpha = 0.99f;
        Game1.screenGlow = false;
        lastUser.temporarilyInvincible = false;
        lastUser.temporaryInvincibilityTimer = 0;
        Game1.displayFarmer = true;
        lastUser.CanMove = true;
        return false;
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"Return Scepter warp patch failed: {ex.Message}", LogLevel.Error);
        return true;
      }
    }
  }
}
