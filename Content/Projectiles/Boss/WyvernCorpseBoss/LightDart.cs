using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Common.Primitives;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using BreadLibrary.Core.Graphics.Particles;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class LightDart : ModProjectile
    {

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public float trailOffset = 0;

        public float YScale = 0.2f;
        public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.Soul;
            trailOffset += 0.04f;
			
			SpriteBatch spriteBatch = Main.spriteBatch;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			if (WaitTimer < 20)
			{
				Opus.DrawTextureOnProj(DTAssetLib.FadeLine, Projectile, DTColorUtils.Pastel(ColorLib.Soul2, 50), false, Projectile.rotation + MathHelper.PiOver2, 4f, 1f);
			}

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, new Vector2(Projectile.scale * 0.5f, (Projectile.scale * 0.5f) * YScale), SpriteEffects.None, 0);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

        public int WaitTimer = 0;
        public bool SoundFlag = false;

		public override void AI()
		{

            if (Main.rand.NextBool(3) && !DTOptimizationsConfig.instance.DisableExcessParticles)
            {
                Spark Spark = new Spark();

                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -(Projectile.velocity * 0.1f), 0f, DTColorUtils.Pastel(ColorLib.Soul, 0.8f), 1f, false, 40, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
                
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (WaitTimer < 20)
            {
                WaitTimer++;
            }

            if (WaitTimer >= 20)
            {
                if (YScale < 1.4f)
                {
                    YScale += 0.05f;
                }
                if (Projectile.velocity.Length() < 16)
                {
                    if (!SoundFlag)
                    {
                        SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TB_Impact") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.15f }, Projectile.Center);
                        SoundFlag = true;
                    }
                    Projectile.velocity *= 1.2f;
                }
                Projectile.netUpdate = true;
            }

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
        }

        public override void OnKill(int timeLeft)
        {
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 4, Projectile.Center, 0, Color.White, 2f, 3f);
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 7, Projectile.Center, 0, Color.White, 0.6f, 0.5f);
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 4, Projectile.Center, 70, ColorLib.Soul, 1f, 2f);
        }
    }
}