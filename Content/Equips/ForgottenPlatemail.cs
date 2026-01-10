using rail;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Rarity.Scepter;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Body)]
	public class ForgottenPlatemail : ModItem
	{
        /*
        public int equipBack = -1;
        public int equipFront = -1;

        public override void Load() {

            if (Main.netMode != NetmodeID.Server) {
                equipBack = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back, this);
                equipFront = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Front}", EquipType.Front, this);
            }
        }

        public override void SetStaticDefaults() {

            ArmorIDs.Body.Sets.IncludedCapeBack[Item.bodySlot] = equipBack;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[Item.bodySlot] = equipBack;
            ArmorIDs.Body.Sets.IncludedCapeFront[Item.bodySlot] = equipFront;
        }
        */


		public override void SetDefaults() {
			Item.width = 42;
			Item.height = 24;
			Item.value = Item.sellPrice(silver: 75);
			Item.rare = ModContent.RarityType<PearlRarity>();
			Item.defense = 7;
		}
	}
}