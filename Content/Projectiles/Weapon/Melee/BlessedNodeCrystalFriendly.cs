using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed
{
    public class BlessedNodeCrystalFriendly : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/Boss/NodeBoss/Blessed/BlessedNodeCrystal";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = new Color(16, 149, 162);
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(1).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 10f, lightColor, trailOffset, 10);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawGlowOnProj(Projectile, lightColor * GlowMult, true);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }

        public float GlowMult = 1f;

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            GlowMult = MathHelper.Lerp(0.25f, 1f, (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.5f + 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<LightInferno>(), 600);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/CrystalBreak") with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);
            for (int g = 0; g < 4; g++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
            }

            int Gore1 = Mod.Find<ModGore>("BlessedShard1").Type;
            int Gore2 = Mod.Find<ModGore>("BlessedShard2").Type;
            int Gore3 = Mod.Find<ModGore>("BlessedShard3").Type;

            var entitySource = Projectile.GetSource_Death();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (optcfg.OptimizeGame == false)
            {
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore1);
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore2);
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore3);
            }
        }
    }

    public class PurityBlessedNodeCrystal : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/Boss/NodeBoss/Blessed/BlessedNodeCrystal";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public Vector2[] TargetPositions;
        Vector2 Target;
        Vector2 Start;

        Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            

            
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Main.DiscoColor * Projectile.Opacity;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(1).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 10f, lightColor, trailOffset, 10);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawGlowOnProj(Projectile, lightColor * GlowMult, true);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        Vector2 SetMouse;
        Vector2[] PreTarget;
        public override void OnSpawn(IEntitySource source)
        {
            Start = Projectile.Center;


            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<PuritySwing>() && proj.owner == Projectile.owner && proj.ModProjectile is PuritySwing puritySwing)
                {
                    TargetPositions = puritySwing.Targets;
                    SetMouse = puritySwing.Mouse;
                    Target = TargetPositions[(int)Projectile.ai[0]];
                }
            }

            PreTarget = Opus.GetEquidistantVectors(8, SetMouse, 300f, 0f);
        }


        public float GlowMult = 1f;
        Vector2[] CurvePoints = new Vector2[4];



        public int timer = 0;
        
        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();


            timer++;
            float Prog = (float)timer / 60f;
            float endRotation = (SetMouse - Target).ToRotation();

            Vector2 startDir = Projectile.rotation.ToRotationVector2();
            Vector2 endDir = endRotation.ToRotationVector2();

            float distance = Vector2.Distance(Start, Target);
            float handle = distance * 0.3f;

            Vector2 c0 = Start + startDir * handle;
            Vector2 c2 = Target - endDir * handle;

            Vector2 c1 = Vector2.Lerp(c0, c2, 0.5f);

            CurvePoints = new Vector2[]
            {
                Start,
                c0,
                c1,
                c2,
                Target
            };

            if (timer <= 60)
            {
                BezierCurve Curve = new BezierCurve(CurvePoints);
                Projectile.Center = Curve.Evaluate(Prog);
                Projectile.rotation = (Curve.Evaluate(Prog) - Curve.Evaluate(Prog - 0.01f)).ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                if (timer == 61)
                {
                    SoundEngine.PlaySound(SoundID.Item105);
                }
                Projectile.velocity = endDir * 20f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                if (Projectile.timeLeft <= 30)
                {
                    Projectile.ai[1] += 1f;

                    Projectile.Opacity = MathHelper.Lerp(1f, 0f, Projectile.ai[1] / 30f);
                }
            }
            
            
            GlowMult = MathHelper.Lerp(0.25f, 1f, (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.5f + 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<LightInferno>(), 600);

            for (int i = 0; i < 7; i++)
            {
                Spark Spark1 = new Spark();
                Spark1.PrepareSpark(target.Center, Projectile.velocity.RotatedByRandom(0.1f), 0f, new Color(16, 149, 162) * Main.rand.NextFloat(0.1f, 0.8f), 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark1);

                Spark Spark2 = new Spark();
                Spark2.PrepareSpark(target.Center, Projectile.velocity.RotatedByRandom(0.1f), 0f, Color.Red * Main.rand.NextFloat(0.1f, 0.8f), 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark2);
            }
        }

        public override void OnKill(int timeLeft)
        {
            
        }
    }
}