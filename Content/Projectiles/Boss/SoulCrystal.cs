using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss
{
    public class SoulCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14; // The width of projectile hitbox
            Projectile.height = 14; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.scale = 0.1f;
        }

        public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.Soul;
			trailOffset += 0.04f;


			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Opus.DrawGlowOnProj(Projectile, lightColor * GlowMult, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

        public override void OnSpawn(IEntitySource source)
        {

        }

        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 400;
        public float GlowMult = 1f;
        public override void AI()
        {
            if (Projectile.scale < 1f)
            {
                Projectile.scale += 0.05f;
            }

            if (Main.rand.NextBool(3) && !DTOptimizationsConfig.instance.DisableExcessParticles)
            {

                Spark Spark = new Spark();

                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), (-Projectile.velocity * 0.5f).RotatedByRandom(0.05f), 0f, DTColorUtils.Pastel(ColorLib.Soul, 0.2f), 1f, false, 40, SparkDrawMode.Additive);
                ParticleEngine.ShaderParticles.Add(Spark);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            GlowMult = MathHelper.Lerp(0.25f, 1f, (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.5f + 0.5f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/CrystalBreak") with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item14);
            for (int g = 0; g < 4; g++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Pixie, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
            }
            int Gore1 = Mod.Find<ModGore>("SoulShard1").Type;
            int Gore2 = Mod.Find<ModGore>("SoulShard2").Type;
            int Gore3 = Mod.Find<ModGore>("SoulShard3").Type;

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
}