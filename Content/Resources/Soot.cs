using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Resources
{
	public class Soot : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 25;
		}

		public override void SetDefaults() {
			Item.width = 28;
			Item.height = 18;
			Item.value = 400;
			Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
		}
	}
}