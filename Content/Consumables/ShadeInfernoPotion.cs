using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class ShadeInfernoPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;

            ItemID.Sets.DrinkParticleColors[Type] = [
                OpusColorUtils.Pastel(ColorLib.TenebrisBeige, 0.8f),
                OpusColorUtils.Pastel(ColorLib.TenebrisMagenta, 0.8f),
                OpusColorUtils.Pastel(ColorLib.TenebrisBlue, 0.8f)
            ];
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item163;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
            Item.value = Item.buyPrice(gold: 1);
            Item.buffType = ModContent.BuffType<ShadeInfernoRingBuff>();
            Item.buffTime = 12000;
        }
    }
}