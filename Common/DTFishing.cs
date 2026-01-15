
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Consumables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
	public class DTFishing : ModPlayer
	{
		public override void SetStaticDefaults() {
		}

		public override void ResetEffects() {

		}

		public override void ModifyFishingAttempt(ref FishingAttempt attempt) {
			
		}

		public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
			if (Player.ZoneRockLayerHeight && DownedBossSystem.downedGolemBoss && attempt.crate) {
				// If the game rolls a crate, we want to give ours to the player if he is in Example Surface Biome

				// We don't want to replace golden/titanium crates (the highest tier crates), as they take highest priority in crate catches
				// Their drop conditions are "veryrare" or "legendary"
				// (After that come biome crates ("rare"), then iron/mythril ("uncommon"), then wood/pearl (none of the previous))
				// Let's replace biome crates 50% of the time (player could be in multiple (modded) biomes, we should respect that)
				if (Main.rand.NextBool(4)) {
					itemDrop = ModContent.ItemType<HeliciteCrate>();
					return; // This is important so your code after this that rolls items will not run
				}
			}
		}

		public override bool? CanConsumeBait(Item bait) {
			PlayerFishingConditions conditions = Player.GetFishingConditions();

			return null;
		}

		public override void ModifyCaughtFish(Item fish) {
			
		}
	}
}