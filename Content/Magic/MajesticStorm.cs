using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles; // Add this line if CT3_Swing is in the Projectiles namespace
using DestroyerTest.Rarity;
using System.Linq;
using UtfUnknown.Core.Models.SingleByte.Italian;

namespace DestroyerTest.Content.Magic
{
	public class MajesticStorm : ModItem
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
			Item.rare = ItemRarityID.Blue;

			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.knockBack = 10;
			Item.autoReuse = true;
			Item.damage = 55;
			Item.DamageType = DamageClass.Magic;
			Item.channel = true;
			Item.mana = 60;
			Item.crit = 5;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useTurn = true;

			Item.shoot = ModContent.ProjectileType<MajesticStormProj>();
            Item.shootSpeed = 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<MajesticStormHoldout>(), 0, 0, player.whoAmI);
            return true;
        }
		
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HekatesMystique>()
                .AddIngredient(ItemID.MagnetSphere)
				.AddIngredient(ItemID.Ectoplasm, 6)
				.AddIngredient(ItemID.PixieDust, 6)
                .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
} 