
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;



namespace DestroyerTest.Content.MeleeWeapons
{
	public class ScarletDragon : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TwistedFaith>();
        }

        public override void SetDefaults() 
		{
			Item.width = 94;
			Item.height = 98;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
			Item.damage = 35; 
			Item.knockBack = 5;
			Item.crit = 6;

			Item.value = Item.buyPrice(gold: 16);
			Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
			Item.UseSound = SoundID.Item71;
		}

        public override bool MeleePrefix()
        {
            return true;
        }

    }
}
