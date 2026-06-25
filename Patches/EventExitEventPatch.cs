using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MPF_Code.Patches
{
  /// <summary>
  /// After a festival ends, warps farmhands to their own cabin instead of the main farmhouse.
  /// Prefix captures <c>isFestival</c> before vanilla clears it; postfix redirects the exit warp.
  /// </summary>
  internal static class EventExitEventPatch
  {
    public static void Prefix(Event __instance, out bool __state)
    {
      __state = __instance.isFestival;
    }

    public static void Postfix(Event __instance, bool __state)
    {
      try
      {
        if (!Context.IsWorldReady || !__state) return;

        FarmHouse? homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
        if (homeOfFarmer is null) return;

        Point frontDoorSpot = homeOfFarmer.getFrontDoorSpot();
        string locationName = homeOfFarmer.GetParentLocation().Name;

        __instance.setExitLocation(locationName, frontDoorSpot.X, frontDoorSpot.Y);
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"Festival exit postfix failed: {ex.Message}", LogLevel.Error);
      }
    }
  }
}
