using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class HeliciteScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = ColorLib.Rift;
            WidthDim = 66;
            HeightDim = 66;
            DustType = ModContent.DustType<RiftDust>();
            base.SetDefaults();
        }

        public override void AI()
        {
            base.AI();
            if (Main.rand.NextBool())
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(0.5f, 0.5f), ModContent.ProjectileType<SolarTrail>(), Projectile.damage / 5, 0f, Projectile.owner);
            }
        }
    }
}


