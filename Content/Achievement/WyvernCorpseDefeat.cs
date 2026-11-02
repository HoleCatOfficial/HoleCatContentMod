using DestroyerTest.Content.Entities;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Achievement
{
    public class WyvernCorpseDefeat : ModAchievement
    {
        public override void SetStaticDefaults()
        {
            AddNPCKilledCondition(ModContent.NPCType<WyvernCorpseHead>());
        }

        public override Position GetDefaultPosition() => new After("CHAMPION_OF_TERRARIA");
    }
}