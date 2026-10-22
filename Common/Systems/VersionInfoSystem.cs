using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace DestroyerTest.Common.Systems
{
    public class VersionInfoSystem : ModSystem
    {
        public Version LatestVersion => Mod.Version;

    }

    public class VersionInfoPlayer : ModPlayer
    {
        List<string> messages = new();
        public bool firstJoin = true;
        public override void OnEnterWorld()
        {
            if (Main.dedServ || firstJoin)
            {

                return;
            }

            using StreamReader changelog = File.OpenText(Path.Combine(Mod.SourceFolder, "changelog.txt"));




            VersionInfoSystem system = ModContent.GetInstance<VersionInfoSystem>();

            Main.NewText($"You are running Talid v{Mod.Version.ToString()}", ColorLib.HoleCatFireMaroon);

            for (int i = 0; i < Main.WorldList.Count - 1; i++)
            {
                WorldFileData world = Main.WorldList[i];

                if (world.Name == Main.worldName && world.TryGetModVersionGeneratedWith("DestroyerTest", out Version worldVersion))
                {
                    if (worldVersion != system.LatestVersion)
                    {
                        Main.NewText($"This world was generated using an older version of Talid: v{worldVersion.ToString()}, while you are running v{system.LatestVersion.ToString()}", ColorLib.HoleCatFireMaroon);

                        Version sectionVersion = null;

                        while (changelog.ReadLine() is string line)
                        {
                            line = line.Trim();

                            if (string.IsNullOrEmpty(line))
                                continue;

                            // Is this line a version header?
                            if (Version.TryParse(line, out Version parsedVersion))
                            {
                                sectionVersion = parsedVersion;
                                continue;
                            }

                            // Is this message relevant to the world's version?
                            if (sectionVersion > worldVersion && sectionVersion <= system.LatestVersion)
                            {
                                messages.Add(line);
                            }
                        }

                        if (messages.Count > 0)
                        {
                            foreach (string msg in messages)
                            {
                                Main.NewText(msg, ColorLib.HoleCatFireMaroon);
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            firstJoin = false;
        }
        public override void UpdateDead()
        {
            firstJoin = true;
        }
    }
}
