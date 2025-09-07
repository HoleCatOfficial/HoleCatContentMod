
using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Equips.DistendedSet
{
	[AutoloadEquip(EquipType.Body)]
    public class DistendedBodyArmor : ModItem
    {
        public int equipBack = -1;
        
        public override void Load()

        { 
            if (Main.netMode != NetmodeID.Server) {
                equipBack = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back, this);
            }
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.IncludedCapeBack[Item.bodySlot] = equipBack;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[Item.bodySlot] = equipBack;
        }
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 34; 
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
			Item.defense = 11;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
                .AddIngredient(ItemID.CrimsonScalemail, 1)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}