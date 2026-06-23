using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Vanilla
{

    public class BloodButchererSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 64;
            SweepColor = Color.Red;
            SwingSpeed = 0.08f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.EvilSwing with { PitchVariance = 0.6f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            //npc.AddBuff(BuffID.BloodButcherer, 300);

            Vector2 toOwner = npc.Center - Owner.Center;
            toOwner.Normalize();
            Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center + (toOwner.RotatedByRandom(0.4f) * -3), Vector2.Zero, ProjectileID.BloodButcherer, Projectile.damage / 8, 0f, Owner.whoAmI);
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.LightGoreCut with { MaxInstances = 0, Pitch = -0.6f, PitchVariance = 1f, Volume = 0.6f }, npc.Center);
        }

        private void DrawSweepFX2()
        {
            Player player = Main.player[Projectile.owner];
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash").Value;
            float TexBasedMod = (Projectile.Size.Length() * 0.015f);
            float rOffset = 0f;

            SpriteEffects FX = SpriteEffects.None;

            if (LastSwing == 1)
            {
                FX = SpriteEffects.FlipHorizontally;
                rOffset = MathHelper.PiOver2;
            }
            else
            {
                FX = SpriteEffects.None;
                rOffset = 0f;
            }

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, (Color.Red * SweepOpacity) * 0.5f, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);


            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }
        public override void DrawUnderBlade()
        {
            DrawSweepFX2();
        }
        public override void DrawOverBlade()
        {

        }

        public override void OnStartSwing()
        {
           
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {

            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[10..30];

            if (Main.rand.NextBool(2))
            { 
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(ppt[Main.rand.Next(15)], DustID.Blood, SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, default, 1.8f);
                    //PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], pt[Main.rand.Next(30)], SwordLine.GetLineRotation.ToRotationVector2() * 2, ColorLib.Wretched3, 0.5f, 20, ai2: 2);
                }
            }

            ScaleMult = 1f;
        }
    }
}