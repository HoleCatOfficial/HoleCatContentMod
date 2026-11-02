using System.Collections.Generic;
using System.Linq;
using DestroyerTest.Content.Entities;
using SoulsBossRushMod.Content.Event;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    [JITWhenModsEnabled("SoulsBossRushMod")]
    [ExtendsFromMod("SoulsBossRushMod")]
    public class FargoRushCompatibilitySystem : ModSystem
    {
        [JITWhenModsEnabled("SoulsBossRushMod")]
        public bool active => BossRushSystem.BossRushIsActive;

        public static bool FargoRushActive => ModLoader.HasMod("SoulsBossRushMod") ? ModContent.GetInstance<FargoRushCompatibilitySystem>().active : false;

        public override void PostSetupContent()
        {
            if (ModLoader.HasMod("SoulsBossRushMod"))
            {
                BossRushSystem.Fight NightmareRoseFight = new(ModContent.NPCType<NightmareRoseBoss>(), npcs: [ModContent.NPCType<CursedFlameNode>()]);
                BossRushSystem.Fight WyvernCorpseFight = new(ModContent.NPCType<WyvernCorpseHead>(), npcs: [ModContent.NPCType<IchorNode>(), ModContent.NPCType<SoulOrb>(), ModContent.NPCType<WyvernCorpseLegs>(), ModContent.NPCType<WyvernCorpseTail>(), ModContent.NPCType<WyvernCorpseBody1>(), ModContent.NPCType<WyvernCorpseBody2>(), ModContent.NPCType<WyvernCorpseBody3>()]);

                List<BossRushSystem.Fight> bf = BossRushSystem.BossFights;

                bf.Insert(bf.IndexOf(bf.First(n => n.BossID == NPCID.MoonLordCore)) + 1, NightmareRoseFight);
                bf.Insert(bf.IndexOf(NightmareRoseFight) + 1, WyvernCorpseFight);
            }
        }

    }
}
