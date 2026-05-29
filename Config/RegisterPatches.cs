using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using MPF_Code.Patches;

namespace MPF_Code.Config
{
    internal static class RegisterPatches
    {
        public static void ApplyAll(Harmony harmony, ModConfig config)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.IsValidBuildingForLocation)),
                postfix: new HarmonyMethod(typeof(IsValidBuildingForLocationPatch), nameof(IsValidBuildingForLocationPatch.Postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMailboxPosition)),
                prefix: new HarmonyMethod(typeof(MailboxPositionPatch), nameof(MailboxPositionPatch.Prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.draw)),
                transpiler: new HarmonyMethod(typeof(FarmDrawPatch), nameof(FarmDrawPatch.Transpiler)));

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), "getFrontDoorSpot"),
                prefix: new HarmonyMethod(typeof(FrontDoorSpotPatch), nameof(FrontDoorSpotPatch.Prefix)));

            if (config.Fix_Festivals)
            {
              harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.exitEvent)),
                prefix: new HarmonyMethod(typeof(EventExitEventPatch), nameof(EventExitEventPatch.Prefix)),
                postfix: new HarmonyMethod(typeof(EventExitEventPatch), nameof(EventExitEventPatch.Postfix)));
            }

            if (config.Return_Scepter)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Wand), "wandWarpForReal"),
                    prefix: new HarmonyMethod(typeof(WandWarpForRealPatch), nameof(WandWarpForRealPatch.Prefix)));
            }

            if (config.Farm_Totem)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Object), "totemWarpForReal"),
                    prefix: new HarmonyMethod(typeof(TotemWarpForRealPatch), nameof(TotemWarpForRealPatch.Prefix)));
            }

            if (config.Lightning_Rod)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), "performLightningUpdate"),
                    prefix: new HarmonyMethod(typeof(LightningUpdatePatch), nameof(LightningUpdatePatch.Prefix)));
            }

            if (config.Mini_Obelisks)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Object), "placementAction"),
                    prefix: new HarmonyMethod(typeof(PlacementActionPatch), nameof(PlacementActionPatch.Prefix)));
            }
        }
    }
}
