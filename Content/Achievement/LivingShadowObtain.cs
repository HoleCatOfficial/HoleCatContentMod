using Terraria.ModLoader;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Achievement
{
	public class LivingShadowObtain: ModAchievement
	{
	public override void SetStaticDefaults() {
        AddItemPickupCondition(ModContent.ItemType<Living_Shadow>());
	}
    
	public override Position GetDefaultPosition() => new After("KILL_THE_SUN");
    }
}