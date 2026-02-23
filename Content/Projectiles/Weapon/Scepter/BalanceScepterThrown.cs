using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
using Terraria.ModLoader.Config;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class BalanceScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.White;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.BeachShell;
            base.SetDefaults();
        }

        public List<Vector2> LightPoints = new List<Vector2>();
        public List<Vector2> NightPoints = new List<Vector2>();
        public List<float> LightRots = new List<float>();
        public List<float> NightRots = new List<float>();

        private const int TrailLength = 400;
        public Vector2 lp = Vector2.Zero;
        public Vector2 np = Vector2.Zero;
        private void CacheTrail1()
        {
            Vector2 lastPos = LightPoints.Count > 0 ? LightPoints[0] : lp;
			Vector2 newPos  = lp;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 0.1f; // how closely to sample. tweak this!

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
					LightPoints.Insert(0, pos);
					LightRots.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				LightPoints.Insert(0, newPos);
				LightRots.Insert(0, Projectile.rotation);
			}

			while (LightPoints.Count > TrailLength)
				LightPoints.RemoveAt(LightPoints.Count - 1);
			while (LightRots.Count > TrailLength)
				LightRots.RemoveAt(LightRots.Count - 1);
        }

        private void CacheTrail2()
        {
            Vector2 lastPos = NightPoints.Count > 0 ? NightPoints[0] : lp;
			Vector2 newPos  = lp;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 0.1f; // how closely to sample. tweak this!

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
					NightPoints.Insert(0, pos);
					NightRots.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				NightPoints.Insert(0, newPos);
				NightRots.Insert(0, Projectile.rotation);
			}

			while (NightPoints.Count > TrailLength)
				NightPoints.RemoveAt(NightPoints.Count - 1);
			while (NightRots.Count > TrailLength)
				NightRots.RemoveAt(NightRots.Count - 1);
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
                        
            trailOffset += 0.01f;
            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            LightTrail();
            NightTrail();
            Opus.ReturnToDefaultDrawing(spriteBatch);

            base.PreDraw(ref lightColor);
            return false;
        }

        public void LightTrail()
        {
            if (LightPoints.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = LightPoints.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)LightPoints.Count); // fade toward tail
					Color b = ColorLib.SoulOfLightColor * t;

					Vector2 dir = (LightPoints[i] - LightPoints[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 20;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 20;

					DTUtils.AddStrips(ve, LightPoints, i, offset, offset2, t, b, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Streak(2).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}
        }

        public void NightTrail()
        {
            if (NightPoints.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = NightPoints.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)NightPoints.Count); // fade toward tail
					Color b = ColorLib.SoulOfNightColor * t;

					Vector2 dir = (NightPoints[i] - NightPoints[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 20;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 20;

					DTUtils.AddStrips(ve, NightPoints, i, offset, offset2, t, b, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Streak(2).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}
        }
  
        public override void AI()
        {
            base.AI();
            lp = Projectile.Center + (new Vector2(-Projectile.width / 2, Projectile.height / 2).RotatedBy(Projectile.rotation));
            np = Projectile.Center + (new Vector2(Projectile.width / 2, -Projectile.height / 2).RotatedBy(Projectile.rotation));

            if (lp != Vector2.Zero && np != Vector2.Zero)
            {
                CacheTrail1();
                CacheTrail2();
            }
            
            if (Main.rand.NextBool(5))
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), lp, Main.rand.NextVector2Circular(15, 15), ModContent.ProjectileType<LightFireball>(), (int)(Projectile.damage * 0.1f), 10, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), np, Main.rand.NextVector2Circular(15, 15), ModContent.ProjectileType<NightFireball>(), (int)(Projectile.damage * 0.1f), 10, Projectile.owner);
            }
        }
    }
}

