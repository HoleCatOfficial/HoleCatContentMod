using System;
using System.Collections.Generic;
using System.IO;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.ScreenShake;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class CalosarSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 94;
            Projectile.height = 100;
            UsesDefaultSweepFX = true;
            SweepScale = 1.9f;
            SweepColor = Color.IndianRed;
            SweepHighlightColor = Color.OrangeRed;
            WaitTimeMultiplier = 3f;
            SwingSpeed = 0.12f;
            Projectile.ArmorPenetration = 50;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.RuneSong with { Pitch = -1f, PitchVariance = 0.1f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);
            npc.AddBuff(BuffID.OnFire3, 300);

            ScreenShakeSystem.ShakeAt(npc.Center, 7f, 20);
            for (int i = 0; i < 12; i++)
            {
                PixelParticle FX = new();
                FX.Initialize(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(10f, 18f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), Color.OrangeRed, 2f, 15);
                ParticleEngine.Particles.Add(FX);

                Spark spark = new();
                spark.PrepareSpark(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(20f, 30f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), npc.Center.DirectionFrom(Owner.Center).ToRotation() + MathHelper.PiOver2, Color.OrangeRed, 0.5f, false, 15, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(spark);
            }

            

            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3());
            Lighting.AddLight(npc.Center, Color.OrangeRed.ToVector3());

            SoundEngine.PlaySound(DTAssetLib.Impacts.LightMetalHit with { MaxInstances = 0, Pitch = -2.6f, PitchVariance = 0.2f, Volume = 0.05f }, npc.Center);

            SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Hit with { MaxInstances = 0, PitchVariance = 0.4f, Volume = 0.5f }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, PitchVariance = 0.4f, Pitch = -0.3f }, npc.Center);
        }

        bool f1 = false;
        public override void BetweenSwing(ref int LastSwing)
        {
            if (!f1)
            {
                SoundEngine.PlaySound(SoundID.Item109 with { Pitch = 0.5f }, Projectile.Center);
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), swordTip, SwordLine.GetLineRotation.ToRotationVector2().RotatedByRandom(0.2f) * (Main.rand.NextFloat(20f, 30f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), ModContent.ProjectileType<CalosarSpark>(), Projectile.damage / 4, 2f, Projectile.owner);
                }
                f1 = true;
            }
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

            //Dust d = Dust.NewDustPerfect(swordTip, DustID.Electric, (Projectile.rotation + (MathHelper.PiOver2 * -Projectile.direction)).ToRotationVector2() * 5f, 60, Scale: 1.5f);
            //d.noGravity = true;

            if (CurrentState != State.Wait)
            {
                f1 = false;
                Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 0.7f);




                PixelParticlePlayer FX = new(Owner);
                FX.Initialize(swordTip, (Projectile.rotation + (MathHelper.PiOver2 * Projectile.direction)).ToRotationVector2() * 0.3f, Color.OrangeRed, 2f, 30);
                ParticleEngine.Particles.Add(FX);
            }


            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}