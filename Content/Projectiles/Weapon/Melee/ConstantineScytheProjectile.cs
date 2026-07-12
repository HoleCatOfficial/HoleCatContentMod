
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using BreadLibrary.Core.Verlet;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using Microsoft.CodeAnalysis;
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
            SwingSpeed = 0.1f;
            RibbonStart = RibbonEnd = Owner.MountedCenter;

        }

        VerletChain Ribbon;
        Vector2 RibbonStart;
        Vector2 RibbonEnd;

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MetalSwing with { MaxInstances = 1, PitchVariance = 0.1f, Pitch = -0.2f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
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

            //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir * 6, ModContent.ProjectileType<ConstantineScytheClone>(), (int)(Projectile.damage *  0.75f), 3, Owner.whoAmI);
        }


        void RibbonDraw()
        {
            Texture2D MainTex = ModContent.Request<Texture2D>("DestroyerTest/Content/Projectiles/Weapon/Melee/ConstantineScytheString").Value;
            Texture2D EndTex = ModContent.Request<Texture2D>("DestroyerTest/Content/Projectiles/Weapon/Melee/ConstantineScytheBead").Value;

            float EndRot = Ribbon.Positions[Ribbon.Positions.Length - 1].AngleTo(Ribbon.Positions[Ribbon.Positions.Length - 2]) + MathHelper.PiOver2;
            int segmentCount = Ribbon.Positions.Length;
            for (var i = 0; i < segmentCount - 1; i++)
            {

                var start = Ribbon.Positions[i];
                var end = Ribbon.Positions[i + 1];

                Vector2 RibbonPos = (start + end) / 2;
                var DrawPos = RibbonPos - Main.screenPosition;

                var style = 0;



                if (i == Ribbon.Positions.Length - 3)
                {
                    style = 0;
                }

                if (i > Ribbon.Positions.Length - 3)
                {
                    style = 1;
                }

                var frame = MainTex.Frame(1, 1, style);

                var rotation = start.AngleTo(end) + MathHelper.PiOver2;


                var t = 0f;

                if (segmentCount > 1)
                {
                    t = i / (float)(segmentCount - 1); // 0 at base, 1 at tip
                }


                // Vertical stretch based on actual distance to next segment and texture height
                var segmentDistance = start.Distance(end);
                var lengthFactor = 1f;
                float denom = Math.Max(1, frame.Height - 5);
                lengthFactor = segmentDistance / denom * 1.2f;

                // Combine into final stretch vector and apply a small global multiplier for visual tuning
                var stretch = new Vector2(1f, lengthFactor) * 1.5f;
                var Origin = frame.Size() * 0.5f;

                if (i % 2 == 0)
                {
                    continue;
                }

                if (i == segmentCount - 2)
                {
                    stretch = Vector2.One;
                    Origin = new Vector2(frame.Width / 2, 2);
                }

                Color drawColor = Lighting.GetSubLight(RibbonPos).ToColor();
                Main.EntitySpriteDraw(MainTex, DrawPos, frame, drawColor, rotation, Origin, stretch, 0);
            }

            Color Color = Lighting.GetSubLight(Ribbon.Positions[^1]).ToColor();
            Main.EntitySpriteDraw(EndTex, Ribbon.Positions[^1] - Main.screenPosition, null, Color, EndRot, EndTex.Size() / 2, 1f, 0);
        }

        public override void DrawOverBlade()
        {
            if (Ribbon != null)
            { 
                RibbonDraw();
            }

            Texture2D EyeTex = ModContent.Request<Texture2D>(Texture + "_Eye").Value;

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (LastSwing == -1)
            {
                if (Projectile.spriteDirection > 0)
                {
                    origin = new Vector2(0, EyeTex.Height);
                    effects = SpriteEffects.None;
                    rotationOffset = MathHelper.ToRadians(45f);
                }
                else
                {
                    origin = new Vector2(0, EyeTex.Height);
                    effects = SpriteEffects.None;
                    rotationOffset = MathHelper.ToRadians(45f);
                }
            }
            else
            {
                if (Projectile.spriteDirection > 0)
                {
                    origin = new Vector2(EyeTex.Width, EyeTex.Height);
                    effects = SpriteEffects.FlipHorizontally;
                    rotationOffset = MathHelper.ToRadians(135f);
                }
                else
                {
                    origin = new Vector2(EyeTex.Width, EyeTex.Height);
                    effects = SpriteEffects.FlipHorizontally;
                    rotationOffset = MathHelper.ToRadians(135f);
                }
            }

            Vector2 ribbonPointFromTex = new Vector2(43, 14);
            Vector2 localOffset = ribbonPointFromTex - origin;

            RibbonStart = Projectile.Center + (localOffset * Projectile.scale).RotatedBy(Projectile.rotation + rotationOffset + RotationManualOffset);
            Main.EntitySpriteDraw(EyeTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Lighting.GetSubLight(Projectile.Center).ToColor()) * Projectile.Opacity, (Projectile.rotation + rotationOffset) + RotationManualOffset, origin, Projectile.scale, effects, 0);
        }


        public override bool PreAI()
        {
            if (RibbonEnd.Distance(Owner.MountedCenter) > 1000)
            {
                RibbonEnd = Owner.MountedCenter;
            }
            if (Ribbon == null)
            {
                Ribbon = new VerletChain(18, 3f, RibbonStart);
            }
            return base.PreAI();
        }
        public Vector2 swordTip;
        public Line SwordLine;

        bool EndFlag = false;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() - 20f) * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);

            ScaleMult = MathHelper.Lerp(1.5f, 2f, Utilities.Convert01To010(SlashProgress));

            //SparkEdge(Owner, 0.75f, Color.HotPink, 2);

            UpdateEnd();

            if (Ribbon == null)
            {
               
            }
            else
            {
                //RibbonStart = Projectile.Center + new Vector2(80, 90).RotatedBy(Projectile.rotation - MathHelper.PiOver4);
                Ribbon.Simulate(Vector2.Zero, RibbonStart, 0f, 0.85f, 10, collideWithTiles: false, collideWithPlayers: false);
                Ribbon.Positions[0] = RibbonStart;
                
                if (!EndFlag)
                {
                    Ribbon.Positions[^1] = Owner.MountedCenter;
                    EndFlag = true;
                }
            }
        }

        Vector2 IdealEnd;
        void UpdateEnd()
        {
            IdealEnd = RibbonStart + new Vector2(0, 110f);

            RibbonEnd = Vector2.Lerp(RibbonEnd, IdealEnd, 0.4f);
        }
    }
}