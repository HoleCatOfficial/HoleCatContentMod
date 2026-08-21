
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
 
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
using static tModPorter.ProgressUpdate;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class MalevolenceSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 60;
            SweepColor = ColorLib.Wretched4;
            SweepHighlightColor = ColorLib.Wretched2;
            UsesDefaultSweepFX = true;
            UsesFireSweepFX = true;
            SweepScale = 1.3f;
            SwingSpeed = 0.17f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MediumHeavySwing;

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.CursedInferno, 300);
            SoundEngine.PlaySound(DTAssetLib.Impacts.Malevolence with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings() { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = npc.Center });
        }

        private void DrawSweepFX2()
        {
          
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

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(ppt[Main.rand.Next(15)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, DTColorUtils.MultiLerp(SlashProgress, ColorLib.WretchedColorMap) * 0.5f, 3f);
                //PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], pt[Main.rand.Next(30)], SwordLine.GetLineRotation.ToRotationVector2() * 2, ColorLib.Wretched3, 0.5f, 20, ai2: 2);
            }

            ScaleMult = 1.25f;

            SparkEdge(Main.player[Projectile.owner], 1f, DTColorUtils.MultiLerp(SlashProgress, ColorLib.WretchedColorMap));
        }
    }
}
