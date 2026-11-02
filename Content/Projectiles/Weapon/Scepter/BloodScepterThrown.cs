using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class BloodScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Red;
            WidthDim = 54;
            HeightDim = 54;
            DustType = DustID.Blood;
            base.SetDefaults();
        }
    }
}

