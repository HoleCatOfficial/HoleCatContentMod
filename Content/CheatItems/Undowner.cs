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
    public class Undowner : ModItem
    {
        public override void SetDefaults()
        {
            Item.UseSound = DTAssetLib.Impacts.EnergyBounce;
            Item.width = 40; // The item texture's width.
            Item.height = 40; // The item texture's height.

            Item.useStyle = ItemUseStyleID.HoldUp; // The useStyle of the Item.
            Item.useTime = 120; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
            Item.useAnimation = 120; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
            Item.autoReuse = false; // Whether the weapon can be used more than once automatically by holding the use button.
            Item.rare = ItemRarityID.Expert; // Give this item our custom rarity.
        }

        public override bool? UseItem(Player player)
        {
            DownedBossSystem.downedEoCBoss = false;
            DownedBossSystem.downedKingSlimeBoss = false;
            DownedBossSystem.downedEoWBoss = false;
            DownedBossSystem.downedBoCBoss = false;
            DownedBossSystem.downedEoWBoss = false;
            DownedBossSystem.downedQueenBeeBoss = false;
            DownedBossSystem.downedDeerclopsMiniBoss = false;
            DownedBossSystem.downedSkeletronBoss = false;
            DownedBossSystem.downedConstitutionBoss = false;
            DownedBossSystem.downedWallBoss = false;
            DownedBossSystem.downedQueenSlimeBoss = false;
            DownedBossSystem.downedDestroyerBoss = false;
            DownedBossSystem.downedTwinsBoss = false;
            DownedBossSystem.downedSkeletronPrimeBoss = false;
            DownedBossSystem.downedNautilusMiniBoss = false;
            DownedBossSystem.downedPlanteraBoss = false;
            DownedBossSystem.downedNodeMiniBoss = false;
            DownedBossSystem.downedGolemBoss = false;
            DownedBossSystem.downedFishronBoss = false;
            DownedBossSystem.downedEmpressBoss = false;
            DownedBossSystem.downedCultistBoss = false;
            DownedBossSystem.downedLunarBoss = false;
            DownedBossSystem.downedWyvernCorpseBoss = false;
            DownedBossSystem.downedNightmareRoseBoss = false;
            return true;
        }
    }
}