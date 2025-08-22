using System.Collections.Generic;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class IchorNodeCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.Ichor;
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D GlowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SimpleParticle").Value;
			
			Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

            Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            for (int i = 0; i < TrailPositions.Count - 1; i++)
            {
                Vector2 start = TrailPositions[i] - Main.screenPosition;
                Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
                Vector2 diff = end - start;

                float length = diff.Length();
                if (length < 0.5f)
                    continue; // skip tiny wiggle segments

                float rotation = diff.ToRotation();

                float width = MathHelper.Lerp(0.01f, 0.0007f, i / (float)TrailLength);
                float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);
                Color color = lightColor * alpha;

                // Instead of stepping pixel by pixel, just draw one scaled pixel segment:
                Main.spriteBatch.Draw(
                    pixel,
                    start,
                    null,
                    color,
                    rotation,
                    new Vector2(pixel.Width / 2, pixel.Height / 2), // Origin is at the left-middle of the scaled pixel
                    new Vector2(length, width),
                    SpriteEffects.None,
                    0f
                );
            }

            Main.spriteBatch.Draw(
                    GlowTexture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    new Vector2(GlowTexture.Width / 2, GlowTexture.Height / 2), // Origin is at the left-middle of the scaled pixel
                    1f,
                    SpriteEffects.None,
                    0f
                );


            Utility.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }

        public bool CanReflect = true;
        int Collisions = 0;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) {
				Projectile.velocity.X = -oldVelocity.X;
				}
			if (Projectile.velocity.Y != oldVelocity.Y) {
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			Projectile.velocity *= 0.75f;
            Collisions++;
            if (Collisions > 3)
            {
                return true;
            }
            if (CanReflect == false)
            {
                return true;
            }
            return false;
        }


        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 40;
        public override void AI()
        {
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 240);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/CrystalBreak") with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);
            for (int g = 0; g < 4; g++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
            }
        }
    }
}