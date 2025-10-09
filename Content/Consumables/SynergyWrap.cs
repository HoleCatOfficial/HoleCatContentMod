using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Consumables
{
    public class SynergyWrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<Dalmon>()] = ModContent.ItemType<SynergyWrap>();
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 48;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 15;
            Item.useTime = 90;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 3);
        }

        public override bool? UseItem(Player player)
        {
            if (player.TryGetModPlayer<DalmonPlayer>(out DalmonPlayer DL))
            {
                DL.PermaBuff = true;
            }
            return true;
        }
	}
}