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
using DestroyerTest.Content.Magic.ScepterSubclass;
using DestroyerTest.Content.Ammunitions;

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
                DownedBossSystem.downedKingSlimeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.EyeofCthulhu && !DownedBossSystem.downedEoCBoss)
            {
                DownedBossSystem.downedEoCBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.EaterofWorldsHead  && !DownedBossSystem.downedEoWBoss)
            {
                DownedBossSystem.downedEoWBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.BrainofCthulhu && !DownedBossSystem.downedBoCBoss)
            {
                DownedBossSystem.downedBoCBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.QueenBee  && !DownedBossSystem.downedQueenBeeBoss)
            {
                DownedBossSystem.downedQueenBeeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Deerclops && !DownedBossSystem.downedDeerclopsMiniBoss)
            {
                DownedBossSystem.downedDeerclopsMiniBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.SkeletronHead && !DownedBossSystem.downedSkeletronBoss)
            {
                DownedBossSystem.downedSkeletronBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<ConstitutionBoss>() && !DownedBossSystem.downedConstitutionBoss)
            {
                DownedBossSystem.downedConstitutionBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.WallofFlesh && !DownedBossSystem.downedWallBoss)
            {
                DownedBossSystem.downedWallBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.TheDestroyer && !DownedBossSystem.downedDestroyerBoss)
            {
                DownedBossSystem.downedDestroyerBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Spazmatism && !DownedBossSystem.downedTwinsBoss)
            {
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
                DownedBossSystem.downedNautilusMiniBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.QueenSlimeBoss && !DownedBossSystem.downedQueenSlimeBoss)
            {
                DownedBossSystem.downedQueenSlimeBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Plantera && !DownedBossSystem.downedPlanteraBoss)
            {
                DownedBossSystem.downedPlanteraBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.Golem && !DownedBossSystem.downedGolemBoss)
            {
                DownedBossSystem.downedGolemBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.DukeFishron && !DownedBossSystem.downedFishronBoss)
            {
                DownedBossSystem.downedFishronBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.HallowBoss && !DownedBossSystem.downedEmpressBoss)
            {
                DownedBossSystem.downedEmpressBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == NPCID.CultistBoss && !DownedBossSystem.downedCultistBoss)
            {
                if (!DownedBossSystem.downedCultistBoss)
                {
                    SoundStyle TenebrisSpawn = new SoundStyle("DestroyerTest/Assets/Audio/TenebrisSpawn");
                    if (Main.dedServ == false)
                    {
                        SoundEngine.PlaySound(TenebrisSpawn);
                        Main.NewText("Strange Energies have been released into your world...", ColorLib.TenebrisMagenta);
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
                DownedBossSystem.downedLunarBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<WyvernCorpseHead>() && !DownedBossSystem.downedWyvernCorpseBoss)
            {
                DownedBossSystem.downedWyvernCorpseBoss = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (npc.type == ModContent.NPCType<NightmareRoseBoss>() && !DownedBossSystem.downedNightmareRoseBoss)
            {
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
                shop.Add<CursedStar>(Condition.DownedSkeletron); // Or wherever your item is
            }

            if (shop.NpcType == NPCID.TravellingMerchant)
            {
                shop.Add<FoxScepter>(Condition.DownedKingSlime); // Or wherever your item is
            }
            
            if (shop.NpcType == NPCID.ArmsDealer && (DownedBossSystem.downedNightmareRoseBoss || DownedBossSystem.downedWyvernCorpseBoss))
            {
                shop.Add<EndlessTenebrisBullets>(Condition.DownedCultist); // Or wherever your item is
            }
        }
	}
}