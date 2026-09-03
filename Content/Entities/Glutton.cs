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
    public class Glutton : ModNPC, IDrawPixelated
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
        }
        public void immunities()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
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
            NPC.width = 90;
            NPC.height = 90;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Back = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/GluttonBack").Value;
            Main.EntitySpriteDraw(Back, NPC.Center - Main.screenPosition, null, Color.White, r, Back.Size() / 2, NPC.scale * 1.4f, SpriteEffects.None);

            //drawColor = Color.White;

            Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, null, Color.White, 0f, TextureAssets.Npc[Type].Value.Size() / 2, NPC.scale, SpriteEffects.None);

            return false;
        }

        public IchorNodeMB Node;
        Line toNode;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;

        public override bool CheckActive()
        {
            return false;
        }

        float r = 0f;
        public override void AI()
        {
            NPC.rotation = 0f;
            r += 0.01f * NPC.velocity.X;
            Lighting.AddLight(NPC.Center, ColorLib.IchorCrystal2.ToVector3());

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
                NPCID.Crimera,
                NPCID.Crimslime,
                NPCID.FaceMonster,
                NPCID.BloodCrawler
            };

            if (Main.rand.NextBool(150))
            {
                for (int i = 0; i < 5; i++)
                {
                    PointGlowPreMultiplied Point = new();
                    Point.Initialize(NPC.Center, Main.rand.NextVector2Circular(3, 3), ColorLib.IchorCrystal3, 2f);
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

            if (toNode != null)
            {
                DTUtils.instance.ScrollingTextureSpine(toNode, DTAssetLib.Streak(3, true), ColorLib.IchorCrystal3, Main.spriteBatch, BlendState.Additive, O, 0.4f);
            }

            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, NPC.Center - Main.screenPosition, null, ColorLib.IchorCrystal3 with { A = 0 }, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

            spriteBatch.ResetToDefault();
        }

        public override void OnKill()
        {
            if (toNode != null)
            {
                Vector2[] Positions = toNode.GetPointsAlongLine(30);

                for (int i = 0; i < Positions.Length; i++)
                {
                    PointGlowPreMultiplied Point = new();
                    Point.Initialize(Positions[i], Main.rand.NextVector2Circular(1, 1), ColorLib.IchorCrystal3, 2f);
                    ParticleEngine.Particles.Add(Point);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                PointGlowPreMultiplied Point = new();
                Point.Initialize(NPC.Center, Main.rand.NextVector2Circular(3, 3), ColorLib.IchorCrystal3, 2f);
                ParticleEngine.Particles.Add(Point);
            }

            if (Node != null)
            {
                Node.SentinelKillTally++;
            }
        }
    }
}