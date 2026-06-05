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
    public abstract class BossTitle
    {
        public abstract string Name();
        public abstract string Title();
        public abstract string MusicTitle();
        public abstract string MusicArtist();

        public abstract Color BackColor();
        public abstract Color TextColor();
        public abstract Color TextAbberationColor1();
        public abstract Color TextAbberationColor2();

        public abstract int Time { get; set; }

        public abstract bool Flip { get; set; }
    }
    public class FablesTitleCardSystem : ModSystem
    {
        public class ConstitutionTitle : BossTitle
        {
            public override int Time { get; set; } = 180;
            public override bool Flip { get; set; } = false;

            public override Color BackColor()
            {
                return ColorLib.StellarFire2;
            }

            public override string MusicArtist()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.Author3");
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.Author3");
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
                }
                return "";
            }

            public override string MusicTitle()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    Language.GetTextValue("Mods.DestroyerTest.Music.ConstitutionBoss.Eternity");
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    Language.GetTextValue("Mods.DestroyerTest.Music.ConstitutionBoss.Eternity");
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    Language.GetTextValue("Mods.DestroyerTest.Music.ConstitutionBoss.Default");
                }
                return "";
            }

            public override string Name()
            {
                return Language.GetTextValue("Mods.DestroyerTest.NPCs.ConstitutionBoss.Fables.Name");
            }

            public override string Title()
            {
                return Language.GetTextValue("Mods.DestroyerTest.NPCs.ConstitutionBoss.Fables.Title");
            }

            public override Color TextAbberationColor1()
            {
                return ColorLib.StellarFire4;
            }

            public override Color TextAbberationColor2()
            {
                return ColorLib.StellarFire3;
            }

            public override Color TextColor()
            {
                return Color.White;
            }
        }

        public class NightmareRoseTitle : BossTitle
        {
            public override int Time { get; set; } = 180;
            public override bool Flip { get; set; } = false;

            public override Color BackColor()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return ColorLib.TenebrisGradient;
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return ColorLib.WretchedGradient();
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return ColorLib.WretchedGradient();
                }
                return Color.White;
            }

            public override string MusicArtist()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.Author5");
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.Author2");
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.Author1");
                }
                return "";
            }

            public override string MusicTitle()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.EvilBoss.Masochist");
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.EvilBoss.Eternity");
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.Music.EvilBoss.Default");
                }
                return "";
            }

            public override string Name()
            {
                return Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.Fables.Name");
            }

            public override string Title()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.Fables.Title.Masochist");
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.Fables.Title.Eternity");
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.Fables.Title.Default");
                }
                return "";
            }

            public override Color TextAbberationColor1()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return ColorLib.TenebrisGradient;
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return ColorLib.TenebrisGradient;
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Color.Red;
                }
                return Color.Red;
            }

            public override Color TextAbberationColor2()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return ColorLib.TenebrisGradient;
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return ColorLib.TenebrisGradient;
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Color.Red;
                }
                return Color.Red;
            }

            public override Color TextColor()
            {
                if (DestroyerTestMod.MasochistIsActive)
                {
                    return ColorLib.TenebrisGradient;
                }
                if (DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Color.White;
                }
                if (!DestroyerTestMod.EternityIsActive && !DestroyerTestMod.MasochistIsActive)
                {
                    return Color.White;
                }
                return Color.White;
            }
        }

        public class WyvernCorpseTitle
        {
            public static string Name = Language.GetTextValue("Mods.DestroyerTest.NPCs.WyvernCorpseHead.Fables.Name");
            public static string Title = Language.GetTextValue("Mods.DestroyerTest.NPCs.WyvernCorpseHead.Fables.Title");
            public static string MusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.EvilBoss.Default");
            public static string MusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author1");

            public static string EternityMusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.EvilBoss.Eternity");
            public static string EternityMusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author2");

            public static string MasoMusicTitle = Language.GetTextValue("Mods.DestroyerTest.Music.EvilBoss.Masochist");
            public static string MasoMusicArtist = Language.GetTextValue("Mods.DestroyerTest.Music.Author5");
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
            if (!DTCrossMod.FablesIsLoaded)
            {
                return;
            }

            string CallName = "vfx.displayBossIntroCard";
            DTCrossMod.FablesMod.Call(CallName, BossName, BossTitle, Duration, flipHorizontal, BorderColor, BossTitleColor, chromaticabberation1, chromaticabberation2, MusicTitle, MusicArtist);
        }

        public static void RegisterFablesBossIntro(BossTitle title)
        {
            if (!DTCrossMod.FablesIsLoaded)
            {
                return;
            }

            string CallName = "vfx.displayBossIntroCard";
            DTCrossMod.FablesMod.Call(CallName, title.Name(), title.Title(), title.Time, title.Flip, title.BackColor(), title.TextColor(), title.TextAbberationColor1(), title.TextAbberationColor2(), title.MusicTitle(), title.MusicArtist());
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
