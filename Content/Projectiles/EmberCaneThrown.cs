using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles
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

