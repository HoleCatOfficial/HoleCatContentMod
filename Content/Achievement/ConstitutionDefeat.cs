using Terraria.ModLoader;
using DestroyerTest.Content.Entities;

namespace DestroyerTest.Content.Achievement
{
public class ConstitutionDefeat : ModAchievement
{
	public override void SetStaticDefaults() {
		AddNPCKilledCondition(ModContent.NPCType<ConstitutionBoss>());
	}

	public override Position GetDefaultPosition() => new After("BONED");
    }
}