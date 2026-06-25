using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Points <see cref="Farmer.getMailboxPosition"/> at the farmhand's own cabin mailbox.
  /// Vanilla only searches the main Farm; this postfix finds the cabin across all locations.
  /// </summary>
  internal static class MailboxPositionPatch
  {
    public static void Postfix(Farmer __instance, ref Point __result)
    {
      try
      {
        string? homeLocationName = __instance.homeLocation.Value;
        if (string.IsNullOrWhiteSpace(homeLocationName)) return;

        foreach (GameLocation location in Game1.locations)
        {
          foreach (Building building in location.buildings)
          {
            if (!building.isCabin || !building.HasIndoorsName(homeLocationName)) continue;

            Point position = building.getMailboxPosition();
            if (ModServices.HasExpandedCabins) position.X += 1;

            __result = position;
            return;
          }
        }

        // No matching cabin: leave vanilla's result (the main mailbox fallback).
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"MailboxPosition postfix failed: {ex.Message}", LogLevel.Error);
      }
    }
  }
}
