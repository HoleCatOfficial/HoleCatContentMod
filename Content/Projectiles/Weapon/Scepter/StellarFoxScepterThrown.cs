using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.DataStructures;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class StellarFoxScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            int baseFlightTime = 60;

            ThemeColor = ColorLib.StellarFireGradient((float)flightTime / (float)baseFlightTime);
            WidthDim = 54;
            HeightDim = 54;
            DustType = DustID.FireworksRGB;
            DustUsesColorOnDraw = true;
            base.SetDefaults();
        }
    }
}

