
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{

	public class HeliciteShank : ModItem
	{

		public override void SetDefaults() 
		{
			Item.width = 64;
			Item.height = 76;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ModContent.RarityType<HeliciteRarity>();
			Item.UseSound = DTAssetLib.SwordSounds.StandardSwing with { Pitch = 1.4f, PitchVariance = 0.1f, MaxInstances = 0 };
			Item.useTime = 7;
			Item.useAnimation = 7;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.knockBack = 7;
			Item.damage = 160;
            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
			Item.noMelee = true;
			Item.noUseGraphic = true;

			Item.shoot = ModContent.ProjectileType<HeliciteShankProjectile>();
		}

		public override bool MeleePrefix() 
		{
			return true;
		}
	}
}