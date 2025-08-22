using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Consumables;

namespace DestroyerTest.Common
{
    public class NodePowerPlayer : ModPlayer
    {
        public bool Pendant;

        public override void ResetEffects()
        {
            Pendant = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Pendant)
            {
                Player.GetDamage(DamageClass.Generic) *= 1.50f;
            }
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            if (Pendant)
            {
                return !Main.rand.NextBool(3, 4);
            }
            return true;
        }

        public override void UpdateLifeRegen()
        {
            Player.lifeRegen += 15;
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            if (Pendant)
            {
                if (item == Player.HeldItem)
                {
                    crit += 16;
                }
            }
        }
    }
}