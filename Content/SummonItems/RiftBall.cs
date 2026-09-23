
using System;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.SummonItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems
{
	public class RiftBall : ModProjectile, IDrawPixelated
	{
        public override string Texture => DTUtils.NoTexture;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        public override void SetStaticDefaults() 
        {
			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.LightPet[Projectile.type] = true;
		}

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 60;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.CloneDefaults(ProjectileID.FairyQueenPet);
            Projectile.netImportant = true;
		}

        float roff = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            roff += 0.3f;
            

            return false;
        }

        float Scale = 0f;
		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];

            Scale = Opus.Sine(0.1f, 0.15f);

            // If the player is no longer active (online) - deactivate (remove) the projectile.
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            // Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
            if (!player.dead && player.HasBuff(ModContent.BuffType<RiftBallBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            if (!player.HasBuff(ModContent.BuffType<RiftBallBuff>()))
            {
                Projectile.Kill();
            }


            Vector2 targetPos = player.Center + new Vector2(0, Projectile.ai[0] - 40) + new Vector2(player.velocity.X, player.velocity.Y);
            int ForcedMovementSpeed = 3;
            if (player.controlUp)
            {
                Projectile.ai[0] -= ForcedMovementSpeed;
            }
            else if (player.controlDown)
            {
                Projectile.ai[0] += ForcedMovementSpeed;
            }

            if (player.controlUp || player.controlDown)
            {
                Projectile.Center = Vector2.SmoothStep(Projectile.Center, targetPos, 0.1f);
            }

            Projectile.ai[0] = MathHelper.Clamp(Projectile.ai[0], -40 , 40 * 3);
            Projectile.velocity = Vector2.SmoothStep(Projectile.velocity += Projectile.Center.DirectionTo(targetPos) * (Projectile.Center.Distance(targetPos) * 0.01f), Projectile.Center.DirectionTo(targetPos) * 3, 0.1f);
            if (Projectile.Center.Distance(targetPos) < 10)
            {
                Projectile.velocity *= 0.8f;
            }
            if (Projectile.Center.Distance(targetPos) < 3 && Projectile.velocity.Length() < 1)
            {
                Projectile.velocity *= 0f;
            }
            float MaxSpeed = MathHelper.Clamp(Projectile.Center.Distance(targetPos) * 0.05f, 6, 12);
            Projectile.velocity = Vector2.Clamp(Projectile.velocity, new Vector2(-MaxSpeed), new Vector2(MaxSpeed));
            
            if (!player.controlDown && !player.controlUp && Projectile.Center.Distance(targetPos) < 100)
            {
                Projectile.velocity *= 0.95f;
            }
            
            if (Projectile.Center.Distance(player.Center) > 1000)
                Projectile.Center = player.Center;

            Projectile.rotation = Projectile.velocity.X * 0.06f;


            if (!Main.dedServ) 
            {
                Lighting.AddLight(Projectile.Center, Projectile.Opacity * 2.55f, Projectile.Opacity * 1.55f, Projectile.Opacity * 0.0f);
            }
        }

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            var Cap = spriteBatch.Capture();
            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.End();
            spriteBatch.Begin(Cap);

            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, ColorLib.DarkRift2 with { A = 0 } * 0.5f, -roff, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.3f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift with { A = 0 }, roff, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.22f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift with { A = 0 }, roff * 0.5f, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.20f) * Projectile.scale, SpriteEffects.None, 0);


            Main.EntitySpriteDraw(DTAssetLib.Circle.Value, Projectile.Center - Main.screenPosition, null, Color.Black, Projectile.rotation, DTAssetLib.Circle.Value.Size() / 2, (Scale * 0.5f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.BloomRing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift with { A = 0 }, roff * 0.5f, DTAssetLib.BloomRing.Value.Size() / 2, (Scale * 2.2f) * Projectile.scale, SpriteEffects.None, 0);

            spriteBatch.ResetToDefault();
        }
    }
}
