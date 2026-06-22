using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;

namespace MPF_Code.Patches
{
    internal static class LightningUpdatePatch
    {
        private static readonly string[] LightningLocationNames = new[]
        {
            "Farm",
            "Custom_MPF_Wilderness",
            "Custom_MPF_Forest",
            "Custom_MPF_Beach",
            "Custom_MPF_Riverland",
            "Custom_MPF_Meadowlands",
            "Custom_MPF_Hilltop",
            "Custom_MPF_4Corners"
        };

        public static bool Prefix(Utility __instance, int time_of_day)
        {
            Random random = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, time_of_day);

            if (random.NextDouble() < 0.125 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() / 100.0)
            {
                Farm.LightningStrikeEvent lightningStrikeEvent = new() { bigFlash = true };
                List<(GameLocation location, Vector2 tile)> lightningRods = CollectLightningRods();

                if (lightningRods.Count > 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var (location, tile) = random.ChooseFrom(lightningRods);

                        if (!location.Objects.TryGetValue(tile, out StardewValley.Object lightningRod))
                            continue;

                        if (lightningRod.heldObject.Value != null)
                            continue;

                        lightningRod.heldObject.Value = ItemRegistry.Create<StardewValley.Object>("(O)787");
                        lightningRod.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
                        lightningRod.shakeTimer = 1000;
                        lightningStrikeEvent.createBolt = true;
                        lightningStrikeEvent.boltPosition = tile * 64f + new Vector2(32f, 0f);

                        var players = Game1.getAllFarmers()
                            .Where(p => p?.currentLocation != null)
                            .ToList();

                        bool anyPlayerInThisLocation = players.Any(p => ReferenceEquals(p.currentLocation, location));

                        bool anyPlayerInLightningOutdoors = players.Any(p =>
                        {
                            GameLocation currentLocation = p.currentLocation;
                            return currentLocation?.IsOutdoors == true && currentLocation.IsLightningHere();
                        });

                        bool anyPlayerOutdoors = players.Any(p => p.currentLocation?.IsOutdoors == true);

                        if (location is Farm farm)
                          farm.lightningStrikeEvent.Fire(lightningStrikeEvent);
                        else if (anyPlayerInThisLocation)
                          Game1.flashAlpha = (float)(0.5 + Game1.random.NextDouble());
                        else if (anyPlayerInLightningOutdoors)
                          DelayedAction.playSoundAfterDelay("thunder_small", Game1.random.Next(500, 1500));

                        if (anyPlayerOutdoors)
                          Game1.playSound("thunder");
                        return false;
                    }
                }

                if (random.NextDouble() < 0.25 - Game1.player.team.AverageDailyLuck() - Game1.player.team.AverageLuckLevel() / 100.0)
                {
                    TryStrikeRandomTerrainFeature(Game1.getFarm(), lightningStrikeEvent);
                }

                Game1.getFarm().lightningStrikeEvent.Fire(lightningStrikeEvent);
            }
            else if (random.NextDouble() < 0.1)
            {
                Farm.LightningStrikeEvent lightningStrikeEvent = new() { smallFlash = true };
                Game1.getFarm().lightningStrikeEvent.Fire(lightningStrikeEvent);
            }

            return false;
        }

        private static List<(GameLocation location, Vector2 tile)> CollectLightningRods()
        {
            List<(GameLocation location, Vector2 tile)> lightningRods = new();

            foreach (GameLocation location in LightningLocationNames.Select(Game1.getLocationFromName))
            {
                if (location is null)
                    continue;

                foreach (var pair in location.Objects.Pairs)
                {
                    if (pair.Value?.QualifiedItemId == "(BC)9")
                        lightningRods.Add((location, pair.Key));
                }
            }

            return lightningRods;
        }

        private static void TryStrikeRandomTerrainFeature(Farm farm, Farm.LightningStrikeEvent lightningStrikeEvent)
        {
            try
            {
                if (!Utility.TryGetRandom(farm.terrainFeatures, out var key, out var value))
                    return;

                if (value is FruitTree fruitTree)
                {
                    fruitTree.struckByLightningCountdown.Value = 4;
                    fruitTree.shake(key, doEvenIfStillShaking: true);
                    lightningStrikeEvent.createBolt = true;
                    lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, -128f);
                    return;
                }

                Crop crop = (value as HoeDirt)?.crop;
                bool cropAlive = crop != null && !crop.dead.Value;

                if (value.performToolAction(null, 50, key))
                {
                    lightningStrikeEvent.destroyedTerrainFeature = true;
                    lightningStrikeEvent.createBolt = true;
                    farm.terrainFeatures.Remove(key);
                    lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, -128f);
                }

                if (cropAlive && crop.dead.Value)
                {
                    lightningStrikeEvent.createBolt = true;
                    lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, 0f);
                }
            }
            catch
            {
                // intentionally ignored
            }
        }
    }
}
