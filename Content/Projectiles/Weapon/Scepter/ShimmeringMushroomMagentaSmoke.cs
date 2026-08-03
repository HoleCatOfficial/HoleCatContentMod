using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using System;
 
using DestroyerTest.Content.Particles;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class ShimmeringMushroomMagentaSmoke : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.frame = Main.rand.Next(3);
            Projectile.Opacity = 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
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
                    lightColor * Projectile.Opacity,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            return false;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.99f;
            Projectile.rotation += 0.03f * Projectile.velocity.X;
            
            if (Projectile.timeLeft < 60)
            {
                Projectile.scale += 0.01f;
                Projectile.Opacity -= 0.05f;
            }

            //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Vector2.Zero, ColorLib.TenebrisMagenta * 0.6f, Main.rand.NextFloat(0.5f, 1f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 600);
        }
    }

}

