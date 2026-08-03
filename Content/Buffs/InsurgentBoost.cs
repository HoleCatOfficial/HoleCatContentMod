
using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Content.Projectiles.player.Potion;
using Humanizer;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class InsurgentBoost : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex) {
			player.jumpSpeedBoost += 4;
            player.GetModPlayer<InsurgentBoostPlayer>().Active = true;
		}
	}

    public class InsurgentBoostPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            var w = Player.GetModPlayer<InsurgentPlayer>();
            if (Active)
            {
                Player.runAcceleration *= 1.4f;
                Player.maxRunSpeed *= 1.1f;

                Player.GetDamage(ModContent.GetInstance<ScepterClass>()) += 0.05f + (0.1f * w.DamageBooster);
            }
        }
    }
}