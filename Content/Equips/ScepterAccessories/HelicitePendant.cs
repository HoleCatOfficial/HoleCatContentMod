
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class HelicitePendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<CerisePinkRarity>();
            Item.accessory = true;
        }

        public static readonly float DMGBonus = 0.04f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DMGBonus - 1f).ToString("P1"));
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += DMGBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_HeliciteCrystal>(12)
                .AddTile<Tile_RiftConfigurator>()
                .Register();
        }
    }
}