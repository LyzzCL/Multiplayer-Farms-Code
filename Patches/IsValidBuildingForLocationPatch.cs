using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley.Menus;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Allows placing cabins on MPF farms via Robin's build menu.
  /// Vanilla only permits cabins on "Farm"; this postfix extends it to MPF farm locations.
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
