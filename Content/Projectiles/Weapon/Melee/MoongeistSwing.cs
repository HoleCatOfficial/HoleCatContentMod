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

    public class MoongeistSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 102;
            Projectile.height = 110;
            UsesDefaultSweepFX = true;
            SweepScale = 2f;
            SweepColor = Color.Aquamarine;
            SweepHighlightColor = Color.White;
            WaitTimeMultiplier = 1f;
            SwingSpeed = 0.3f;
            Projectile.ArmorPenetration = 90;

            ScaleMult = 1.25f;
            Projectile.extraUpdates = 3;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MagicSwing with { Pitch = LastSwing == -1 ? -0.3f : 0f, PitchVariance = 0.1f, MaxInstances = 0 };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);
            

            if (DTConfig.instance.ScreenshakeEffects)
            {
                ScreenShakeSystem.ShakeAt(npc.Center, 7f, 20);
            }

            for (int i = 0; i < 12; i++)
            {
                PixelParticle FX = new();
                FX.Initialize(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(10f, 18f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), Color.Aquamarine, 2f, 15);
                ParticleEngine.Particles.Add(FX);

                Spark spark = new();
                spark.PrepareSpark(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(20f, 30f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), npc.Center.DirectionFrom(Owner.Center).ToRotation() + MathHelper.PiOver2, Color.Aquamarine, 0.5f, false, 15, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(spark);
            }


            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = npc.Center + new Vector2(Main.rand.Next(-200, 200), -800);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, pos.DirectionTo(npc.Center) * (8f * Owner.GetTotalAttackSpeed(DamageClass.Melee)), ModContent.ProjectileType<MoongeistFlare>(), Projectile.damage / 10, 2f, Projectile.owner);
            }


            Lighting.AddLight(Projectile.Center, Color.Aquamarine.ToVector3());
            Lighting.AddLight(npc.Center, Color.Aquamarine.ToVector3());

            SoundEngine.PlaySound(DTAssetLib.Impacts.MagicHit with { MaxInstances = 0, PitchVariance = 0.6f, Variants = new(3) }, npc.Center);
           // SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, PitchVariance = 0.4f, Pitch = -0.3f }, npc.Center);
        }

        bool f1 = false;
        public override void BetweenSwing(ref int LastSwing)
        {
            if (!f1)
            {
                SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/HopeScabbardTele") { PitchVariance = 0.5f, MaxInstances = 0 }, Projectile.Center);
                for (int i = 0; i < 5; i++)
                {
                    StarParticle star = new();
                    star.Initialize(swordTip, Main.rand.NextVector2Circular(2f, 2f), Color.Aquamarine, 1f, Main.rand.NextFloat(-0.1f, 0.1f), 90);
                    ParticleEngine.Particles.Add(star);
                }
                
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), swordTip, SwordLine.GetLineRotation.ToRotationVector2().RotatedByRandom(0.2f) * (Main.rand.NextFloat(20f, 30f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), ModContent.ProjectileType<MoongeistBolt>(), Projectile.damage / 12, 2f, Projectile.owner);
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
            Vector2[] ppt = pt[10..30];

            //Dust d = Dust.NewDustPerfect(swordTip, DustID.Electric, (Projectile.rotation + (MathHelper.PiOver2 * -Projectile.direction)).ToRotationVector2() * 5f, 60, Scale: 1.5f);
            //d.noGravity = true;

            if (CurrentState != State.Wait)
            {
                f1 = false;
                Lighting.AddLight(Projectile.Center, Color.Aquamarine.ToVector3() * 0.7f);


                StarParticle star = new();
                star.Initialize(ppt[Main.rand.Next(ppt.Length)], (Projectile.rotation + (MathHelper.PiOver2 * Projectile.direction)).ToRotationVector2() * 0.5f, Color.Aquamarine, 0.4f, Main.rand.NextFloat(-0.1f, 0.1f), 90);
                ParticleEngine.Particles.Add(star);
                

                PixelParticlePlayer FX = new(Owner);
                FX.Initialize(swordTip, (Projectile.rotation + (MathHelper.PiOver2 * Projectile.direction)).ToRotationVector2() * 0.3f, Color.Aquamarine, 2f, 30);
                ParticleEngine.Particles.Add(FX);
            }


            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}