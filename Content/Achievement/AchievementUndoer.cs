using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Achievement
{
    public class AchievementUndoer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 98;
            Item.height = 90;
            Item.UseSound = SoundID.Item116;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}
