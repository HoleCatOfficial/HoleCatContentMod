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

    public class CelestineSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 384;
            Projectile.height = 384;
            UsesDefaultSweepFX = true;
            SweepScale = 7f;
            SweepColor = ColorLib.CelestialGradient;
            SweepHighlightColor = Color.White;
            WaitTimeMultiplier = 2f;
            SwingSpeed = 0.15f;
            Projectile.ArmorPenetration = 200;
            Projectile.extraUpdates = 3;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MagicSwing with { Pitch = LastSwing == -1 ? -0.3f : 0f, PitchVariance = 0.1f, MaxInstances = 0 };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);


            if (DTConfig.instance.ScreenshakeEffects)
            {
                ScreenShakeSystem.ShakeAt(npc.Center, 10f, 20);
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


            for (int i = 0; i < 9; i++)
            {
                Vector2 pos = npc.Center + new Vector2(Main.rand.Next(-200, 200), -800);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, pos.DirectionTo(npc.Center) * (10f * Owner.GetTotalAttackSpeed(DamageClass.Melee)), ModContent.ProjectileType<MoongeistFlare>(), Projectile.damage / 10, 2f, Projectile.owner);
            }

            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<MoongeistBolt>(), 10, npc.Center, Projectile.damage / 4, 3, 22);


            Lighting.AddLight(Projectile.Center, Color.Aquamarine.ToVector3());
            Lighting.AddLight(npc.Center, Color.Aquamarine.ToVector3());

            SoundEngine.PlaySound(DTAssetLib.Impacts.MagicHit with { MaxInstances = 0, PitchVariance = 0.6f, Variants = new(3) }, npc.Center);
            // SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, PitchVariance = 0.4f, Pitch = -0.3f }, npc.Center);
        }

        bool f1 = false;
        public override void BetweenSwing(ref int LastSwing)
        {

        }

        float rot = 0f;
        public override void DrawUnderBlade()
        {
            
        }
        public override void DrawOverBlade()
        {
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Color").Value;

            //I swear to FUCKING GOD.
            //dont touch this shit.
            //FUCK ROTATIONS DUDE.

            if (LastSwing == -1)
            {
                if (Projectile.spriteDirection > 0)
                {
                    origin = new Vector2(0, texture.Height);
                    effects = SpriteEffects.None;
                    rotationOffset = MathHelper.ToRadians(45f);
                }
                else
                {
                    origin = new Vector2(0, texture.Height);
                    effects = SpriteEffects.None;
                    rotationOffset = MathHelper.ToRadians(45f);
                }
            }
            else
            {
                if (Projectile.spriteDirection > 0)
                {
                    origin = new Vector2(texture.Width, texture.Height);
                    effects = SpriteEffects.FlipHorizontally;
                    rotationOffset = MathHelper.ToRadians(135f);
                }
                else
                {
                    origin = new Vector2(texture.Width, texture.Height);
                    effects = SpriteEffects.FlipHorizontally;
                    rotationOffset = MathHelper.ToRadians(135f);
                }
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, ColorLib.CelestialGradient * Projectile.Opacity, (Projectile.rotation + rotationOffset) + RotationManualOffset, origin, Projectile.scale * TungstenEnchantAdjustment, effects, 0);
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
                star.Initialize(ppt[Main.rand.Next(ppt.Length)], (Projectile.rotation + (MathHelper.PiOver2 * Projectile.direction)).ToRotationVector2() * 0.5f, ColorLib.CelestialGradient, 1f, Main.rand.NextFloat(-0.1f, 0.1f), 90);
                ParticleEngine.Particles.Add(star);
            }


            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}