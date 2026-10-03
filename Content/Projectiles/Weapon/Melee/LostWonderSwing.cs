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

    public class LostWonderSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 84;
            Projectile.height = 84;
            UsesDefaultSweepFX = true;
            SweepScale = 1.9f;
            SweepColor = Color.SteelBlue;
            SweepHighlightColor = Color.LightSteelBlue;
            WaitTimeMultiplier = 4f;
            SwingSpeed = 0.2f;
            Projectile.ArmorPenetration = 40;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.RuneSong with { Pitch = -0.3f, PitchVariance = 0.1f};

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);
            ScreenShakeSystem.ShakeAt(npc.Center, 7f, 20);
            for (int i = 0; i < 12; i++)
            {
                PixelParticle FX = new();
                FX.Initialize(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(10f, 18f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), new Color(94, 242, 219), 2f, 15);
                ParticleEngine.Particles.Add(FX);

                Spark spark = new();
                spark.PrepareSpark(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(20f, 30f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), npc.Center.DirectionFrom(Owner.Center).ToRotation() + MathHelper.PiOver2, new Color(94, 242, 219), 0.5f, false, 15, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(spark);
            }

            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(20f, 30f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), ModContent.ProjectileType<LostWonderSpark>(), Projectile.damage / 4, 2f, Projectile.owner);
            }

            Lighting.AddLight(Projectile.Center, new Color(94, 242, 219).ToVector3());
            Lighting.AddLight(npc.Center, new Color(94, 242, 219).ToVector3());

            SoundEngine.PlaySound(DTAssetLib.Impacts.LightMetalHit with { MaxInstances = 0, Pitch = -2f, PitchVariance = 0.2f, Volume = 0.05f }, npc.Center);

            SoundEngine.PlaySound(DTAssetLib.Impacts.MetalImpact with { MaxInstances = 0, PitchVariance = 0.4f, Volume = 0.3f }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);
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
                Lighting.AddLight(Projectile.Center, new Color(94, 242, 219).ToVector3() * 0.7f);

                PixelParticlePlayer FX = new(Owner);
                FX.Initialize(swordTip, (Projectile.rotation + (MathHelper.PiOver2 * Projectile.direction)).ToRotationVector2() * 0.3f, new Color(94, 242, 219), 2f, 30);
                ParticleEngine.Particles.Add(FX);
            }


            ScaleMult = 1.15f;

            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}