using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Scepter
{
    public class EmberCane : ScepterItem
    {
        public override int Width => 46;
        public override int Height => 38;

        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 20;
            ShootCrit = 2;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ItemRarityID.LightRed;

            // Assign projectile types
            ShootID = ProjectileID.Spark;
            ThrowID = ModContent.ProjectileType<EmberCaneThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AshWood, 12)
                .Register();
        }
    }
}