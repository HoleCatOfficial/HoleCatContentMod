using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class SoulSlugProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 4;
        }

        float SC = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            SC -= 0.4f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (Projectile.oldPos.Length > 2)
            {
                DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(3).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 10, ColorLib.Soul, SC, 5);
            }
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Projectile.velocity * 0.5f, 0, ColorLib.Soul3, 0.5f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.IceImpact with { Pitch = 0.7f, PitchVariance = 0.2f, Volume = 0.5f }, Projectile.Center);

            SimpleExplosionParticle Explosion = new SimpleExplosionParticle();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Soul, 0.3f, 0.001f, 1f, BlendState.Additive);
            ParticleEngine.Particles.Add(Explosion);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, Color.White, 0.1f, 0.001f, 0.2f, BlendState.Additive);
            ParticleEngine.Particles.Add(Ring);
           

            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 2, Projectile.Center, 0, Color.White, 2f, 3f);
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 3, Projectile.Center, 0, Color.White, 0.6f, 0.5f);
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 5, Projectile.Center, 70, ColorLib.Soul, 1f, 0.7f);
        }
    }
}