using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class CoralScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.SkyBlue;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.BeachShell;
            base.SetDefaults();
        }
    }
}

