using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class SoulVolley : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        private Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture);
        public override bool PreDraw(ref Color lightColor)
        {
            /*
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Mult = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                Color color = Color.Lerp(ColorLib.Soul2, ColorLib.Soul3, (float)i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (color with { A = 0 } * 0.7f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

                Main.EntitySpriteDraw(ProjTex.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (color with { A = 0 } * 0.8f * Mult) * Projectile.Opacity, Projectile.oldRot[i] + MathHelper.PiOver2, ProjTex.Value.Size() / 2, 1f, SpriteEffects.None);
            }
            */

            Opus.DrawTextureOnProj(ProjTex, Projectile, ColorLib.Soul with { A = 0 }, true, Projectile.rotation, 1f, 0.2f * Projectile.velocity.Length());

            return false;
        }


   
        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, ColorLib.Soul.ToVector3() * 0.2f);

            Projectile.velocity.Y += 0.2f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

            target.AddBuff(ModContent.BuffType<SoulInferno>(), 300);

        }

        public override void OnKill(int timeLeft)
        {

        }

    }
    
}
