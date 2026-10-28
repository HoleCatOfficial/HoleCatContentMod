using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class MidnightBloomThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Blue;
            WidthDim = 56;
            HeightDim = 54;
            DustType = DustID.BlueTorch;
            base.SetDefaults();
        }
    }
}

