using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Common;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Terraria.GameContent;
using OpusLib;
using Terraria.Audio;
using System;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.EntitiesProjectiles;

namespace DestroyerTest.Content.Entities
{
    public class SunscorchedDjinn : ModNPC
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 20;
            NPC.defense = 15;
            NPC.lifeMax = 400;
            NPC.value = 1670f;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = -1;
            NPC.HitSound = DTAssetLib.Djinn.Hit;
            NPC.DeathSound = DTAssetLib.Djinn.Kill;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }


        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.ToRotation() * 0.1f;
            Lighting.AddLight(NPC.Center, ColorLib.Rift.ToVector3() * 0.6f);
            if (NPC.HasValidTarget || NPC.Distance(player.Center) > 500f)
            {
                AI_HoverNear(player);
                return;
            }
            else
            {
                AI_Idle(player);
                return;
            }
        }

        public void AI_Idle(Player player)
        {
            wait = 0;
            NPC.localAI[0] += 1f;
            if (NPC.localAI[0] % 120f == 0f)
            {
                NPC.velocity += new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-20f, -40f));
            }

            if (NPC.velocity.Length() > 0.1f)
            {
                NPC.velocity *= 0.98f;
            }

            AI_ValidateTarget(player);
            
        }

        public void AI_ValidateTarget(Player target)
        {
            if(target.dead || !target.active || Vector2.Distance(NPC.Center, target.Center) > 2000f)
            {
                NPC.TargetClosest();
            }
        }

        private Vector2 targetpos = Vector2.Zero;
        private int wait = 0;
        public void AI_HoverNear(Player target)
        {
            NPC.localAI[1] += 1f;
            if (NPC.localAI[1] % 300f == 0f)
            {
                targetpos = target.Center + Main.rand.NextVector2Circular(1000, 1000);
                NPC.localAI[1] = 0f;
                NPC.localAI[2] ++;
                if (NPC.localAI[2] % 3 == 0f)
                {
                    SoundEngine.PlaySound(DTAssetLib.Djinn.Laugh, NPC.Center);
                }
            }

            if (targetpos != Vector2.Zero)
            {
                Vector2 direction = targetpos - NPC.Center;
                direction.Normalize();
                direction *= 10f;

                NPC.velocity = (NPC.velocity * 20f + direction) / 21f;
            }

            if (wait++ > 180f)
            {
                AI_Attack();
            }
        }

        private bool releasestars = false;
        private List<Projectile> OwnedStars = new List<Projectile>();

        public void AI_Attack()
        {
            int numStars = Main.expertMode ? 10 : 5;

            NPC.localAI[3]++;

            bool orbiting = NPC.localAI[3] < 120f; // orbit briefly
            bool resetting = NPC.localAI[3] >= 660f;

            if (resetting)
            {
                NPC.localAI[3] = 0f;
                OwnedStars.Clear();
                return;
            }

            // Spawn once, up to cap
            while (OwnedStars.Count < numStars)
            {
                Projectile star = Projectile.NewProjectileDirect(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<SunscorchedDjinnStar>(),
                    10,
                    3
                );

                OwnedStars.Add(star);
            }

            foreach (Projectile s in OwnedStars)
            {
                if (!orbiting)
                {
                    s.ai[1] = 1f;
                    s.velocity += new Vector2(0, 0.01f);
                }
            }

            if (!orbiting)
            {
                return;
            }

            var orbitPos = Opus.GetEquidistantOrbitVectors(
                numStars,
                NPC.Center,
                0.5f,
                100f
            );

            for (int i = 0; i < OwnedStars.Count; i++)
            {
                Projectile star = OwnedStars[i];
                if (!star.active)
                    continue;

                star.Center = orbitPos[i];
                star.velocity = Vector2.Zero;
            }
        }



        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            bool v = ModContent.GetInstance<RiftDesertUnderground>().IsBiomeActive(spawnInfo.Player);
			if (v)
			{
				return 0.3f;
			}
			return 0f;
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                }
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(6, 6), 99);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SunscorchedCinder>(), 2, 2, 10));
        }
    }
}