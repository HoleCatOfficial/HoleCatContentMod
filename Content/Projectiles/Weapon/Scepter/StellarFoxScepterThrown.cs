using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class StellarFoxScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            int baseFlightTime = 60;

            ThemeColor = ColorLib.StellarFireGradient(flightTime / (float)baseFlightTime);
            WidthDim = 54;
            HeightDim = 54;
            DustType = DustID.FireworksRGB;
            DustUsesColorOnDraw = true;
            base.SetDefaults();
        }
    }
}

