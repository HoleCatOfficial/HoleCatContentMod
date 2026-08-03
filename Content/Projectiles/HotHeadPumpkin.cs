using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles
{
    public class HotHeadPumpkin : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.HorsemanPumpkin}";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 48;

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

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Texture2D value = DTAssetLib.FaintGlow.Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 val = new(Projectile.width / 2f, Projectile.height / 2f);
                Rectangle val2 = value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame, 0, 0);
                Vector2 val3 = Projectile.oldPos[i] - Main.screenPosition + val;
                float num = Projectile.scale * (Projectile.oldPos.Length - i) / (Projectile.oldPos.Length * 0.8f);
                Color val4 = Color.Orange * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(value, val3, (Rectangle?)val2, val4, Projectile.velocity.ToRotation(), val2.Size() / 2f, num, 0, 0f);
            }
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            SpriteEffects FX = SpriteEffects.None;

            float rot = Projectile.rotation;

            if (rot > MathHelper.PiOver2 || rot < -MathHelper.PiOver2)
            {
                FX = SpriteEffects.FlipVertically;
            }
            else
            {
                FX = SpriteEffects.None;
            }

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, Projectile.scale, FX, 0f);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.2f);
        }
    }
}