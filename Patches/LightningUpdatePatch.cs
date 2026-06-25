using Microsoft.Xna.Framework;
using MPF_Code.Farms;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;

namespace MPF_Code.Patches
{
  /// <summary>
  /// Extends lightning rod charging to all MPF farms.
  /// Replaces vanilla logic to collect rods from all farm locations; emulates strike effects on non-Farm locations.
  /// </summary>
  internal static class LightningUpdatePatch
  {
    public static bool Prefix(Utility __instance, int time_of_day)
    {
      try
      {
        Random random = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, time_of_day);

        if (random.NextDouble() <
            0.125 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() / 100.0)
        {
          Farm.LightningStrikeEvent lightningStrikeEvent = new() { bigFlash = true };
          List<(GameLocation location, Vector2 tile)> lightningRods = CollectLightningRods();

          if (lightningRods.Count > 0)
          {
            for (int i = 0; i < 2; i++)
            {
              var (location, tile) = random.ChooseFrom(lightningRods);

              if (!location.Objects.TryGetValue(tile, out StardewValley.Object lightningRod)) continue;

              if (lightningRod.heldObject.Value != null) continue;

              lightningRod.heldObject.Value =
                ItemRegistry.Create<StardewValley.Object>(FarmRegistry.BatteryQualifiedId);
              lightningRod.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
              lightningRod.shakeTimer = 1000;
              lightningStrikeEvent.createBolt = true;
              lightningStrikeEvent.boltPosition = tile * 64f + new Vector2(32f, 0f);

              var players = Game1.getAllFarmers().Where(p => p?.currentLocation != null).ToList();

              bool anyPlayerInThisLocation = players.Any(p => ReferenceEquals(p.currentLocation, location));

              bool anyPlayerInLightningOutdoors = players.Any(p =>
              {
                GameLocation currentLocation = p.currentLocation;
                return currentLocation?.IsOutdoors == true && currentLocation.IsLightningHere();
              });

              bool anyPlayerOutdoors = players.Any(p => p.currentLocation?.IsOutdoors == true);

              // lightningStrikeEvent is a NetEvent that only exists on Farm; emulate it elsewhere.
              if (location is Farm farm)
                farm.lightningStrikeEvent.Fire(lightningStrikeEvent);
              else if (anyPlayerInThisLocation)
                Game1.flashAlpha = (float) (0.5 + Game1.random.NextDouble());
              else if (anyPlayerInLightningOutdoors)
                DelayedAction.playSoundAfterDelay("thunder_small", Game1.random.Next(500, 1500));

              if (anyPlayerOutdoors) Game1.playSound("thunder");

              return false;
            }
          }

          // No rod charged: fall back to vanilla's terrain strike on the main farm.
          if (random.NextDouble() <
              0.25 - Game1.player.team.AverageDailyLuck() - Game1.player.team.AverageLuckLevel() / 100.0)
            TryStrikeRandomTerrainFeature(Game1.getFarm(), lightningStrikeEvent);

          Game1.getFarm().lightningStrikeEvent.Fire(lightningStrikeEvent);
        }
        else if (random.NextDouble() < 0.1)
        {
          Farm.LightningStrikeEvent lightningStrikeEvent = new() { smallFlash = true };
          Game1.getFarm().lightningStrikeEvent.Fire(lightningStrikeEvent);
        }

        return false;
      }
      catch (Exception ex)
      {
        ModServices.Monitor.LogOnce($"Lightning update patch failed ({ex.Message}); deferring to vanilla.",
          LogLevel.Error);
        return true;
      }
    }

    private static List<(GameLocation location, Vector2 tile)> CollectLightningRods()
    {
      List<(GameLocation location, Vector2 tile)> lightningRods = new();

      foreach (string locationName in FarmRegistry.LightningRodLocationIds)
      {
        GameLocation? location = Game1.getLocationFromName(locationName);
        if (location is null) continue;

        foreach (var pair in location.Objects.Pairs)
        {
          if (pair.Value?.QualifiedItemId == FarmRegistry.LightningRodQualifiedId)
            lightningRods.Add((location, pair.Key));
        }
      }

      return lightningRods;
    }

    private static void TryStrikeRandomTerrainFeature(Farm farm, Farm.LightningStrikeEvent lightningStrikeEvent)
    {
      try
      {
        if (!Utility.TryGetRandom(farm.terrainFeatures, out var key, out var value)) return;

        if (value is FruitTree fruitTree)
        {
          fruitTree.struckByLightningCountdown.Value = 4;
          fruitTree.shake(key, doEvenIfStillShaking: true);
          lightningStrikeEvent.createBolt = true;
          lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, -128f);
          return;
        }

        Crop? crop = (value as HoeDirt)?.crop;
        bool cropAlive = crop != null && !crop.dead.Value;

        if (value.performToolAction(null, 50, key))
        {
          lightningStrikeEvent.destroyedTerrainFeature = true;
          lightningStrikeEvent.createBolt = true;
          farm.terrainFeatures.Remove(key);
          lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, -128f);
        }

        if (cropAlive && crop!.dead.Value)
        {
          lightningStrikeEvent.createBolt = true;
          lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, 0f);
        }
      }
      catch (Exception ex)
      {
        // Vanilla swallows this silently; keep that behavior but leave a breadcrumb.
        ModServices.Monitor.LogOnce($"Lightning terrain strike skipped: {ex.Message}", LogLevel.Trace);
      }
    }
  }
}
