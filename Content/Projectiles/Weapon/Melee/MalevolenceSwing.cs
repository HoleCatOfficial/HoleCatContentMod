
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
 
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class MalevolenceSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 60;
            SweepColor = ColorLib.Wretched4;
            SweepHighlightColor = ColorLib.Wretched2;
            UsesDefaultSweepFX = true;
            UsesFireSweepFX = true;
            SweepScale = 1.3f;
            SwingSpeed = 0.18f;
            WaitTimeMultiplier = 3.2f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MediumSwing with { MaxInstances = 0, Pitch = -0.9f, PitchVariance = 0.2f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.CursedInferno, 300);

            SoundEngine.PlaySound(DTAssetLib.Impacts.SpiritOfJusticeParry with { MaxInstances = 0, Pitch = -0.9f, PitchVariance = 0.2f, Volume = 0.7f }, npc.Center);

            SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { MaxInstances = 0, Pitch = -0.9f, PitchVariance = 0.2f }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { MaxInstances = 0, Pitch = -0.5f, PitchVariance = 0.2f }, npc.Center);

            SoundEngine.PlaySound(DTAssetLib.Impacts.Malevolence with { MaxInstances = 0, Pitch = -0.2f, PitchVariance = 0.2f }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.Malevolence with { MaxInstances = 0, Pitch = 0.3f, PitchVariance = 0.2f }, npc.Center);
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings() { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = npc.Center });
        
            for (int i = 0; i < 9; i++)
            {
                WretchedPointGlow Glow = new();
                Glow.Prepare(npc.Center, Owner.Center.DirectionTo(npc.Center).RotatedByRandom(0.4f) * Main.rand.NextFloat(2f, 7f), 2.3f);
                ParticleEngine.Particles.Add(Glow);
            }
        }

        private void DrawSweepFX2()
        {
          
        }
        public override void DrawUnderBlade()
        {

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
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            if (CurrentState != State.Wait)
            {
                for (int i = 0; i < 2; i++)
                {
                    LerpingFire backfire = new();
                    backfire.PrepareFire(ppt[Main.rand.Next(15)], (SwordLine.GetLineRotation + (LastSwing == 1 ? MathHelper.PiOver2 : -MathHelper.PiOver2)).ToRotationVector2() * 4, DTUtils.RandomDirection(2), Main.rand.NextFloat(0.01f, 0.2f), OpusColorUtils.Darken(ColorLib.Wretched6, 0.6f), ColorLib.Wretched7, 1f, 30, FireDrawMode.AlphaBlend);
                    ParticleEngine.BehindProjectiles.Add(backfire);

                    LerpingFire fire = new();
                    fire.PrepareFire(ppt[Main.rand.Next(15)], (SwordLine.GetLineRotation + (LastSwing == 1 ? MathHelper.PiOver2 : -MathHelper.PiOver2)).ToRotationVector2() * 4, DTUtils.RandomDirection(2), Main.rand.NextFloat(0.01f, 0.2f), ColorLib.WretchedColorMap, 0.5f, 30, FireDrawMode.Additive);
                    ParticleEngine.BehindProjectiles.Add(fire);
                }

                if (Main.rand.NextBool(3))
                {
                    WretchedPointGlow Glow = new();
                    Glow.Prepare(ppt[Main.rand.Next(15)], (SwordLine.GetLineRotation + (LastSwing == 1 ? MathHelper.PiOver2 : -MathHelper.PiOver2)).ToRotationVector2() * 4, 2.3f);
                    ParticleEngine.Particles.Add(Glow);
                }    
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    LerpingFire backfire = new();
                    backfire.PrepareFire(ppt[Main.rand.Next(15)], (SwordLine.GetLineRotation).ToRotationVector2() * 2, DTUtils.RandomDirection(2), Main.rand.NextFloat(0.01f, 0.2f), OpusColorUtils.Darken(ColorLib.Wretched6, 0.6f), ColorLib.Wretched7, 1f, 30, FireDrawMode.AlphaBlend);
                    ParticleEngine.BehindProjectiles.Add(backfire);

                    LerpingFire fire = new();
                    fire.PrepareFire(ppt[Main.rand.Next(15)], (SwordLine.GetLineRotation).ToRotationVector2() * 2, DTUtils.RandomDirection(2), Main.rand.NextFloat(0.01f, 0.2f), ColorLib.WretchedColorMap, 0.5f, 30, FireDrawMode.Additive);
                    ParticleEngine.BehindProjectiles.Add(fire);
                }
            }

           

            ScaleMult = 1.9f;

            //SparkEdge(Main.player[Projectile.owner], 1f, DTColorUtils.MultiLerp(SlashProgress, ColorLib.WretchedColorMap), DistBase: 60f);
        }
    }
}
