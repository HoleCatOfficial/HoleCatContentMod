using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Consumables;
using System.Security.Permissions;

namespace DestroyerTest.Content.Buffs
{
	public class Vigor : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<DalmonPlayer>().VigorBuff = true;
		}
	}

    public class DalmonPlayer : ModPlayer
    {
        public bool VigorBuff = false;
        public bool PermaBuff = false;
        public bool PermaBuffEffects = false;

        public override void ResetEffects()
        {
            VigorBuff = false;
            PermaBuffEffects = false;
        }

        public override void PostUpdateMiscEffects()
        {
            float speedBonus = 1f;

            if (PermaBuff)
            {
                PermaBuffEffects = true;
            }

            if (VigorBuff)
                speedBonus += 0.08f;

            if (PermaBuffEffects)
                speedBonus += 0.12f;

            Player.GetAttackSpeed(DamageClass.Melee) *= speedBonus;
        }

        public override void PostUpdateRunSpeeds()
        {
            float runSpeedMult = 1f;
            float flySpeedMult = 1f;
            float accelMult = 1f;

            if (VigorBuff)
            {
                runSpeedMult += 0.10f;
                flySpeedMult += 0.08f;
                accelMult += 1.0f;
            }

            if (PermaBuffEffects)
            {
                runSpeedMult += 0.20f;
                flySpeedMult += 0.16f;
                accelMult += 1.0f;
            }

            Player.maxRunSpeed *= runSpeedMult;
            Player.runAcceleration *= accelMult;
            Player.wingAccRunSpeed *= flySpeedMult;
        }
	}
}