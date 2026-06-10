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

