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
            ThemeColor = Color.Magenta;
            WidthDim = 38;
            HeightDim = 38;
            DustType = DustID.UndergroundHallowedEnemies;
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
           
        }

        private void CacheTrail2()
        {
            Vector2 lastPos = NightPoints.Count > 0 ? NightPoints[0] : np;
			Vector2 newPos  = np;

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

        }

        public void NightTrail()
        {
          
        }
  
        public override void AI()
        {
            base.AI();

            if (Main.rand.NextBool(15))
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, ModContent.ProjectileType<SoulOfLight_Projectile>(), (int)(Projectile.damage * 0.1f), 10, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, ModContent.ProjectileType<SoulOfNight_Projectile>(), (int)(Projectile.damage * 0.1f), 10, Projectile.owner);
            }
        }
    }
}

