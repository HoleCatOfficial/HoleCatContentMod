
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class RadiantPendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<WineRarity>();
            Item.accessory = true;
        }

        public static readonly float DMGBonus = 1.10f;
        public static readonly float CritBonus = 1.26f;
        public static readonly int RangeBonus = 30;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DMGBonus - 1f).ToString("P1"), CritBonus.ToString("F1") + "%", RangeBonus);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= DMGBonus;
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += CritBonus;
            ScepterClassStats.Range += RangeBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PalladiumBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}