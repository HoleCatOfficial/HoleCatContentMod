using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Body)]
    public class RiftVeil : ModItem
    {
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ModContent.RarityType<RiftRarity1>();
            Item.defense = 26;
        }

        public override void UpdateEquip(Player player)
        {
			player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += 10;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<Item_RiftStone>(40)
                .AddIngredient<Living_Shadow>(60)
				.AddTile(TileID.MythrilAnvil)
				.Register();
        }
	}
}