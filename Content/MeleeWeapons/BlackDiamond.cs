using DestroyerTest.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class BlackDiamond : ModItem
    {

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 160;
            Item.height = 160;

            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 10);

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = DTAssetLib.SwordSounds.EvilSwing;
            Item.autoReuse = true;

            Item.damage = 195;
            Item.knockBack = 6.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<BlackDiamondProjectile>();
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(Item.UseSound, player.Center);
            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gungnir, 1)
                .AddIngredient<Tenebris>(12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

}

