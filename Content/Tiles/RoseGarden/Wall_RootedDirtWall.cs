using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
	public class Wall_RootedDirtWall : ModWall
	{
		public override void SetStaticDefaults() {
			AddMapEntry(new Color(69, 54, 63));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}
	}
}