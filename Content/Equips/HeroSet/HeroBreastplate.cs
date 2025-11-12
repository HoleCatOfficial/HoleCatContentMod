
using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Equips.HeroSet
{
// This item is meant to mirror the effects of the Hallowed Plate Mail, which equips a Cape without needing a separate cape Item. 

	[AutoloadEquip(EquipType.Body)] // As usual, we must tell the game what part of the body the item will be equipped on.
    	public class HeroBreastplate : ModItem
		{
        public int equipBack = -1; // It would be best not to tamper with this.
        
        public override void Load() // This fetches the texture we need

        { 
            if (Main.netMode != NetmodeID.Server) {
                equipBack = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back, this);
            }
        }

        public override void SetStaticDefaults() // These will display the texture we fetched, and are specifically for this purpose.
        {
            ArmorIDs.Body.Sets.IncludedCapeBack[Item.bodySlot] = equipBack;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[Item.bodySlot] = equipBack;
        }
		public override void SetDefaults() // Simple item properties. Nothing new here.
		{
			Item.width = 18;
			Item.height = 18; 
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<GoliathRarity>();
			Item.defense = 5;
		}
	}
}