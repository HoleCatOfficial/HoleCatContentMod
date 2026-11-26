using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Entities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common.BloodMoon
{
    public class BloodMoonUpgrade : ModSystem
    {
        public bool Golem = false;
        public bool Moonlord = false;

        public override void PostUpdateWorld()
        {
            Golem = DownedBossSystem.downedGolemBoss;
            Moonlord = DownedBossSystem.downedLunarBoss;
        }
    }

    public class BloodMoonUpgradeGlobal : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public static void Register(NPC toRegister)
        {
            BloodMoonNPCs.Add(toRegister.type);
        }

        public static void Register(int typeToRegister)
        {
            BloodMoonNPCs.Add(typeToRegister);
        }

        public static List<int> BloodMoonNPCs = new()
        {
            NPCID.TheGroom,
            NPCID.TheBride,
            NPCID.BloodZombie,
            NPCID.Drippler,
            NPCID.CorruptBunny,
            NPCID.CorruptGoldfish,
            NPCID.CorruptPenguin,
            NPCID.CrimsonBunny,
            NPCID.CrimsonGoldfish,
            NPCID.CrimsonPenguin,
            NPCID.EyeballFlyingFish,
            NPCID.ZombieMerman,
            NPCID.Clown,
            NPCID.ChatteringTeethBomb,
            NPCID.BloodNautilus,
            NPCID.BloodSquid,
            NPCID.BloodEelHead,
            NPCID.BloodEelBody,
            NPCID.BloodEelTail,
            NPCID.GoblinShark,
            ModContent.NPCType<BloodGazer>()
        };
        public override void SetDefaults(NPC entity)
        {
            var system = ModContent.GetInstance<BloodMoonUpgrade>();

            if (system.Golem)
            {
                if (BloodMoonNPCs.Contains(entity.type))
                {
                    entity.life = entity.lifeMax = (int)(entity.lifeMax * 8f);
                    entity.damage = (int)(entity.damage * 3f);
                }
            }
            if (system.Moonlord)
            {
                if (BloodMoonNPCs.Contains(entity.type))
                {
                    entity.life = entity.lifeMax = (int)(entity.lifeMax * 15f);
                    entity.damage = (int)(entity.damage * 5f);
                }
            }
        }
    }
}
