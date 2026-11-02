using DestroyerTest.Content.Resources;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Achievement
{
    public class LivingShadowObtain : ModAchievement
    {
        public override void SetStaticDefaults()
        {
            AddItemPickupCondition(ModContent.ItemType<Living_Shadow>());
        }

        public override Position GetDefaultPosition() => new After("KILL_THE_SUN");
    }
}