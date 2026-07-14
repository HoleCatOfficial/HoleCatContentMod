  
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria.Audio;
using DestroyerTest.Content.Tools;
using DestroyerTest.Common;
using System.Collections.Generic;

using Terraria.Localization;
 
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Projectiles.Weapon.Melee;

namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftClaymore : ModItem, IRechargeFunctionality
	{
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }
        
		public override void SetDefaults() 
		{
			Item.width = 46;
			Item.height = 48;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ModContent.RarityType<RiftRarity1>();

			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/ManaBurst") with { PitchVariance = 0.2f };

			Item.knockBack = 7;
			Item.autoReuse = true;
			Item.damage = 88;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;

			Item.shoot = ModContent.ProjectileType<RiftClaymoreSlash>();
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
				.AddIngredient<Item_Riftplate>(28)
				.AddTile<Tile_RiftConfigurator>()
			.Register();
		}
	}
}