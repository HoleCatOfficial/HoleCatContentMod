using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Ranged;
using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
    public class StellarBowHoldout : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/RangedItems/StellarBow";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
        }

        SoundStyle ShootRegular = new SoundStyle($"DestroyerTest/Assets/Audio/StellarBow/StellarBowShoot", 3) with
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        SoundStyle ShootEmpowered = new SoundStyle($"DestroyerTest/Assets/Audio/StellarBow/StellarBowEmpoweredShoot", 3) with
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public int ShotCount = 0;
        public int type = -1;
        public SoundStyle Shot;
        public enum State
        {
            Default,
            Empowered
        }
        public State state;

        public override void OnSpawn(IEntitySource source)
        {
            state = State.Default;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity *= 0;

            if (player.HeldItem.type == ModContent.ItemType<StellarBow>() && player.controlUseItem)
            {
                Projectile.timeLeft = 2;
                // Lock projectile to player/cursor
                float holdDistance = 15f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

                Projectile.Center = desiredPos;
                Projectile.rotation = toCursor.ToRotation();
                Projectile.direction = toCursor.X > 0 ? 1 : -1;

                if (Main.GameUpdateCount % player.HeldItem.useTime == 0)
                {
                    SoundEngine.PlaySound(Shot);
                    Vector2 Launch = Projectile.rotation.ToRotationVector2() * 30;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Launch, type, Projectile.damage, Projectile.knockBack, player.whoAmI);
                    if (state == State.Empowered)
                    {

                        for (int r = 0; r < Main.rand.Next(3, 6); r++)
                        {
                            Vector2 LaunchRand = Launch.RotatedByRandom(1.35);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, LaunchRand, ModContent.ProjectileType<ConstitutionStarFriendly>(), Projectile.damage / 10, Projectile.knockBack, player.whoAmI, ai2: 1);
                        }
                    }
                    ShotCount++;
                }

                if (ShotCount >= 10)
                {
                    state = State.Empowered;
                }

                switch (state)
                {
                    case State.Default:
                        {
                            type = ModContent.ProjectileType<GalantineArrow>();
                            Shot = ShootRegular;
                            break;
                        }
                    case State.Empowered:
                        {
                            Shot = ShootEmpowered;
                            type = ModContent.ProjectileType<GalantineLanceFriendly>();
                            break;
                        }
                }
            }
            else
            {
                Projectile.Kill();
            }
        }


    }
}