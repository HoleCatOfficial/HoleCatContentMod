
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Humanizer;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class LightInferno : ModBuff
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

	public class LITarget : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool lifeRegenDebuff;

        public override void ResetEffects(NPC npc) 
		{
            
			lifeRegenDebuff = false;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);
            if (lifeRegenDebuff)
            {
                npc.velocity *= 1.2f;
            }
        }

        public override void PostAI(NPC npc)
        {
            if (lifeRegenDebuff)
            {
                npc.velocity *= 0.8f;
            }
        }



        public override void UpdateLifeRegen(NPC npc, ref int damage) 
        {
            Color SoulOfLightColor = new Color(220, 29, 183);
            if (lifeRegenDebuff) 
            {
            

				PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Main.rand.NextVector2FromRectangle(npc.Hitbox), new Vector2(0f, -0.1f), SoulOfLightColor * 0.35f, 1.0f, 40, ai2: 2);
				if (Main.rand.NextBool(6))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), Vector2.Zero, DTColorUtils.Pastel(SoulOfLightColor, 75f), 1f);
				}

                if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

                npc.lifeRegen -= 24;
            }
        }
    }

	public class LIPlayer : ModPlayer
	{
		public bool lifeRegenDebuff;

		public override void ResetEffects() {
			lifeRegenDebuff = false;
		}

        public override void PostUpdateRunSpeeds()
        {
            if (lifeRegenDebuff)
            {
                Player.maxRunSpeed *= 1.15f;
            }
        }

		public override void UpdateBadLifeRegen() 
        {
			Player player = Main.LocalPlayer;
            Color SoulOfLightColor = new Color(220, 29, 183);
			if (lifeRegenDebuff)
			{
				PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Main.rand.NextVector2FromRectangle(player.Hitbox), new Vector2(0f, -0.1f),  SoulOfLightColor * 0.35f, 1f, 40, ai2: 2);
				if (Main.rand.NextBool(6))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Main.rand.NextVector2FromRectangle(player.Hitbox), Vector2.Zero, DTColorUtils.Pastel(SoulOfLightColor, 75f), 1f);
				} 
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
			
				Player.lifeRegenTime = 0;
				Player.lifeRegen -= 18;
			}
		}
	}
}