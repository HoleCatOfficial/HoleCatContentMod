
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.fire;
using Humanizer;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class NightInferno : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true; 
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true; 
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<LIPlayer>().lifeRegenDebuff = true;
		}
		
		public override void Update(NPC target, ref int buffIndex)
		{
			if (target.TryGetGlobalNPC<LITarget>(out var modNPC))
			{
				modNPC.lifeRegenDebuff = true;
			}
		}
	}

	public class NITarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool lifeRegenDebuff;

        public override void ResetEffects(NPC npc) {
            lifeRegenDebuff = false;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);
            if (lifeRegenDebuff)
            {
                npc.damage = (int)(npc.damage * 1.25f);
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage) 
        {
            Color SoulOfNightColor = new Color(123, 29, 120);
            if (lifeRegenDebuff) 
            {

                Fire fire = new Fire();
                fire.PrepareFire(Main.rand.NextVector2FromRectangle(npc.Hitbox), new Vector2(0f, -0.1f), DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), SoulOfNightColor * 0.8f, 0.5f, 40, FireDrawMode.Additive);
                ParticleEngine.ShaderParticles.Add(fire);

				if (Main.rand.NextBool(6))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), Vector2.Zero, DTColorUtils.Pastel(SoulOfNightColor, 75f), 0.25f);
				}

                if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

                npc.lifeRegen -= 24;
            }
        }
    }

	public class NIPlayer : ModPlayer
	{
		public bool lifeRegenDebuff;

		public override void ResetEffects() {
			lifeRegenDebuff = false;
		}

        public override void PostUpdateBuffs()
        {
            if (lifeRegenDebuff)
            {
                Player.GetDamage(DamageClass.Generic) *= 1.25f;
            }
        }

		public override void UpdateBadLifeRegen() 
        {
			Player player = Main.LocalPlayer;
            Color SoulOfNightColor = new Color(123, 29, 120);
			if (lifeRegenDebuff)
			{
                Fire fire = new Fire();
                fire.PrepareFire(Main.rand.NextVector2FromRectangle(Player.Hitbox), new Vector2(0f, -0.1f), Main.rand.Next(1, 3), 0.08f, SoulOfNightColor * 0.8f, 0.5f, 40, FireDrawMode.Additive);
                ParticleEngine.ShaderParticles.Add(fire);

                if (Main.rand.NextBool(6))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Main.rand.NextVector2FromRectangle(player.Hitbox), Vector2.Zero, DTColorUtils.Pastel(SoulOfNightColor, 75f), 0.5f);
				} 
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
			
				Player.lifeRegenTime = 0;
				Player.lifeRegen -= 18;
			}
		}
	}
}