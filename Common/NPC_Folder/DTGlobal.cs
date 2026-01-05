using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Entities;
using Terraria.Audio;
using DestroyerTest.Content.BossBars;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Ammunitions;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Tools;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Common.NPC_Folder
{
	internal class DTGlobal : GlobalNPC
	{
		public override bool InstancePerEntity => true;
        private bool KilledSpaz = false;
        private bool KilledRet = false;
        public override void OnKill(NPC npc)
        {
            if (npc.type == NPCID.KingSlime && !DownedBossSystem.downedKingSlimeBoss)
            {
                DTUtils.NPCDownTally[NPCID.KingSlime]++;
                DownedBossSystem.downedKingSlimeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.EyeofCthulhu && !DownedBossSystem.downedEoCBoss)
            {
                DTUtils.NPCDownTally[NPCID.EyeofCthulhu]++;
                DownedBossSystem.downedEoCBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.EaterofWorldsHead  && !DownedBossSystem.downedEoWBoss)
            {
                DTUtils.NPCDownTally[NPCID.EaterofWorldsHead]++;
                DownedBossSystem.downedEoWBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.BrainofCthulhu && !DownedBossSystem.downedBoCBoss)
            {
                DTUtils.NPCDownTally[NPCID.BrainofCthulhu]++;
                DownedBossSystem.downedBoCBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.QueenBee  && !DownedBossSystem.downedQueenBeeBoss)
            {
                DTUtils.NPCDownTally[NPCID.QueenBee]++;
                DownedBossSystem.downedQueenBeeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Deerclops && !DownedBossSystem.downedDeerclopsMiniBoss)
            {
                DTUtils.NPCDownTally[NPCID.Deerclops]++;
                DownedBossSystem.downedDeerclopsMiniBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.SkeletronHead && !DownedBossSystem.downedSkeletronBoss)
            {
                DTUtils.NPCDownTally[NPCID.SkeletronHead]++;
                DownedBossSystem.downedSkeletronBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<ConstitutionBoss>() && !DownedBossSystem.downedConstitutionBoss)
            {
                DTUtils.NPCDownTally[ModContent.NPCType<ConstitutionBoss>()]++;
                DownedBossSystem.downedConstitutionBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.WallofFlesh && !DownedBossSystem.downedWallBoss)
            {
                DTUtils.NPCDownTally[NPCID.WallofFlesh]++;
                DownedBossSystem.downedWallBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.TheDestroyer && !DownedBossSystem.downedDestroyerBoss)
            {
                DTUtils.NPCDownTally[NPCID.TheDestroyer]++;
                DownedBossSystem.downedDestroyerBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Spazmatism && !DownedBossSystem.downedTwinsBoss)
            {
                DTUtils.NPCDownTally[NPCID.Spazmatism]++;
                KilledSpaz = true;
                if (KilledRet == true)
                {
                    DownedBossSystem.downedTwinsBoss = true;
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Retinazer && !DownedBossSystem.downedTwinsBoss)
            {
                DTUtils.NPCDownTally[NPCID.Retinazer]++;
                KilledRet = true;
                if (KilledSpaz == true)
                {
                    DownedBossSystem.downedTwinsBoss = true;
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.BloodNautilus && !DownedBossSystem.downedNautilusMiniBoss)
            {
                DTUtils.NPCDownTally[NPCID.BloodNautilus]++;
                DownedBossSystem.downedNautilusMiniBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.QueenSlimeBoss && !DownedBossSystem.downedQueenSlimeBoss)
            {
                DTUtils.NPCDownTally[NPCID.QueenSlimeBoss]++;
                DownedBossSystem.downedQueenSlimeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Plantera && !DownedBossSystem.downedPlanteraBoss)
            {
                DTUtils.NPCDownTally[NPCID.Plantera]++;
                DownedBossSystem.downedPlanteraBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Golem && !DownedBossSystem.downedGolemBoss)
            {
                DTUtils.NPCDownTally[NPCID.Golem]++;
                DownedBossSystem.downedGolemBoss = true;
                ModContent.GetInstance<HeliciteSystem>().BlessWorldWithHelicite();
                //Main.NewText("Fragments of the ancient sun embed themselves in the rock deep down...", ColorLib.TenebrisMagenta);
                
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.DukeFishron && !DownedBossSystem.downedFishronBoss)
            {
                DTUtils.NPCDownTally[NPCID.DukeFishron]++;
                DownedBossSystem.downedFishronBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.HallowBoss && !DownedBossSystem.downedEmpressBoss)
            {
                DTUtils.NPCDownTally[NPCID.HallowBoss]++;
                DownedBossSystem.downedEmpressBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.CultistBoss && !DownedBossSystem.downedCultistBoss)
            {
                DTUtils.NPCDownTally[NPCID.CultistBoss]++;
                if (!DownedBossSystem.downedCultistBoss)
                {
                    SoundStyle TenebrisSpawn = new SoundStyle("DestroyerTest/Assets/Audio/TenebrisSpawn");
                    if (Main.dedServ == false)
                    {
                        SoundEngine.PlaySound(TenebrisSpawn);
                        Main.NewText("Your world and the Shade World have connected!", ColorLib.TenebrisMagenta);
                    }
                }
                DownedBossSystem.downedCultistBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.MoonLordCore && !DownedBossSystem.downedLunarBoss)
            {
                DTUtils.NPCDownTally[NPCID.MoonLordCore]++;
                DownedBossSystem.downedLunarBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<WyvernCorpseHead>() && !DownedBossSystem.downedWyvernCorpseBoss)
            {
                DTUtils.NPCDownTally[ModContent.NPCType<WyvernCorpseHead>()]++;
                DownedBossSystem.downedWyvernCorpseBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<NightmareRoseBoss>() && !DownedBossSystem.downedNightmareRoseBoss)
            {
                DTUtils.NPCDownTally[ModContent.NPCType<NightmareRoseBoss>()]++;
                DownedBossSystem.downedNightmareRoseBoss = true;
                if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }
            }
        }


        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.SantaClaus)
            {
                shop.Add<WinterWonderland>(Condition.Hardmode);
            }

            if (shop.NpcType == NPCID.Merchant)
            {
                shop.Add<OilFlask>();
                shop.Add<BrineFlask>();
                shop.Add<MineralOil>();
                shop.Add<CursedStar>(Condition.DownedSkeletron);
            }

            if (shop.NpcType == NPCID.TravellingMerchant)
            {
                shop.Add<FoxScepter>(Condition.DownedKingSlime);
            }

            if (shop.NpcType == ModContent.NPCType<Scholar>())
            {
                shop.Add<ShiningObelisk>(Condition.DownedCultist);
            }

            if (shop.NpcType == NPCID.ArmsDealer && (DownedBossSystem.downedNightmareRoseBoss || DownedBossSystem.downedWyvernCorpseBoss))
            {
                shop.Add<EndlessTenebrisBullets>(Condition.DownedCultist);
                shop.Add<EndlessHeliciteRounds>(Condition.DownedGolem);
                shop.Add<EndlessRiftRounds>(Condition.DownedMechBossAll);
            }
        }
	}
}