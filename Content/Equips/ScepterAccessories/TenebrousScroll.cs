
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class TenebrousScroll : LateHardmodeScroll
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.buyPrice(10);
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
			{
				Scptr.TenebrousScroll = true;
			}
        }
    }
}