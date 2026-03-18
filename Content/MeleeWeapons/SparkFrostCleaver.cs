  
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class SparkFrostCleaver : ModItem
	{

        public override void SetDefaults() {
			Item.width = 162;
			Item.height = 162;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Green;
			Item.useTime = 120;
			Item.useAnimation = 120;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 70;
			Item.autoReuse = false;
			Item.damage = 95;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true; 
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<SparkFrostCleaverSwing>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

		public override void AddRecipes() 
		{
			CreateRecipe()
                .AddIngredient(ItemID.WandofSparking)
				.AddIngredient(ItemID.WandofFrosting)
				.AddIngredient(ItemID.SoulofMight, 12)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}