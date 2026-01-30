using System;
using System.IO;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter
{
    // This example is similar to the Wooden Arrow projectile
    public class NebulaFlameSpawner : ModProjectile
    {

        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 2; // The width of projectile hitbox
            Projectile.height = 2; // The height of projectile hitbox
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 120;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
        }

        public Vector2 SpawnPosition;
        public Vector2 directionToCenter;
        public float radius;

        public override void AI()
        {
            Vector2 center = Projectile.Center;
            float radius = 250f; // Your radius value

            if (Main.rand.NextBool(6))
            {
                Vector2 spawnPosition = center + Main.rand.NextVector2Circular(radius, radius);
                Vector2 directionToCenter = (center - spawnPosition).SafeNormalize(Vector2.Zero) * 5f; // Adjust speed as needed

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    directionToCenter,
                    ModContent.ProjectileType<NebulaFlame>(),
                    50,
                    2,
                    Main.myPlayer
                );
            }
        }
	}
}