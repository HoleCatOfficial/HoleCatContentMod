using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.NightmareRose
{
    // This Example shows how to implement a simple homing projectile with animation
    public class CorruptPetalHostile : ModProjectile
    {
        // Correct asset path
        public override string Texture => "DestroyerTest/Content/Projectiles/CorruptPetal";

    

        public override void SetStaticDefaults() {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            Main.projFrames[Projectile.type] = 4; // Set the number of frames in the sprite sheet
        }

        public override void SetDefaults()
        {
            Projectile.width = 30; // The width of projectile hitbox
            Projectile.height = 14; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.frame = 0; // Start at the first frame
            Projectile.netImportant = true;
			Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawGlowOnProj(Projectile, Color.Purple, false);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return true;
        }


        // Custom AI
        public override void AI()
        {
            // Handle animation
            AnimateProjectile();
            Projectile.rotation = Projectile.velocity.ToRotation();
            
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int g = 0; g < 4; g++)
            {
                Dust Trail = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 2f);
                Trail.noGravity = true;
            }
        }

        private void AnimateProjectile()
        {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

    }
}