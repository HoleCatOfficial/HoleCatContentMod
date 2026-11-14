using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Magic
{
    public class GhostBoa : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {

            Item.shoot = ModContent.ProjectileType<GhostBoaHoldout>();
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.width = 92;
            Item.height = 92;
            Item.autoReuse = true;
            Item.crit = 12;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.InterruptChannelOnHurt = false;
            Item.StopAnimationOnHurt = false;
            
            Item.useTurn = true;

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 150;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalSerpent)
                .AddIngredient(ItemID.SpectreStaff)
                .AddIngredient<SunscorchedCinder>(6)
                .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}