using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter.Power
{
    public class FusionSplittingDart : ModProjectile, IDrawPixelated, IHomingProjectile
    {

        public float DelayTimer;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 8f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => DelayTimer >= 35;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 150;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            Opus.DrawTextureOnProj(DTAssetLib.MiscSparkle144, Projectile, ColorLib.CelestialGradient with { A = 0 }, false, Projectile.rotation + MathHelper.PiOver2);
            Opus.DrawTextureOnProj(DTAssetLib.MiscSparkle144, Projectile, ColorLib.CelestialGradient with { A = 0 }, false, Projectile.rotation, ScaleY: 2f);

            Opus.DrawTextureOnProj(DTAssetLib.MiscSparkle144, Projectile, Color.White with { A = 0 }, false, Projectile.rotation + MathHelper.PiOver2);
            Opus.DrawTextureOnProj(DTAssetLib.MiscSparkle144, Projectile, Color.White with { A = 0 }, false, Projectile.rotation, ScaleY: 2f);

            return false;
        }


        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 35 && Projectile.ManualCanHitFriendly(target);
        }


        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, ColorLib.Stardust.ToVector3());

            if (DelayTimer < 35)
            {
                DelayTimer++;
                return;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.DarkMagicImpact, Projectile.Center);
                BloomRingSharp Ring = new();
                Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.CelestialGradient, 0.3f, 0.01f, 2f, BlendState.Additive);
                ParticleEngine.Particles.Add(Ring);

                for (int i = 0; i < 14; i++)
                {
                    StarParticle star = new();
                    star.Initialize(Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), ColorLib.CelestialGradient, 1f);
                    ParticleEngine.Particles.Add(star);
                }

                Opus.RadialSpreadProjectile(ModContent.ProjectileType<FusionSmallDart>(), 8, Projectile.Center, Projectile.damage / 2, 0, 4, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            trailOffset += 0.01f;

            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.Additive, DTAssetLib.Streak(1, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 12, ColorLib.CelestialGradient with { A = 0 }, trailOffset, 10);

            spriteBatch.ResetToDefault();
        }
    }
}