using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftArsenal
{
    [AutoloadGlowmask]
    public class RiftStaff : ModItem, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }

        public override string Texture => "DestroyerTest/Content/RiftArsenal/RiftStaff";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<RiftStaffHoldout>();
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.width = 92;
            Item.height = 92;
            Item.autoReuse = true;
            Item.crit = 12;
            Item.rare = ModContent.RarityType<RiftRarity>();
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/RiftClaymorePowerStrike");
            Item.channel = true;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 70;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowCircuitry>(4)
                .AddIngredient<Item_Riftplate>(20)
                .AddTile<Tile_RiftConfigurator>()
            .Register();
        }
    }
}