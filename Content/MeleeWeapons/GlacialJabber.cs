using DestroyerTest.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class GlacialJabber : ModItem
    {
        public override void SetStaticDefaults() 
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults() 
        {
            Item.width = 150; 
            Item.height = 150; 

            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 10);

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 16;
            Item.knockBack = 6.5f;
            Item.noUseGraphic = true; 
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<GlacialJabberProjectile>();
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<GlacialJabberAltProjectile>();
            }
            else
            {
                type = ModContent.ProjectileType<GlacialJabberProjectile>();
            }
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<GlacialJabberAltProjectile>()] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
        public override void AddRecipes() 
        {
			CreateRecipe()
                .AddIngredient(ItemID.IceBlock, 5)
                .AddIngredient(ItemID.PlatinumBar, 3)
                .AddIngredient(ItemID.BorealWood, 3)
                .AddTile(TileID.Anvils)
				.Register();
		    }
        }
}

		