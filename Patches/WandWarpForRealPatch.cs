using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;

namespace MPF_Code.Patches
{
  internal static class WandWarpForRealPatch
  {
    public static bool Prefix(Wand __instance)
    {
      Farmer player = Game1.player;
      Farmer lastUser = __instance.lastUser ?? player;
      FarmHouse? homeOfFarmer = Utility.getHomeOfFarmer(player);

      if (homeOfFarmer is null)
        return true;

      Point frontDoorSpot = homeOfFarmer.getFrontDoorSpot();
      Game1.warpFarmer(homeOfFarmer.GetParentLocation().Name, frontDoorSpot.X, frontDoorSpot.Y, flip: false);
      Game1.fadeToBlackAlpha = 0.99f;
      Game1.screenGlow = false;
      lastUser.temporarilyInvincible = false;
      lastUser.temporaryInvincibilityTimer = 0;
      Game1.displayFarmer = true;
      lastUser.CanMove = true;

      return false;
    }
  }
}
