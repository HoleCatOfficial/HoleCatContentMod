using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Equips.Cards.AstirDeck;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class TinySun : ModProjectile, IDrawPixelated 
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
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
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
                Color val4 = Color.OrangeRed * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(value, Projectile.OldCenter()[i] - Main.screenPosition, null, val4, 0f, value.Size() / 2f, num, 0, 0f);
            }

            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.5f, Rot, Tex.Value.Size() / 2, Projectile.scale * Sc, SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.5f, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.65f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.Gold, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.4f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.White, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.3f, SpriteEffects.None, 0f);
            

            Opus.ReturnToDefaultDrawing(spriteBatch);
            */
            return false;
        }

        public PixelLayer PixelLayer => PixelLayer.AbovePlayer;

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            var Tex = TextureAssets.Projectile[Type];

            Texture2D value = DTAssetLib.FaintGlow.Value;

            var Cap = spriteBatch.Capture();

            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            
            spriteBatch.Begin(Cap);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float num = (Projectile.scale * 1.3f) * (Projectile.oldPos.Length - i) / (Projectile.oldPos.Length * 0.8f);
                Color val4 = Color.OrangeRed with { A = 0 } * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(value, Projectile.OldCenter()[i] - Main.screenPosition, null, val4, 0f, value.Size() / 2f, num, 0, 0f);
            }

            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 } * 0.15f, Rot, Tex.Value.Size() / 2, Projectile.scale * Sc, SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 } * 0.25f, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.65f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.Gold with { A = 0 }, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.4f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.3f, SpriteEffects.None, 0f);

            spriteBatch.ResetToDefault();
        }


        Player Owner => Main.player[Projectile.owner];

        public override bool PreAI()
        {
            if (Owner.TryGetModPlayer<SolaPlayer>(out var sola))
            {
                if (sola.Active)
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

            Dis = Opus.Sine(80f, 140f, 0.01f) * Owner.GetTotalAttackSpeed(DamageClass.Generic);

            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * Sc);

            Projectile.Center = Owner.MountedCenter + new Vector2(Dis, 0).RotatedBy(Rot);

            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.width, DustID.Firefly, 0f, 0f, 40, default, 1f);
                d.noGravity = true;
                d.velocity *= 0.5f;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
