using DestroyerTest.Content.Projectiles.ParentClasses;
using Terraria;

namespace DestroyerTest.Content.Projectiles
{
    public class TestSwing : UpDownSwingProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
        }


        public override void AI()
        {

        }
    }
}