using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.ScreenShake;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;

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

    public class TibelSwing : BaseBroadswordProjectileFullSwing
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            UsesDefaultSweepFX = true;
            SweepScale = 1.4f;
            SweepColor = VoidColorDark;
            SweepHighlightColor = VoidColor;
            WaitTimeMultiplier = 0.8f;
            SwingSpeed = 0.11f;
            Projectile.ArmorPenetration = 120;

            ScaleMult = 1.8f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.HeavySwing with { Pitch = -0.2f, PitchVariance = 0.1f };

        Color VoidColorDark = new Color(72, 61, 134);
        Color VoidColor = new Color(107, 110, 196);

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
                FX.Initialize(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(10f, 18f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), VoidColorDark, 2f, 15);
                ParticleEngine.Particles.Add(FX);

                Spark spark = new();
                spark.PrepareSpark(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(20f, 30f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), npc.Center.DirectionFrom(Owner.Center).ToRotation() + MathHelper.PiOver2, VoidColor, 0.5f, false, 15, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(spark);
            }

            if (DTCrossMod.CallOfVoidIsLoaded)
            {
                DTCrossMod.CallofVoid_AddVoidTouch(npc, 300, 2f, 1000, 16);
                 if (DTCrossMod.CallOfVoidMod.TryFind("VoidStarF", out ModProjectile voidStar))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center, Main.rand.NextVector2Circular((Main.rand.NextFloat(10f, 25f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), (Main.rand.NextFloat(10f, 25f) * Owner.GetTotalAttackSpeed(DamageClass.Melee))), voidStar.Type, Projectile.damage / 4, 2f, Projectile.owner);
                    }
                }
            }

            Lighting.AddLight(Projectile.Center, VoidColorDark.ToVector3());
            Lighting.AddLight(npc.Center, VoidColorDark.ToVector3());

            SoundEngine.PlaySound(DTAssetLib.Impacts.MagicHit with { MaxInstances = 0, PitchVariance = 0.6f, Variants = new(3) }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, Pitch = -1f, PitchVariance = 0.4f }, npc.Center);
        }

        bool f1 = false;
        public override void BetweenSwing()
        {
            if (!f1)
            {
                if (DTCrossMod.CallOfVoidIsLoaded)
                {
                    if (DTCrossMod.CallOfVoidMod.TryFind("VoidStarF", out ModProjectile voidStar))
                    {
                        SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, PitchVariance = 0.4f }, Projectile.Center);

                        Opus.RadialSpreadProjectile(voidStar.Type, 5, swordTip, Projectile.damage / 10, 3, (Main.rand.NextFloat(20f, 30f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), offset: Main.rand.NextFloat(MathHelper.TwoPi));
                    }
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

            //Dust d = Dust.NewDustPerfect(swordTip, DustID.Electric, (Projectile.rotation + (MathHelper.PiOver2 * -Projectile.direction)).ToRotationVector2() * 5f, 60, Scale: 1.5f);
            //d.noGravity = true;

            if (CurrentState != State.Wait)
            {
                Lighting.AddLight(Projectile.Center, VoidColor.ToVector3() * 0.7f);

                PixelParticlePlayer FX = new(Owner);
                FX.Initialize(swordTip, (Projectile.rotation + (MathHelper.PiOver2 * Projectile.direction)).ToRotationVector2() * 0.3f, VoidColor, 2f, 30);
                ParticleEngine.Particles.Add(FX);
                f1 = false;
            }

            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}