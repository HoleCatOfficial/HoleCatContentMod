using System;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Thrower
{
    public class TenebrousChakramThrown : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects fx = Math.Sign(Projectile.velocity.X) == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(DTAssetLib.CircularSwingThin.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, Projectile.rotation + MathHelper.PiOver4, DTAssetLib.CircularSwingThin.Value.Size() / 2, 0.7f, fx);
            Main.EntitySpriteDraw(DTAssetLib.CircularSwingThin.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, Projectile.rotation + MathHelper.Pi + MathHelper.PiOver4, DTAssetLib.CircularSwingThin.Value.Size() / 2, 0.7f, fx);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, lightColor) with { effect = fx });
            return false;
        }

        public override void AI()
        {
            Projectile.rotation += 1f * Projectile.direction;
            Projectile.velocity *= 0.97f;
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 5 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 0, PitchVariance = 0.5f, Pitch = -0.3f }, Projectile.Center);
            }

            if (Projectile.ai[0] > 120)
            {
                if (Projectile.ai[0] % 60 == 0)
                {
                    SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);
                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarFriendly>(), 4, Projectile.Center, Projectile.damage / 2, 3, 12f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                }
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit, target.Center);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, 50, targetHitbox);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(DTAssetLib.Impacts.MetalImpact with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);

            Projectile.penetrate--;
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.75f;

            return Projectile.penetrate <= 0;
        }

    }
}

