using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class SplitGhoulProjectile : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 10f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.01f;

        float IHomingProjectile.HomingMaxAccel => 20f;

        float IHomingProjectile.DetectRadius => 600;

        bool IHomingProjectile.CanHome => Projectile.ai[0] > 240;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            Dust.NewDustPerfect(Projectile.Center, DustID.SpectreStaff, Vector2.Zero, 50, default, 2f).noGravity = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] > 240;
        }
    }
}
