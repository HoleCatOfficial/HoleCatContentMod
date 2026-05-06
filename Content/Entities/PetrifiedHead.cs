using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.EntitiesProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using UtfUnknown.Core.Models.SingleByte.Finnish;

namespace DestroyerTest.Content.Entities
{
    public class PetrifiedHead : ModNPC
    {

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f,
                Direction = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
		}

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 28;
            NPC.damage = 55;
            NPC.defense = 5;
            NPC.lifeMax = 500;
            NPC.noGravity = true;
            NPC.damage = 15;
            NPC.aiStyle = NPCAIStyleID.CursedSkull;
            AIType = NPCID.CursedSkull;
            // Sets the above
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.0f;
            NPC.HitSound = SoundID.Item52;
            NPC.DeathSound = SoundID.Item110;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = SpriteEffects.None;

            // Horizontal flip still based on direction
            if (NPC.spriteDirection == -1)
                effects |= SpriteEffects.FlipHorizontally;

            // Determine if rotation goes past "upside-down"
            float rot = MathHelper.WrapAngle(NPC.rotation);

            // If upside-down (between 90° and 270°), flip vertically
            if (rot > MathHelper.PiOver2 || rot < -MathHelper.PiOver2)
                effects |= SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2), NPC.scale, effects, 0);
            return false;
        }


        public override void AI()
        {
            NPC.TargetClosest();
            Player player;
            player = Main.player[NPC.target];

            int basespd = 3;

            if (NPC.velocity.Length() > basespd)
            {
                NPC.velocity *= 0.99f;
            }

            Vector2 look = player.Center - NPC.Center;
            NPC.rotation = look.ToRotation() + MathHelper.PiOver2;
            NPC.spriteDirection = look.X > 0 ? 1 : -1;

            Vector2 direction = player.Center - NPC.Center;
            direction.Normalize();

            if (NPC.Center.Distance(player.Center) < 200f)
            {
                if (Main.GameUpdateCount % 120 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item72);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction.ToRotation().ToRotationVector2() * 10f, ModContent.ProjectileType<PetrifiedHeadRiftLaser>(), 16, 5f);
                }
            }

            if (NPC.Center.Distance(player.Center) > 200f)
            {
                if (Main.rand.NextBool(200))
                {
                    Opus.RingSpreadDust(ModContent.DustType<RiftDust>(), 30, NPC.Center, 10, 0, default, 1.2f, 3f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                    NPC.velocity += direction.ToRotation().ToRotationVector2() * 5f;
                }
            }

            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RiftDust>(), 0, 0, 0, default, 1.0f);
            }
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
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<RiftDust>(), new Vector2(Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-8, -12)), 0, default, 1.5f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CarbonizedFlesh>(), 3, 2, 9));
        }
    }
}