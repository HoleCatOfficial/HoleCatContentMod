using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Scepter
{
    public class DarkRapture : ScepterItem
    {
        public override int Width => 56;
        public override int Height => 56;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 175;
            ShootCrit = 6;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();


            // Assign projectile types
            ShootID = ModContent.ProjectileType<ShimmeringSpark>();
            ThrowID = ModContent.ProjectileType<DarkRaptureThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item156;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tenebris>(12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}