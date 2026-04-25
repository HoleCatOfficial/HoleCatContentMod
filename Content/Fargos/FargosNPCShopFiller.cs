using Terraria;
using Terraria.ModLoader;
using DestroyerTest.Content.MeleeWeapons;
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
            if (ModLoader.TryGetMod(DTCrossMod.FargosMutantName, out Mod fargos))
            {
                var Mutant = fargos.TryFind<ModNPC>("Mutant", out ModNPC mutant);
                if (shop.NpcType == mutant.Type && Mutant)
                {
                    shop.Add<CrimsonCaller>(DownedBossSystem.downedNodeCondition);
                    shop.Add<CorruptionCaller>(DownedBossSystem.downedNodeCondition);
                }
            }
        }
	}
}