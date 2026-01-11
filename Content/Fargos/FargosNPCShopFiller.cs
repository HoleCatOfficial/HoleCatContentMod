using Terraria;
using Terraria.ModLoader;
using DestroyerTest.Content.MeleeWeapons;
using Fargowiltas.NPCs;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Fargos.ReusableSummons;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Fargos
{
    [JITWhenModsEnabled(DTCrossMod.FargosMutantName)]
	public class FargosNPCShopFiller : GlobalNPC
	{
		public override bool InstancePerEntity => true;
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == ModContent.NPCType<Mutant>())
            {
                shop.Add<CrimsonCaller>(DownedBossSystem.downedNodeCondition);
                shop.Add<CorruptionCaller>(DownedBossSystem.downedNodeCondition);
            }
        }
	}
}