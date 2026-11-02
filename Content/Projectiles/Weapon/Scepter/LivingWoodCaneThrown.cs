using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class LivingWoodCaneThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.LawnGreen;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.GrassBlades;
            base.SetDefaults();
        }
    }
}

