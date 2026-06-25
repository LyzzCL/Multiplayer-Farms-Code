using MPF_Code.Config;
using StardewModdingAPI;

namespace MPF_Code
{
  /// <summary>
  /// Shared SMAPI services for static helpers and Harmony patches.
  /// </summary>
  internal static class ModServices
  {
    public static IMonitor Monitor { get; private set; } = null!;
    public static IModHelper Helper { get; private set; } = null!;
    public static IManifest Manifest => ModEntry.Instance.ModManifest;

    /// <summary>Current config, read live so GMCM resets are reflected.</summary>
    public static ModConfig Config => ModEntry.Instance.Config;

    private const string ExpandedCabinsModId = "jenf1.cabinstofarmhouses";
    private static bool? _hasExpandedCabins;

    /// <summary>Cached check for the Cabins to Farmhouses integration.</summary>
    public static bool HasExpandedCabins => _hasExpandedCabins ??= Helper.ModRegistry.IsLoaded(ExpandedCabinsModId);

    public static void Init(IMonitor monitor, IModHelper helper)
    {
      Monitor = monitor;
      Helper = helper;
    }
  }
}
