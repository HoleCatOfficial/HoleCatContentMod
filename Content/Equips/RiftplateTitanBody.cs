using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{

	[AutoloadEquip(EquipType.Body)]
    	public class RiftplateTitanBody : ModItem, IRechargeFunctionality
		{
            public bool Energized
            {
                get
                {
                    return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
                }
            }
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
			Item.width = 18;
			Item.height = 18; 
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.defense = 22;
		}

        public override void UpdateEquip(Player player)
        {
            player.endurance += 0.05f;

            if (Energized)
            {
                player.endurance += 0.02f;
            }
        }

		public override void AddRecipes()
		{
			CreateRecipe()
                .AddIngredient<Living_Shadow>(20)
                .AddIngredient<Item_Riftplate>(20)
                .AddTile<Tile_RiftConfigurator>()
                .Register();
		}
	}
}