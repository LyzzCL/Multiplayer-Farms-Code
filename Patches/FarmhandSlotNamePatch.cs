using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MPF_Code.Patches
{
  /// <summary>
  /// In the join menu, shows a farmhand slot's farm name as the cabin's MPF location name (stamped into
  /// the farmhand's modData by the host, see <see cref="FarmhandNameStamper"/>). Scoped to
  /// <see cref="FarmhandMenu.FarmhandSlot"/>; falls back to vanilla when the key is absent.
  /// </summary>
  internal static class FarmhandSlotNamePatch
  {
    public static void Postfix(LoadGameMenu.SaveFileSlot __instance, ref string __result)
    {
      try
      {
        if (__instance is not FarmhandMenu.FarmhandSlot) return;

        Farmer? farmer = __instance.Farmer;
        if (farmer?.modData is null) return;
        if (!farmer.modData.TryGetValue(FarmRegistry.HomeFarmNameModDataKey, out string name) ||
            string.IsNullOrWhiteSpace(name))
          return;

        // Reuse vanilla's localized "{0} Farm" format string so it matches the original styling.
        __result = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11019", name);
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"FarmhandSlotName postfix failed: {ex.Message}", LogLevel.Error);
      }
    }
  }
}
