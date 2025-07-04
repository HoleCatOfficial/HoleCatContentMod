using DestroyerTest.Content.MetallurgySeries;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dye
{
	public class TenebrousBlueDye : ModItem
	{
		public override void SetStaticDefaults() {
			// Avoid loading assets on dedicated servers. They don't use graphics cards.
			if (!Main.dedServ) {
				// The following code creates an effect (shader) reference and associates it with this item's type Id.
				//GameShaders.Armor.BindShader(
					//Item.type,
					//new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/TenebrousBlue"), "DyePass")
					//.UseColor(new Color(87, 99, 186))
					//.UseSecondaryColor(new Color(0, 0, 0))
				//);

			}

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;

			Item.ResearchUnlockCount = 3;
		}
		
		

		public override void SetDefaults()
		{
			// Item.dye will already be assigned to this item prior to SetDefaults because of the above GameShaders.Armor.BindShader code in Load().
			// This code here remembers Item.dye so that information isn't lost during CloneDefaults.
			//int dye = Item.dye;
			Item.CloneDefaults(ItemID.GelDye);
			//Item.dye = dye;
			Item.width = 16;
			Item.height = 24;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Cyan; // The rarity of the item
			Item.maxStack = 999; // The maximum stack size of the item
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tenebris>(2)
                .AddIngredient<EchoFluid>()
                .AddTile(TileID.DyeVat)
                .Register();
        }
	}
}