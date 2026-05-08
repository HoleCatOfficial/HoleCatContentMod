using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Common;

namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftSpine : ModItem, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }
        
		public override void SetStaticDefaults() {

		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shootSpeed = 22f;
			Item.shoot = ModContent.ProjectileType<RiftSpine_Thrown>();
			Item.width = 24;
			Item.height = 96;
			Item.maxStack = 1;
			Item.consumable = false;
			Item.UseSound = SoundID.Item71;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 0, 20, 0);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.damage = 160;
			Item.DamageType = ModContent.GetInstance<DTRogueClass>();
			Item.autoReuse = true;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<RiftMaker>()
                .AddIngredient<Item_HeliciteCrystal>(15)
				.AddTile<Tile_RiftConfiguratorWeaponry>()
				.Register();
		}
	}
}