using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class NodeDefilementAura : ModProjectile, IDrawPixelated
    {
        public override string Texture => DTUtils.NoTexture;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AbovePlayer;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public float EffectRadius = 0;

        public float MaxEffectRadius = 200f;

        float r = 0f;

        bool IDrawPixelated.ShouldDrawPixelated => true;

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            r += 0.02f;

            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            var tex = DTAssetLib.BarrierRing.Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, col with { A = 0 }, r, tex.Size() / 2, tex.ScaleRingTextureToMatchRadius(EffectRadius, 1300), SpriteEffects.None);

            spriteBatch.ResetToDefault();
        }

        Color col = Color.Black;
        public override void AI()
        {
            if (Main.npc[(int)Projectile.ai[2]].active)
            {
                Projectile.Center = Main.npc[(int)Projectile.ai[2]].Center;
                Projectile.ai[0]++;

                col = OpusColorUtils.MultiLerp((EffectRadius / 1f).Inverse(), ColorLib.WretchedColorMap);

                if (Projectile.ai[0] < 90)
                {
                    EffectRadius = MathHelper.Lerp(0f, MaxEffectRadius, Projectile.ai[0] / 90f);
                }
                if (Projectile.ai[0] > 210)
                {
                    Projectile.ai[1]++;
                    EffectRadius = MathHelper.Lerp(MaxEffectRadius, 0f, Projectile.ai[1] / 90f);
                }
            }
            else
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShatter, Projectile.Center);

                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Item.NewItem(Projectile.GetSource_Death(), Projectile.Hitbox, ItemID.Heart);
                }
                Opus.RingSpreadDust(DustID.CursedTorch, 40, Projectile.Center, EffectRadius, 40, default, 3f, 6f);
                Projectile.Kill();
            }


        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, EffectRadius, targetHitbox);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Defilement>(), 300);
        }
    }
}
