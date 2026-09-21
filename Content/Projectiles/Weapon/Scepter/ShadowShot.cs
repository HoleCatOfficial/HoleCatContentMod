using System.Collections.Generic;
using System.IO;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class ShadowShot : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => TSpeed;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 10f;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => Projectile.timeLeft > 60;

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
            Projectile.timeLeft = 240;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Mult = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (Color.DarkMagenta with { A = 0 } * 0.2f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (Color.DarkMagenta with { A = 0 } * 0.3f * Mult) * Projectile.Opacity, i == 0 ? Projectile.velocity.ToRotation() : Projectile.oldRot[i], DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.1f, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));
            return false;
        }

        float TSpeed = 1f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            TSpeed += 0.1f;

            if (Projectile.timeLeft < 60)
            {
                Projectile.velocity *= 0.96f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowShotExplosion>(), Projectile.damage / 2, 4, Projectile.owner);
        }
    }

    public class ShadowShotExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
        }

        public override void SetStaticDefaults()
        {

        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact, Projectile.Center);

            ShockwaveExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Magenta, 0.01f, 0.001f, 0.2f, BlendState.Additive);
            ParticleEngine.Particles.Add(Explosion);

            FireRing fire1 = new();
            fire1.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta, 0.01f, 0.001f, 0.2f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire1);

            FireRing fire2 = new();
            fire2.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta, 0.01f, 0.001f, 0.15f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire2);

            FireRing fire3 = new();
            fire3.Prepare(Projectile.Center, Vector2.Zero, Color.DarkMagenta, 0.01f, 0.001f, 0.1f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire3);

            for (int i = 0; i < 9; i++)
            {
                HeatseekerSilohSpark spark = new();
                spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), 0f, Color.Magenta, 1f, false, 30, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark);
            }
        }

        public override void AI()
        {

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }
    }
}