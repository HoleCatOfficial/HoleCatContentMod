using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons.AncientSword;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Fargos.Melee
{
    public class Celestine : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 384;
            Item.height = 384;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 700;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<CelestialRarity>();
            Item.shoot = ModContent.ProjectileType<CelestineSwing>();
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

        public override void AddRecipes()
        {
            /*
            if (DTCrossMod.FargosSoulsIsLoaded)
            {

                CreateRecipe()
                    .AddIngredient<Moongeist>()
                    .AddIngredient(DTCrossMod.GetItem(DTCrossMod.FargosSoulsMod, "Eridanium"), 14)
                    .AddTile(TileID.LunarCraftingStation)
                    .Register();
            }
            */
        }
    }
}