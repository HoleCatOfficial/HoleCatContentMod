using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Scepter
{
    public class HeliciteScepter : ScepterItem
    {
        public override int Width => 34;
        public override int Height => 34;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 100;
            ShootCrit = 4;
            ThrowCrit = 64;
            KB = 1;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PaleFuchsiaRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<SquareCrystal>();
            ThrowID = ModContent.ProjectileType<HeliciteScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<HolyScepter>()
            .AddIngredient<Item_HeliciteCrystal>(35)
            .AddTile<Tile_RiftConfigurator>()
            .Register();
        }
    }
}