using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.fire;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class DarkFireSpirit : ModProjectile
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
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D T = TextureAssets.Projectile[Type].Value;

            int frameHeight = T.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                T.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(T.Width / 2f, frameHeight / 2f);

            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawGlowOnProj(Projectile, Color.DarkMagenta, true);

            Vector2 drawOrigin = new Vector2(T.Width * 0.5f, T.Height * 0.5f);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2;
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(T, drawPos, frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }


            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(T, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }



        public override void AI()
        {
            AnimateProjectile();
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(4))
            {
                Fire fire = new Fire();
                fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), Color.DarkMagenta, 1f, 100, FireDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(fire);
            }

            Lighting.AddLight(Projectile.Center, Color.DarkMagenta.ToVector3() * 0.2f);

            if (Projectile.timeLeft % 20 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.01f, ModContent.ProjectileType<HomingShadowflame>(), Projectile.damage / 3, 8, Projectile.owner);
            }

        }
    }
}