
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class WoodBracelet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.White;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.02f;
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += 4f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}