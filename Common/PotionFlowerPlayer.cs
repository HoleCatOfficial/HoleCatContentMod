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

                        player.statLife = Math.Min(player.statLife + heal, player.statLifeMax2);
                        player.HealEffect(heal);
                        player.AddBuff(BuffID.PotionSickness, 30 * 60);
                        return true;
                    }
                }
            }
            return false;
        }


        public bool RadiantRose = false;
        public bool EphemeralSolvent = false;
        public bool LifeTalisman = false;
        public bool Lillies = false;
        public override void ResetEffects()
        {
            RadiantRose = false;
            EphemeralSolvent = false;
            LifeTalisman = false;
            Item.lifeGrabRange = 0;
            Lillies = false;
        }
        public int UseCooldown = 120;
        public override void PostUpdateMiscEffects()
        {
            if (Lillies)
            {
                Player.buffImmune[BuffID.PotionSickness] = true;
            }
            if (RadiantRose || EphemeralSolvent || Lillies)
                {
                    if (Player.statLife < Player.statLifeMax2 / 2)
                    {
                        if (Main.rand.NextBool(5))
                        {
                            Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.YellowTorch, Player.velocity.X * 0.5f, Player.velocity.Y * 0.5f, 0, default, 2.25f);
                            if (EphemeralSolvent)
                            {
                                Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.PinkTorch, Player.velocity.X * 0.5f, Player.velocity.Y * 0.5f, 0, default, 2.25f);
                            }
                            if (Lillies)
                            {
                                Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.LastPrism, Player.velocity.X * 0.5f, Player.velocity.Y * 0.5f, 0, default, 2.25f);
                            }
                        }
                        if (UseCooldown >= 600)
                        {
                            if (TryConsumeBestHealingPotion(Player))
                            {
                                UseCooldown = 0;
                            }
                        }
                    }

                    if (UseCooldown < 600)
                    {
                        UseCooldown++;
                    }

                    if (Player.HasBuff(BuffID.PotionSickness))
                    {
                        Player.GetDamage(DamageClass.Generic) *= 0.85f;
                        Player.statDefense *= 0.95f;
                    }
                }
            if (LifeTalisman)
            {
                Player.lifeMagnet = true;
                Item.lifeGrabRange = 80;
            }
            if (EphemeralSolvent)
            {
                Player.lifeMagnet = true;
                Item.lifeGrabRange = 180;
            }
        }

        public override void UpdateLifeRegen()
        {
            if (RadiantRose)
            {
                Player.lifeRegen += 8;
            }
            if (EphemeralSolvent)
            {
                Player.lifeRegen += 24;
            }
        }
    }

    public class ModifyPotionsItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool RadiantRose = false;
        public bool EphemeralSolvent = false;
        public bool Lillies;

        public override void UpdateInventory(Item item, Player player)
        {
            if (item.type == ItemID.LesserHealingPotion ||
            item.type == ItemID.HealingPotion ||
            item.type == ItemID.GreaterHealingPotion ||
            item.type == ItemID.SuperHealingPotion)
            {
                if (!item.TryGetGlobalItem<ModifyPotionsItem>(out var g))
                    return;

                if (RadiantRose || EphemeralSolvent | Lillies)
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