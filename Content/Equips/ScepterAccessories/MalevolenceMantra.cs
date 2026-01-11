
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
    public class MalevolenceMantra : EarlyHardmodeScroll
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
			{
				Scptr.HellfireScroll1 = true;
			}
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 9)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}