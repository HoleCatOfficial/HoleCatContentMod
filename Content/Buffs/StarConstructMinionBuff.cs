
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using Humanizer;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	// This class serves as an example of a debuff that causes constant loss of life
	// See ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information
	public class StarConstructMinionBuff : ModBuff
    {
        public static readonly int FrameCount = 8; // Amount of frames we have on our animation spritesheet.
		public static readonly int AnimationSpeed = 4; // In ticks.
        public static readonly string AnimationSheetPath = "DestroyerTest/Content/Buffs/StarConstructMinionBuffAnim";
        private Asset<Texture2D> animatedTexture;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
            animatedTexture = ModContent.Request<Texture2D>(AnimationSheetPath);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            // You can use this hook to make something special happen when the buff icon is drawn (such as reposition it, pick a different texture, etc.).

            // We draw our special texture here with a specific animation.

            // Use our animation spritesheet.
            Texture2D ourTexture = animatedTexture.Value;
            // Choose the frame to display, here based on constants and the game's tick count.
            Rectangle ourSourceRectangle = ourTexture.Frame(verticalFrames: FrameCount, frameY: (int)Main.GameUpdateCount / AnimationSpeed % FrameCount);

            // Other stuff you can do in this hook
            /*
			// Here we make the icon have a lime green tint.
			drawParams.drawColor = Color.LimeGreen * Main.buffAlpha[buffIndex];
			*/

            // Be aware of the fact that drawParams.mouseRectangle exists: it defaults to the size of the autoloaded buffs' sprite,
            // it handles mouseovering and clicking on the buff icon. Since our frame in the animation is 32x32 (same as the autoloaded sprite),
            // and we don't change drawParams.position, we don't have to do anything. If you offset the position, or have a non-standard size, change it accordingly.

            // We have two options here:
            // Option 1 is the recommended one, as it requires less code.
            // Option 2 allows you to customize drawing even more, but then you are on your own.

            // For demonstration, both options' codes are written down, but the latter is commented out using /* and */.

            // OPTION 1 - Let the game draw it for us. Therefore we have to assign our variables to drawParams:
            drawParams.Texture = ourTexture;
            drawParams.SourceRectangle = ourSourceRectangle;
            // Return true to let the game draw the buff icon.
            return true;

            /*
			// OPTION 2 - Draw our buff manually:
			spriteBatch.Draw(ourTexture, drawParams.position, ourSourceRectangle, drawParams.drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			// Return false to prevent drawing the icon, since we have already drawn it.
			return false;
			*/
        }

        int grace = 120;
		public override void Update(Player player, ref int buffIndex) {
			// If the minions exist reset the buff time, otherwise remove the buff from the player
            if (grace > 0)
            {
                grace--;
            }
			if ((player.ownedProjectileCounts[ModContent.ProjectileType<StarConstructMinion>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<StarConstructInactive>()] > 0) || grace > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}