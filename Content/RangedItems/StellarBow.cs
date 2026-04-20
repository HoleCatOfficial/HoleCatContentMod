using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;  
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.RangedItems
{
	public class StellarBow : ModItem
	{
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
    }
} 