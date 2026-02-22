using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;

namespace DestroyerTest.Content.RogueItems
{
    public class GalantineLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<GalantineLanceFriendly>();
            Item.width = 18;
            Item.height = 102;
            Item.UseSound = SoundID.Item71;
            Item.useAnimation = 30; // Increased for custom animation timing
            Item.useTime = 30;      // Match useAnimation for proper animation flow
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.damage = 30;
            Item.autoReuse = false; // Prevents animation overlap
        }

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<StellarMatter>(), 10)
                .AddIngredient(ItemID.Javelin, 1)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}