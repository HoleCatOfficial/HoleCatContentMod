using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class HallowedPall : ModItem
	{


		public override void SetStaticDefaults() 
        {


		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 26;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<WineRarity>();
			Item.defense = 16;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ItemID.HallowedPlateMail && legs.type == ItemID.HallowedGreaves;
		}

        public override void UpdateEquip(Player player)
        {
            ScepterClassStats.Range += 25;
        }	

		public override void UpdateArmorSet(Player player) 
		{
			player.DefaultSetBonusText(player.armor[0]);
			player.GetDamage<ScepterClass>() += 0.12f;
		}

		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}