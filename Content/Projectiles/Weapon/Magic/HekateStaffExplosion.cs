using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
	public class HekateStaffExplosion : ModProjectile
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
            for (int u = 0; u < 15; u++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2CircularEdge(5, 5), 0, new Color(184, 45, 117), 2);
            }
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 20 * 60);
        }
	}
}