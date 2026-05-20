
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using ReLogic.Peripherals.RGB;
using System;
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
	public class ConstantineScytheProjectile : BaseBroadswordProjectile
	{
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 94;
            Projectile.height = 102;
            SweepColor = Color.DeepPink;
            SweepHighlightColor = Color.HotPink;
            UsesDefaultSweepFX = true;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
            SwingSpeed = 0.23f;
        }

        public override SoundStyle Swing => new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionT3Slash") with { MaxInstances = 0, PitchVariance = 0.6f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.LightGoreCut with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);

            int splatterdir = npc.position.X > Owner.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, Color.HotPink * Main.rand.NextFloat(0.1f, 1f), 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }
        }

        public override void OnStartSwing()
        {
            Vector2 dir = Main.MouseWorld - Projectile.Center;
            dir.Normalize();

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir * 6, ModContent.ProjectileType<ConstantineScytheClone>(), (int)(Projectile.damage *  0.75f), 3, Owner.whoAmI);
        }
        public override void DrawOverBlade()
        {
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() - 20f) * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);

            ScaleMult = 1f;

            SparkEdge(Owner, 0.75f, Color.HotPink, 2);
        }
    }
}