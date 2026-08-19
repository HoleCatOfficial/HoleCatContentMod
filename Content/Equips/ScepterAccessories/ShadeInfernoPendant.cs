
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
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
    public class ShadeInfernoPendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<WineRarity>();
            Item.accessory = true;
        }

        
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ScepterClass>() += 0.055f;
            player.AddBuff(-1, 60);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FireFeather, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}