using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class CursedHammer : ModItem
    {

        public override void SetStaticDefaults()
        {
            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BlossomBeater>();
            }
        }

        public int HitCount = 0;

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 6;
            Item.autoReuse = true;
            Item.damage = 200;
            Item.DamageType = DamageClass.Throwing;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<CursedHammerThrown>();
            Item.shootSpeed = 55f;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.WGIIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<NightmareRoseArtifact>(3)
                    .Register();
            }
        }
    }
}