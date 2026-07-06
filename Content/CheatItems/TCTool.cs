using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;


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