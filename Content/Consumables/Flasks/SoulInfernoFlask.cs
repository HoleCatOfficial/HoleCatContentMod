using DestroyerTest.Common;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public class SoulInfernoFlask : BaseFlask
    {
        public override Color[] DrinkColors => [ColorLib.Soul, ColorLib.Soul2, ColorLib.Soul3];

        public override int BuffType => ModContent.BuffType<WeaponImbueSoulInferno>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ModContent.RarityType<SoulRarity>();
    }
}
