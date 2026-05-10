using System;
using System.Collections;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common.Systems
{
	// Acts as a container for "downed boss" flags.
	// Set a flag like this in your bosses OnKill hook:
	//    NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

	// Saving and loading these flags requires TagCompounds, a guide exists on the wiki: https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
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

		public static bool downedNodeMiniBoss = false;

		public static bool downedGolemBoss = false;

		public static bool downedFishronBoss = false;

		public static bool downedEmpressBoss = false;

		public static bool downedCultistBoss = false;

		public static bool downedLunarBoss = false;

		public static bool downedWyvernCorpseBoss = false;

		public static bool downedNightmareRoseBoss = false;

		public static Func<bool> downedConstitutionConditionbool = () => downedConstitutionBoss;
		public static Condition downedConstitutionCondition = new Condition("Mods.DestroyerTest.Conditions.ConstitutionBossDowned", downedConstitutionConditionbool);

		public static Func<bool> downedNodeConditionbool = () => downedNodeMiniBoss;
		public static Condition downedNodeCondition = new Condition("Mods.DestroyerTest.Conditions.NodeBossDowned", downedNodeConditionbool);

		public static Func<bool> downedWyvernCorpseBossConditionbool = () => downedWyvernCorpseBoss;
		public static Condition downedWyvernCorpseBossCondition = new Condition("Mods.DestroyerTest.Conditions.WyvernCorpseBossDowned", downedWyvernCorpseBossConditionbool);
		
		public static Func<bool> downedNightmareRoseBossConditionbool = () => downedNightmareRoseBoss;
		public static Condition downedNightmareRoseBossCondition = new Condition("Mods.DestroyerTest.Conditions.NightmareRoseBossDowned", downedNightmareRoseBossConditionbool);

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
			downedNodeMiniBoss = false;
			downedFishronBoss = false;
			downedEmpressBoss = false;
			downedGolemBoss = false;
			downedCultistBoss = false;
			downedLunarBoss = false;
			downedWyvernCorpseBoss = false;
			downedNightmareRoseBoss = false;
		}

		// We save our data sets using TagCompounds.
		// NOTE: The tag instance provided here is always empty by default.
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
            tag.Add("downedNodeMiniBoss", downedNodeMiniBoss);
            tag.Add("downedGolemBoss", downedGolemBoss);
            tag.Add("downedFishronBoss", downedFishronBoss);
            tag.Add("downedEmpressBoss", downedEmpressBoss);
            tag.Add("downedCultistBoss", downedCultistBoss);
            tag.Add("downedLunarBoss", downedLunarBoss);
            tag.Add("downedWyvernCorpseBoss", downedWyvernCorpseBoss);
            tag.Add("downedNightmareRoseBoss", downedNightmareRoseBoss);
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

            if (tag.ContainsKey("downedNodeMiniBoss"))
            {
                downedNodeMiniBoss = tag.GetBool("downedNodeMiniBoss");
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
        }

        public override void NetSend(BinaryWriter writer) {
			// Order of parameters is important and has to match that of NetReceive
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
				downedNodeMiniBoss
			);
			writer.WriteFlags(
				downedGolemBoss,
				downedFishronBoss,
				downedEmpressBoss, 
				downedCultistBoss, 
				downedLunarBoss,
				downedWyvernCorpseBoss,
				downedNightmareRoseBoss
			);
			// WriteFlags supports up to 8 entries, if you have more than 8 flags to sync, call WriteFlags again.

			// If you need to send a large number of flags, such as a flag per item type or something similar, BitArray can be used to efficiently send them. See Utils.SendBitArray documentation.
		}

		public override void NetReceive(BinaryReader reader) {
			// Order of parameters is important and has to match that of NetSend
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
				out downedNodeMiniBoss
			);
			reader.ReadFlags(
				out downedGolemBoss,
				out downedFishronBoss,
				out downedEmpressBoss, 
				out downedCultistBoss, 
				out downedLunarBoss,
				out downedWyvernCorpseBoss,
				out downedNightmareRoseBoss
			);
			// ReadFlags supports up to 8 entries, if you have more than 8 flags to sync, call ReadFlags again.
		}
	}

	
}