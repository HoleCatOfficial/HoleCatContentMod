
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;


using log4net.Appender;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class IdriGreatswordSwing : BaseBroadswordProjectileFullSwing
    {
        public SoundStyle Hit = DTAssetLib.IdriGreatswordSlice(ChildSafety.Disabled) with { PitchVariance = 0.4f, MaxInstances = 0 };

        public override SoundStyle Swing => DTAssetLib.SwordSounds.QuickSwing with { Pitch = -1f };
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 134;
            Projectile.height = 146;
            SweepColor = ColorLib.Soul3;
            SweepHighlightColor = ColorLib.Soul2;
            SwingSpeed = 0.12f;
            UsesDefaultSweepFX = true;
            ScaleMult = 1.6f;
            SweepScale = 1.8f;
            Projectile.ArmorPenetration = 100;
            Projectile.extraUpdates = 3;
        }
        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            base.HitNPCEffects(npc, hit, damageDone);
            SoundEngine.PlaySound(Hit);

            for (int i = 0; i < 7; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(npc.Center, npc.DirectionFrom(Owner.Center).RotatedByRandom(0.5f) * Main.rand.NextFloat(4f, 12f), 0f, Color.Red, 0.75f, false, 30, SparkDrawMode.AlphaBlend, 2.8f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }


            var ScreenShake = Owner.GetModPlayer<ScreenshakePlayer>();

            if (hit.Crit)
            {
                SoundEngine.PlaySound(DTAssetLib.TenebrousSlinger.Shoot with { Pitch = -0.7f });
                ScreenShake.screenshakeMagnitude = 4;
                ScreenShake.screenshakeTimer = 60;

                npc.AddBuff(ModContent.BuffType<SoulInferno>(), 300);

                for (int i = 0; i < 7; i++)
                {
                    Spark Spark = new Spark();
                    Spark.PrepareSpark(npc.Center, npc.DirectionFrom(Owner.Center).RotatedByRandom(0.5f) * Main.rand.NextFloat(4f, 12f), 0f, ColorLib.Soul, 0.75f, false, 30, SparkDrawMode.Additive, 2.8f);
                    ParticleEngine.BehindProjectiles.Add(Spark);
                }
            }
            else
            {
                ScreenShake.screenshakeMagnitude = 1;
                ScreenShake.screenshakeTimer = 20;
            }
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {

            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            SwordLine = new Line(Owner.Center, swordTip);

            Vector2[] pt = SwordLine.GetPointsAlongLine(50);
            Vector2[] ppt = pt[10..50];

            int threshold = CurrentState == State.Wait ? 0 : (int)MathHelper.Lerp(0, 5, SlashProgress);

            for (int i = 0; i < threshold; i++)
            {
                float SPD = Owner.direction == 1 ? Main.rand.NextFloat(2f, 10f) : Main.rand.NextFloat(-10f, -2f);
                Dust D = Dust.NewDustPerfect(ppt[Main.rand.Next(40)], DustID.FireworksRGB, (SwordLine.GetLineRotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.2f) * SPD, 0, ColorLib.Soul, 1f);
                D.noGravity = true;


            }

            int threshold2 = CurrentState == State.Wait ? 0 : (int)MathHelper.Lerp(0, 9, SlashProgress);

            for (int i = 0; i < threshold2; i++)
            {
                float SPD = Owner.direction == 1 ? Main.rand.NextFloat(1f, 3f) : Main.rand.NextFloat(-3f, -1f);
                PointGlowPreMultiplied glow = new();
                glow.Initialize(ppt[Main.rand.Next(40)], (SwordLine.GetLineRotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.2f) * SPD, ColorLib.Soul2, 1f, 30);
                ParticleEngine.Particles.Add(glow);
                


            }

        }
    }
}