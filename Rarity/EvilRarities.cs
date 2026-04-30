using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace DestroyerTest.Rarity
{
	public class CrimsonSpecialRarity : ModRarity
    {
        public override Color RarityColor => new Color(50, 0, 0); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0) 
            {
                return ItemRarityID.Green; // Upgrade to next rarity if necessary
            }

            return Type; // No lower tier, so return itself
        }
    }

    public class CorruptionSpecialRarity : ModRarity
    {
        public override Color RarityColor => new Color(50, 50, 0); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0) 
            {
                return ItemRarityID.Green; // Upgrade to next rarity if necessary
            }

            return Type; // No lower tier, so return itself
        }
    }

    public class HallowedSpecialRarity : ModRarity
    {
        public override Color RarityColor => new Color(20, 60, 0); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0) 
            {
                return ItemRarityID.Green; // Upgrade to next rarity if necessary
            }

            return Type; // No lower tier, so return itself
        }
    }

    public class WretchedRarity : ModRarity
    {
        public override Color RarityColor => Opus.Sine(ColorLib.Wretched6, ColorLib.Wretched7, 0.01f); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ModContent.RarityType<CorruptionSpecialRarity>();
            }

            return Type;
        }
    }

    public class PrimalRarity : ModRarity
    {
        public override Color RarityColor => Opus.Sine(Color.DarkRed, Color.MediumVioletRed, 0.1f); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ModContent.RarityType<CrimsonSpecialRarity>();
            }

            return Type;
        }
    }

}