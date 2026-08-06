using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using BreadLibrary.Core.Verlet;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Entities
{
    public class BloodGazer : ModNPC, IDrawPixelated
    {
        public override string Texture => DTUtils.NoTexture;




        public int Timer
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        public static Asset<Texture2D> Hand;
        public override void SetStaticDefaults()
        {
            Banner = Type;
            BannerItem = Mod.Find<ModItem>("Item_BloodGazerBanner").Type;
            Hand = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/GazerHand");
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
            NPC.HitSound = DTAssetLib.Impacts.StellarFox with { MaxInstances = 0, Pitch = -0.7f, PitchVariance = 0.2f };
            NPC.DeathSound = SoundID.Item74;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
            boundingBox.Inflate(2, 2);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(DTUtils.GetModNPCLocalizationEntry(this, 1)),
            });
        }



        public Vector2[] Offsets = new[]
        {
            new Vector2(-25, 0),
            new Vector2(25, 0)
        };

        public List<VerletChain> Ropes = null;
        private void RenderRope(Vector2 screenPos, Color drawColor, VerletChain Rope)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return;
            }

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



        public float TextureRotationOffset = 0f;
        public Vector2 LookDir = Vector2.Zero;
        public float LookRange = 300;


        public void UpdateHands()
        {
            for (int i = 0; i < Ropes.Count; i++)
            {
                Vector2 Root = NPC.Center + Offsets[i];
                Vector2 AdjustedVelocity = Vector2.UnitX * MathF.Sin(Main.GameUpdateCount * 0.05f + NPC.whoAmI + i * MathHelper.Pi);
                AdjustedVelocity *= 1-Math.Clamp(NPC.velocity.Length(), 0, 1);
                Ropes[i].Simulate(AdjustedVelocity, Root, 1f, 0.5f, collideWithTiles: false, collideWithPlayers: false);
                Ropes[i].Positions[0] = Root;
            }
        }


        public const int MaxArms = 2;
        public override bool PreAI()
        {
            if (Ropes is null)
            {
                Ropes = new List<VerletChain>(MaxArms);
                for (int i = 0; i < MaxArms; i++)
                {
                    Ropes.Add(new VerletChain(18, 2, NPC.Center + Offsets[i]));
                }
            }


            return base.PreAI();
        }
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            if(NPC.Distance(player.Center)<LookRange *2)
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + Vector2.UnitY * -60+ Vector2.UnitY* MathF.Sin(Timer*0.1f)*10)* NPC.velocity.Length(), 0.65f);



            if (player.Center.Distance(NPC.Center) < LookRange)
            {
                LookDir = NPC.DirectionTo(player.Center);

            }
            else
            {
                LookDir = Vector2.Lerp(LookDir, NPC.velocity, 0.2f);
            }
            LookDir = Vector2.Clamp(LookDir, new Vector2(0), new Vector2(5));
            TextureRotationOffset -= 0.02f;
            UpdateHands();
            Timer++;
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

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {

        }
        #region Drawcode
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawCrystalCore(spriteBatch, NPC.Center, Color.Black, Color.DarkRed, TextureRotationOffset, 1.5f);
            return true;
        }

        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center, Color colorIN, Color colorOUT, float TextureRotationOffset, float Scale = 1f)
        {
            DTUtils Utility = new DTUtils();
            float OuterScale = Scale * 0.12f;

            spriteBatch.UseBlendState(BlendState.NonPremultiplied);

            var tex = DTAssetLib.Cyclone(2).Value;
            var drawPos = Center - Main.screenPosition;
            Vector2 Origin = tex.Size() / 2;
            Main.spriteBatch.Draw(tex, drawPos, null, colorOUT, TextureRotationOffset,
                Origin,
                OuterScale,
                SpriteEffects.None,
                1f
            );

            tex = DTAssetLib.FeatheredCircle.Value;
            Main.spriteBatch.Draw(tex, drawPos, null, colorIN,
                0f,
                tex.Size() / 2f,
                Scale,
                SpriteEffects.None,
                1f
            );

            spriteBatch.ResetToDefault();
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            if (Ropes is not null)
                for (int i = 0; i < Ropes.Count; i++)
                {
                    RenderRope(screenPos, Color.Black, Ropes[i]);
                    float rot = Ropes[i].Positions[^1].AngleFrom(Ropes[i].Positions[Ropes[i].Positions.Length - 2]);

                    SpriteEffects flip = i % 2 != 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                    Main.EntitySpriteDraw(Hand.Value, Ropes[i].Positions[^1] - screenPos, null, Color.Black, rot, Hand.Value.Size() / 2, 1, flip);
                }

            Vector2 DrawPos = NPC.Center - screenPos;
            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, DrawPos + LookDir * 10, null, Color.Red, 0f, DTAssetLib.Star(3).Value.Size() / 2, new Vector2(0.8f, 1.2f), SpriteEffects.None, 0f);
        }
        #endregion
    }
}