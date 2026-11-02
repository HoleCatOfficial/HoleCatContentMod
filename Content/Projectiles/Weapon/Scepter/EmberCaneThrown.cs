using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class EmberCaneThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.OrangeRed;
            WidthDim = 46;
            HeightDim = 38;
            DustType = DustID.Torch;
            base.SetDefaults();
        }
    }
}

