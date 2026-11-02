using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class StarFangNecklace : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.maxStack = 1;
            Item.value = 320;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<StellarRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration(DamageClass.Melee) += 13;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SharkToothNecklace)
                .AddIngredient<StellarMatter>(10)
                .Register();
        }
    }
}
