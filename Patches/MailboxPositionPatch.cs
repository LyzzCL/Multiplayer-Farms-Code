using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace MPF_Code.Patches
{
  internal static class MailboxPositionPatch
  {
    internal static IModHelper Helper = null!;
    private const string ExpandedCabinsModId = "jenf1.cabinstofarmhouses";
    
    public static bool Prefix(ref Point __result, Farmer __instance)
    {
      string? homeLocationName = __instance.homeLocation.Value;
      
      if (string.IsNullOrWhiteSpace(homeLocationName))
        return true;
      
      bool hasExpandedCabins = Helper.ModRegistry.IsLoaded(ExpandedCabinsModId);

      foreach (GameLocation location in Game1.locations)
      {
        foreach (Building building in location.buildings)
        {
          if (building.isCabin && building.HasIndoorsName(homeLocationName))
          {
            __result = building.getMailboxPosition();

            if (hasExpandedCabins)
              __result.X += 1;
            
            return false;
          }
        }
      }

      __result = Game1.getFarm().GetMainMailboxPosition();
      return false;
    }
  }
}
