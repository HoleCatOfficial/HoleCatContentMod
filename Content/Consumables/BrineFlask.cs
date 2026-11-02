using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Content.Consumables.Flasks;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class BrineFlask : BaseFlask
    {
        public override Color[] DrinkColors => [Color.SkyBlue, Color.White];

        public override int BuffType => ModContent.BuffType<WeaponImbueBrine>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ItemRarityID.White;

    }
}