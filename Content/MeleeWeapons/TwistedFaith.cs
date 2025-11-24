  
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

namespace DestroyerTest.Content.MeleeWeapons
{
	public class TwistedFaith : ModItem
	{
		public override void SetDefaults() {
			Item.width = 96;
			Item.height = 97;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 35;
			Item.knockBack = 12; 
			Item.crit = 6;
			Item.shoot = ModContent.ProjectileType<TwistedFaithSwing>();
			Item.shootsEveryUse = true;
			Item.autoReuse = true;

			Item.value = Item.buyPrice(gold: 16); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
			Item.UseSound = Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionT3Slash") with { MaxInstances = 0, PitchVariance = 0.25f };
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
			Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI); // Sync the changes in multiplayer.

			Vector2 MS = player.Center - Main.MouseWorld;
            float Rot = MS.ToRotation();
            Projectile.NewProjectile(source, position, new Vector2(-4, 0).RotatedBy(Rot), ModContent.ProjectileType<TwistedFaithProj>(), damage / 2, 1.5f, player.whoAmI);
            

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}
	}
}
