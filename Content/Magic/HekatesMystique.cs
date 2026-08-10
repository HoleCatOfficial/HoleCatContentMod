using DestroyerTest.Content.Projectiles;  
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UtfUnknown.Core.Models.SingleByte.Italian;

namespace DestroyerTest.Content.Magic
{
	public class HekatesMystique : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
		public override void SetDefaults()
		{
			Item.width = 50;
			Item.height = 60;
			Item.value = Item.sellPrice(gold: 25, silver: 70);
			Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();

			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.knockBack = 10;
			Item.autoReuse = true;
			Item.damage = 25;
			Item.DamageType = DamageClass.Magic;
			Item.channel = true;
			Item.mana = 25;
			Item.crit = 5;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useTurn = true;

			Item.shoot = ModContent.ProjectileType<HekateStaffProj>();
            Item.shootSpeed = 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<HekatesMystiqueHoldout>(), 0, 0, player.whoAmI);
            return true;
        }
		
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(Type)
                .AddIngredient<Dyrn>(8)
                .Register();
        }
    }
} 