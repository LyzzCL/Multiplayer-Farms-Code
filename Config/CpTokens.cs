using System.Globalization;
using ContentPatcher;
using Microsoft.Xna.Framework;
using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace MPF_Code.Config
{
  /// <summary>
  /// Registers the Content Patcher tokens used by the companion packs.
  /// </summary>
  internal static class CpTokens
  {
    public static void RegisterAll(IContentPatcherAPI cp)
    {
      IManifest mod = ModServices.Manifest;
      var names = new List<string>();

      // Token names are part of the CP/FTM pack contract; don't rename them casually.
      void Register(string name, Func<IEnumerable<string>?> getValue)
      {
        cp.RegisterToken(mod, name, getValue);
        names.Add(name);
      }

      RegisterToggleTokens(Register);
      RegisterFarmTokens(Register);
      RegisterEventTokens(Register);

      ModServices.Monitor.Log($"Registered {names.Count} Content Patcher tokens: {string.Join(", ", names)}",
        LogLevel.Trace);
    }

    private static void RegisterToggleTokens(Action<string, Func<IEnumerable<string>?>> register)
    {
      register("FreeCabins", () => Value(ConfigBinder.BoolText(ModServices.Config.Free_Cabins)));
      register("StarterCabins", () => Value(ConfigBinder.BoolText(ModServices.Config.Starter_Cabins)));
      register("StarterItems", () => Value(ConfigBinder.BoolText(ModServices.Config.Starter_Items)));
      register("StarterBuilds", () => Value(ConfigBinder.BoolText(ModServices.Config.Starter_Builds)));
      register("FixTrees", () => Value(ConfigBinder.BoolText(ModServices.Config.Fix_Trees)));
      register("FixEvents", () => Value(ConfigBinder.BoolText(ModServices.Config.Fix_Events)));
      register("FixFestivals", () => Value(ConfigBinder.BoolText(ModServices.Config.Fix_Festivals)));
    }

    private static void RegisterFarmTokens(Action<string, Func<IEnumerable<string>?>> register)
    {
      foreach (MpfFarm farm in FarmRegistry.Farms)
      {
        MpfFarm f = farm;

        register(f.EnabledToken, () => Value(ConfigBinder.BoolText(ConfigBinder.GetEnabled(f))));
        register(f.NameToken, () => Value(ConfigBinder.GetName(f)));
        register(f.TypeToken, () => Value(ConfigBinder.GetFarmType(f)));

        for (int i = 0; i < f.Warps.Length; i++)
        {
          int warpIndex = i;
          string prefix = f.Warps[i].TokenPrefix;

          register($"{prefix}X", () =>
          {
            var (x, _) = ConfigBinder.ParseCoords(ConfigBinder.GetCoords(f, warpIndex));
            return Value(x);
          });

          register($"{prefix}Y", () =>
          {
            var (_, y) = ConfigBinder.ParseCoords(ConfigBinder.GetCoords(f, warpIndex));
            return Value(y);
          });

          register($"{prefix}WarpMode", () => Value(ConfigBinder.GetWarpMode(f, warpIndex)));
        }
      }
    }

    private static void RegisterEventTokens(Action<string, Func<IEnumerable<string>?>> register)
    {
      // Expanded Cabins can't change mid-session, so offsets are resolved once.
      EventOffsets offsets = FarmRegistry.ChooseEventOffsets(ModServices.HasExpandedCabins);

      foreach (var (locationId, eventPrefix) in FarmRegistry.EventLocations)
      {
        string loc = locationId;
        string p = eventPrefix;

        register($"{p}CabinX", () => Value(CabinPoint(loc).X));
        register($"{p}CabinY", () => Value(CabinPoint(loc).Y));
        register($"{p}CameraX", () => Value(CabinPoint(loc).X + offsets.CameraDx));
        register($"{p}CameraY", () => Value(CabinPoint(loc).Y + offsets.CameraDy));
        register($"{p}FarmerX", () => Value(CabinPoint(loc).X + offsets.FarmerDx));
        register($"{p}FarmerY", () => Value(CabinPoint(loc).Y + offsets.FarmerDy));
        register($"{p}NpcX", () => Value(CabinPoint(loc).X + offsets.NpcDx));
        register($"{p}NpcY", () => Value(CabinPoint(loc).Y + offsets.NpcDy));
      }
    }

    /// <summary>
    /// Finds a cabin tile for an event location, or returns the registry fallback.
    /// </summary>
    private static Point CabinPoint(string locationName)
    {
      if (TryGetCabin(locationName, out Building? cabin) && cabin is not null)
        return new Point(cabin.tileX.Value, cabin.tileY.Value);

      return FarmRegistry.CabinFallback;
    }

    private static bool TryGetCabin(string locationName, out Building? cabin)
    {
      cabin = null;

      if (!Context.IsWorldReady) return false;

      GameLocation? location = Game1.getLocationFromName(locationName);
      if (location is null) return false;

      string homeLocationName = Game1.player.homeLocation.Value;

      // Prefer the player's own cabin, then fall back to any cabin on that location.
      if (!string.IsNullOrWhiteSpace(homeLocationName))
      {
        cabin = location.buildings.FirstOrDefault(b => b.isCabin && b.HasIndoorsName(homeLocationName));
        if (cabin is not null) return true;
      }

      cabin = location.buildings.FirstOrDefault(b => b.isCabin);
      return cabin is not null;
    }

    private static IEnumerable<string> Value(string text) => new[] { text };

    private static IEnumerable<string> Value(int number) => new[] { number.ToString(CultureInfo.InvariantCulture) };
  }
}
