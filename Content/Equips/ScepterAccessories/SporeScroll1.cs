
using DestroyerTest.Common;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
	public class SporeScroll1 : PreHardmodeScroll
	{

		public override void SetDefaults() {
			Item.width = 32;
			Item.height = 30;
            Item.accessory = true;
		}

		public override void UpdateEquip(Player player) {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.TryGetGlobalProjectile<ScrollScepterProj>(out ScrollScepterProj Scptr))
                {
                    Scptr.SporeScroll = true;
                }
            }
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 9)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}
}
