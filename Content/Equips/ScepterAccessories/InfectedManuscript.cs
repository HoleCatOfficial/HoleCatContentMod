
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class InfectedManuscript : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.TryGetGlobalProjectile<ScrollScepterProj>(out ScrollScepterProj Scptr))
                {
                    Scptr.IchorScroll = true;
                    Scptr.CursedFlameScroll = true;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<IchorScroll>(), 1)
                .AddIngredient(ModContent.ItemType<CursedFlameScroll>(), 1)
                .AddIngredient(ModContent.ItemType<WretchedShards>(), 3)
                .AddIngredient(ModContent.ItemType<PrimalShards>(), 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}