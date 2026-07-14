using DestroyerTest.Common;
using DestroyerTest.Content.Particles.Stellar;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class CommittmentThrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.netImportant = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D P = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(P, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, P.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ManualCanHitFriendly(target);
        }

        public bool returning = false;
        public Player Owner => Main.player[Projectile.owner];
        public override void AI()
        {

            if (!returning)
            {
                Projectile.velocity *= 0.96f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;


                //PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Vector2.Zero, default, 1f);

                if (Projectile.velocity.Length() <= 0.8f)
                {
                    returning = true;
                }
            }
            else
            {
                Vector2 toOwner = Owner.Center - Projectile.Center;
                toOwner.Normalize();

                //Projectile.velocity = Vector2.Lerp(Projectile.velocity, toOwner * 64, 0.1f);
                //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;

                Owner.velocity = Vector2.Lerp(Owner.velocity, toOwner * -32, 0.1f);

                if (Projectile.Center.Distance(Owner.Center) < 30)
                {
                    Projectile.Kill();
                    Owner.gravity = 1f;
                    Owner.velocity *= 0.25f;
                }
                else
                {
                    Owner.gravity = 0f;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D T = TextureAssets.Projectile[Type].Value;
            Vector2 start = Projectile.Center - ((T.Size() / 2) * Projectile.scale).RotatedBy(Projectile.rotation);
            Vector2 end = Projectile.Center + ((T.Size() / 2) * Projectile.scale).RotatedBy(Projectile.rotation);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }
    }
}
