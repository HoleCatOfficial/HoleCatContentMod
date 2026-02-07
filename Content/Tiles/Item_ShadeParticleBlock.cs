
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles
{
	public class Item_ShadeParticleBlock : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tile_ShadeParticleBlock>());
			Item.width = 16;
			Item.height = 16;
		}

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
			Texture2D texture = TextureAssets.Item[Type].Value;
			spriteBatch.Draw(texture, position, frame, ColorLib.TenebrisGradient, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
			Texture2D texture = TextureAssets.Item[Type].Value;
			spriteBatch.Draw(texture, Item.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AddRecipes()
        {
			CreateRecipe(40)
				.AddIngredient<ShadeParticle>(4)
				.AddIngredient(ItemID.StoneBlock, 10)
				.AddTile(TileID.Blendomatic)
				.Register();
        }

	}
}