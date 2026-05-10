using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Equips.Cards.AstirDeck;
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

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class TinyMoon : ModProjectile, IDrawPixelated
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            var Tex = TextureAssets.Projectile[Type];

            /*
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Deferred);

            Texture2D value = DTAssetLib.FaintGlow.Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float num = (Projectile.scale * 1.3f) * (Projectile.oldPos.Length - i) / (Projectile.oldPos.Length * 0.8f);
                Color val4 = Color.MediumPurple * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(value, Projectile.OldCenter()[i] - Main.screenPosition, null, val4, 0f, value.Size() / 2f, num, 0, 0f);
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);

            */

            
            return false;
        }

        public PixelLayer PixelLayer => PixelLayer.AbovePlayer;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            var Tex = TextureAssets.Projectile[Type];

            Texture2D value = DTAssetLib.FaintGlow.Value;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, PixelationSystem.PixelationMatrix);
            
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float num = (Projectile.scale * 1.3f) * (Projectile.oldPos.Length - i) / (Projectile.oldPos.Length * 0.8f);
                Color val4 = Color.MediumPurple with { A = 0 } * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(value, Projectile.OldCenter()[i] - Main.screenPosition, null, val4, 0f, value.Size() / 2f, num, 0, 0f);
            }

            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.White, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.03f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, PixelationSystem.PixelationMatrix);
        }

        Player Owner => Main.player[Projectile.owner];

        public override bool PreAI()
        {
            if (Owner.TryGetModPlayer<LunaPlayer>(out var luna))
            {
                if (luna.Active)
                {
                    return true;
                }
                else
                {
                    Projectile.Kill();
                    return false;
                }
            }
            else
            {
                Projectile.Kill();
                return false;
            }
        }

        float Rot = 0f;
        float Sc = 0f;
        float Dis = 80f;
        public override void AI()
        {
            Projectile.timeLeft = 60;

            Projectile.scale = 1f + Owner.GetAdjustedItemScale(Owner.HeldItem);

            float spd = Opus.Sine(0.02f, 0.09f, 0.01f);
            Rot += spd;
            Sc = Opus.Sine(0.2f, 0.8f, 0.01f);

            Dis = Opus.Sine(-80f, -140f, 0.01f) * Owner.GetTotalAttackSpeed(DamageClass.Generic);

            Lighting.AddLight(Projectile.Center, Color.Lavender.ToVector3() * Sc);

            Projectile.Center = Owner.MountedCenter + new Vector2(Dis, 0).RotatedBy(Rot);

            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.width, DustID.AncientLight, 0f, 0f, 40, default, 1f);
            d.noGravity = true;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.BrokenArmor, 300);
        }
    }
}
