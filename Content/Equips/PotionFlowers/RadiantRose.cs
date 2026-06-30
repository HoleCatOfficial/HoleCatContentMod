using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class RadiantRose : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack.Add(Type);
            DTUtils.IncompatibleWith(Type, ItemID.BandofRegeneration);
            DTUtils.IncompatibleWith(Type, ItemID.CharmofMyths);

            if (DTCrossMod.FargosSoulsIsLoaded)
            {
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("ConcentratedRainbowMatter", out ModItem CRM))
                {
                    DTUtils.IncompatibleWith(Type, CRM.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("BionomicCluster", out ModItem BC))
                {
                    DTUtils.IncompatibleWith(Type, BC.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("MasochistSoul", out ModItem SM))
                {
                    DTUtils.IncompatibleWith(Type, SM.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("EternitySoul", out ModItem SE))
                {
                    DTUtils.IncompatibleWith(Type, SE.Type);
                }
            }

        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 64;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<PotionFlowerPlayer>(out PotionFlowerPlayer flower))
            {
                flower.RadiantRose = true;
            }

        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.JungleRose, 1)
                .AddIngredient(ItemID.HealingPotion, 10)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddIngredient(ItemID.FallenStar, 6)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.JungleRose, 1)
                .AddIngredient(ItemID.HealingPotion, 10)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddIngredient(ItemID.FallenStar, 6)
                .Register();
        }
	}
}