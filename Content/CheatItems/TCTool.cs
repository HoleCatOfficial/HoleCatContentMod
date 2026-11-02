using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace DestroyerTest.Content.CheatItems
{
    public class TCTool : ModItem
    {
        public override void SetDefaults()
        {
            Item.UseSound = DTAssetLib.Impacts.Void;
            Item.width = 40;
            Item.height = 40;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.autoReuse = false;
            Item.rare = ItemRarityID.Expert;
        }

        public override bool? UseItem(Player player)
        {
            DTFlags.TenebrisCanSpawnInWorldEvilBiome = true;
            CorruptionModificationSystem.GenTenebris = false;
            return true;
        }
    }
}