using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MPF_Code.Patches
{
  internal static class EventExitEventPatch
  {
    public static void Prefix(Event __instance, out bool __state)
    {
      __state = __instance.isFestival;
    }

    public static void Postfix(Event __instance, bool __state)
    {
      if (!Context.IsWorldReady)
        return;

      if (!__state)
        return;

      FarmHouse? homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
      if (homeOfFarmer == null)
        return;

      Point frontDoorSpot = homeOfFarmer.getFrontDoorSpot();
      string locationName = homeOfFarmer.GetParentLocation().Name;

      __instance.setExitLocation(locationName, frontDoorSpot.X, frontDoorSpot.Y);
    }
  }
}
