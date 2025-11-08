
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
    public class StellarManuscript : PreHardmodeScroll
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 32;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
            {
                Scptr.StarScroll = true;
                Scptr.GalantineScroll = true;
            }
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<StarScroll>(), 1)
                .AddIngredient(ModContent.ItemType<GalantineScroll>(), 1)
                .AddIngredient(ModContent.ItemType<StellarMatter>(), 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}