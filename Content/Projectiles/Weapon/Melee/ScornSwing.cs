using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using InnoVault.PRT;
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

    public class ScornSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 68;
            Projectile.height = 68;
            SweepColor = ColorLib.IchorCrystal2;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MediumHeavySwing;

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            npc.AddBuff(BuffID.Ichor, 300);
            SoundEngine.PlaySound(DTAssetLib.Impacts.Malevolence with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.Excalibur, new ParticleOrchestraSettings() { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = npc.Center });
        }

        private void DrawSweepFX2()
        {
            Player player = Main.player[Projectile.owner];
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash4").Value;
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
            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, DTColorUtils.MultiLerp(SlashProgress, ColorLib.IchorCrystalColorMap) * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod) * ScaleMult, FX);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
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

            player.heldProj = Type;

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(40);
            Vector2[] ppt = pt[15..40];

            for (int i = 0; i < 2; i++)
            {
                Dust P = Dust.NewDustPerfect(ppt[Main.rand.Next(25)], DustID.IchorTorch, (SwordLine.GetLineRotation - MathHelper.PiOver2).ToRotationVector2() * 2, 0, default, 1.7f);
                P.noGravity = true;
            }

            ScaleMult = 1.25f;
            SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.IchorCrystal3);
        }
    }
}
