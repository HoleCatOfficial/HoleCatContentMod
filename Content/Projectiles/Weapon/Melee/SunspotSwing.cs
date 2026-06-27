using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
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

    public class SunspotSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 96;
            Projectile.height = 96;
            SweepColor = ColorLib.DarkRift2;
            SweepHighlightColor = ColorLib.Rift;
            SwingSpeed = 0.15f;
            WaitTimeMultiplier = 1.3f;
            ScaleMult = 1.6f;

            UsesDefaultSweepFX = true;

            //Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MagicSwing with { PitchVariance = 0.2f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, npc.Center);
            
           

            Vector2 T = npc.Center - Owner.Center;
            T.Normalize();


            for (int i = 0; i < 5; i++)
            {
                Spark S = new();
                Vector2 V = (T * 4).RotatedByRandom(0.4f);
                S.PrepareSpark(npc.Center, V, V.ToRotation(), SweepColor, 0.6f, false, 60, SparkDrawMode.Additive, 1.5f);
                ParticleEngine.Particles.Add(S);

                if (hit.Crit)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, V * 2, ModContent.ProjectileType<SunspotSpark>(), Projectile.damage / 10, 5, Owner.whoAmI);
                }
            }

            if (!npc.active)
            {
                Owner.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 4;
                Owner.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 20;
                SoundEngine.PlaySound(DTAssetLib.Impacts.MagicHit with { MaxInstances = 0, PitchVariance = 0.6f }, npc.Center);
                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Break with { MaxInstances = 0, PitchVariance = 0.6f, Volume = 0.65f }, npc.Center);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<SolarTrail>(), 8, npc.Center, Projectile.damage / 10, 4, 0.25f);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<SolarTrail>(), 12, npc.Center, Projectile.damage / 15, 4, 0.5f);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<SolarTrail>(), 16, npc.Center, Projectile.damage / 20, 4, 0.75f);
                SmallShine Shine = new();
                Shine.Prepare(npc.Center, Vector2.Zero, Color.White, 3f);
                ParticleEngine.ShaderParticles.Add(Shine);
            }
            else
            {
                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Hit with { MaxInstances = 0, PitchVariance = 0.6f }, npc.Center);
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
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, SwordLine.GetLineRotation.ToRotationVector2() * 12, ModContent.ProjectileType<SunspotSpark>(), Projectile.damage / 10, 5, Owner.whoAmI);
                }
            }


        }
    }
}