using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using ReLogic.Content;
using Terraria.Audio;
using OpusLib;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using System.Collections.Generic;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Dusts;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee.Flail
{
    public class AmbitionSpark : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            lightColor = new Color(207, 207, 207);
            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
					Color b = lightColor * t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 4;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 4;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, 0);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Square.Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}

			Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 300;
        private void CacheTrail()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 0.5f; // how closely to sample. tweak this!

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
					TrailPositions.Insert(0, pos);
					TrailRotations.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				TrailPositions.Insert(0, newPos);
				TrailRotations.Insert(0, Projectile.rotation);
			}


			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            CacheTrail();
            Projectile.ai[0] += 1f;

            Dust traildust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f, 0, default, 0.5f);
            traildust.noGravity = true;

            if(Main.rand.NextBool(3))
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Projectile.Center, Vector2.Zero, Color.White, 0.5f);
            }

            if (LifeTime < 30)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.velocity.Y += 0.2f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
           
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);

            Opus.RadialDustRandomDir(ModContent.DustType<ColorableNeonDust>(), 10, Projectile.Center, 0, Color.White, 2f, 2f);
        }
    }
}
