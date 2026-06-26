using HarmonyLib;
using MPF_Code.Config;
using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Makes the "Farm Name" field editable in farmhand character creation (MPF cabins only), auto-filled
  /// with the farm's current name; on OK, requests the rename via modData for the host to apply (see
  /// <see cref="Farms.FarmhandNameStamper"/>).
  ///
  /// <para>Vanilla locks the box for <c>NewFarmhand</c>: <c>receiveLeftClick</c> skips
  /// <c>farmnameBox.Update()</c> and forces the host's farm name each click. We replicate the NewGame
  /// path (call <c>Update()</c> on click, undo the forced text, capture the typed value each frame), so
  /// the box selects on click/A like the other fields with no controller auto-open on hover.</para>
  /// </summary>
  internal static class FarmhandCreationFarmNamePatch
  {
    private static readonly AccessTools.FieldRef<CharacterCustomization, TextBox> FarmnameBoxRef =
      AccessTools.FieldRefAccess<CharacterCustomization, TextBox>("farmnameBox");

    // Single CharacterCustomization is open at a time; track the active MPF-farmhand instance + text.
    private static CharacterCustomization? _menu;
    private static string _typed = string.Empty;

    private static bool IsActive(CharacterCustomization menu) => _menu == menu;

    private static string MainFarmName => Game1.MasterPlayer?.farmName?.Value ?? string.Empty;

    /// <summary>Resolves the current name of the MPF farm whose cabin this farmhand owns, or null.</summary>
    private static string? ResolveCurrentName()
    {
      Farmer? player = Game1.player;
      if (player is null) return null;

      // The host stamps the display name onto the farmhand before sending it to the client.
      if (player.modData.TryGetValue(FarmRegistry.HomeFarmNameModDataKey, out string stamped) &&
          !string.IsNullOrWhiteSpace(stamped))
        return stamped;

      // Local/split-screen (master) fallback: the stamp may lag a tick; resolve directly.
      if (Game1.IsMasterGame)
      {
        GameLocation? parent = FarmhandNameStamper.FindCabinParent(player.homeLocation.Value);
        MpfFarm? farm = FarmRegistry.FindByLocation(parent?.Name);
        if (farm is not null)
        {
          string name = ConfigBinder.GetName(farm);
          return string.IsNullOrWhiteSpace(name) ? farm.DefaultName : name;
        }
      }

      return null;
    }

    public static void ConstructorPostfix(CharacterCustomization __instance)
    {
      try
      {
        _menu = null;
        _typed = string.Empty;

        if (__instance.source != CharacterCustomization.Source.NewFarmhand) return;

        string? current = ResolveCurrentName();
        if (string.IsNullOrWhiteSpace(current)) return; // Not an MPF cabin → leave vanilla as-is.

        TextBox? box = FarmnameBoxRef(__instance);
        if (box is null) return;

        box.Text = current;
        _typed = current;
        _menu = __instance;
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"FarmhandCreation ctor postfix failed: {ex.Message}", LogLevel.Error);
      }
    }

    /// <summary>Per-frame: capture the typed text, or undo vanilla's forced main-farm-name after a click.</summary>
    public static void UpdatePostfix(CharacterCustomization __instance)
    {
      if (!IsActive(__instance)) return;
      try
      {
        TextBox? box = FarmnameBoxRef(__instance);
        if (box is null) return;

        if ((box.Text ?? string.Empty) == MainFarmName)
          box.Text = _typed;          // vanilla forced it on a click → restore our value
        else
          _typed = box.Text ?? string.Empty; // genuine edit (typing / keyboard) → capture it
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"FarmhandCreation update postfix failed: {ex.Message}", LogLevel.Error);
      }
    }

    /// <summary>On click/A: restore our value and call <c>Update()</c> to select the box (NewGame path).</summary>
    public static void ReceiveLeftClickPostfix(CharacterCustomization __instance)
    {
      if (!IsActive(__instance)) return;
      try
      {
        TextBox? box = FarmnameBoxRef(__instance);
        if (box is null) return;

        box.Text = _typed;  // before Update(), so the on-screen keyboard opens with our value
        box.Update();       // selection (click/A); no per-frame call ⇒ no auto-open on hover
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"FarmhandCreation click postfix failed: {ex.Message}", LogLevel.Error);
      }
    }

    /// <summary>On OK, request the host rename this cabin's MPF farm. Blank → keep current (no write).</summary>
    public static void OptionButtonClickPrefix(CharacterCustomization __instance, string name)
    {
      if (!IsActive(__instance) || name != "OK") return;
      try
      {
        string value = (_typed ?? string.Empty).Trim();
        if (value.Length == 0) return; // Empty field → keep the current name.

        Game1.player.modData[FarmRegistry.RequestedFarmNameModDataKey] = value;
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"FarmhandCreation OK prefix failed: {ex.Message}", LogLevel.Error);
      }
    }
  }
}
