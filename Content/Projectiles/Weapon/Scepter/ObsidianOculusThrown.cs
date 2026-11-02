using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class ObsidianOculusThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.White;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.Obsidian;
            base.SetDefaults();
        }
    }

}

