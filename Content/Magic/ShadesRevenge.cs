using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Rarity;
using System.Linq;
using UtfUnknown.Core.Models.SingleByte.Italian;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Magic
{
	public class ShadesRevenge : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
		public override void SetDefaults()
		{
			Item.width = 76;
			Item.height = 78;
			Item.value = Item.sellPrice(gold: 25, silver: 70);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();

			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.knockBack = 10;
			Item.autoReuse = true;
			Item.damage = 240;
			Item.DamageType = DamageClass.Magic;
			Item.channel = true;
			Item.mana = 75;
			Item.crit = 21;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useTurn = true;

			Item.shoot = ModContent.ProjectileType<ShadesRevengeProj>();
            Item.shootSpeed = 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShadesRevengeHoldout>(), 0, 0, player.whoAmI);
            return true;
        }
		
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MajesticStorm>()
                .AddIngredient<Tenebris>(30)
                .AddIngredient<ShimmeringShards>(8)
                .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
} 