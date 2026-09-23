using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class SpectrumBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color baseColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);

            Vector3 HSL = Main.rgbToHsl(baseColor);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = (float)i / Projectile.oldPos.Length;

                float shiftedHue = (HSL.X - progress) % 1f;

                float Mult = MathHelper.Lerp(1f, 0f, progress);

                Color col = Main.hslToRgb(new Vector3(shiftedHue, HSL.Y, HSL.Z));

                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (col with { A = 0 } * 0.2f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.StarAura.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (col with { A = 0 } * 0.5f * Mult) * Projectile.Opacity, Projectile.oldRot[i], DTAssetLib.StarAura.Value.Size() / 2, 0.5f, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Main.DiscoColor with { A = 0 }));
            return false;
        }

        float TSpeed = 1f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            TSpeed += 0.1f;

            Lighting.AddLight(Projectile.Center, Main.DiscoColor.ToVector3() * 0.5f);

            if (Projectile.timeLeft < 60)
            {
                Projectile.velocity *= 0.96f;
                Projectile.Opacity -= 0.02f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2[] dirs = Opus.GetEquidistantVectors(4, target.Center, 4, Main.rand.NextFloat(MathHelper.TwoPi));

            SoundEngine.PlaySound(DTAssetLib.Impacts.EnergyBounce with { PitchVariance = 0.4f }, Projectile.Center);

            for (int i = 0; i < dirs.Length; i++)
            {
                HeatseekerSilohSpark spark = new();
                spark.PrepareSpark(target.Center, dirs[i].DirectionFrom(target.Center) * 6, dirs[i].ToRotation() + MathHelper.PiOver2, Main.DiscoColor, 0.6f, false, 30, SparkDrawMode.Additive, 3f);
                ParticleEngine.Particles.Add(spark);
            }
        }
    }
}
