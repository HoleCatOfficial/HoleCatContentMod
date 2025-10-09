using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.NightmareRose;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using UtfUnknown.Core.Models.SingleByte.Finnish;

namespace DestroyerTest.Content.Entities
{
    public class CorruptSigil : ModNPC
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
                new FlavorTextBestiaryInfoElement("A nebulous formation of energy in the shape of the Deific Mark of the Corruption. It is very rare for this to form naturally."),
                ModContent.GetInstance<ShadeWorldBestiary>().ModBiomeBestiaryInfoElement,
            });
		}

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 55;
            NPC.defense = 50;
            NPC.lifeMax = 100;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            // Sets the above
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.0f;
            NPC.dontTakeDamage = true;
		}

        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> Sigil = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CorruptSigil");
            DTUtils Utility = new DTUtils();

            Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(
                Sigil.Value,
                NPC.Center - screenPos,
                null,
                ColorLib.CursedFlames,
                0f,
                Sigil.Size() / 2,
                1f,
                SpriteEffects.None,
                0
            );
            
            Texture2D tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/LongLaser").Value;

            DTUtils.DrawLaser(Main.spriteBatch, tex, NPC.Center, Color.White, NPC.rotation);

            // Optional: get AABBV line for collisions or visual debugging
            var (start, end) = DTUtils.GetLaserLine(tex, NPC.Center, NPC.rotation);

            // Check if the player intersects the laser line
            if (Collision.CheckAABBvLineCollision(
                Main.player[NPC.target].Hitbox.TopLeft(),
                Main.player[NPC.target].Hitbox.Size(),
                start,
                end))
            {
                // Optionally, you could apply effects or debug draw here
                Main.player[NPC.target].AddBuff(BuffID.Cursed, 120);
            }

            Utility.ReturnToDefaultDrawing(spriteBatch);

            return true;
        }

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player;
            player = Main.player[NPC.target];

            NPC.rotation = 0.05f * NPC.velocity.Length();
            Vector2 direction = player.Center - NPC.Center;
            direction.Normalize();

            NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 3f, 0.05f);

            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0, 0, 0, default, 2.0f);
            }

            if (Main.GameUpdateCount % 60 == 0)
            {
                NPC.life -= 2;
                SoundEngine.PlaySound(SoundID.Item45, NPC.Center);
                for (int a = 0; a < 5; a++)
                {
                    Vector2 Outer = NPC.Center + Main.rand.NextVector2CircularEdge(1000, 1000);
                    Vector2 toOrigin = NPC.Center - Outer;
                    toOrigin = toOrigin.SafeNormalize(Vector2.UnitY);
                    Vector2 shootdirection = toOrigin * 7f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), Outer, shootdirection, ModContent.ProjectileType<CursedFlameProj>(), 30, 1, ai2: 4);
                }
            }

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.CursedInferno, 120, true, false);
        }
    }
}