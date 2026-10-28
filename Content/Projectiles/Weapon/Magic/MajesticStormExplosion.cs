using System;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
	public class MajesticStormExplosion : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, new Color(29, 226, 186) * 0.1f, 0.02f, 0.01f, 0.6f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring);

            for (int u = 0; u < 15; u++)
            {
                Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(220f, 220f);
                var curve = DTUtils.EasyBezier(Projectile.Center, Projectile.Center.DirectionTo(outer).RotatedByRandom(3f), outer, outer.DirectionFrom(Projectile.Center).RotatedByRandom(3f), 0.5f);
                var points = curve.GetEvenlySpacedPoints(15);
                for (int i = 0; i < points.Count; i++)
                {
                    PointGlowPreMultiplied ArcGlow = new();
                    ArcGlow.Initialize(points[i], Main.rand.NextVector2Circular(0.2f, 0.2f), new Color(29, 226, 186) with { A = 0 } * 0.05f, 2.5f);
                    ParticleEngine.BehindProjectiles.Add(ArcGlow);

                    PixelParticle Arc = new();
                    Arc.Initialize(points[i], Vector2.Zero, new Color(29, 226, 186) with { A = 0 }, 2f, 60);
                    ParticleEngine.BehindProjectiles.Add(Arc);
                }

                
            }
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 20 * 60);
        }
	}
}