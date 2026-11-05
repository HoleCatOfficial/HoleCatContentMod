using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class MoonlordFusionBolt : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 80;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item124, Projectile.Center);
        }
        float offset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            offset -= 0.02f;
            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(2).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 30f, ColorLib.CelestialGradient, offset);

            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.Center - Main.screenPosition, null, ColorLib.CelestialGradient with { A = 0 }, Projectile.rotation, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 2f, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.Star(1).Value, Projectile.Center - Main.screenPosition, null, ColorLib.CelestialGradient with { A = 0 }, Projectile.rotation, DTAssetLib.Star(1).Value.Size() / 2, new Vector2(0.6f, 0.2f), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Star(1).Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, DTAssetLib.Star(1).Value.Size() / 2, new Vector2(0.6f, 0.2f) * 0.5f, SpriteEffects.None);

            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.ResetExcessTrailPoints();

            /*
            Spark spark2 = new();
            spark2.PrepareSpark(Projectile.Center + new Vector2(0, Opus.Sine(-6f, 6f, 0.3f)).RotatedBy(Projectile.velocity.ToRotation()), Projectile.velocity * 0.1f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.CelestialGradient, 0.1f, false, 9, SparkDrawMode.Additive, 3f);
            ParticleEngine.Particles.Add(spark2);

            Spark spark3 = new();
            spark3.PrepareSpark(Projectile.Center + new Vector2(0, Opus.Sine(6f, -6f, 0.3f)).RotatedBy(Projectile.velocity.ToRotation()), Projectile.velocity * 0.1f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.CelestialGradient, 0.1f, false, 9, SparkDrawMode.Additive, 3f);
            ParticleEngine.Particles.Add(spark3);
            */
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 240);
        }


        public override void OnKill(int timeLeft)
        {

        }
    }
}
