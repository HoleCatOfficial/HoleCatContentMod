using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;  
using DestroyerTest.Content.Resources;  
using DestroyerTest.Rarity;
using System.Linq;
using UtfUnknown.Core.Models.SingleByte.Italian;
using DestroyerTest.Content.Projectiles.Weapon.Magic;

namespace DestroyerTest.Content.Magic
{
	public class Manifest : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
		public override void SetDefaults()
		{
			Item.width = 74;
			Item.height = 74;
			Item.value = Item.sellPrice(gold: 25, silver: 70);
			Item.rare = ModContent.RarityType<VesperRarity>();

			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.knockBack = 10;
			Item.autoReuse = true;
			Item.damage = 10;
			Item.DamageType = DamageClass.Magic;
			Item.channel = true;
			Item.mana = 5;
			Item.crit = 35;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useTurn = true;

			Item.shoot = ModContent.ProjectileType<ManifestHoldout>();
            Item.shootSpeed = 1;
		}
		
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Vesper>(12)
                .AddTile(TileID.Anvils)
            .Register();
        }
    }
} 