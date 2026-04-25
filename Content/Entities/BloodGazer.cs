using BreadLibrary.Core.Verlet;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DestroyerTest.Content.Entities
{
    public class BloodGazer : ModNPC
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.damage = 250;
            NPC.defense = 50;
            NPC.lifeMax = 30;
            NPC.value = 100f;
            NPC.knockBackResist = 0.3f;
            NPC.aiStyle = NPCAIStyleID.StarCell;
            //NPC.aiStyle = NPCAIStyleID.AncientVision;
            NPC.HitSound = DTAssetLib.Impacts.StellarFox with { MaxInstances = 0, Pitch = -0.7f, PitchVariance = 0.2f };
            NPC.DeathSound = SoundID.Item74;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawCrystalCore(spriteBatch, NPC.Center, Color.Black, Color.DarkRed, TextureRotationOffset, 1.5f);
            return true;
        }

        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center, Color colorIN, Color colorOUT, float TextureRotationOffset, float Scale = 1f)
        {
            DTUtils Utility = new DTUtils();
            float OuterScale = Scale * 0.12f;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);

            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                colorOUT,
                TextureRotationOffset,
                new Vector2(DTAssetLib.Cyclone(2).Value.Width / 2f, DTAssetLib.Cyclone(2).Value.Height / 2f),
                OuterScale,
                SpriteEffects.None,
                1f
            );

            Main.spriteBatch.Draw(
                DTAssetLib.FeatheredCircle.Value,
                Center - Main.screenPosition,
                null,
                colorIN,
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                Scale,
                SpriteEffects.None,
                1f
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            RenderRope(screenPos, Color.Black, Rope1);
            RenderRope(screenPos, Color.Black, Rope2);
            Hands();

            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, drawPos - Main.screenPosition, null, Color.Red, 0f, DTAssetLib.Star(3).Value.Size() / 2, new Vector2(0.8f, 1.2f), SpriteEffects.None, 0f);
        }

        private VerletChain Rope1;
        private VerletChain Rope2;

        public Vector2 Rope1Start;
        public Vector2 Rope2Start;

        private void RenderRope(Vector2 screenPos, Color drawColor, VerletChain Rope)
        {

            var tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/GazerHandRope").Value;


            int segmentCount = Rope.Positions.Length;
            for (var i = 0; i < segmentCount - 1; i++)
            {

                var start = Rope.Positions[i];
                var end = Rope.Positions[i + 1];

                Vector2 VinePos = (start + end) / 2;
                var DrawPos = VinePos - screenPos;

                var style = 0;



                if (i == Rope.Positions.Length - 3)
                {
                    style = 0;
                }

                if (i > Rope.Positions.Length - 3)
                {
                    style = 1;
                }

                var frame = tex.Frame(1, 1, style);

                var rotation = start.AngleTo(end);


                var t = 0f;

                if (segmentCount > 1)
                {
                    t = i / (float)(segmentCount - 1); // 0 at base, 1 at tip
                }


                // Vertical stretch based on actual distance to next segment and texture height
                var segmentDistance = start.Distance(end);
                var lengthFactor = 1f;
                float denom = Math.Max(1, frame.Height - 5);
                lengthFactor = segmentDistance / denom * 1.2f;

                // Combine into final stretch vector and apply a small global multiplier for visual tuning
                var stretch = new Vector2(lengthFactor, 1f) * 1.2f;
                var Origin = frame.Size() * 0.5f;

                if (i % 2 == 0)
                {
                    continue;
                }

                if (i == segmentCount - 2)
                {
                    stretch = Vector2.One;
                    Origin = new Vector2(frame.Width / 2, 2);
                }
                Main.EntitySpriteDraw(tex, DrawPos, frame, drawColor, rotation, Origin, stretch, 0);
            }
        }


        public Texture2D Hand = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/GazerHand").Value;
        public Vector2 Offset1;
        public Vector2 Offset2;
        public void Hands()
        {
            NPC.localAI[0]++;
            if (NPC.localAI[0] > 1)
            {
                Vector2 Offset1Ideal = NPC.Center + new Vector2(-25, 200);
                Offset1 = Vector2.Lerp(Offset1, Offset1Ideal, 0.05f);
                Vector2 Offset2Ideal = NPC.Center + new Vector2(25, 200);
                Offset2 = Vector2.Lerp(Offset2, Offset2Ideal, 0.05f);


                Main.EntitySpriteDraw(Hand, Offset1 - Main.screenPosition, null, Color.Black, MathHelper.PiOver2, (Hand.Size() / 2), 1f, SpriteEffects.FlipVertically, 0f);
                Main.EntitySpriteDraw(Hand, Offset2 - Main.screenPosition, null, Color.Black, MathHelper.PiOver2, (Hand.Size() / 2), 1f, SpriteEffects.None, 0f);
            }
        }

        public float TextureRotationOffset = 0f;
        public float LookDir = 0f;
        public float LookRange = 300;

        public Vector2 drawPos;

        public override void OnSpawn(IEntitySource source)
        {
            Offset1 = NPC.Center;
            Offset2 = NPC.Center;

            Rope1Start = NPC.Center + new Vector2(-25, 0);
            Rope2Start = NPC.Center + new Vector2(25, 0);

            Line R1 = new Line(Rope1Start, Offset1);
            Line R2 = new Line(Rope2Start, Offset2);

            if (Rope1 == null)
            {
                Rope1 = new VerletChain(18, 2, Rope1Start);

                Vector2[] pt = R1.GetPointsAlongLine(18);

                for (int k = 0; k < pt.Length - 1; k++)
                {
                    Rope1.Positions[k] = pt[k];
                }
            }

            if (Rope2 == null)
            {
                Rope2 = new VerletChain(18, 2, Rope2Start);

                Vector2[] pt = R2.GetPointsAlongLine(18);

                for (int k = 0; k < pt.Length - 1; k++)
                {
                    Rope2.Positions[k] = pt[k];
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            Rope1Start = NPC.Center + new Vector2(-25, 0);
            Rope2Start = NPC.Center + new Vector2(25, 0);

            Line R1 = new Line(Rope1Start, Offset1);
            Line R2 = new Line(Rope2Start, Offset2);

            if (Rope1 == null)
            {
                Rope1 = new VerletChain(18, 2, Rope1Start);

                Vector2[] pt = R1.GetPointsAlongLine(18);

                for (int k = 0; k < pt.Length - 1; k++)
                {
                    Rope1.Positions[k] = pt[k];
                }
            }

            if (Rope2 == null)
            {
                Rope2 = new VerletChain(18, 2, Rope2Start);

                Vector2[] pt = R2.GetPointsAlongLine(18);

                for (int k = 0; k < pt.Length - 1; k++)
                {
                    Rope2.Positions[k] = pt[k];
                }
            }

            if (Rope1 != null)
            {
                Rope1.Positions[^1] = Offset1;
                Rope1.Simulate(Vector2.Zero, Rope1Start, 1.5f, 1f);
            }

            if (Rope2 != null)
            {
                Rope2.Positions[^1] = Offset2;
                Rope2.Simulate(Vector2.Zero, Rope2Start, 1.5f, 1f);
            }

            if (player.Center.Distance(NPC.Center) < LookRange)
            {
                LookDir = (player.Center - NPC.Center).ToRotation();
                if (drawPos != NPC.Center + new Vector2(15, 0).RotatedBy(LookDir))
                {
                    drawPos = Vector2.Lerp(drawPos, NPC.Center + new Vector2(15, 0).RotatedBy(LookDir), 0.5f);
                }
                else
                {
                    drawPos = NPC.Center + new Vector2(15, 0).RotatedBy(LookDir);
                }
            }
            else
            {
                drawPos = Vector2.Lerp(drawPos, NPC.Center, 0.8f);
            }

            TextureRotationOffset -= 0.02f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            bool v = Main.bloodMoon;
            if (v)
            {
                return 0.15f;
            }
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {

        }
    }
}