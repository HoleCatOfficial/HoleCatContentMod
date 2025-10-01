using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class TenebrisGemstone : ModProjectile
    {
        private const float OrbitRadius = 60f;
        private float OrbitSpeed = Main.rand.NextFloat(-0.5f, 0.5f);

        private bool orbiting = false;
        private int orbitPlayer = -1;
        private float orbitAngle = 0f;

        public int Variant = Main.rand.Next(1, 4);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 26;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Variant;
        }

        public void ColorAffectedFX(Color color)
        {
            Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.6f);
            if (Main.rand.NextBool(5))
            {
                PRTLoader.NewParticle(Projectile.Center, new Vector2((Projectile.velocity.X / 2) + Main.rand.NextFloat(-0.5f, 0.5f), (Projectile.velocity.Y / 2) + Main.rand.NextFloat(-0.5f, 0.5f)), PRTLoader.GetParticleID<StarParticle>(), Color.White, 0.25f);
            }
            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, 0, 0, 0, color, 1.5f);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Variant == 1)
            {
                ColorAffectedFX(ColorLib.TenebrisMagenta);
            }
            if (Variant == 2)
            {
                ColorAffectedFX(ColorLib.TenebrisBeige);
            }
            if (Variant == 3)
            {
                ColorAffectedFX(ColorLib.TenebrisBlue);
            }
            if (!orbiting)
            {
                float hoverOffset = (float)System.Math.Sin(Main.GameUpdateCount * 0.05f) * 2f;
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Projectile.Center with { Y = Projectile.Center.Y + hoverOffset };

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (player.active && !player.dead && Projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        Projectile.timeLeft = 600;
                        orbiting = true;
                        orbitPlayer = i;
                        orbitAngle = 0f;
                        break;
                    }
                }
            }
            else
            {
                if (!player.active || player.dead)
                {
                    orbiting = false;
                    orbitPlayer = -1;
                    return;
                }

                orbitAngle += OrbitSpeed;
                Vector2 orbitOffset = new Vector2(
                    (float)System.Math.Cos(orbitAngle),
                    (float)System.Math.Sin(orbitAngle)
                ) * OrbitRadius;

                Projectile.Center = player.Center + orbitOffset;
                Projectile.velocity = Vector2.Zero;

                if (Variant == 1)
                {
                    player.GetDamage(DamageClass.Ranged) *= 1.2f;
                    Dust.NewDust(player.position, player.Hitbox.Width, player.Hitbox.Height, DustID.TintableDustLighted, 0, 0, 0, ColorLib.TenebrisMagenta, 1.5f);
                }
                if (Variant == 2)
                {
                    player.moveSpeed *= 1.25f;
                    Dust.NewDust(player.position, player.Hitbox.Width, player.Hitbox.Height, DustID.TintableDustLighted, 0, 0, 0, ColorLib.TenebrisBeige, 1.5f);
                }
                if (Variant == 3)
                {
                    player.ammoCost75 = true;
                    Dust.NewDust(player.position, player.Hitbox.Width, player.Hitbox.Height, DustID.TintableDustLighted, 0, 0, 0, ColorLib.TenebrisBlue, 1.5f);
                }
            }
        }
    }
}