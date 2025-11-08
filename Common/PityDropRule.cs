using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace DestroyerTest.Common
{
    public abstract class PityDropRule : IItemDropRule
    {
        public int ItemID;
        public int SourceID; // NPC type or bag item ID
        public float BaseChance; // e.g. 0.05f = 5%
        public int BaseAmount = 1;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; } = new();

        public PityDropRule(int sourceID, int itemID, float baseChance, int baseAmount = 1)
        {
            SourceID = sourceID;
            ItemID = itemID;
            BaseChance = baseChance;
            BaseAmount = baseAmount;
        }

        public virtual bool CanDrop(DropAttemptInfo info) => true;

        public abstract ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info);

        protected void OnSuccess(DropAttemptInfo info)
        {
            PitySystem.ResetPity(SourceID, ItemID);
        }

        protected void OnFail()
        {
            PitySystem.IncrementPity(SourceID, ItemID);
        }

        public virtual void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            // Default report — subclasses will calculate effective chance
            float dropRate = BaseChance * ratesInfo.parentDroprateChance;
            drops.Add(new DropRateInfo(ItemID, BaseAmount, BaseAmount, dropRate, ratesInfo.conditions));

            Chains.ReportDroprates(ChainedRules, dropRate, drops, ratesInfo);
        }
    }

    public class PityChanceDropRule : PityDropRule
    {
        public float IncrementPerFail;
        public float MaxChance;

        public PityChanceDropRule(int sourceID, int itemID, float baseChance, float incrementPerFail, float maxChance = 1f)
            : base(sourceID, itemID, baseChance)
        {
            IncrementPerFail = incrementPerFail;
            MaxChance = maxChance;
        }

        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            var result = new ItemDropAttemptResult();
            int pity = PitySystem.GetPity(SourceID, ItemID);
            float chance = MathHelper.Clamp(BaseChance + IncrementPerFail * pity, 0f, MaxChance);

            if (info.rng.NextFloat() < chance)
            {
                CommonCode.DropItem(info, ItemID, BaseAmount);
                result.State = ItemDropAttemptResultState.Success;
                OnSuccess(info);
            }
            else
            {
                result.State = ItemDropAttemptResultState.FailedRandomRoll;
                OnFail();
            }

            return result;
        }

        public override void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            // Approximate effective drop rate for display
            // Use midpoint between base and max for simplicity
            float avgChance = (BaseChance + MaxChance) / 2f;
            float finalChance = avgChance * ratesInfo.parentDroprateChance;

            drops.Add(new DropRateInfo(ItemID, BaseAmount, BaseAmount, finalChance, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, finalChance, drops, ratesInfo);
        }
    }

    public class PityAmountDropRule : PityDropRule
    {
        public int IncrementPerFail;
        public int MaxAmount;

        public PityAmountDropRule(int sourceID, int itemID, float baseChance, int baseAmount, int incrementPerFail, int maxAmount)
            : base(sourceID, itemID, baseChance, baseAmount)
        {
            IncrementPerFail = incrementPerFail;
            MaxAmount = maxAmount;
        }

        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            var result = new ItemDropAttemptResult();
            int pity = PitySystem.GetPity(SourceID, ItemID);
            int amount = Math.Min(BaseAmount + IncrementPerFail * pity, MaxAmount);

            if (info.rng.NextFloat() < BaseChance)
            {
                CommonCode.DropItem(info, ItemID, amount);
                result.State = ItemDropAttemptResultState.Success;
                OnSuccess(info);
            }
            else
            {
                result.State = ItemDropAttemptResultState.FailedRandomRoll;
                OnFail();
            }

            return result;
        }

        public override void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            float dropRate = BaseChance * ratesInfo.parentDroprateChance;
            int avgAmount = (BaseAmount + MaxAmount) / 2;

            drops.Add(new DropRateInfo(ItemID, BaseAmount, avgAmount, dropRate, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, dropRate, drops, ratesInfo);
        }
    }

}
