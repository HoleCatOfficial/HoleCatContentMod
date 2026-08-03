using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

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

            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), Color.Indigo, 1f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire);

            Fire fireX = new Fire();
            fireX.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), DTColorUtils.Pastel(Color.Purple, 0.4f), 0.6f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fireX);

            if (Main.rand.NextBool(4))
            {
                Fire fire2 = new Fire();
                fire2.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), Color.DarkMagenta, 1f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
                ParticleEngine.BehindProjectiles.Add(fire2);

                Fire fire3 = new Fire();
                fire3.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), DTColorUtils.Pastel(Color.HotPink, 0.5f), 0.5f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
                ParticleEngine.BehindProjectiles.Add(fire3);
            }

            Lighting.AddLight(Projectile.Center, Color.DarkMagenta.ToVector3() * 0.2f);

        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowExplosion>(), Projectile.damage / 2, 10, Projectile.owner);
        }
    }
}