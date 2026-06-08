using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.GameContent.Drawing;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using OpusLib;
using ReLogic.Content;

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

