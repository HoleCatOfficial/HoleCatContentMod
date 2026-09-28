using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class CaveDeck : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<DepthsPlayer>(out var deep))
            {
                deep.Active = true;
            }
            player.pickSpeed -= 0.22f;
            player.moveSpeed += 0.1f;
            if (player.TryGetModPlayer<FallenPlayer>(out var Fall))
            {
                Fall.Active = true;
            }
            if (player.TryGetModPlayer<InstinctPlayer>(out var instinct))
            {
                instinct.Active = true;
            }

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Instinct>()
                .AddIngredient<Expedition>()
                .AddIngredient<Depths>()
                .AddIngredient<Fallen>()
                .AddIngredient(ItemID.DirtBlock, 4)
                .AddIngredient(ItemID.StoneBlock, 4)
                .AddIngredient(ItemID.Obsidian, 4)
                .AddIngredient(ItemID.Hellstone, 4)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}
