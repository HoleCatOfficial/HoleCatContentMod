using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
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

    public class CrystalBladeSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 70;
            Projectile.height = 70;
            SweepColor = Color.DeepPink;
            SweepHighlightColor = Color.Pink;
            SwingSpeed = 0.15f;
            WaitTimeMultiplier = 1.3f;
            ScaleMult = 1.6f;

            UsesDefaultSweepFX = true;

            //Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        int[] SparkTypes = new int[3]
            {
                ModContent.ProjectileType<CrystalBladeSparkBlue>(),
                ModContent.ProjectileType<CrystalBladeSparkPurple>(),
                ModContent.ProjectileType<CrystalBladeSparkPink>(),
            };

        public override SoundStyle Swing => DTAssetLib.SwordSounds.BigBasicSwing with { PitchVariance = 0.2f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.IceImpact with { MaxInstances = 0, PitchVariance = 0.6f, Volume = 0.1f }, npc.Center);

            Vector2 T = npc.Center - Owner.Center;
            T.Normalize();

           
            for (int i = 0; i < 5; i++)
            {
                Spark S = new();
                Vector2 V = (T * 4).RotatedByRandom(0.4f);
                S.PrepareSpark(npc.Center,  V, V.ToRotation(), SweepColor, 0.6f, false, 60, SparkDrawMode.Additive, 1.5f);
                ParticleEngine.Particles.Add(S);

                if (hit.Crit)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, V * 2, SparkTypes[Main.rand.Next(SparkTypes.Length)], Projectile.damage / 10, 5, Owner.whoAmI);
                }
            }


        }

        public override void OnStartSwing()
        {
          
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public int Counter = 0;
        public override void ExtraEffects()
        {
            Counter++;

            SweepColor = Color.Lerp(Color.DeepPink, Color.DeepSkyBlue, SlashProgress.Inverse());
            SweepHighlightColor = Color.Lerp(Color.Pink, Color.SkyBlue, SlashProgress.Inverse());


            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            for (int i = 0; i < 2; i++)
            {
                //Dust.NewDustPerfect(ppt[Main.rand.Next(15)], DustID.CrystalSerpent, SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, default, 3f);
                //PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], pt[Main.rand.Next(30)], SwordLine.GetLineRotation.ToRotationVector2() * 2, ColorLib.Wretched3, 0.5f, 20, ai2: 2);
            }

            

            int SparkAmt = (int)MathHelper.Lerp(0, 3, Utilities.Convert01To010(SlashProgress));

            for (int i = 0; i < SparkAmt; i++)
            {
                if (Main.GameUpdateCount % 3 == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, SwordLine.GetLineRotation.ToRotationVector2() * 12, SparkTypes[Main.rand.Next(SparkTypes.Length)], Projectile.damage / 10, 5, Owner.whoAmI);
                }
            }


        }
    }
}