using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.ParentClasses;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class HeliciteScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = ColorLib.Rift;
            WidthDim = 68;
            HeightDim = 68;
            base.SetDefaults();
        }

        public override void AI()
        {
            base.AI();

            ThemeColor = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);


            if (Main.rand.NextBool())
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(0.5f, 0.5f), ModContent.ProjectileType<SolarTrail>(), Projectile.damage / 5, 0f, Projectile.owner);
            }
        }
    }
}


