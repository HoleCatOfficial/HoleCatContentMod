using System.Collections;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common.Systems
{
    public class DTAchievement : ModSystem
    {
        public static bool LivingShadowEmpower = false;
        public static AchievementCondition LivingShadowEmpowerCondition = new CustomFlagCondition("LivingShadowEmpower");

        public override void ClearWorld()
        {
            LivingShadowEmpower = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (LivingShadowEmpower)
            {
                tag["LivingShadowEmpower"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            LivingShadowEmpower = tag.ContainsKey("LivingShadowEmpower");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteFlags(
                LivingShadowEmpower
            );
        }

        public override void NetReceive(BinaryReader reader) {
            reader.ReadFlags(
                out LivingShadowEmpower
            );
        }
    }
}