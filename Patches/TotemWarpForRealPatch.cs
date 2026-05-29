using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace MPF_Code.Patches
{
    internal static class TotemWarpForRealPatch
    {
        public static bool Prefix(StardewValley.Object __instance)
        {
            switch (__instance.QualifiedItemId)
            {
                case "(O)688":
                {
                    FarmHouse? homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
                    if (homeOfFarmer is not null)
                    {
                        Point frontDoorSpot = homeOfFarmer.getFrontDoorSpot();
                        Game1.warpFarmer(homeOfFarmer.GetParentLocation().Name, frontDoorSpot.X, frontDoorSpot.Y, flip: false);
                    }
                    else if (!Game1.getFarm().TryGetMapPropertyAs("WarpTotemEntry", out Point parsed, required: false))
                    {
                        parsed = Game1.whichFarm switch
                        {
                            6 => new Point(82, 29),
                            5 => new Point(48, 39),
                            _ => new Point(48, 7),
                        };

                        Game1.warpFarmer("Farm", parsed.X, parsed.Y, flip: false);
                    }
                    else
                    {
                        Game1.warpFarmer("Farm", parsed.X, parsed.Y, flip: false);
                    }

                    break;
                }
                case "(O)689":
                    Game1.warpFarmer("Mountain", 31, 20, flip: false);
                    break;
                case "(O)690":
                    Game1.warpFarmer("Beach", 20, 4, flip: false);
                    break;
                case "(O)261":
                    Game1.warpFarmer("Desert", 35, 43, flip: false);
                    break;
                case "(O)886":
                    Game1.warpFarmer("IslandSouth", 11, 11, flip: false);
                    break;
                default:
                    return true;
            }

            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
            return false;
        }
    }
}
