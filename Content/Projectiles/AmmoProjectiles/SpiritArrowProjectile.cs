using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class SpiritArrowProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 480;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(1, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 10, ColorLib.LifeEcho, 0f);
            Main.spriteBatch.ResetToDefault();
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                PointGlowPreMultiplied Particle = new();
                Particle.Initialize(Projectile.Center, Main.rand.NextVector2Circular(4, 4), ColorLib.LifeEcho, 0.75f);
                ParticleEngine.Particles.Add(Particle);
            }
        }
    }
}