using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class FoxScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Orange;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.Iron;
            base.SetDefaults();
        }
    }
}

