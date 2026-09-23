using DestroyerTest.Common;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Projectiles;  
using DestroyerTest.Content.Remnants;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
	public class StellarBow : ModItem
	{
        public override void SetStaticDefaults()
        {
            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<StellarFlames>();
            }
        }
        public override void SetDefaults() 
		{
			Item.width = 28;
			Item.height = 72;
			Item.value = Item.sellPrice(gold: 25, silver: 70);
			Item.rare = ModContent.RarityType<StellarRarity>();
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 10;
			Item.autoReuse = true;
			Item.damage = 35;
			Item.DamageType = DamageClass.Ranged; 
            Item.channel = true;
            Item.crit = 16;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<StellarBowHoldout>();
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.RemnantsIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<ConstitutionArtifact>(3)
                    .Register();
            }
        }
    }
} 