using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class ExasperationDart : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D T = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(T.Width * 0.5f, T.Height * 0.5f);

            SpriteBatch spriteBatch = Main.spriteBatch;

            Main.EntitySpriteDraw(T, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawGlowOnProj(Projectile, Color.DarkMagenta, true);

            
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2;
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(T, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }


            Opus.ReturnToDefaultDrawing(spriteBatch);

            

            return false;
        }



        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(Opus.Sine(-0.2f, 0.2f, 0.5f));
            Projectile.rotation = (Projectile.velocity.ToRotation() ) + MathHelper.PiOver4;

            if (Main.rand.NextBool(12))
            {
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center + Main.rand.NextVector2Circular(10, 10), Vector2.Zero, Color.Indigo * 0.5f, 0.1f, 40, ai2: 2);
            }

            Lighting.AddLight(Projectile.Center, Color.Indigo.ToVector3() * 0.2f);

        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowExplosion>(), Projectile.damage / 2, 10, Projectile.owner);
        }
    }
}