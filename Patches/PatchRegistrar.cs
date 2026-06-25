using HarmonyLib;
using MPF_Code.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Registers all Harmony patches. Core patches are always applied; optional ones are config-gated.
  /// </summary>
  internal static class PatchRegistrar
  {
    public static void ApplyAll(Harmony harmony, ModConfig config)
    {
      // ----- always on -----
      Apply("CarpenterMenu.IsValidBuildingForLocation",
        () => harmony.Patch(AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.IsValidBuildingForLocation)),
          postfix: Method(typeof(IsValidBuildingForLocationPatch), nameof(IsValidBuildingForLocationPatch.Postfix))));

      Apply("Farmer.getMailboxPosition",
        () => harmony.Patch(AccessTools.Method(typeof(Farmer), nameof(Farmer.getMailboxPosition)),
          postfix: Method(typeof(MailboxPositionPatch), nameof(MailboxPositionPatch.Postfix))));

      Apply("Farm.draw",
        () => harmony.Patch(AccessTools.Method(typeof(Farm), nameof(Farm.draw)),
          transpiler: Method(typeof(FarmDrawPatch), nameof(FarmDrawPatch.Transpiler))));

      Apply("FarmHouse.getFrontDoorSpot",
        () => harmony.Patch(AccessTools.Method(typeof(FarmHouse), "getFrontDoorSpot"),
          postfix: Method(typeof(FrontDoorSpotPatch), nameof(FrontDoorSpotPatch.Postfix))));

      // ----- config-gated -----
      if (config.Fix_Festivals)
        Apply("Event.exitEvent",
          () => harmony.Patch(AccessTools.Method(typeof(Event), nameof(Event.exitEvent)),
            prefix: Method(typeof(EventExitEventPatch), nameof(EventExitEventPatch.Prefix)),
            postfix: Method(typeof(EventExitEventPatch), nameof(EventExitEventPatch.Postfix))));

      if (config.Return_Scepter)
        Apply("Wand.wandWarpForReal",
          () => harmony.Patch(AccessTools.Method(typeof(Wand), "wandWarpForReal"),
            prefix: Method(typeof(WandWarpForRealPatch), nameof(WandWarpForRealPatch.Prefix))));

      if (config.Farm_Totem)
        Apply("Object.totemWarpForReal",
          () => harmony.Patch(AccessTools.Method(typeof(StardewValley.Object), "totemWarpForReal"),
            prefix: Method(typeof(TotemWarpForRealPatch), nameof(TotemWarpForRealPatch.Prefix))));

      if (config.Lightning_Rod)
        Apply("Utility.performLightningUpdate",
          () => harmony.Patch(AccessTools.Method(typeof(Utility), "performLightningUpdate"),
            prefix: Method(typeof(LightningUpdatePatch), nameof(LightningUpdatePatch.Prefix))));

      if (config.Mini_Obelisks)
        Apply("Object.placementAction",
          () => harmony.Patch(AccessTools.Method(typeof(StardewValley.Object), "placementAction"),
            prefix: Method(typeof(PlacementActionPatch), nameof(PlacementActionPatch.Prefix))));
      
      if (config.Fix_Placements)
        Apply("StardewValley.Object.placementAction",
          () => harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.Object), "placementAction"),
            prefix: new HarmonyMethod(typeof(FruitTreePatch), nameof(FruitTreePatch.Prefix))));
    }

    private static HarmonyMethod Method(Type type, string name) => new(type, name);

    private static void Apply(string label, Action apply)
    {
      try
      {
        apply();
        ModServices.Monitor.Log($"Applied Harmony patch: {label}", LogLevel.Trace);
      }
      catch (Exception ex)
      {
        ModServices.Monitor.Log($"Failed to apply patch {label}:\n{ex}", LogLevel.Error);
      }
    }
  }
}
