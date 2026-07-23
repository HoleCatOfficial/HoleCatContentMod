
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
using InnoVault;
using InnoVault.PRT;
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

        public override void ExtraEffects()
        {
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

            Fire[] fire = new Fire[5]
            {
                new Fire(),
                new Fire(),
                new Fire(),
                new Fire(),
                new Fire()
            };

            fire[0].PrepareFire(swordTip, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), Color.Red, 2f, 40, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[0]);

            fire[1].PrepareFire(Pos[0], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), Color.Red * 0.8f, 2f, 35, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[1]);

            fire[2].PrepareFire(Pos[1], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), Color.Red * 0.6f, 2f, 30, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[2]);

            fire[3].PrepareFire(Pos[2], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), Color.Red * 0.4f, 2f, 25, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[3]);

            fire[4].PrepareFire(Pos[3], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), Color.Red * 0.2f, 2f, 10, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[4]);
        }
    }
}