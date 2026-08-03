using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class DescendantInferno : ModBuff
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
            player.GetModPlayer<DIPlayer>().lifeRegenDebuff = true;
        }
        public override void Update(NPC target, ref int buffIndex)
        {
            if (target.TryGetGlobalNPC<DITarget>(out var modNPC))
            {
                modNPC.lifeRegenDebuff = true;
            }
        }
    }

    public class DITarget : GlobalNPC
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
                StellarPointGlow G = new();
                G.Prepare(p1, d);
                ParticleEngine.ShaderParticles.Add(G);

                if (Main.rand.NextBool(10))
                {
                    FlatStarStellar S = new FlatStarStellar();
                    S.Prepare(Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(npc.Center, npc.Size)), Vector2.Zero, 0.15f);
                    ParticleEngine.ShaderParticles.Add(S);
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
                npc.lifeRegen -= 35;
            }
        }
    }

    public class DIPlayer : ModPlayer
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
                StellarPointGlow G = new();
                G.Prepare(p1, d);
                ParticleEngine.ShaderParticles.Add(G);

                if (Main.rand.NextBool(10))
                {
                    FlatStarStellar S = new FlatStarStellar();
                    S.Prepare(Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Player.Center, Player.Size)), Vector2.Zero, 0.15f);
                    ParticleEngine.ShaderParticles.Add(S);
                }
            }
        }
        public override void UpdateBadLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 35;
            }
        }
    }
}