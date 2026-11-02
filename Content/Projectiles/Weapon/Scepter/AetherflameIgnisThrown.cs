using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class AetherflameIgnisThrown : ThrownScepter
    {
        public override Asset<Texture2D> GlowMask => ModContent.Request<Texture2D>(Texture + "_Glow");

        public override void SetDefaults()
        {
            ThemeColor = Color.Coral;
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

