  
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Projectiles;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class TwistedFaith : ModItem
	{
        public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ScarletDragon>();
        }
		public override void SetDefaults() {
			Item.width = 96;
			Item.height = 97;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 35;
			Item.useAnimation = 35;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 35;
			Item.knockBack = 12; 
			Item.crit = 6;
			Item.shoot = ModContent.ProjectileType<TwistedFaithSwing>();
			Item.shootsEveryUse = true;
			Item.autoReuse = true;

			Item.value = Item.buyPrice(gold: 16); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
			Item.UseSound = Item.UseSound = DTAssetLib.SwordSounds.ConSwing;
		}

        public override bool MeleePrefix()
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
			Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI); // Sync the changes in multiplayer.
            

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
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
