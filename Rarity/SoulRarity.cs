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
    public class SoulRarity : ModRarity
    {
        public override Color RarityColor => ColorLib.Soul3;

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ItemRarityID.Master;
            }

            return Type;
        }
    }

}