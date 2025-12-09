using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace DestroyerTest.Content.Entities
{
    public class PetrifiedLurker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 48;
            NPC.damage = 20;
            NPC.defense = 15;
            NPC.lifeMax = 1200;
            NPC.value = 100f;
            NPC.knockBackResist = 0.75f;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            AIType = NPCID.Skeleton;
            NPC.HitSound = SoundID.Item52;
            NPC.DeathSound = SoundID.NPCDeath6;
        }

        public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 5;
            int frameSpeed = 5;
            NPC.frameCounter += 0.5f;
            NPC.frameCounter += NPC.velocity.Length() / 10f;
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }

        public override void AI()
        {
            base.AI();

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
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.direction == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2), NPC.scale, effects, 0);
            return false;
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
                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<PetrifiedHead>());
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(6, 6), 99);
                }
            }
        }
    }
}