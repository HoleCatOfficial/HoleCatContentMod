using System;
using ReLogic.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class PotionFlowerPlayer : ModPlayer
    {
        private static readonly (int itemID, int heal)[] HealingPotions =
        {
            (ItemID.SuperHealingPotion, 200),
            (ItemID.GreaterHealingPotion, 150),
            (ItemID.HealingPotion, 100),
            (ItemID.LesserHealingPotion, 50)
        };

        private bool TryConsumeBestHealingPotion(Player player)
        {
            foreach (var (id, heal) in HealingPotions)
            {
                for (int i = 0; i < player.inventory.Length; i++) // Main inventory
                {
                    Item item = player.inventory[i];
                    if (item.type == id && item.stack > 0)
                    {
                        item.stack--; // manually consume one
                        if (item.stack <= 0)
                            item.TurnToAir(); // remove if empty

                        player.statLife += heal;
                        player.HealEffect(heal);
                        return true;
                    }
                }
            }
            return false;
        }


        public bool RadiantRose = false;
        public bool LifeTalisman = false;
        public override void ResetEffects()
        {
            RadiantRose = false;
            LifeTalisman = false;
            Item.lifeGrabRange -= 36;
        }
        public int UseCooldown = 120;
        public override void PostUpdateMiscEffects()
        {
            if (RadiantRose)
            {
                if (Player.statLife < Player.statLifeMax2 / 2)
                {
                    if (UseCooldown >= 120)
                    {
                        if (TryConsumeBestHealingPotion(Player))
                        {
                        }
                        UseCooldown = 0;
                    }
                }

                if (UseCooldown < 120)
                {
                    UseCooldown++;
                }

                if (Player.HasBuff(BuffID.PotionSickness))
                    Player.GetDamage(DamageClass.Generic) *= 0.70f;
            }
            if (LifeTalisman)
            {
                Player.lifeMagnet = true;
                Item.lifeGrabRange += 36;
            }
        }

        public override void UpdateLifeRegen()
        {
            if (RadiantRose)
            {
                Player.lifeRegen += 8;
            }
        }
    }

    public class ModifyPotionsItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool RadiantRose = false;

        public override void UpdateInventory(Item item, Player player)
        {
            if (item.type == ItemID.LesserHealingPotion ||
                item.type == ItemID.HealingPotion ||
                item.type == ItemID.GreaterHealingPotion ||
                item.type == ItemID.SuperHealingPotion ||
                item.type == ItemID.RegenerationPotion)
            {
                if (!item.TryGetGlobalItem<ModifyPotionsItem>(out var g))
                    return;

                if (RadiantRose)
                {
                    item.buffTime = 54 * 60;
                }
                else
                {
                    item.buffTime = 60 * 60; 
                }
            }
        }
    }
}