using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using OpusLib;
using OpusLib.Content.Helpers;
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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DestroyerTest.Content.Entities
{
    public class CursedFlameSentinel : ModNPC, IDrawPixelated
    {

        public override void SetStaticDefaults()
        {
            immunities();
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f,
                Direction = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            Main.npcFrameCount[Type] = 6;
        }
        public void immunities()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Oiled] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1))
            });

            bestiaryEntry.Info.AddRange([
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption
            ]);
        }

        SoundStyle Kill = new SoundStyle("DestroyerTest/Assets/Audio/TPKill")
        {
            Volume = 0.3f,
            PitchVariance = 1f,
            MaxInstances = 0
        };

        SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/DAHit")
        {
            Volume = 0.3f,
            PitchVariance = 1f,
            MaxInstances = 0
        };

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 44;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 400;
            NPC.HitSound = Hit;
            NPC.DeathSound = Kill;
            NPC.noGravity = true;
            NPC.aiStyle = NPCAIStyleID.FlyingFish;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.1f;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            drawColor = Color.White;
            return true;
        }

        int XOff = 0;
        int YOff = 0;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.GameUpdateCount % 60 == 0)
            {
                XOff = Main.rand.Next(4);
                YOff = Main.rand.Next(4);
            }

            Vector2 origin = new Vector2(TextureAssets.Npc[Type].Value.Width / 2f, (TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type]) / 2f);


            Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, (NPC.Center + new Vector2(XOff, YOff)) - Main.screenPosition, NPC.frame, Color.White with { A = 0 } * 0.5f, NPC.rotation, origin, NPC.scale, SpriteEffects.None);
        }

        public CursedFlameNodeMB Node;
        Line toNode;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            NPC.rotation = 0.02f * NPC.velocity.Length() * NPC.direction;
            Lighting.AddLight(NPC.Center, ColorLib.Wretched1.ToVector3());

            if (Node != null)
            {
                toNode = new Line(NPC.Center, Node.NPC.Center);

                if (NPC.Distance(Node.NPC.Center) > 1200)
                {
                    NPC.Center = Node.NPC.Center + new Vector2(1190, 0).RotatedBy(Node.NPC.Center.DirectionTo(NPC.Center).ToRotation());
                }
            }

            List<int> Enemies = new List<int>
            {
                NPCID.EaterofSouls,
                NPCID.Corruptor,
                NPCID.Slimer
            };

            if (Main.rand.NextBool(150))
            {
                for (int i = 0; i < 5; i++)
                {
                    WretchedPointGlow Point = new();
                    Point.Prepare(NPC.Center, Main.rand.NextVector2Circular(3, 3), 2f);
                    ParticleEngine.Particles.Add(Point);
                }

                SoundEngine.PlaySound(SoundID.Zombie43, NPC.Center);
                NPC wavenpc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, Enemies[Main.rand.Next(Enemies.Count)]);
            }
        }

        int O = 0;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            O += 20;
            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            Texture2D Wing = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/CursedFlameSentinelWing").Value;
            float WingScale = Opus.Sine(0f, 1f);

            //Right
            Main.EntitySpriteDraw(Wing, (NPC.Center + new Vector2(20, -10)) - Main.screenPosition, null, Color.White with { A = 0 }, 0f, new Vector2(0f, Wing.Height / 2), new Vector2(WingScale, 1f), SpriteEffects.None);

            //Left
            Main.EntitySpriteDraw(Wing, (NPC.Center + new Vector2(-20, -10)) - Main.screenPosition, null, Color.White with { A = 0 }, 0f, new Vector2(Wing.Width, Wing.Height / 2), new Vector2(WingScale, 1f), SpriteEffects.FlipHorizontally);

            if (toNode != null)
            {
                DTUtils.instance.ScrollingTextureSpine(toNode, DTAssetLib.Streak(3, true), ColorLib.Wretched2, Main.spriteBatch, BlendState.Additive, O, 0.4f);
            }

            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, NPC.Center - Main.screenPosition, null, ColorLib.Wretched2 with { A = 0 }, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

            spriteBatch.ResetToDefault();
        }

        public override void OnKill()
        {
            if (toNode != null)
            {
                Vector2[] Positions = toNode.GetPointsAlongLine(30);

                for (int i = 0; i < Positions.Length; i++)
                {
                    WretchedPointGlow P = new();
                    P.Prepare(Positions[i], Main.rand.NextVector2Circular(1, 1), 2f);
                    ParticleEngine.Particles.Add(P);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                WretchedPointGlow Point = new();
                Point.Prepare(NPC.Center, Main.rand.NextVector2Circular(3, 3), 2f);
                ParticleEngine.Particles.Add(Point);
            }

            if (Node != null)
            {
                Node.SentinelKillTally++;
            }
        }
    }
}