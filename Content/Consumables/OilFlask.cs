using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity.Scepter;

namespace DestroyerTest.Content.Consumables
{
	public class OilFlask : ModItem
	{
		public override void SetStaticDefaults() 
		{
			ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<OilVial>()] = Type;
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				Color.Black
			];
		}

		public override void SetDefaults() 
		{
			Item.width = 22;
			Item.height = 32;
			Item.DefaultToFlask(ModContent.BuffType<WeaponImbueFire>(), ModContent.RarityType<PearlRarity>(), Item.sellPrice(0, 0, 5));
		}
	}
}