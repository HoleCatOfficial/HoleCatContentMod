using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class CosmicPingThrown : ThrownScepter
    {

        public override void SetDefaults()
        {
            ThemeColor = ColorLib.StellarFire3;
            WidthDim = 48;
            HeightDim = 48;
            DustType = DustID.Torch;


            base.SetDefaults();
        }

        public override void PostAI()
        {
            base.PostAI();


        }
    }
}

