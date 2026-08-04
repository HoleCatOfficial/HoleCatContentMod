using System;
using System.Collections;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common.Systems
{
	public class DownedBossSystem : ModSystem
	{	
		public static bool downedEoCBoss = false;

		public static bool downedKingSlimeBoss = false;

		public static bool downedBoCBoss = false;

		public static bool downedEoWBoss = false;

		public static bool downedQueenBeeBoss = false;

		public static bool downedDeerclopsMiniBoss = false;

		public static bool downedSkeletronBoss = false;

		public static bool downedConstitutionBoss = false;

		public static bool downedWallBoss = false;

		public static bool downedQueenSlimeBoss = false;

		public static bool downedDestroyerBoss = false;

		public static bool downedTwinsBoss = false;

		public static bool downedSkeletronPrimeBoss = false;

		public static bool downedNautilusMiniBoss = false;

		public static bool downedPlanteraBoss = false;

		public static bool downedIchorNodeMiniBoss = false;

        public static bool downedCursedFlameNodeMiniBoss = false;

        public static bool downedBlessedNodeMiniBoss = false;

        public static bool downedGolemBoss = false;

		public static bool downedFishronBoss = false;

		public static bool downedEmpressBoss = false;

		public static bool downedCultistBoss = false;

		public static bool downedLunarBoss = false;

		public static bool downedWyvernCorpseBoss = false;

		public static bool downedNightmareRoseBoss = false;

        public static bool downedTenebrousConstructBoss = false;

        public static Func<bool> downedConstitutionConditionbool = () => downedConstitutionBoss;
		public static Condition downedConstitutionCondition = new Condition("Mods.DestroyerTest.Conditions.ConstitutionBossDowned", downedConstitutionConditionbool);


		public static Func<bool> downedAnyNodeConditionbool = () => (downedIchorNodeMiniBoss || downedCursedFlameNodeMiniBoss || downedBlessedNodeMiniBoss);
		public static Condition downedAnyNodeCondition = new Condition("Mods.DestroyerTest.Conditions.AnyNodeBossDowned", downedAnyNodeConditionbool);

        public static Func<bool> downedIchorNodeConditionbool = () => downedIchorNodeMiniBoss;
        public static Condition downedIchorNodeCondition = new Condition("Mods.DestroyerTest.Conditions.IchorNodeBossDowned", downedIchorNodeConditionbool);

        public static Func<bool> downedCursedFlameNodeConditionbool = () => downedCursedFlameNodeMiniBoss;
        public static Condition downedCursedFlameNodeCondition = new Condition("Mods.DestroyerTest.Conditions.CursedFlameNodeBossDowned", downedCursedFlameNodeConditionbool);

        public static Func<bool> downedBlessedNodeConditionbool = () => downedBlessedNodeMiniBoss;
        public static Condition downedBlessedNodeCondition = new Condition("Mods.DestroyerTest.Conditions.BlessedNodeBossDowned", downedBlessedNodeConditionbool);


        public static Func<bool> downedWyvernCorpseBossConditionbool = () => downedWyvernCorpseBoss;
		public static Condition downedWyvernCorpseBossCondition = new Condition("Mods.DestroyerTest.Conditions.WyvernCorpseBossDowned", downedWyvernCorpseBossConditionbool);
		
		public static Func<bool> downedNightmareRoseBossConditionbool = () => downedNightmareRoseBoss;
		public static Condition downedNightmareRoseBossCondition = new Condition("Mods.DestroyerTest.Conditions.NightmareRoseBossDowned", downedNightmareRoseBossConditionbool);

        public static Func<bool> downedTenebrousConstructBossConditionbool = () => downedTenebrousConstructBoss;
        public static Condition downedTenebrousConstructBossCondition = new Condition("Mods.DestroyerTest.Conditions.TenebrousConstructBossDowned", downedTenebrousConstructBossConditionbool);

        public override void ClearWorld()
		{
			downedEoCBoss = false;
			downedKingSlimeBoss = false;
			downedBoCBoss = false;
			downedEoWBoss = false;
			downedQueenBeeBoss = false;
			downedDeerclopsMiniBoss = false;
			downedSkeletronBoss = false;
			downedConstitutionBoss = false;
			downedWallBoss = false;
			downedQueenSlimeBoss = false;
			downedDestroyerBoss = false;
			downedTwinsBoss = false;
			downedSkeletronPrimeBoss = false;
			downedNautilusMiniBoss = false;
			downedPlanteraBoss = false;
			downedIchorNodeMiniBoss = false;
			downedCursedFlameNodeMiniBoss = false;
			downedBlessedNodeMiniBoss = false;
			downedFishronBoss = false;
			downedEmpressBoss = false;
			downedGolemBoss = false;
			downedCultistBoss = false;
			downedLunarBoss = false;
			downedWyvernCorpseBoss = false;
			downedNightmareRoseBoss = false;
            downedTenebrousConstructBoss = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("downedEoCBoss", downedEoCBoss);
            tag.Add("downedBoCBoss", downedBoCBoss);
            tag.Add("downedEoWBoss", downedEoWBoss);
            tag.Add("downedDeerclopsMiniBoss", downedDeerclopsMiniBoss);
            tag.Add("downedQueenBeeBoss", downedQueenBeeBoss);
            tag.Add("downedSkeletronBoss", downedSkeletronBoss);
            tag.Add("downedConstitutionBoss", downedConstitutionBoss);
            tag.Add("downedWallBoss", downedWallBoss);
            tag.Add("downedQueenSlimeBoss", downedQueenSlimeBoss);
            tag.Add("downedDestroyerBoss", downedDestroyerBoss);
            tag.Add("downedTwinsBoss", downedTwinsBoss);
            tag.Add("downedPrimeBoss", downedSkeletronPrimeBoss);
            tag.Add("downedNautilusMiniBoss", downedNautilusMiniBoss);
            tag.Add("downedPlanteraBoss", downedPlanteraBoss);
            tag.Add("downedIchorNodeMiniBoss", downedIchorNodeMiniBoss);
            tag.Add("downedCursedFlameNodeMiniBoss", downedCursedFlameNodeMiniBoss);
            tag.Add("downedBlessedNodeMiniBoss", downedBlessedNodeMiniBoss);
            tag.Add("downedGolemBoss", downedGolemBoss);
            tag.Add("downedFishronBoss", downedFishronBoss);
            tag.Add("downedEmpressBoss", downedEmpressBoss);
            tag.Add("downedCultistBoss", downedCultistBoss);
            tag.Add("downedLunarBoss", downedLunarBoss);
            tag.Add("downedWyvernCorpseBoss", downedWyvernCorpseBoss);
            tag.Add("downedNightmareRoseBoss", downedNightmareRoseBoss);
            tag.Add("downedTenebrousConstruct Boss", downedTenebrousConstructBoss);
        }


        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("downedEoCBoss"))
            {
                downedEoCBoss = tag.GetBool("downedEoCBoss");
            }

            if (tag.ContainsKey("downedBoCBoss"))
            {
                downedBoCBoss = tag.GetBool("downedBoCBoss");
            }

            if (tag.ContainsKey("downedEoWBoss"))
            {
                downedEoWBoss = tag.GetBool("downedEoWBoss");
            }

            if (tag.ContainsKey("downedDeerclopsMiniBoss"))
            {
                downedDeerclopsMiniBoss = tag.GetBool("downedDeerclopsMiniBoss");
            }

            if (tag.ContainsKey("downedQueenBeeBoss"))
            {
                downedQueenBeeBoss = tag.GetBool("downedQueenBeeBoss");
            }

            if (tag.ContainsKey("downedSkeletronBoss"))
            {
                downedSkeletronBoss = tag.GetBool("downedSkeletronBoss");
            }

            if (tag.ContainsKey("downedConstitutionBoss"))
            {
                downedConstitutionBoss = tag.GetBool("downedConstitutionBoss");
            }

            if (tag.ContainsKey("downedWallBoss"))
            {
                downedWallBoss = tag.GetBool("downedWallBoss");
            }

            if (tag.ContainsKey("downedQueenSlimeBoss"))
            {
                downedQueenSlimeBoss = tag.GetBool("downedQueenSlimeBoss");
            }

            if (tag.ContainsKey("downedDestroyerBoss"))
            {
                downedDestroyerBoss = tag.GetBool("downedDestroyerBoss");
            }

            if (tag.ContainsKey("downedTwinsBoss"))
            {
                downedTwinsBoss = tag.GetBool("downedTwinsBoss");
            }

            if (tag.ContainsKey("downedPrimeBoss"))
            {
                downedSkeletronPrimeBoss = tag.GetBool("downedPrimeBoss");
            }

            if (tag.ContainsKey("downedNautilusMiniBoss"))
            {
                downedNautilusMiniBoss = tag.GetBool("downedNautilusMiniBoss");
            }

            if (tag.ContainsKey("downedPlanteraBoss"))
            {
                downedPlanteraBoss = tag.GetBool("downedPlanteraBoss");
            }

            if (tag.ContainsKey("downedIchorNodeMiniBoss"))
            {
                downedIchorNodeMiniBoss = tag.GetBool("downedIchorNodeMiniBoss");
            }

            if (tag.ContainsKey("downedCursedFlameNodeMiniBoss"))
            {
                downedCursedFlameNodeMiniBoss = tag.GetBool("downedCursedFlameNodeMiniBoss");
            }

            if (tag.ContainsKey("downedBlessedNodeMiniBoss"))
            {
                downedBlessedNodeMiniBoss = tag.GetBool("downedBlessedNodeMiniBoss");
            }

            if (tag.ContainsKey("downedTenebrousConstructBoss"))
            {
                downedTenebrousConstructBoss = tag.GetBool("downedTenebrousConstructBoss");
            }

            if (tag.ContainsKey("downedGolemBoss"))
            {
                downedGolemBoss = tag.GetBool("downedGolemBoss");
            }

            if (tag.ContainsKey("downedFishronBoss"))
            {
                downedFishronBoss = tag.GetBool("downedFishronBoss");
            }

            if (tag.ContainsKey("downedEmpressBoss"))
            {
                downedEmpressBoss = tag.GetBool("downedEmpressBoss");
            }

            if (tag.ContainsKey("downedCultistBoss"))
            {
                downedCultistBoss = tag.GetBool("downedCultistBoss");
            }

            if (tag.ContainsKey("downedLunarBoss"))
            {
                downedLunarBoss = tag.GetBool("downedLunarBoss");
            }

            if (tag.ContainsKey("downedWyvernCorpseBoss"))
            {
                downedWyvernCorpseBoss = tag.GetBool("downedWyvernCorpseBoss");
            }

            if (tag.ContainsKey("downedNightmareRoseBoss"))
            {
                downedNightmareRoseBoss = tag.GetBool("downedNightmareRoseBoss");
            }

            if (tag.ContainsKey("downedTenebrousConstructBoss"))
            {
                downedTenebrousConstructBoss = tag.GetBool("downedTenebrousConstructBoss");
            }
        }

        public override void NetSend(BinaryWriter writer) {
			writer.WriteFlags(
				downedEoCBoss, 
				downedKingSlimeBoss, 
				downedBoCBoss, 
				downedEoWBoss, 
				downedQueenBeeBoss, 
				downedDeerclopsMiniBoss, 
				downedSkeletronBoss, 
				downedConstitutionBoss
				
			);
			writer.WriteFlags(
				downedWallBoss,
				downedQueenSlimeBoss, 
				downedDestroyerBoss, 
				downedTwinsBoss, 
				downedSkeletronPrimeBoss, 
				downedNautilusMiniBoss, 
				downedPlanteraBoss, 
				downedIchorNodeMiniBoss
			);
			writer.WriteFlags(
                downedCursedFlameNodeMiniBoss,
                downedBlessedNodeMiniBoss,
				downedGolemBoss,
				downedFishronBoss,
				downedEmpressBoss, 
				downedCultistBoss
			);

            writer.WriteFlags(
                downedLunarBoss,
                downedWyvernCorpseBoss,
                downedNightmareRoseBoss,
                downedTenebrousConstructBoss
            );

        }

		public override void NetReceive(BinaryReader reader) {
			reader.ReadFlags(
				out downedEoCBoss, 
				out downedKingSlimeBoss, 
				out downedBoCBoss, 
				out downedEoWBoss, 
				out downedQueenBeeBoss, 
				out downedDeerclopsMiniBoss, 
				out downedSkeletronBoss, 
				out downedConstitutionBoss
			);
			reader.ReadFlags(
				out downedWallBoss,
				out downedQueenSlimeBoss, 
				out downedDestroyerBoss, 
				out downedTwinsBoss, 
				out downedSkeletronPrimeBoss, 
				out downedNautilusMiniBoss, 
				out downedPlanteraBoss, 
				out downedIchorNodeMiniBoss
			);
			reader.ReadFlags(
                out downedCursedFlameNodeMiniBoss,
                out downedBlessedNodeMiniBoss,
				out downedGolemBoss,
				out downedFishronBoss,
				out downedEmpressBoss, 
				out downedCultistBoss
			);

            reader.ReadFlags(
                out downedLunarBoss,
                out downedWyvernCorpseBoss,
                out downedNightmareRoseBoss,
                out downedTenebrousConstructBoss
            );

        }
	}

	
}