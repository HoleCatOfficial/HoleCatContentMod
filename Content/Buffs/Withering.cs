using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Comaceratic;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class Withering : ModBuff
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
            player.GetModPlayer<WPlayer>().lifeRegenDebuff = true;
        }
        public override void Update(NPC target, ref int buffIndex)
        {
            if (target.TryGetGlobalNPC<WTarget>(out var modNPC))
            {
                modNPC.lifeRegenDebuff = true;
            }
        }
    }

    public class WTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool lifeRegenDebuff;

        public override void ResetEffects(NPC npc)
        {
            lifeRegenDebuff = false;
        }

        public override void AI(NPC npc)
        {
            if (lifeRegenDebuff)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<ColorableNeonDust>(), 0.0f, 0.5f, 0, Color.DarkMagenta, 1);

                if (Main.rand.NextBool(8))
                {
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<WitheringSpark>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), Main.rand.NextVector2Circular(2f, 2f), Color.DarkMagenta, Main.rand.NextFloat(0.05f, 0.1f));
                }


                if (npc.boss == false)
                {
                    npc.velocity *= 0.65f;
                }
            }
            base.AI(npc);
        }


        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (lifeRegenDebuff)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 70;
            }
        }
    }

    public class WPlayer : ModPlayer
    {
        public bool lifeRegenDebuff;

        public override void ResetEffects()
        {
            lifeRegenDebuff = false;
        }

        public override void PostUpdateBuffs()
        {
            if (lifeRegenDebuff)
            {
                Player.moveSpeed *= 0.85f;


                Dust.NewDust(Player.position, Player.width, Player.height, ModContent.DustType<ColorableNeonDust>(), 0.0f, 0.5f, 0, Color.DarkMagenta, 1);

                if (Main.rand.NextBool(8))
                {
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<WitheringSpark>(), Main.rand.NextVector2FromRectangle(Player.Hitbox), Main.rand.NextVector2Circular(2f, 2f), Color.DarkMagenta, Main.rand.NextFloat(0.05f, 0.1f));
                }
            }
        }
        public override void UpdateBadLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 50;
            }
        }
    }
}