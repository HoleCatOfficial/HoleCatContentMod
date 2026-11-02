
using DestroyerTest.Common;
using DestroyerTest.Content.Remnants;
using Terraria;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class GalantineScroll : PreHardmodeScroll
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
            {
                Scptr.GalantineScroll = true;
            }
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.RemnantsIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<ConstitutionArtifact>(3)
                    .Register();
            }
        }
    }
}