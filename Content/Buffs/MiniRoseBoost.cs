
using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Content.Projectiles.player.Potion;
using Humanizer;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class MiniRoseBoost : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex) 
        {
            player.GetModPlayer<MiniRoseBoostPlayer>().Active = true;
		}
	}

    public class MiniRoseBoostPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Active)
            {
                if (Main.rand.NextBool())
                {
                    //BasePRT Effect = PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ColorLib.CursedFlames, 1f);
                }
                Player.endurance += 0.06f;
                Player.statDefense += 10;
            }
        }
    }
}