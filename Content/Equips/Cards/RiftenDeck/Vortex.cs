
using DestroyerTest.Content.RiftBiome.RiftDesertResources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.Cards.RiftenDeck
{
    public class Vortex : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<RiftRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            foreach (Item item in Main.item)
            {
                int tm = player.GetItemGrabRange(item);
                tm = (int)(tm * 1.10f);
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<ShineShadeDeck>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_RiftStone>(6)
                .AddIngredient<Item_RiftSilt>(6)
            .Register();
        }
    }
}