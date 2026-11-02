using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class QITNThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = ColorLib.LifeEcho;
            WidthDim = 56;
            HeightDim = 56;
            DustType = DustID.Glass;
            base.SetDefaults();
        }
    }
}

