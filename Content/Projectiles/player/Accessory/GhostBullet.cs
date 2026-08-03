using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class GhostBullet : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.2f;
        }

        public override void AI()
        {
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit, Vector2.Zero);
            d.noGravity = true;
            d.scale = 1.5f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff);
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f;
            }
        }
    }
}
