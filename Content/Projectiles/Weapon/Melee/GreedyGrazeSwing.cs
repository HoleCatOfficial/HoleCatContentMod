
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
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
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class GreedyGrazeSwing : BaseBroadswordProjectileFullSwing
    {
        public SoundStyle Hit = DTAssetLib.Impacts.FleshHit with { Pitch = -1f, PitchVariance = 0.4f, MaxInstances = 0 };

        public override SoundStyle Swing => DTAssetLib.SwordSounds.RuneSong with { Volume = 1.0f, Pitch = -0.7f, PitchVariance = 0.2f, MaxInstances = 0 };
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 88;
            Projectile.height = 92;
            SweepColor = ColorLib.IchorCrystal4;
            SweepHighlightColor = ColorLib.IchorCrystal3;
            SwingSpeed = 0.14f;
            Projectile.extraUpdates = 1;
            UsesDefaultSweepFX = true;
            SweepScale = 1f;
            Glowmask = TextureAssets.Projectile[Type];
        }
        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Hit);
            Player player = Main.player[Projectile.owner];

            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();

            int splatterdir = npc.position.X > Owner.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, ColorLib.Ichor, 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            if (Main.rand.NextBool(10))
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.AmbitionChargeBurst with { Pitch = -1f, PitchVariance = 0.4f, MaxInstances = 0 });
                player.Heal((int)(damageDone * 0.05f));
            }
         
        }

        public Vector2 swordTip;
        public Line SwordLine;

        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            SwordLine = new Line(Owner.Center, swordTip);

            ScaleMult = 1.6f;

        }
    }
}