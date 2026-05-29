using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace MPF_Code
{
    internal sealed partial class ModEntry
    {
        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.IsMasterGame)
                return;

            if (Game1.player.mailbox.Count <= 0)
                return;

            if (Game1.currentLocation.GetData()?.CanPlantHere != true)
                return;

            var homeLocationName = Game1.player.homeLocation.Value;
            if (string.IsNullOrWhiteSpace(homeLocationName))
                return;

            if (!HasMatchingCabin(Game1.currentLocation, homeLocationName))
                return;

            DrawMailboxNotification(e.SpriteBatch, Game1.player.getMailboxPosition());
        }

        private static bool HasMatchingCabin(GameLocation location, string homeLocationName)
        {
            foreach (Building building in location.buildings)
            {
                if (building.isCabin && building.HasIndoorsName(homeLocationName))
                    return true;
            }

            return false;
        }

        private static void DrawMailboxNotification(SpriteBatch spriteBatch, Point mailboxPosition)
        {
            float bob = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
            float layerDepth = ((mailboxPosition.X + 1) * 64f) / 10000f + (mailboxPosition.Y * 64f) / 10000f;

            spriteBatch.Draw(
                Game1.mouseCursors,
                Game1.GlobalToLocal(Game1.viewport,
                    new Vector2(mailboxPosition.X * 64, mailboxPosition.Y * 64 - 96 - 48 + bob)),
                new Rectangle(141, 465, 20, 24),
                Color.White * 0.75f,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                layerDepth + 0.000001f);

            spriteBatch.Draw(
                Game1.mouseCursors,
                Game1.GlobalToLocal(Game1.viewport,
                    new Vector2(mailboxPosition.X * 64 + 32 + 4, mailboxPosition.Y * 64 - 64 - 24 - 8 + bob)),
                new Rectangle(189, 423, 15, 13),
                Color.White,
                0f,
                new Vector2(7f, 6f),
                4f,
                SpriteEffects.None,
                layerDepth + 0.00001f);
        }
    }
}
