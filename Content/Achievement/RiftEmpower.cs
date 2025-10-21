using Terraria.ModLoader;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Resources;
using DestroyerTest.Common.Systems;

namespace DestroyerTest.Content.Achievement
{
	public class RiftEmpower : ModAchievement
	{
	public override void SetStaticDefaults() {
        Achievement.AddCondition(DTAchievement.LivingShadowEmpowerCondition);
	}
    
	public override Position GetDefaultPosition() => new After("KILL_THE_SUN");
    }
}