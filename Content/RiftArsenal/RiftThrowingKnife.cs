using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftArsenal
{
    public class RiftThrowingKnife : ModItem, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 01f;
            Item.shoot = ModContent.ProjectileType<RiftThrowingKnifeThrown>();
            Item.width = 14;
            Item.height = 34;
            Item.maxStack = 100;
            Item.consumable = true;
            Item.UseSound = SoundID.Item71;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ModContent.RarityType<RiftRarity>();
            Item.damage = 60;
            Item.DamageType = DamageClass.Throwing;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_Riftplate>(10)
                .AddTile<Tile_RiftConfigurator>()
            .Register();
        }
    }
}