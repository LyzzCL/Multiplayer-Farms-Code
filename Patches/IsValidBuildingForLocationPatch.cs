using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley.Menus;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Lets Robin's build menu place cabins on MPF farms (vanilla allows only "Farm").
  /// </summary>
  internal static class IsValidBuildingForLocationPatch
  {
    public static void Postfix(CarpenterMenu __instance, string typeId, ref bool __result)
    {
      try
      {
        if (typeId != "Cabin") return;

        // The main Farm is already handled by vanilla (returns true); only extend to MPF farms.
        if (FarmRegistry.IsCustomFarmLocation(__instance.TargetLocation?.Name)) __result = true;
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"IsValidBuildingForLocation postfix failed: {ex.Message}", LogLevel.Error);
      }
    }
  }
}
