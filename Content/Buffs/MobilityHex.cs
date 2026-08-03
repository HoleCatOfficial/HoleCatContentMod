using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class MobilityHex : ModBuff
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
			player.GetModPlayer<MobilityHexPlayer>().Locked = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<MobilityHexTarget>(out var modNPC)) {
                modNPC.Locked = true;
            }
		}
	}
	
	public class MobilityHexTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool Locked;

        public override void ResetEffects(NPC npc)
        {
            Locked = false;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Locked)
            {
                Main.EntitySpriteDraw(DTAssetLib.MobilityHexDoll.Value, (npc.Center + new Vector2(0, -80)) - Main.screenPosition, null, Color.White, 0f, DTAssetLib.MobilityHexDoll.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void AI(NPC npc)
        {
            if (Locked)
            {
                Dust.NewDust(npc.position, npc.Hitbox.Width, npc.Hitbox.Height, DustID.TintableDustLighted, 0f, 0f, 100, Color.Red, 1);
                Dust.NewDust(npc.position, npc.Hitbox.Width, npc.Hitbox.Height, DustID.TintableDustLighted, 0f, 0f, 100, Color.DarkMagenta, 1);

                npc.velocity *= 0.015f;
            }
            base.AI(npc);
        }
        
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (Locked)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HoleCatHookFreeze"));
                target.AddBuff(ModContent.BuffType<MobilityHex>(), 200);
            }
        }

    }

	public class MobilityHexPlayer : ModPlayer
	{
		public bool Locked;

        public override void ResetEffects()
        {
            Locked = false;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Locked)
            {
                Main.EntitySpriteDraw(DTAssetLib.MobilityHexDoll.Value, (Player.Center + new Vector2(0, -80)) - Main.screenPosition, null, Color.White, 0f, DTAssetLib.MobilityHexDoll.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void PostUpdateBuffs()
        {
            if (Locked)
            {
                Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.TintableDustLighted, 0f, 0f, 100, Color.Red, 2);
                Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.TintableDustLighted, 0f, 0f, 100, Color.DarkMagenta, 2);

                Player.canCarpet = Player.canRocket = Player.channel = false;
                Player.moveSpeed *= 0.015f;
			}
        }
	}
}