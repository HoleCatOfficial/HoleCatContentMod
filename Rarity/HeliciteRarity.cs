using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria.ModLoader;

namespace DestroyerTest.Rarity
{
    public class HeliciteRarity : ModRarity
    {
        public override Color RarityColor => OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type;
        }
    }

}