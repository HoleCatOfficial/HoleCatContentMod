using DestroyerTest;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class Gargantua : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item101;
            Item.width = 122;
            Item.height = 122;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 87;
            Item.knockBack = 6;
            Item.crit = 10;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.shoot = ModContent.ProjectileType<GargantuaProjectile>();
            Item.noUseGraphic = true;
            Item.channel = true;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<Goliath>(1)
                .AddIngredient<LivingDiamond>(14)
                .AddIngredient(ItemID.SpectreBar, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
}