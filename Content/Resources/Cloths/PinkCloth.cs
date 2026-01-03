using DestroyerTest.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources.Cloths
{
	public class PinkCloth : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.StaticDefaultToCloth();
		}

		public override void SetDefaults() 
		{
			Item.DefaultToCloth();
		}

		public override void AddRecipes() 
		{
			Item.DefaultRecipe(ItemID.Plum);
		}
	}
}