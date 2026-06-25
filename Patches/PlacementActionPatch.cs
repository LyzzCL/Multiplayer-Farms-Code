using Microsoft.Xna.Framework;
using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Allows Mini-Obelisk placement on MPF farms (max 2 per farm, matching vanilla).
  /// Only intercepts custom farms; the main Farm is handled by vanilla.
  /// </summary>
  internal static class PlacementActionPatch
  {
    public static bool Prefix(StardewValley.Object __instance, ref bool __result, GameLocation location, int x, int y,
      Farmer? who = null)
    {
      try
      {
        if (__instance.QualifiedItemId != FarmRegistry.MiniObeliskQualifiedId) return true;

        // Vanilla already allows (and limits) mini-obelisks on "Farm"; only extend to MPF farms.
        if (!FarmRegistry.IsCustomFarmLocation(location.Name)) return true;

        int obeliskCount =
          location.objects.Values.Count(obj => obj?.QualifiedItemId == FarmRegistry.MiniObeliskQualifiedId);
        if (obeliskCount >= 2)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceTwo"));
          __result = false;
          return false;
        }

        var obeliskTile = new Vector2(x / 64, y / 64);
        if (location.objects.ContainsKey(obeliskTile))
        {
          __result = false;
          return false;
        }

        var toPlace = (StardewValley.Object) __instance.getOne();
        toPlace.shakeTimer = 50;
        toPlace.tileLocation.Value = obeliskTile;
        toPlace.performDropDownAction(who);
        location.objects.Add(obeliskTile, toPlace);
        toPlace.initializeLightSource(obeliskTile);
        location.playSound("woodyStep");

        __result = true;
        return false;
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"Mini-obelisk placement patch failed: {ex.Message}", LogLevel.Error);
        return true;
      }
    }
  }
}
