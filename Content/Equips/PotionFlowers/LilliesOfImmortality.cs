using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class LilliesOfImmortality : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 98;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (equippedItem.type == ItemID.AnkhShield || incomingItem.type == ItemID.AnkhShield)
            {
                return false;
            }
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<PotionFlowerPlayer>(out PotionFlowerPlayer flower))
            {
                flower.Lillies = true;
            }
            if (Item.TryGetGlobalItem<ModifyPotionsItem>(out ModifyPotionsItem Potions))
            {
                Potions.Lillies = true;
            }
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EphemeralSolvent>(1)
                .AddIngredient(ItemID.AnkhCharm, 1)
                .AddIngredient(ItemID.LunarOre, 8)
                .AddIngredient<Tenebris>(6)
                .Register();
        }
	}
}