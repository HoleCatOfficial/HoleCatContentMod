
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class NobleBulwarkGold : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 23;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.defense = 12;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += 0.8f;
            ScepterClassStats.Range += 8;
            player.endurance = 1f - (0.1f * (1f - player.endurance));
        }

        

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 12)
                .AddIngredient<VesperOre>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    
    [AutoloadEquip(EquipType.Shield)]
	public class NobleBulwarkPlatinum : ModItem
	{
		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 23;
			Item.value = Item.buyPrice(10);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
			Item.defense = 12;
		}
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += 0.8f;
            ScepterClassStats.Range += 8;
            player.endurance = 1f - (0.1f * (1f - player.endurance));
        }

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddIngredient<VesperOre>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}
}