using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class MidnightBloomThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Blue;
            WidthDim = 56;
            HeightDim = 54;
            DustType = DustID.BlueTorch;
            base.SetDefaults();
        }
    }
}

