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

        private const int TrailLength = 200;
        public Vector2 lp;
        public Vector2 np;
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
  
        public override void AI()
        {
            lp = Projectile.Center + new Vector2(-Projectile.width / 2, Projectile.height / 2);
            np = Projectile.Center + new Vector2(Projectile.width / 2, -Projectile.height / 2);

            CacheTrail1();
            CacheTrail2();
            
        }

        
    }
}

