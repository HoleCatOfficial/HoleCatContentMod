using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class LilliesOfImmortality : ModItem
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
            if(player.TryGetModPlayer<DjedPillarCharmPlayer>(out DjedPillarCharmPlayer modPlayer))
            {
                modPlayer.Active = true;
            }
            if(player.TryGetModPlayer<SpiritFlameDash>(out SpiritFlameDash Dash))
            {
                Dash.Active = true;
            }
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EphemeralSolvent>(1)
                .AddIngredient<DjedPillarCharm>(1)
                .AddIngredient(ItemID.LunarOre, 8)
                .AddIngredient<Tenebris>(6)
                .Register();
        }
	}
}