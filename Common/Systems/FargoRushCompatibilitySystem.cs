using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.UI;
using Microsoft.Xna.Framework;
using SoulsBossRushMod.Content.Event;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    [JITWhenModsEnabled("SoulsBossRushMod")]
    [ExtendsFromMod("SoulsBossRushMod")]
    public class FargoRushCompatibilitySystem : ModSystem
    {
        [JITWhenModsEnabled("SoulsBossRushMod")]
        public static bool FargoRushActive => ModLoader.HasMod("SoulsBossRushMod") ? BossRushSystem.BossRushIsActive : false;

        public override void PostSetupContent()
        {
            if (ModLoader.HasMod("SoulsBossRushMod"))
            {
                BossRushSystem.Fight NightmareRoseFight = new(ModContent.NPCType<NightmareRoseBoss>(), npcs: [ModContent.NPCType<CursedFlameNode>()] );
                BossRushSystem.Fight WyvernCorpseFight = new(ModContent.NPCType<WyvernCorpseHead>(), npcs: [ModContent.NPCType<IchorNode>(), ModContent.NPCType<SoulOrb>(), ModContent.NPCType<WyvernCorpseLegs>(), ModContent.NPCType<WyvernCorpseTail>(), ModContent.NPCType<WyvernCorpseBody1>(), ModContent.NPCType<WyvernCorpseBody2>(), ModContent.NPCType<WyvernCorpseBody3>()]);

                List<BossRushSystem.Fight> bf = BossRushSystem.BossFights;

                bf.Insert(bf.IndexOf(bf.First(n => n.BossID == NPCID.MoonLordCore)) + 1, NightmareRoseFight);
                bf.Insert(bf.IndexOf(NightmareRoseFight) + 1, WyvernCorpseFight);
            }
        }

    }
}
