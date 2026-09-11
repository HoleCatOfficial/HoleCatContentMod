using System.Collections.Generic;
using System.IO;
using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter.Power
{
    public class FusionHighVelocityDart : ModProjectile, IDrawPixelated
    {

        public float DelayTimer;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 150;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = -1;
        }

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);



            Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    ColorLib.CelestialGradient with { A = 0 },
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }

        public void DustSpawn1()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, -12).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, 12).RotatedBy(Projectile.rotation + MathHelper.PiOver2);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.5f);

            Spark spark1 = new();
            spark1.PrepareSpark(DustPos, -Projectile.velocity * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.CelestialGradient, 0.4f, false, 30, SparkDrawMode.Additive, 2.6f);
            ParticleEngine.Particles.Add(spark1);

            Vector2 Pos3 = Projectile.Center + new Vector2(0, 12).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
            Vector2 Pos4 = Projectile.Center + new Vector2(0, -12).RotatedBy(Projectile.rotation + MathHelper.PiOver2);

            Vector2 DustPos2 = Opus.Sine(Pos3, Pos4, 0.5f);

            Spark spark2 = new();
            spark2.PrepareSpark(DustPos2, -Projectile.velocity * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.CelestialGradient, 0.4f, false, 30, SparkDrawMode.Additive, 2.6f);
            ParticleEngine.Particles.Add(spark2);
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.rotation = Projectile.velocity.ToRotation();
            AnimateProjectile();

            Lighting.AddLight(Projectile.Center, ColorLib.CelestialGradient.ToVector3());

            DustSpawn1();

            if (DelayTimer < 35)
            {
                DelayTimer++;
                return;
            }

            AnimateProjectile();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {  
            SoundEngine.PlaySound(DTAssetLib.Impacts.Malevolence with { Pitch = -0.5f }, Projectile.Center);

            BloomRingSharp Ring = new();
            Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.CelestialGradient, 0.3f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.Particles.Add(Ring);

            for (int i = 0; i < 14; i++)
            {
                StarParticle star = new();
                star.Initialize(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), ColorLib.CelestialGradient, 1f);
                ParticleEngine.Particles.Add(star);
            }

            if (DTCrossMod.CalamityIsLoaded && DTCrossMod.CalamityMod != null)
            {
                if (DTCrossMod.CalamityMod.TryFind("ElementalMix", out ModBuff ElementalMix))
                {
                    target.AddBuff(ElementalMix.Type, 300);
                }
            }
        }

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            trailOffset += 0.01f;

            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.Additive, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 12, ColorLib.Vortex with { A = 0 }, trailOffset, 10);

            spriteBatch.ResetToDefault();
        }
    }
}