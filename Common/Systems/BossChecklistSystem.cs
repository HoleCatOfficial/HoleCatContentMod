using DestroyerTest.Content.BossSummons;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class BossChecklistEntry
    {
        public float Weight { get; set; }
        public Action<SpriteBatch, Rectangle, Color> Portrait { get; set; }
        public LocalizedText BossName { get; set; }
        public Condition DownCondition { get; set; }
        public LocalizedText Hint { get; set; }
        public int InternalID { get; set; }
        public int SpawnItem { get; set; } = -1;
        public List<int> LootTable { get; set; } = new List<int>();

        public BossChecklistEntry(LocalizedText bossName, LocalizedText hint, int internalID, float weight, Condition downCondition, Action<SpriteBatch, Rectangle, Color> portrait)
        {
            BossName = bossName;
            Hint = hint;
            InternalID = internalID;
            Weight = weight;
            DownCondition = downCondition;
            Portrait = portrait;
        }

        public BossChecklistEntry(LocalizedText bossName, LocalizedText hint, int internalID, float weight, Condition downCondition, Action<SpriteBatch, Rectangle, Color> portrait, int spawnItem)
        {
            BossName = bossName;
            Hint = hint;
            InternalID = internalID;
            Weight = weight;
            DownCondition = downCondition;
            Portrait = portrait;
            SpawnItem = spawnItem;
        }

        public BossChecklistEntry(LocalizedText bossName, LocalizedText hint, int internalID, float weight, Condition downCondition, Action<SpriteBatch, Rectangle, Color> portrait, int spawnItem, List<int> lootTable)
        {
            BossName = bossName;
            Hint = hint;
            InternalID = internalID;
            Weight = weight;
            DownCondition = downCondition;
            Portrait = portrait;
            SpawnItem = spawnItem;
            LootTable = lootTable;
        }
    }

    public class BossChecklistSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // https://forums.terraria.org/index.php?threads/.50668/
            DoBossChecklistIntegration(DTBossEntries.Constitution);
            DoBossChecklistIntegration_NoSpawnItem(DTBossEntries.IchorNode);
            DoBossChecklistIntegration_NoSpawnItem(DTBossEntries.CursedFlameNode);
            DoBossChecklistIntegration_NoSpawnItem(DTBossEntries.BlessedNode);
            DoBossChecklistIntegration(DTBossEntries.WyvernCorpse);
            DoBossChecklistIntegration(DTBossEntries.NightmareRose);
        }

        public struct DTLootTables
        {
            public static List<int> ConstitutionLootTable = new List<int>
            {
                ModContent.ItemType<Constitution>(),
                ModContent.ItemType<GalantineKnife>(),
                ModContent.ItemType<GalantineLance>(),
                ModContent.ItemType<StellarBow>(),
                ModContent.ItemType<StellarFoxScepter>(),
                ModContent.ItemType<StellarFlames>(),

                ModContent.ItemType<GalantineIncense>(),
                ModContent.ItemType<StellarTintedGoggles>(),
                ModContent.ItemType<StarBadge>(),

                ModContent.ItemType<StellarMatter>(),
                ModContent.ItemType<StellarFlamesFlask>(),

                ModContent.ItemType<Item_ConstitutionTrophy>(),
                ModContent.ItemType<Item_ConstitutionRelic>(),
            };

            public static List<int> IchorNodeLootTable = new List<int>
            {
                ModContent.ItemType<PrimalShards>(),
                ModContent.ItemType<DistendedPike>(),
                ModContent.ItemType<Scorn>(),
                ModContent.ItemType<IchorScroll>(),
                ModContent.ItemType<HaepienNodeCharm>(),
                ModContent.ItemType<Item_IchorNodeTrophy>(),
                ModContent.ItemType<Item_IchorNodeRelic>(),
            };

            public static List<int> CursedFlameNodeLootTable = new List<int>
            {
                ModContent.ItemType<WretchedShards>(),
                ModContent.ItemType<Malevolence>(),
                ModContent.ItemType<CursedFlameScroll>(),
                ModContent.ItemType<Item_CursedFlameNodeTrophy>(),
                ModContent.ItemType<Item_CursedFlameNodeRelic>(),
            };

            public static List<int> BlessedNodeLootTable = new List<int>
            {
                ModContent.ItemType<Purity>(),
                ModContent.ItemType<GloryOrb>(),
                ModContent.ItemType<Item_BlessedNodeTrophy>(),
                ModContent.ItemType<Item_BlessedNodeRelic>(),
            };

            public static List<int> WyvernCorpseLootTable = new List<int>
            {
                ModContent.ItemType<WyvernSoul>(),

                ModContent.ItemType<GreatFlayer>(),
                ModContent.ItemType<WyvernTail>(),
                ModContent.ItemType<RibChainsaw>(),
                ModContent.ItemType<KeeperStaff>(),
                ModContent.ItemType<SoulBoundWhip>(),

                ModContent.ItemType<WyvernSkull>(),

                ModContent.ItemType<DivineVessel>(),
                ModContent.ItemType<Item_WyvernCorpseTrophy>(),
                ModContent.ItemType<Item_WyvernCorpseRelic>(),
            };

            public static List<int> NightmareRoseLootTable = new List<int>
            {
                ModContent.ItemType<RoseSoul>(),

                ModContent.ItemType<Contempt>(),
                ModContent.ItemType<BlossomBeater>(),
                ModContent.ItemType<CursedHammer>(),

                ModContent.ItemType<DeadlyBlossom>(),
                ModContent.ItemType<HaepienNodeCharm>(),

                ModContent.ItemType<NightmarePowder>(),
                ModContent.ItemType<Item_NightmareRoseTrophy>(),
                ModContent.ItemType<Item_NightmareRoseRelic>(),
            };
        }
        public struct DTBossEntries
        {
            public static Action<SpriteBatch, Rectangle, Color> ConstitutionPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/ConstitutionBossChecklist").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };
            public static BossChecklistEntry Constitution = new BossChecklistEntry(
                Language.GetText("Mods.DestroyerTest.NPCs.ConstitutionBoss.InternalName"), 
                Language.GetText("Mods.DestroyerTest.BossChecklist.ConstitutionBoss.Hint"), 
                ModContent.NPCType<ConstitutionBoss>(), 
                6.9999999f, 
                DownedBossSystem.downedConstitutionCondition,
                ConstitutionPortrait, 
                ModContent.ItemType<CursedStar>(), 
                DTLootTables.ConstitutionLootTable);


            public static Action<SpriteBatch, Rectangle, Color> BlessedNodePortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/BlessedNodeBCL").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };
            public static BossChecklistEntry BlessedNode = new BossChecklistEntry(
                Language.GetText("Mods.DestroyerTest.NPCs.BlessedNodeMB.InternalName"),
                Language.GetText("Mods.DestroyerTest.BossChecklist.BlessedNodeMB.Hint"),
                ModContent.NPCType<BlessedNodeMB>(), 
                12.8f,
                DownedBossSystem.downedNodeCondition,
                BlessedNodePortrait,
                -1,
                DTLootTables.BlessedNodeLootTable);


            public static Action<SpriteBatch, Rectangle, Color> IchorNodePortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/IchorNodeBCL").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };
            public static BossChecklistEntry IchorNode = new BossChecklistEntry(
                Language.GetText("Mods.DestroyerTest.NPCs.IchorNodeMB.InternalName"), 
                Language.GetText("Mods.DestroyerTest.BossChecklist.IchorNodeMB.Hint"), 
                ModContent.NPCType<IchorNodeMB>(), 
                12.7f, 
                DownedBossSystem.downedNodeCondition, 
                IchorNodePortrait, 
                -1, 
                DTLootTables.IchorNodeLootTable);

            public static Action<SpriteBatch, Rectangle, Color> CursedFlameNodePortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CursedFlameNodeBCL").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };
            public static BossChecklistEntry CursedFlameNode = new BossChecklistEntry(
                Language.GetText("Mods.DestroyerTest.NPCs.CursedFlameNodeMB.InternalName"), 
                Language.GetText("Mods.DestroyerTest.BossChecklist.CursedFlameNodeMB.Hint"), 
                ModContent.NPCType<CursedFlameNodeMB>(), 
                12.7f, 
                DownedBossSystem.downedNodeCondition, 
                CursedFlameNodePortrait, 
                -1, 
                DTLootTables.CursedFlameNodeLootTable);


            public static Action<SpriteBatch, Rectangle, Color> WyvernCorpsePortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/WyvernCorpseBossChecklist").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };
            public static BossChecklistEntry WyvernCorpse = new BossChecklistEntry(
                Language.GetText("Mods.DestroyerTest.NPCs.WyvernCorpseHead.InternalName"),
                Language.GetText("Mods.DestroyerTest.BossChecklist.WyvernCorpseHead.Hint"),
                ModContent.NPCType<WyvernCorpseHead>(),
                18.0001f,
                DownedBossSystem.downedWyvernCorpseBossCondition,
                WyvernCorpsePortrait,
                ModContent.ItemType<DivineWell>(),
                DTLootTables.WyvernCorpseLootTable);

            public static Action<SpriteBatch, Rectangle, Color> NightmareRosePortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/NightmareRoseBossBossChecklist").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };
            public static BossChecklistEntry NightmareRose = new BossChecklistEntry(
                Language.GetText("Mods.DestroyerTest.NPCs.NightmareRoseBoss.InternalName"),
                Language.GetText("Mods.DestroyerTest.BossChecklist.NightmareRoseBoss.Hint"),
                ModContent.NPCType<NightmareRoseBoss>(),
                18.0001f,
                DownedBossSystem.downedNightmareRoseBossCondition,
                NightmareRosePortrait,
                ModContent.ItemType<TheBotanistsCurse>(),
                DTLootTables.NightmareRoseLootTable);

        }


        private void DoBossChecklistIntegration(BossChecklistEntry Entry)
        {

            // The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call
            // If we navigate the wiki, we can find the "LogBoss" method, which we want in this case
            // A feature of the call is that it will create an entry in the localization file of the specified NPC type for its spawn info, so make sure to visit the localization file after your mod runs once to edit it

            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.6
            // Usually mods either provide that information themselves in some way, or it's found on the GitHub through commit history/blame
            if (bossChecklistMod.Version < new Version(1, 6))
            {
                return;
            }


            Entry.DownCondition.Deconstruct(out LocalizedText description, out Func<bool> predicate);

            if (Entry.SpawnItem == -1)
            {
                Mod.Logger.WarnFormat("Failed to register spawn item for {0}. Use DoBossChecklistIntegration_NoSpawnItem if this was intentional.", Entry.BossName.Value);
            }
            if (Entry.Hint == null)
            {
                Mod.Logger.WarnFormat("Failed to register the hint for {0}. Verify that the correct localization key was used.", Entry.BossName.Value);
            }
            if (Entry.Portrait == null)
            {
                Mod.Logger.ErrorFormat("Failed to register the portrait {0}. Verify that the portrait was set up correctly.", Entry.BossName.Value);
            }

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                Entry.BossName.Value,
                Entry.Weight,
                predicate,
                Entry.InternalID,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = Entry.SpawnItem,
                    ["collectibles"] = Entry.LootTable,
                    ["customPortrait"] = Entry.Portrait,
                    ["spawnInfo"] = Entry.Hint,
                }
            );

            Mod.Logger.InfoFormat("Successfully registered Boss Checklist entry for {0}", Entry.BossName.Value);
        }
 

        private void DoBossChecklistIntegration_NoSpawnItem(BossChecklistEntry Entry)
        {

            // The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call
            // If we navigate the wiki, we can find the "LogBoss" method, which we want in this case
            // A feature of the call is that it will create an entry in the localization file of the specified NPC type for its spawn info, so make sure to visit the localization file after your mod runs once to edit it

            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.6
            // Usually mods either provide that information themselves in some way, or it's found on the GitHub through commit history/blame
            if (bossChecklistMod.Version < new Version(1, 6))
            {
                return;
            }


            Entry.DownCondition.Deconstruct(out LocalizedText description, out Func<bool> predicate);

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                Entry.BossName.Value,
                Entry.Weight,
                predicate,
                Entry.InternalID,
                new Dictionary<string, object>()
                {
                    ["collectibles"] = Entry.LootTable,
                    ["customPortrait"] = Entry.Portrait,
                    ["spawnInfo"] = Entry.Hint,
                }
            );

            Mod.Logger.InfoFormat("Successfully registered Boss Checklist entry (with no spawn item) for {0}", Entry.BossName.Value);
        }
    }
}
