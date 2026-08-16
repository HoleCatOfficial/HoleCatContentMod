using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using Terraria.Audio;
using DestroyerTest.Common;

namespace DestroyerTest.Content.RogueItems
{
    public class DreamDisc : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<DreamDiscThrown>();
            Item.width = 64;
            Item.height = 64;
            Item.UseSound = DTAssetLib.SwordSounds.QuickSwing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.reuseDelay = 25;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.White;
            Item.damage = 16;
            Item.knockBack = 5;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Throwing;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Vesper>(18)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}