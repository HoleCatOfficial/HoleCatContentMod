using DestroyerTest.Common.Systems;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Achievement
{
    public class RiftEmpower : ModAchievement
    {
        public override void SetStaticDefaults()
        {
            Achievement.AddCondition(DTAchievement.LivingShadowEmpowerCondition);
        }

        public override Position GetDefaultPosition() => new After("KILL_THE_SUN");
    }
}