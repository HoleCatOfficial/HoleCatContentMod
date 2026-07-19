
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class PendantofUnity : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(5000);
            Item.rare = ModContent.RarityType<WineRarity>();
            Item.accessory = true;
        }

        public static List<int> ItemsThatPendantofUnityCannotPairWith = new List<int>
        {
            ModContent.ItemType<InfectedPendant>(),
            ModContent.ItemType<ElementalPendant>(),
            ModContent.ItemType<AzurePendant>(),
            ModContent.ItemType<RadiantPendant>(),
            ModContent.ItemType<MythicPendant>(),
            ModContent.ItemType<OrchidPendant>(),
            ModContent.ItemType<AdamantPendant>(),
            ModContent.ItemType<TitanPendant>()
        };


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += 0.2f;
            player.GetArmorPenetration(ModContent.GetInstance<ScepterClass>()) += 10;
            player.ScepterClass().Range += 120;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AzurePendant>()
                .AddIngredient<RadiantPendant>()
                .AddIngredient<MythicPendant>()
                .AddIngredient<OrchidPendant>()
                .AddIngredient<AdamantPendant>()
                .AddIngredient<TitanPendant>()
                .AddIngredient(ItemID.SoulofFright, 16)
                .AddIngredient(ItemID.SoulofMight, 16)
                .AddIngredient(ItemID.SoulofSight, 16)
                .AddIngredient(ItemID.ChlorophyteBar, 6)
                .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}