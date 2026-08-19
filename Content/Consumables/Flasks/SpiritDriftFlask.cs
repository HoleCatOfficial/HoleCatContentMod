using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public class SpiritDriftFlask : BaseFlask
    {
        public override Color[] DrinkColors => [Color.SkyBlue, Color.Navy];

        public override int BuffType => ModContent.BuffType<WeaponImbueSpiritDrift>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ModContent.RarityType<SoulRarity>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Ectoplasm, 4)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }
}
