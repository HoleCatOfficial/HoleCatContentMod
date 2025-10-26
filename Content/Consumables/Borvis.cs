using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Consumables
{
	public class Borvis : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;
		}

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 14;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 15;
            Item.useTime = 90;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item2;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 3);
            Item.buffType = BuffID.Lucky;
            Item.buffTime = 60 * 240; //60 ticks per second. Times 240, which is 4 times the 60 seconds in a minute. 4 minute buff time.
        }

        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(BuffID.WellFed3);
        }
	}
}