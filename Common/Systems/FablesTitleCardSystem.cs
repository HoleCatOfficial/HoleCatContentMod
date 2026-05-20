using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class FablesTitleCardSystem : ModSystem
    {
        public class ConstitutionTitle
        {
            public static string Name = Language.GetTextValue("Mods.DestroyerTest.NPCs.ConstitutionBoss.Fables.Name");
            public static string Title = Language.GetTextValue("Mods.DestroyerTest.NPCs.ConstitutionBoss.Fables.Title");
            public static string MusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.ConstitutionBoss.Default");
            public static string MusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
        }

        public class NightmareRoseTitle
        {
            public static string Name = Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.Fables.Name");
            public static string Title = Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.Fables.Title");
            public static string MusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.EvilBoss.Default");
            public static string MusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
        }

        public class WyvernCorpseTitle
        {
            public static string Name = Language.GetTextValue("Mods.DestroyerTest.NPCs.WyvernCorpseHead.Fables.Name");
            public static string Title = Language.GetTextValue("Mods.DestroyerTest.NPCs.WyvernCorpseHead.Fables.Title");
            public static string MusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.EvilBoss.Default");
            public static string MusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
        }

        public class CursedFlameNodeTitle
        {
            public static string Name = Language.GetTextValue("Mods.DestroyerTest.NPCs.CursedFlameNodeMB.Fables.Name");
            public static string Title = Language.GetTextValue("Mods.DestroyerTest.NPCs.CursedFlameNodeMB.Fables.Title");
            public static string MusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.NodeBoss.Fight");
            public static string MusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
        }

        public class IchorNodeTitle
        {
            public static string Name = Language.GetTextValue("Mods.DestroyerTest.NPCs.IchorNodeMB.Fables.Name");
            public static string Title = Language.GetTextValue("Mods.DestroyerTest.NPCs.ichorNodeMB.Fables.Title");
            public static string MusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.NodeBoss.Fight");
            public static string MusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
        }

        public class BlessedNodeTitle
        {
            public static string Name = Language.GetTextValue("Mods.DestroyerTest.NPCs.BlessedNodeMB.Fables.Name");
            public static string Title = Language.GetTextValue("Mods.DestroyerTest.NPCs.BlessedNodeMB.Fables.Title");
            public static string MusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.NodeBoss.FightHallow");
            public static string MusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
        }

        public static void RegisterFablesBossIntro(string BossName, string BossTitle, int Duration, bool flipHorizontal, Color BorderColor, Color BossTitleColor, Color chromaticabberation1, Color chromaticabberation2, string MusicTitle, string MusicArtist)
        {
            string CallName = "vfx.displayBossIntroCard";
            DTCrossMod.FablesMod.Call(CallName, BossName, BossTitle, Duration, flipHorizontal, BorderColor, BossTitleColor, chromaticabberation1, chromaticabberation2, MusicTitle, MusicArtist);
        }

        public override void PostSetupContent()
        {
            if (!DTCrossMod.FablesIsLoaded)
            {
                return;
            }

            

            

            
        }
    }
}
