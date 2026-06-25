using System.Globalization;
using System.Reflection;
using MPF_Code.Farms;
using StardewModdingAPI;

namespace MPF_Code.Config
{
  /// <summary>
  /// Binds farm registry entries to the flat ModConfig naming convention.
  /// </summary>
  internal static class ConfigBinder
  {
    private static readonly Dictionary<string, PropertyInfo> Cache = new(StringComparer.Ordinal);
    
    public static bool GetEnabled(MpfFarm farm) => GetBool($"{farm.ConfigKey}_Enabled");
    public static void SetEnabled(MpfFarm farm, bool value) => SetValue($"{farm.ConfigKey}_Enabled", value);
    
    public static string GetName(MpfFarm farm) => GetString($"{farm.ConfigKey}_Name");
    public static void SetName(MpfFarm farm, string value) => SetValue($"{farm.ConfigKey}_Name", value);
    
    public static string GetFarmType(MpfFarm farm) => GetString($"{farm.ConfigKey}_Type");
    public static void SetFarmType(MpfFarm farm, string value) => SetValue($"{farm.ConfigKey}_Type", value);
    
    public static string GetCoords(MpfFarm farm, int warpIndex) => GetString(CoordsKey(farm, warpIndex));
    public static void SetCoords(MpfFarm farm, int warpIndex, string value) => SetValue(CoordsKey(farm, warpIndex), value);

    public static string GetWarpMode(MpfFarm farm, int warpIndex) => GetString(ModeKey(farm, warpIndex));
    public static void SetWarpMode(MpfFarm farm, int warpIndex, string value) => SetValue(ModeKey(farm, warpIndex), value);

    // Registry warp indexes are 0-based; config coordinate slots are 1-based.
    private static string CoordsKey(MpfFarm farm, int warpIndex) => $"{farm.ConfigKey}_Coords_{warpIndex + 1}";
    private static string ModeKey(MpfFarm farm, int warpIndex) => $"{farm.ConfigKey}_Coords_{warpIndex + 1}_Mode";
    public static string BoolText(bool value) => value ? "true" : "false";

    /// <summary>
    /// Parses a config coordinate string, using legacy fallbacks for empty or invalid values.
    /// </summary>
    public static (int x, int y) ParseCoords(string? input)
    {
      if (string.IsNullOrWhiteSpace(input)) return (0, 0); // Legacy empty-value fallback.
      string cleaned = input.Replace("(", string.Empty).Replace(")", string.Empty);
      string[] parts = cleaned.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
      if (parts.Length >= 2 && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x) &&
          int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y))
      {
        return (x, y);
      }

      ModServices.Monitor.LogOnce($"Could not parse coordinates \"{input}\"; using fallback (16, 16).", LogLevel.Warn);
      return (16, 16); // Legacy invalid-value fallback.
    }

    /// <summary>
    /// Validates that every registry-based config property exists on ModConfig.
    /// </summary>
    public static void Validate()
    {
      var missing = new List<string>();
      foreach (MpfFarm farm in FarmRegistry.Farms)
      {
        Require($"{farm.ConfigKey}_Enabled", typeof(bool), missing);
        Require($"{farm.ConfigKey}_Name", typeof(string), missing);
        Require($"{farm.ConfigKey}_Type", typeof(string), missing);
        for (int i = 0; i < farm.Warps.Length; i++)
        {
          Require(CoordsKey(farm, i), typeof(string), missing);
          Require(ModeKey(farm, i), typeof(string), missing);
        }
      }

      if (missing.Count > 0)
      {
        ModServices.Monitor.Log(
          "ModConfig is missing properties the farm registry expects: " + string.Join(", ", missing) +
          ". Those will fall back to defaults — check ModConfig against FarmRegistry.", LogLevel.Error);
      }
    }

    private static void Require(string name, Type expected, List<string> missing)
    {
      PropertyInfo? prop = Resolve(name);
      if (prop is null || prop.PropertyType != expected) missing.Add(name);
    }

    private static bool GetBool(string name)
    {
      PropertyInfo? prop = Resolve(name);
      return prop is not null && prop.PropertyType == typeof(bool) && (bool) prop.GetValue(ModServices.Config)!;
    }

    private static string GetString(string name)
    {
      PropertyInfo? prop = Resolve(name);
      if (prop is null || prop.PropertyType != typeof(string)) return string.Empty;
      return (string?) prop.GetValue(ModServices.Config) ?? string.Empty;
    }

    private static void SetValue(string name, object value)
    {
      PropertyInfo? prop = Resolve(name);
      if (prop is null || !prop.CanWrite || prop.PropertyType != value.GetType())
      {
        ModServices.Monitor.LogOnce($"Cannot write config property \"{name}\".", LogLevel.Warn);
        return;
      }

      prop.SetValue(ModServices.Config, value);
    }

    private static PropertyInfo? Resolve(string name)
    {
      if (Cache.TryGetValue(name, out PropertyInfo? cached)) return cached;
      PropertyInfo? prop = typeof(ModConfig).GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
      if (prop is not null) Cache[name] = prop;
      return prop;
    }
  }
}
