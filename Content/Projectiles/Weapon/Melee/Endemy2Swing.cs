
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using InnoVault.PRT;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class Endemy2Swing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 140;
            Projectile.height = 142;
            SweepColor = Color.Goldenrod;
            SwingSpeed = 0.1f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Highlight");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.BigBasicSwing;

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);
            SoundEngine.PlaySound(DTAssetLib.Impacts.SpiritOfJusticeParry with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), npc.Center, Vector2.Zero, Color.Goldenrod, 0.01f, 1f);
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<StarParticle>(), npc.Center, Vector2.Zero, Color.White, 4f);
        }

        private void DrawSweepFX2()
        {
            Player player = Main.player[Projectile.owner];
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash").Value;
            var TexH = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash2").Value;
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
            Main.EntitySpriteDraw(TexH, player.MountedCenter - Main.screenPosition, null, Color.White * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }
        public override void DrawUnderBlade()
        {
            DrawSweepFX2();
        }
        public override void DrawOverBlade()
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, swordTip, null, Color.Goldenrod, 0f, DTAssetLib.MiscSparkle144.Value.Size() / 2, 2f, SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);

            /*
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(ppt[Main.rand.Next(15)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, ColorLib.CursedFlames * 0.5f, 3f);
                //PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], pt[Main.rand.Next(30)], SwordLine.GetLineRotation.ToRotationVector2() * 2, ColorLib.Wretched3, 0.5f, 20, ai2: 2);
            }
            */

            

            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}
