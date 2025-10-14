using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class BloodHex : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;  // Is it a debuff?
			Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
			Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
			BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
		}

		// Allows you to make this buff give certain effects to the given player
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<BloodHexPlayer>().lifeRegenDebuff = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<BloodHexTarget>(out var modNPC)) {
                modNPC.lifeRegenDebuff = true;
            }
		}
	}
	
	public class BloodHexTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool lifeRegenDebuff;

        public override void ResetEffects(NPC npc)
        {
            lifeRegenDebuff = false;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (lifeRegenDebuff)
            {
                Main.EntitySpriteDraw(DTAssetLib.BloodHexHeart.Value, (npc.Center + new Vector2(0, -80)) - Main.screenPosition, null, Color.White, 0f, DTAssetLib.BloodHexHeart.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void AI(NPC npc)
        {
			if (lifeRegenDebuff)
			{
				Dust.NewDust(npc.position, npc.Hitbox.Width, npc.Hitbox.Height, DustID.TintableDustLighted, 0f, 0f, 0, Color.Red, 1);
                Dust.NewDust(npc.position, npc.Hitbox.Width, npc.Hitbox.Height, DustID.TintableDustLighted, 0f, 0f, 0, Color.DarkMagenta, 1);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), Vector2.Zero, Color.Red, 1f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), Vector2.Zero, Color.DarkMagenta, 1f);
			}
            base.AI(npc);
        }


        public void UpdateLifeRegen(NPC npc, Player player, ref int damage)
		{
			if (lifeRegenDebuff)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 19;
			}
		}
    }

	public class BloodHexPlayer : ModPlayer
	{
		public bool lifeRegenDebuff;

        public override void ResetEffects()
        {
            lifeRegenDebuff = false;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (lifeRegenDebuff)
            {
                Main.EntitySpriteDraw(DTAssetLib.BloodHexHeart.Value, (Player.Center + new Vector2(0, -80)) - Main.screenPosition, null, Color.White, 0f, DTAssetLib.BloodHexHeart.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void PostUpdateBuffs()
        {
            if (lifeRegenDebuff)
            {
                Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.TintableDustLighted, 0f, 0f, 0, Color.Red, 1);
                Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.TintableDustLighted, 0f, 0f, 0, Color.DarkMagenta, 1);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, Color.Red, 1f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, Color.DarkMagenta, 1f);
			}
        }
		public override void UpdateBadLifeRegen()
		{
			if (lifeRegenDebuff)
			{
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				Player.lifeRegenTime = 0;
			}
		}
	}
}