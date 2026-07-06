using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class Defilement : ModBuff
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
            player.GetModPlayer<DPlayer>().lifeRegenDebuff = true;
        }
        public override void Update(NPC target, ref int buffIndex)
        {
            if (target.TryGetGlobalNPC<DTarget>(out var modNPC))
            {
                modNPC.lifeRegenDebuff = true;
            }
        }
    }

    public class DTarget : GlobalNPC
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
                Vector2 p1 = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(npc.Center, npc.Size));
                Vector2 d = npc.Center - p1;
                d.Normalize();

                var P = DamnationParticle.Create(p1, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-0.2f, 0.2f)), Main.rand.NextFloat(0.2f, 1.4f), 60, PixelLayer.AboveNPCs);
                ParticleEngine.ShaderParticles.Add(P);
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
                npc.lifeRegen -= 80;
            }
        }
    }

    public class DPlayer : ModPlayer
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
                Vector2 p1 = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Player.Center, Player.Size));
                Vector2 d = Player.Center - p1;
                d.Normalize();
                
                var P = DamnationParticle.Create(p1, new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.2f, 0.2f)), Main.rand.NextFloat(0.2f, 1.4f), 60, PixelLayer.AbovePlayer);
                ParticleEngine.ShaderParticles.Add(P);

                
            }
        }

        public override void PostUpdateRunSpeeds()
        {
            if (lifeRegenDebuff)
            {
                Player.maxRunSpeed *= 0.666666666666666f;
                Player.runAcceleration *= 0.333333333333333f;
                Player.wingAccRunSpeed *= 0.75f;
            }
        }
        public override void UpdateBadLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 90;
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (lifeRegenDebuff)
            {
                damageSource.CustomReason = NetworkText.FromFormattable("{0} succumbed to hopelessness", Player.name);
            }
        }
    }
}