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

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class HeliciteScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = ColorLib.Rift;
            WidthDim = 66;
            HeightDim = 66;
            DustType = ModContent.DustType<RiftDust>();
            base.SetDefaults();
        }
    }
}


