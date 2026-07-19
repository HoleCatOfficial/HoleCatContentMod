
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
    public class AzurePendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<WineRarity>();
            Item.accessory = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return (incomingItem.type != ModContent.ItemType<PendantofUnity>() || incomingItem.type != ModContent.ItemType<ElementalPendant>());
        }

        public static readonly float DMGBonus = 0.10f;
        public static readonly int CritBonus = 10;
        public static readonly int RangeBonus = 25;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DMGBonus, CritBonus.ToString("F1") + "%", RangeBonus);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += DMGBonus;
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += CritBonus;
            player.ScepterClass().Range += RangeBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}