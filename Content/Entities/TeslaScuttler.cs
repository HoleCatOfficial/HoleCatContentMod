using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Common;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.EntitiesProjectiles;

namespace DestroyerTest.Content.Entities
{
    public class TeslaScuttler : ModNPC
    {
        private enum State { Burrowed, Charge, Zap }
        private State currentState = State.Burrowed;
        private int stateTimer = 0;
        private Vector2 chargeDirection = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 52;
            NPC.height = 22;
            NPC.damage = 40;
            NPC.defense = 60;
            NPC.lifeMax = 400;
            NPC.knockBackResist = 0.5f;
            NPC.GravityIgnoresSpace = true;
            NPC.HitSound = SoundID.DD2_LightningBugHurt;
            NPC.DeathSound = SoundID.NPCDeath36;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            bool v = (ModContent.GetInstance<RiftSurface>().IsBiomeActive(spawnInfo.Player) || 
            ModContent.GetInstance<RiftUnderground>().IsBiomeActive(spawnInfo.Player) ||
            ModContent.GetInstance<RiftDesert>().IsBiomeActive(spawnInfo.Player) ||
            ModContent.GetInstance<RiftDesertUnderground>().IsBiomeActive(spawnInfo.Player) ||
            ModContent.GetInstance<RiftTundra>().IsBiomeActive(spawnInfo.Player));
			if (v)
			{
				return 0.5f;
			}
			return 0f;
		}

        public override void AI()
        {
            NPC.TargetClosest();

            NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;

            NPC.velocity.Y += 1f; // Increase as needed
            if (NPC.velocity.Y > 100f)
                NPC.velocity.Y = 100f; // Terminal velocity clamp

            NPC.GravityMultiplier *= 4;

            float stepSpeed = 0.6f;
            float gfxOffY = 0f;

            if (NPC.collideX && NPC.velocity.Y == 0f) {
                Collision.StepUp(
                    ref NPC.position,
                    ref NPC.velocity,
                    NPC.width,
                    NPC.height,
                    ref stepSpeed,
                    ref gfxOffY
                );
                NPC.gfxOffY = gfxOffY;
            }
            

            switch (currentState)
            {
                case State.Burrowed:
                    NPC.frame.Y = 0;
                    NPC.velocity = Vector2.Zero;
                    break;

                case State.Charge:
                    UpdateCharge();
                    break;

                case State.Zap:
                    Sound1 = false;
                    UpdateZap();
                    break;
            }

            stateTimer++;
        }

        private bool Sound1 = false;
        private void UpdateCharge()
        {
            Player player = Main.player[NPC.target];
            NPC.direction = (NPC.Center.X > player.Center.X) ? -1 : 1;
            if (!Sound1)
            {
                SoundEngine.PlaySound(SoundID.Zombie78, NPC.Center);
                Sound1 = true;
            }
            if (stateTimer == 0)
            {
                float dir = Math.Sign(player.Center.X - NPC.Center.X);
                chargeDirection = new Vector2(dir, 0f);
            }


            NPC.velocity = chargeDirection * 8f;
            UpdateFrames();

            if (stateTimer >= 120)
            {
                currentState = State.Zap;
                stateTimer = 0;
                NPC.velocity = Vector2.Zero;
            }
        }

        private void UpdateZap()
        {
            NPC.velocity = Vector2.Zero;
            
            UpdateFrames();

            if (stateTimer % 30 == 0)
            {
                ApplyDebuffInRadius();
            }

            if (stateTimer >= 360)
            {
                currentState = State.Charge;
                stateTimer = 0;

                Player player = Main.player[NPC.target];
                chargeDirection = Vector2.Normalize(player.Center - NPC.Center);
            }
        }

        private void ApplyDebuffInRadius()
        {
            const float radius = 600f;

            for (int t = 0; t < 25; t++)
            {
                Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius), DustID.FireworksRGB, null, 0, ColorLib.Rift, 1.2f);
            }

            foreach (Player player in Main.player)
            {
                
                if (player.active && Vector2.Distance(NPC.Center, player.Center) <= radius)
                {
                    player.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
                    int DamDir1 = (NPC.Center.X > player.Center.X) ? -1 : 1;
                    player.Hurt(new PlayerDeathReason() { CustomReason = NetworkText.FromKey("Mods.DestroyerTest.NPCs.TeslaScuttler.ZapDeathReason")}, 20, 0, false, false, DamDir1, false, 10, 5, 6);
                }
            }

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type != NPC.type && Vector2.Distance(NPC.Center, npc.Center) <= radius)
                {
                    npc.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
                    int DamDir2 = (NPC.Center.X > npc.Center.X) ? -1 : 1;
                    NPC.HitInfo hit = new NPC.HitInfo { Crit = false, Damage = 20, HitDirection = DamDir2, Knockback = 3};
                    npc.StrikeNPC(hit, false, true);
                }
            }

            SoundEngine.PlaySound(SoundID.NPCHit34, NPC.Center);
        }

        private void UpdateFrames()
        {
            int frameIndex = (stateTimer / 10) % 4 + 1;
            NPC.frame.Y = frameIndex * NPC.frame.Height;
        }

        int HitCount = 0;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (currentState == State.Burrowed)
            {
                HitCount++;
                for(int u = 0; u < 10; u++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RiftClayDust>(), Main.rand.NextFloat(-5, 5), -10, 0, default, 1.5f);
                }
                if (HitCount > 4)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TeslaScuttlerScaredExplosion>(), 20, 16);
                    currentState = State.Charge;
                    stateTimer = 0;
                    HitCount = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Living_Shadow>(), 1, 3, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Item_RiftClay>(), 1, 1, 5));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RiftenOverloader>(), 10, 1, 1));
        }
    }
}