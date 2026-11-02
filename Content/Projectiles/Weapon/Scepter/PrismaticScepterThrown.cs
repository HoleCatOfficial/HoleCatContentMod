using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class PrismaticScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Main.DiscoColor;
            WidthDim = 40;
            HeightDim = 40;
            DustType = ModContent.DustType<ColorableNeonDust>();
            base.SetDefaults();
        }
    }
}

