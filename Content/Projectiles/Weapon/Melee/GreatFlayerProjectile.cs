using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
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

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

	public class GreatFlayerProjectile : BaseBroadswordProjectile
	{
        public override string Texture => "DestroyerTest/Content/MeleeWeapons/GreatFlayer";
        public override SoundStyle Swing => new SoundStyle("DestroyerTest/Assets/Audio/Constitution/ConSwing", 6);
		public SoundStyle Tooth = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/ToothShoot") with { MaxInstances = 0 };
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 140;
            Projectile.height = 142;
            UsesDefaultSweepFX = true;
            SweepColor = ColorLib.Ichor;
            SweepHighlightColor = ColorLib.IchorCrystal4;
            SwingSpeed = 0.13f;
            WaitTimeMultiplier = 3f;
            ScaleMult = 1.1f;
            Glowmask = ModContent.Request<Texture2D>(Texture + "_Glow");
        }


        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.TenebrisSwing with { MaxInstances = 0, PitchVariance = 0.2f, Pitch = -0.7f, Volume = 0.1f }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { MaxInstances = 0, PitchVariance = 0.4f, Pitch = -0.7f }, npc.Center);

           
            Owner.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 1;
            Owner.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 20;

            for (int sp = 0; sp < 3; sp++)
            {
                Vector2 ran = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-12f, -2f));

                Spark spark = new();
                spark.PrepareSpark(npc.Center, ran, ran.ToRotation(), ColorLib.IchorCrystal2, Main.rand.NextFloat(0.25f, 1.4f), true, 120, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(spark);
            }

            for (int d = 0; d < 12; d++)
            {
                Vector2 ran = new Vector2(Main.rand.NextFloat(-9f, 9f), Main.rand.NextFloat(-12f, -2f));

                Dust.NewDustPerfect(npc.Center, DustID.FireworksRGB, ran, (int)MathHelper.Lerp(255, 0, Main.rand.NextFloat(0.5f, 1f)), ColorLib.Ichor, Main.rand.NextFloat(0.7f, 1.4f));
            }

            for (int d2 = 0; d2 < 7; d2++)
            {
                Vector2 ran = new Vector2(Main.rand.NextFloat(-9f, 9f), Main.rand.NextFloat(-12f, -2f));

                Dust bloo = Dust.NewDustPerfect(npc.Center, DustID.Blood, ran, (int)MathHelper.Lerp(255, 0, Main.rand.NextFloat(0.75f, 1f)), default, Main.rand.NextFloat(1f, 2f));
                bloo.noGravity = false;
            }
        }

        private void DrawSweepFX2()
        {
            /*
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

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, Color.PaleGoldenrod * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            */
        }
        public override void DrawUnderBlade()
        {
            DrawSweepFX2();
        }
        public override void DrawOverBlade()
        {

        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            var pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            if (CurrentState != State.Wait)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust F = Dust.NewDustPerfect(ppt[Main.rand.Next(15)], DustID.IchorTorch, Main.rand.NextVector2Circular(4, 4), 0, default, 2f);
                    F.noGravity = true;
                }
            }

        }
    }
}