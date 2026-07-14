using DestroyerTest.Content.Resources;
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
    public class RiftplateAgilityArmor : ModItem
	{
		public override void SetDefaults() // Simple item properties. Nothing new here.
		{
			Item.width = 18;
			Item.height = 18; 
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<RiftRarity2>(); // The rarity of the item
			Item.defense = 43;
		}

        public override void UpdateEquip(Player player) 
		{
            player.GetModPlayer<RiftAgilityRunSpeeds>().Body = true;
		}

		public override void AddRecipes() //Added to make the item obtainable without needing cheat mods, since many swear by never using cheats, ever.
		{
			CreateRecipe()
                .AddIngredient<Living_Shadow>(20)
                .AddIngredient<Item_Riftplate>(20)
				.AddIngredient(ItemID.AnkletoftheWind)
                .AddTile<Tile_RiftConfigurator>()
                .Register();
		}
	}

    public class RiftAgilityRunSpeeds : ModPlayer
    {
        public bool Body = false;
        public bool Legs = false;
        public override void ResetEffects()
        {
            Body = false;
            Legs = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (Legs)
            {
                Player.maxRunSpeed *= 1.8f;
            }
            if (Body)
            {
                Player.runAcceleration *= 1.75f;
            }
        }
    }
}