using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using GlowmaskHelper.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons.AncientSword
{
    [AutoloadGlowmask]
    public class Calosar : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 100;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 400;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<CalosarSwing>();
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

            CreateRecipe()
                .AddIngredient<Lifeblade>()
                .AddIngredient<HellArmorScrap>(14)
                .AddTile(TileID.MythrilAnvil)
                .Register();

        }
    }
}