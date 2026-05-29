using Microsoft.Xna.Framework;
using StardewValley;

namespace MPF_Code.Patches
{
  internal static class PlacementActionPatch
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

    public static bool Prefix(StardewValley.Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer? who = null)
    {
      if (__instance.QualifiedItemId != "(BC)238")
        return true;

      if (!ValidLocationNames.Contains(location.Name))
        return true;

      int obeliskCount = location.objects.Values.Count(obj => obj?.QualifiedItemId == "(BC)238");
      if (obeliskCount >= 2)
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceTwo"));
        __result = false;
        return false;
      }

      Vector2 obeliskTile = new(x / 64, y / 64);
      StardewValley.Object toPlace = (StardewValley.Object)__instance.getOne();
      toPlace.shakeTimer = 50;
      toPlace.tileLocation.Value = obeliskTile;
      toPlace.performDropDownAction(who);
      location.objects.Add(obeliskTile, toPlace);
      toPlace.initializeLightSource(obeliskTile);
      location.playSound("woodyStep");

      __result = true;
      return false;
    }
  }
}
