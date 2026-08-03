
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
    public class GargantuaProjectile : BaseBroadswordProjectileFullSwing
    {
        public SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/DreamHit", 3) with { PitchVariance = 0.4f, MaxInstances = 0 };

        public override SoundStyle Swing => DTAssetLib.SwordSounds.ConSwing;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 122;
            Projectile.height = 122;
            SweepColor = Color.Red;
            SwingSpeed = 0.12f;
            UsesDefaultSweepFX = true;
            SweepScale = 1.6f;

            Projectile.extraUpdates = 3;
        }
        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            base.HitNPCEffects(npc, hit, damageDone);
            SoundEngine.PlaySound(Hit);
         
            var ScreenShake = Owner.GetModPlayer<ScreenshakePlayer>();

            int splatterdir = npc.position.X > Owner.MountedCenter.X ? 1 : -1;
                for (int i = 0; i< 7; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, Color.Red, 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            GargantuaParticle FX = new GargantuaParticle();


            FX.Initiate(npc.Center);
            ParticleEngine.ShaderParticles.Add(FX);

            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<GargantuaStar>(), 2, npc.Center, (int)(Projectile.damage * 0.2f), (int)(Projectile.knockBack * 0.5f), 14f);

            if (hit.Crit)
			{
                ScreenShake.screenshakeMagnitude = 4;
                ScreenShake.screenshakeTimer = 20;
                for (int t = 0; t < 2; t++)
				{
					Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center, new Vector2(20f * splatterdir, 0).RotatedByRandom(0.1f), ModContent.ProjectileType<GoliathPhantom>(), (int)(Projectile.damage * 0.2f), 4, Projectile.owner);
				}
			}
        }

        public Vector2 swordTip;
        public Line SwordLine;
        int timer = 0;
        public override void ExtraEffects()
        {
            timer++;
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            SwordLine = new Line(Owner.Center, swordTip);

            ScaleMult = 1.6f;

            Vector2[] Pos = new Vector2[4]
            {
                Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * (Projectile.scale * 0.80f)),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * (Projectile.scale * 0.60f)),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * (Projectile.scale * 0.40f)),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * (Projectile.scale * 0.20f)),
            };

            

            Vector2[] positions =
            {
                swordTip,
                Pos[0],
                Pos[1],
                Pos[2],
                Pos[3]
            };

            float[] alpha =
            {
                1f,
                0.8f,
                0.6f,
                0.4f,
                0.2f
            };

            int[] lifetime =
            {
                40,
                35,
                30,
                25,
                10
            };

            for (int i = 0; i < positions.Length; i++)
            {
                Fire fire = new Fire();
                Vector2 RandSpeed = (SwordLine.GetLineRotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.2f) * Main.rand.NextFloat(2f, 6f);

                fire.PrepareFire(
                    positions[Main.rand.Next(positions.Length)],
                    RandSpeed,
                    DTUtils.RandomDirection(2),
                    Main.rand.NextFloat(-0.12f, 0.12f),
                    Color.Red * 0.3f,
                    1.25f,
                    lifetime[i],
                    FireDrawMode.Additive,
                    PixelLayer.AboveProjectiles);

                ParticleEngine.BehindProjectiles.Add(fire);
            }
        }
    }
}