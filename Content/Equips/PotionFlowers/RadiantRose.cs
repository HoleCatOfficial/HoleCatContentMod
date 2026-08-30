using System.Collections.Generic;
using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class RadiantRose : ModItem
    {
        List<int> blocked;
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;

            blocked = [ItemID.BandofRegeneration, ItemID.CharmofMyths];


            if (DTCrossMod.FargosSoulsIsLoaded)
            {
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("ConcentratedRainbowMatter", out ModItem CRM))
                {
                    blocked.Add(CRM.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("BionomicCluster", out ModItem BC))
                {
                    blocked.Add(BC.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("MasochistSoul", out ModItem SM))
                {
                    blocked.Add(SM.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("EternitySoul", out ModItem SE))
                {
                    blocked.Add(SE.Type);
                }
            }

            DTUtils.IncompatibleWith(Type, blocked.ToArray());
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