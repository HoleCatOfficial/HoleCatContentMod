using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Buffs;
using System.Collections.Generic;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class RiftOrb : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DTUtils.DrawRiftBall(Projectile.Center, 0.08f, Main.spriteBatch, BlendState.Additive, TrailPositions, 0.75f);
            return false;
        }

        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 100;
        
        public void CacheTrail()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 1f; // how closely to sample. tweak this!

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
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            CacheTrail();

            if (!Validate(owner))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 120;

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ColorableNeonDust>(), 0f, 0f, 0, ColorLib.Rift, 1.2f);
        }

        private bool Validate(Player owner)
        {
            foreach(Item i in owner.armor)
            {
                if (i.type == ModContent.ItemType<RiftCanister>())
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.RiftExplosion, Projectile.Center);
            Projectile.Hitbox.Inflate(10, 10);
            Opus.RadialDustRandomDir(ModContent.DustType<ColorableNeonDust>(), 16, Projectile.Center, 0, ColorLib.Rift, 2f, 2);
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, ColorLib.Rift, 0.001f, 1f);
        }
    }
}