using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using DestroyerTest.Common;

namespace DestroyerTest.Rarity
{
    public class CelestialRarity : ModRarity
    {
        public override Color RarityColor => ColorLib.CelestialGradient;

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type;
        }
    }

}