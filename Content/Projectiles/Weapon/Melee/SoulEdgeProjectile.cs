using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class SoulEdgeProjectile : ModProjectile
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
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240; // 10 seconds max lifespan
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.tileCollide = false;
        }

        public int trailLength = 10;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = new Color(161, 215, 232);

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects FX = SpriteEffects.None;

            if (Projectile.direction < 0)
            {
                FX = SpriteEffects.FlipHorizontally;
            }
            else
            {

                FX = SpriteEffects.None;
            }

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(DTAssetLib.CutSwing.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, DTAssetLib.CutSwing.Value.Size() / 2, Projectile.scale * 0.4f, FX, 0);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, FX, 0);

            return false;
        }

        public override void AI()
        {
            

            if (Projectile.timeLeft <= 60)
            {
                Projectile.velocity *= 0.88f;
                Projectile.Opacity *= 0.99f;
            }
            else
            {
                if (Projectile.velocity.Length() < 18)
                {
                    Projectile.velocity *= 1.15f;
                }
            }

            
            // Always spinning
            Projectile.rotation += (Projectile.velocity.Length() * 0.06f) * Projectile.direction;

            // Generate flying dust effect
            if (Main.rand.NextBool(3)) // 33% chance per tick
            {
                //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(3, 3), new Color(184, 228, 242), 0.5f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Chilled, 240);
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 300);
        }

    }
}
