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
using DestroyerTest.Assets.Menu.V5;

namespace DestroyerTest.Common.NPC_Folder
{
	public class DTGlobal : GlobalNPC
	{
		public override bool InstancePerEntity => true;
        private bool KilledSpaz = false;
        private bool KilledRet = false;
        public static bool HasKilledAMechBoss = false;

        public override void PostAI(NPC npc)
        {
            HasKilledAMechBoss = (DownedBossSystem.downedDestroyerBoss || DownedBossSystem.downedSkeletronPrimeBoss || DownedBossSystem.downedTwinsBoss);
        }

        public void UpdateDivinePlayers()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (DivineServantSystem.IsServant[i])
                {
                    DivineServantSystem.Level[i]++;
                }
            }
        }
        public override void OnKill(NPC npc)
        {
            if (npc.type == NPCID.KingSlime && !DownedBossSystem.downedKingSlimeBoss)
            {
                DTUtils.NPCDownTally[NPCID.KingSlime]++;

                UpdateDivinePlayers();
                DownedBossSystem.downedKingSlimeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.EyeofCthulhu && !DownedBossSystem.downedEoCBoss)
            {
                DTUtils.NPCDownTally[NPCID.EyeofCthulhu]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedEoCBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.EaterofWorldsHead  && !DownedBossSystem.downedEoWBoss)
            {
                DTUtils.NPCDownTally[NPCID.EaterofWorldsHead]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedEoWBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.BrainofCthulhu && !DownedBossSystem.downedBoCBoss)
            {
                DTUtils.NPCDownTally[NPCID.BrainofCthulhu]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedBoCBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.QueenBee  && !DownedBossSystem.downedQueenBeeBoss)
            {
                DTUtils.NPCDownTally[NPCID.QueenBee]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedQueenBeeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Deerclops && !DownedBossSystem.downedDeerclopsMiniBoss)
            {
                DTUtils.NPCDownTally[NPCID.Deerclops]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedDeerclopsMiniBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.SkeletronHead && !DownedBossSystem.downedSkeletronBoss)
            {
                DTUtils.NPCDownTally[NPCID.SkeletronHead]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedSkeletronBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<ConstitutionBoss>() && !DownedBossSystem.downedConstitutionBoss)
            {
                DTUtils.NPCDownTally[ModContent.NPCType<ConstitutionBoss>()]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedConstitutionBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.WallofFlesh && !DownedBossSystem.downedWallBoss)
            {
                DTUtils.NPCDownTally[NPCID.WallofFlesh]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedWallBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.TheDestroyer && !DownedBossSystem.downedDestroyerBoss)
            {
                DTUtils.NPCDownTally[NPCID.TheDestroyer]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedDestroyerBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Spazmatism && !DownedBossSystem.downedTwinsBoss)
            {
                DTUtils.NPCDownTally[NPCID.Spazmatism]++;
                UpdateDivinePlayers();
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
                UpdateDivinePlayers();
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
                UpdateDivinePlayers();
                DownedBossSystem.downedNautilusMiniBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.QueenSlimeBoss && !DownedBossSystem.downedQueenSlimeBoss)
            {
                DTUtils.NPCDownTally[NPCID.QueenSlimeBoss]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedQueenSlimeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Plantera && !DownedBossSystem.downedPlanteraBoss)
            {
                DTUtils.NPCDownTally[NPCID.Plantera]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedPlanteraBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<IchorNodeMB>() && !DownedBossSystem.downedNodeMiniBoss)
            {
                DTUtils.NPCDownTally[ModContent.NPCType<IchorNodeMB>()]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedNodeMiniBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<CursedFlameNodeMB>() && !DownedBossSystem.downedNodeMiniBoss)
            {
                DTUtils.NPCDownTally[ModContent.NPCType<CursedFlameNodeMB>()]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedNodeMiniBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Golem && !DownedBossSystem.downedGolemBoss)
            {
                DTUtils.NPCDownTally[NPCID.Golem]++;
                UpdateDivinePlayers();
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
                UpdateDivinePlayers();
                DownedBossSystem.downedFishronBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.HallowBoss && !DownedBossSystem.downedEmpressBoss)
            {
                DTUtils.NPCDownTally[NPCID.HallowBoss]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedEmpressBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.CultistBoss && !DownedBossSystem.downedCultistBoss)
            {
                DTUtils.NPCDownTally[NPCID.CultistBoss]++;
                UpdateDivinePlayers();
                if (!DownedBossSystem.downedCultistBoss && !WorldGen.crimson)
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
                UpdateDivinePlayers();
                DownedBossSystem.downedLunarBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<WyvernCorpseHead>() && !DownedBossSystem.downedWyvernCorpseBoss)
            {
                DTUtils.NPCDownTally[ModContent.NPCType<WyvernCorpseHead>()]++;
                UpdateDivinePlayers();
                DownedBossSystem.downedWyvernCorpseBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<NightmareRoseBoss>() && !DownedBossSystem.downedNightmareRoseBoss)
            {
                DTUtils.NPCDownTally[ModContent.NPCType<NightmareRoseBoss>()]++;
                UpdateDivinePlayers();
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

            if (shop.NpcType == NPCID.Mechanic)
            {
                shop.Add<MechanicalEnhancements>();
            }
        }
	}
}