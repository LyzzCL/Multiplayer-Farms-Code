using MPF_Code.Config;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System.Drawing;
using System.Globalization;

namespace MPF_Code
{
  internal sealed partial class ModEntry
  {
    private sealed record CabinEventOffsets(
      int CameraDx,
      int CameraDy,
      int FarmerDx,
      int FarmerDy,
      int NpcDx,
      int NpcDy);

    private static readonly Dictionary<string, string> CabinTokenPrefix = new()
    {
      ["Custom_MPF_Wilderness"] = "Wilderness",
      ["Custom_MPF_Forest"] = "Forest",
      ["Custom_MPF_Beach"] = "Beach",
      ["Custom_MPF_Riverland"] = "Riverland",
      ["Custom_MPF_Meadowlands"] = "Meadowlands",
      ["Custom_MPF_Hilltop"] = "Hilltop",
      ["Custom_MPF_4Corners"] = "FourCorners",
    };

    private Dictionary<string, CabinEventOffsets> EventOffsets = null!;

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
      bool hasExpandedCabins = Helper.ModRegistry.IsLoaded("jenf1.cabinstofarmhouses");

      EventOffsets = BuildEventOffsets(hasExpandedCabins);

      var configManager = new ModConfigManager(this);
      configManager.RegisterConfigMenu();
      configManager.RegisterContentPatcherTokens();

      var cp = Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
      if (cp is null)
        return;

      foreach (string locationName in EventOffsets.Keys)
        RegisterCabinEventTokens(cp, locationName);
    }

    private static Dictionary<string, CabinEventOffsets> BuildEventOffsets(bool hasExpandedCabins)
    {
      var normal = new CabinEventOffsets(2, 3, 2, 2, 2, 3);
      var expanded = new CabinEventOffsets(2, 3, 5, 3, 5, 4); 
      
      CabinEventOffsets chosen = hasExpandedCabins ? expanded : normal;

      return new()
      {
        ["Custom_MPF_Wilderness"] = chosen,
        ["Custom_MPF_Forest"] = chosen,
        ["Custom_MPF_Beach"] = chosen,
        ["Custom_MPF_Riverland"] = chosen,
        ["Custom_MPF_Meadowlands"] = chosen,
        ["Custom_MPF_Hilltop"] = chosen,
        ["Custom_MPF_4Corners"] = chosen,
      };
    }

    private void RegisterCabinEventTokens(ContentPatcher.IContentPatcherAPI cp, string locationName)
    {
      string prefix = CabinTokenPrefix[locationName];

      cp.RegisterToken(ModManifest, $"{prefix}CabinX", () =>
      {
        Point pos = GetCabinPointOrFallback(locationName);
        return new[] { pos.X.ToString(CultureInfo.InvariantCulture) };
      });

      cp.RegisterToken(ModManifest, $"{prefix}CabinY", () =>
      {
        Point pos = GetCabinPointOrFallback(locationName);
        return new[] { pos.Y.ToString(CultureInfo.InvariantCulture) };
      });

      cp.RegisterToken(ModManifest, $"{prefix}CameraX", () =>
      {
        Point pos = GetCabinPointOrFallback(locationName);
        var offsets = EventOffsets[locationName];
        return new[] { (pos.X + offsets.CameraDx).ToString(CultureInfo.InvariantCulture) };
      });

      cp.RegisterToken(ModManifest, $"{prefix}CameraY", () =>
      {
        Point pos = GetCabinPointOrFallback(locationName);
        var offsets = EventOffsets[locationName];
        return new[] { (pos.Y + offsets.CameraDy).ToString(CultureInfo.InvariantCulture) };
      });

      cp.RegisterToken(ModManifest, $"{prefix}FarmerX", () =>
      {
        Point pos = GetCabinPointOrFallback(locationName);
        var offsets = EventOffsets[locationName];
        return new[] { (pos.X + offsets.FarmerDx).ToString(CultureInfo.InvariantCulture) };
      });

      cp.RegisterToken(ModManifest, $"{prefix}FarmerY", () =>
      {
        Point pos = GetCabinPointOrFallback(locationName);
        var offsets = EventOffsets[locationName];
        return new[] { (pos.Y + offsets.FarmerDy).ToString(CultureInfo.InvariantCulture) };
      });

      cp.RegisterToken(ModManifest, $"{prefix}NpcX", () =>
      {
        Point pos = GetCabinPointOrFallback(locationName);
        var offsets = EventOffsets[locationName];
        return new[] { (pos.X + offsets.NpcDx).ToString(CultureInfo.InvariantCulture) };
      });

      cp.RegisterToken(ModManifest, $"{prefix}NpcY", () =>
      {
        Point pos = GetCabinPointOrFallback(locationName);
        var offsets = EventOffsets[locationName];
        return new[] { (pos.Y + offsets.NpcDy).ToString(CultureInfo.InvariantCulture) };
      });
    }

    private static bool TryGetCabin(string locationName, out Building? cabin)
    {
      cabin = null;

      if (!Context.IsWorldReady)
        return false;

      GameLocation? location = Game1.getLocationFromName(locationName);
      if (location is null)
        return false;

      string homeLocationName = Game1.player.homeLocation.Value;

      // 1) Cabaña del jugador
      if (!string.IsNullOrWhiteSpace(homeLocationName))
      {
        cabin = location.buildings.FirstOrDefault(b => b.isCabin && b.HasIndoorsName(homeLocationName));
        if (cabin is not null)
          return true;
      }

      // 2) Cualquier cabaña
      cabin = location.buildings.FirstOrDefault(b => b.isCabin);
      return cabin is not null;
    }
    
    private static Point GetCabinPointOrFallback(string locationName)
    {
      if (TryGetCabin(locationName, out Building? cabin))
        return new Point(cabin.tileX.Value, cabin.tileY.Value);

      return new Point(64, 15);
    }
  }
}
