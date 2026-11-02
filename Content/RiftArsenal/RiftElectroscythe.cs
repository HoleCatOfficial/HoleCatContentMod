using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftArsenal
{
    public class RiftElectroscythe : ModItem, IRechargeFunctionality
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
            Item.rare = ModContent.RarityType<RiftRarity>();

            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = true;


            Item.knockBack = 7;
            Item.autoReuse = true;
            Item.damage = 88;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.shoot = ModContent.ProjectileType<RiftElectroscytheProjectile>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
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