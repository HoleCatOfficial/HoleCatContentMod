
using System;
using System.Collections.Generic;
using DestroyerTest.Content.Entities;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace DestroyerTest.Content.Consumables
{
    // This is the item used to summon a boss, in this case the modded Minion Boss from Example Mod. For vanilla boss summons, see comments in SetStaticDefaults
    public class EuthanizedViciousBunny : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

            // If this would be for a vanilla boss that has no summon item, you would have to include this line here:
            // NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;

            // Otherwise the UseItem code to spawn it will not work in multiplayer
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.maxStack = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }

        public override bool CanUseItem(Player player)
        {
            // If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss.
            // If you want more constraints for the summon item, combine them as boolean expressions:
            //    return !Main.IsItDay() && !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>()); would mean "not daytime and no MinionBossBody currently alive"
            return !NPC.AnyNPCs(ModContent.NPCType<WyvernCorpseHead>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // If the player using the item is the client
                // (explicitly excluded serverside here)
                SoundStyle Summon = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/CorpseRoar1");
                SoundEngine.PlaySound(Summon, player.position);

                int type = ModContent.NPCType<WyvernCorpseHead>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // If the player is not in multiplayer, spawn directly
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    // If the player is in multiplayer, request a spawn
                    // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string customText = "WARNING: THIS FIGHT CAN CAUSE MAJOR LAG SPIKES EVEN ON THE LOWEST SETTINGS. I HAVE DONE ALL THAT I CAN.";

            TooltipLine line = new TooltipLine(Mod, "CustomTooltip", customText)
            {
                OverrideColor = Color.DarkSlateGray
            };
            tooltips.Add(line);
        }


        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "CustomTooltip" && line.Mod == Mod.Name)
            {
                // Smoothly interpolate between stroke and text colors using sine wave
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color strokeColor = Color.Lerp(Color.Red, Color.OrangeRed, lerpAmount);
                Color textColor = Color.Lerp(Color.Black, Color.DarkSlateGray, lerpAmount);

                // Define the font and position
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                Vector2 position = new Vector2(line.X, line.Y);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        Vector2 offsetPosition = position + new Vector2(i, j);
                        ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, offsetPosition, strokeColor, 0f, Vector2.Zero, Vector2.One);
                    }
                }
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);
                return false;
            }
            return true;
        }


    }
    
    public class WyvernSummon_DROP_NPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {

			if (npc.type == NPCID.CrimsonBunny) {
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EuthanizedViciousBunny>(), 4, 1, 1));
			}
		}
	}
}