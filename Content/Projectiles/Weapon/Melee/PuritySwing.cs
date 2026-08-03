
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using DestroyerTest.Content.Projectiles.ParentClasses;
using log4net.Appender;
using Microsoft.Build.Evaluation;
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
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class PuritySwing : BaseBroadswordProjectileFullSwing
    {
        public override SoundStyle Swing => SoundID.Item71;
        public SoundStyle Hit = DTAssetLib.SwordSounds.LightGoreCut with { PitchVariance = 0.4f, MaxInstances = 0 };
        public Color MainColor = new Color(16, 149, 162);

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 94;
            Projectile.height = 94;
            SweepColor = Main.DiscoColor;
            SwingSpeed = 0.10f;
            UsesDefaultSweepFX = true;
            SweepScale = 1.7f;
        }

        public Vector2[] Targets;
        public Vector2 Mouse;
        public override void OnStartSwing()
        {
            Mouse = Main.MouseWorld;
            Targets = Opus.GetEquidistantVectors(8, Mouse, 200f, 0f);
            for (int i = 0; i < Targets.Length; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.MountedCenter, Vector2.One, ModContent.ProjectileType<PurityBlessedNodeCrystal>(), Projectile.damage / 4, 4f, Owner.whoAmI, i);
            }
        }

        public override void ExtraEffects()
        {
            SweepColor = Main.DiscoColor;
        }


        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Hit);
            Player player = Main.player[Projectile.owner];
            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();

            int splatterdir = npc.position.X > Owner.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                Spark Spark1 = new Spark();
                Spark1.PrepareSpark(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, MainColor * Main.rand.NextFloat(0.1f, 0.8f), 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark1);

                Spark Spark2 = new Spark();
                Spark2.PrepareSpark(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, Color.Red * Main.rand.NextFloat(0.1f, 0.8f), 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark2);
            }
        }
    }
}