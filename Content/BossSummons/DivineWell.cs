
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Altar;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace DestroyerTest.Content.BossSummons
{
    public class DivineWell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 84;
            Item.maxStack = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Expert;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.maxStack = 1;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<WyvernCorpseHead>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundStyle Summon = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/CorpseRoar1");
                SoundEngine.PlaySound(Summon, player.position);

                int type = ModContent.NPCType<WyvernCorpseHead>();

                if (Main.netMode != NetmodeID.MultiplayerClient && player.ZoneCrimson)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShimmerBlock, 60)
                .AddIngredient<Item_BlessingAltar>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}