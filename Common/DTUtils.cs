
using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Rarity.Scepter;
 
using InnoVault.TileProcessors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using ReLogic.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Modules;
using Terraria.UI.Chat;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static DestroyerTest.Common.Room;
using static System.Net.Mime.MediaTypeNames;

namespace DestroyerTest.Common
{
    public class DTFlags : ModSystem
    {
        public static DTFlags instance = ModContent.GetInstance<DTFlags>();

        public static bool PromiseEquipped = false;
        public static bool StellarGogglesEquipped = false;
        public static bool NodeCharmEquipped = false;
        public static bool ConsumeWyvernSoul = false;
        public static bool ConsumeRoseSoul = false;
        public static bool TenebrisCanSpawnInWorldEvilBiome = /*(DownedBossSystem.downedCultistBoss && !WorldGen.crimson)*/ false;
        public static bool TenebrisCanSpawnInShimmerBiome = /*(DownedBossSystem.downedCultistBoss && !WorldGen.crimson)*/ false;

        public override void ClearWorld()
        {
            PromiseEquipped = false;
            StellarGogglesEquipped = false;
            NodeCharmEquipped = false;
            ConsumeWyvernSoul = false;
            ConsumeRoseSoul = false;
            TenebrisCanSpawnInWorldEvilBiome = false;
            TenebrisCanSpawnInShimmerBiome = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag["TenebrisCanSpawnInWorldEvilBiome"] = (DownedBossSystem.downedCultistBoss && !WorldGen.crimson);
            tag["TenebrisCanSpawnInShimmerBiome"] = (DownedBossSystem.downedCultistBoss && !WorldGen.crimson);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("TenebrisCanSpawnInWorldEvilBiome"))
            {
                TenebrisCanSpawnInWorldEvilBiome = tag.GetBool("TenebrisCanSpawnInWorldEvilBiome");
            }

            if (tag.ContainsKey("TenebrisCanSpawnInShimmerBiome"))
            {
                TenebrisCanSpawnInShimmerBiome = tag.GetBool("TenebrisCanSpawnInShimmerBiome");
            }
        }
    }
    public class DTUtils : ModSystem
    {
        public static DTUtils instance = null;

        public override void Load()
        {
            GameShaders.Misc["DrawBarrier"] = new MiscShaderData(ModContent.Request<Effect>("DestroyerTest/Assets/Effects/RadialBarrier"), "DrawBarrier");
        }

        public override void PostSetupContent()
        {
            instance = ModContent.GetInstance<DTUtils>();
        }
        
        public static string GetModNPCLocalizationEntry(ModNPC npc, int variant = 1)
        {
            return Language.GetTextValue($"Mods.DestroyerTest.NPCs.{npc.Name}.BestiaryEntry{variant}");
        }

        public static string NoTexture = "DestroyerTest/Content/Extras/NoTexture";
        public static int[] TenebrisBuffImmunities;

        /// <summary>
        /// Contrary to what the name suggests, this code was first used in the Hollow Star code, and the name comes from this effect only being used for projectiles used by Constitution.
        /// </summary>
        /// <param name="projectile"></param>
        public static void ConstitutionStarExplosionEffects(Projectile projectile)
        {
            if (!DTOptimizationsConfig.instance.DisableExcessParticles)
            {
                List<Vector2> Star2 = Polar.GenerateCurvedStar(5, 4, 10, projectile.Center, inwardPull: 0.5f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                foreach (Vector2 p2 in Star2)
                {
                    Vector2 Vel = p2 - projectile.Center;

                    ConstitutionParticle Particle = new();
                    Particle.Initialize(projectile.Center, Vel, 1f, 30);
                    ParticleEngine.BehindProjectiles.Add(Particle);
                }
            }

            StellarParticleUtils.FlatStar(projectile.Center, 1f, ParticleEngine.BehindProjectiles);
        }

        //From Cal Entropy. Too good not to have due to its usefulness.
        public static void DrawChargeBar(float barScale, Vector2 position, float progress, Color color)
        {
            var barBG = DTAssetLib.Barback.Value;
            var barFG = DTAssetLib.Barfront.Value;

            Vector2 barOrigin = barBG.Size() * 0.5f;
            Vector2 drawPos = position;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(progress * barFG.Width), barFG.Height);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(barBG, drawPos, null, color, 0f, barOrigin, barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, barOrigin, barScale, 0f, 0f);
        }

        public static void DrawHallowChargeBar(float barScale, Vector2 position, float progress, float Opacity)
        {
            var barBG = DTAssetLib.HallowedBar.Back.Value;
            var barFG = DTAssetLib.HallowedBar.Front.Value;
            var barFrame = DTAssetLib.HallowedBar.Frame.Value;

            Vector2 barOrigin = barBG.Size() * 0.5f;
            Vector2 drawPos = position;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(progress * barFG.Width), barFG.Height);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(barBG, drawPos, null, Color.White * Opacity, 0f, barOrigin, barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, Color.White * Opacity, 0f, barOrigin, barScale, 0f, 0f);
            spriteBatch.Draw(barFrame, drawPos, null, Color.White * Opacity, 0f, barOrigin, barScale, 0f, 0f);
        }

        public static void AddStrips(List<ColoredVertex> List, List<Vector2> Vex, int Index, Vector2 off1, Vector2 off2, float Fade, Color CLR, float stripMotion = 0f)
        {
            List.Add(new ColoredVertex(Vex[Index] - Main.screenPosition + off1, new Vector3(Fade - stripMotion, 1, 1), CLR));
            List.Add(new ColoredVertex(Vex[Index] - Main.screenPosition + off2, new Vector3(Fade - stripMotion, 0, 1), CLR));
        }

        public static void AddStrips_ArenaWalls(
            List<ColoredVertex> list,
            List<Vector2> vex,
            int index,
            Vector2 off1,
            Vector2 off2,
            float u,
            Color clr)
        {
            list.Add(new ColoredVertex(
                vex[index] - Main.screenPosition + off1,
                new Vector3(u, 0, 1),
                clr));

            list.Add(new ColoredVertex(
                vex[index] - Main.screenPosition + off2,
                new Vector3(u, 1, 1),
                clr));
        }



        public static int GetScepterArmorSellPricePerRarity(int rarity)
        {
            switch (rarity)
            {
                case var _ when rarity == ModContent.RarityType<PearlRarity>():
                    return Item.sellPrice(0, 0, 4, 65);

                case var _ when rarity == ModContent.RarityType<PaleFuchsiaRarity>():
                    return Item.sellPrice(0, 2, 8, 65);

                case var _ when rarity == ModContent.RarityType<WineRarity>():
                    return Item.sellPrice(0, 4, 12, 85);

                case var _ when rarity == ModContent.RarityType<CerisePinkRarity>():
                    return Item.sellPrice(0, 12, 36, 85);

                case var _ when rarity == ModContent.RarityType<IncarnadineRarity>():
                    return Item.sellPrice(1, 24, 60, 85);

                default:
                    return -1;
            }
        }

        public static int[] NPCDownTally = new int[99999];

        public static void InfectedScepter_RingSpreadProjectileAlternating(int ID1, int ID2, int Amount, Vector2 CTR, float Radius, int Dmg = 0, int KB = 0, float Speed = 2, float AI0 = 0, float AI1 = 0, float AI2 = 0, bool RandomOffset = false)
		{
			float rotationStep = MathHelper.TwoPi / Amount;
			float baseRotation = RandomOffset ? Main.rand.NextFloat(MathHelper.TwoPi) : 0f;

			for (int i = 0; i < Amount; i++)
            {
                float angle = rotationStep * i + baseRotation;
                Vector2 position = CTR + new Vector2(Radius, 0f).RotatedBy(angle);
                Vector2 velocity = new Vector2(Speed, 0f).RotatedBy(angle);

                int projType = ((i & 1) == 0) ? ID1 : ID2;

                Projectile.NewProjectile(
                    Projectile.GetSource_None(),
                    position,
                    velocity,
                    projType,
                    Dmg,
                    KB,
                    ai0: AI0,
                    ai1: AI1,
                    ai2: AI2
                );
            }
		}

        public static List<int> RiftEnemies = new List<int>
        {
            ModContent.NPCType<PetrifiedWisp3>(),
            ModContent.NPCType<PetrifiedHead>(),  
            ModContent.NPCType<RiftDiggerHead>(),  
        };

        private int ScrollingTextureTexOffset = 0;
        /// <summary>
        /// Creates a scrolling texture, similar to a trail, but confined to two points.
        /// <br/> Must be called in a draw-related override.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="texture"></param>
        /// <param name="scrollspeed"></param>
        public void ScrollingTextureSpine(Line line, Asset<Texture2D> texture, Color drawColor, SpriteBatch spriteBatch,  BlendState blendState, int TexOffset, float Width = 1f)
        {

            if (texture == null)
            {
                Main.NewText("ScrollingTextureSpine: Texture is null. Aborted draw.", Color.Red);
                return;
            }

            spriteBatch.UseBlendState(blendState);
            var Cap = spriteBatch.Capture();
            Cap.SamplerState = SamplerState.LinearWrap;
            spriteBatch.End();
            spriteBatch.Begin(Cap);
            spriteBatch.Draw(texture.Value, line.Start - Main.screenPosition, new Rectangle(TexOffset, 0, (int)line.GetLineLength, texture.Value.Height), drawColor, line.GetLineRotation, new Vector2(0, texture.Value.Height) / 2, new Vector2(1, Width), SpriteEffects.None, 0);
            spriteBatch.ResetToDefault();
        }

        /// <summary>
        /// Creates a scrolling texture, similar to a trail, but confined to two points.
        /// <br/> Must be called in a draw-related override.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="texture"></param>
        /// <param name="scrollspeed"></param>
        public void ScrollingTextureSpine(Line line, Asset<Texture2D> texture, Color drawColor, SpriteBatch spriteBatch, BlendState blendState, int TexOffset, float Width = 1f, float Stretch = 1f)
        {

            if (texture == null)
            {
                Main.NewText("ScrollingTextureSpine: Texture is null. Aborted draw.", Color.Red);
                return;
            }

            spriteBatch.UseBlendState(blendState);
            var Cap = spriteBatch.Capture();
            Cap.SamplerState = SamplerState.LinearWrap;
            spriteBatch.End();
            spriteBatch.Begin(Cap);
            spriteBatch.Draw(texture.Value, line.Start - Main.screenPosition, new Rectangle(TexOffset, 0, (int)line.GetLineLength, texture.Value.Height), drawColor, line.GetLineRotation, new Vector2(0, texture.Value.Height) / 2, new Vector2(Stretch, Width), SpriteEffects.None, 0);
            spriteBatch.ResetToDefault();
        }

        public static void SweepColorOverString(string input, Color[] colors, Vector2 textPos, float speed = 6f)
        {
            if (string.IsNullOrEmpty(input) || colors == null || colors.Length == 0)
                return;

            float time = Main.GlobalTimeWrappedHourly * 6f; // speed control
            int offset = (int)time;

            TextSnippet[] snippets = new TextSnippet[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                int colorIndex = (i + offset) % colors.Length;

                snippets[i] = new TextSnippet(
                    input[i].ToString(),
                    colors[colorIndex]
                );
            }

            ChatManager.DrawColorCodedString(
                Main.spriteBatch,
                FontAssets.MouseText.Value,
                snippets,
                textPos,
                Color.White,
                0f,
                Vector2.Zero,
                Vector2.One,
                out _,
                float.MaxValue
            );

        }

        public static void GenericSparkleEffect(Vector2 Center)
        {
            SmallShine shine = new SmallShine();
            shine.Prepare(Center, Vector2.Zero, Color.White, 1f);
            ParticleEngine.BehindProjectiles.Add(shine);

            Vector2[] dir = Opus.RadialVectorOutwardRandom(6, Center, 1.5f);

            for (int i = 0; i < 6; i++)
            {
                PointGlowPreMultiplied Glow = new PointGlowPreMultiplied();
                Glow.Initialize(Center, dir[i], Color.White, 1f);
                ParticleEngine.BehindProjectiles.Add(Glow);
            }
        }

        public static DrawData CenteredDraw(Projectile projectile, Color color)
        {
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);
            return new DrawData(texture, projectile.Center - Main.screenPosition, frame, color * projectile.Opacity, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
        }


        /// <summary>
        /// A scale of 1 is equal to the size of the smallest variant of Petrified Wisp.
        /// The outer ring will always be 20% larger than the inner black circle.
        /// </summary>
        /// <param name="Center"></param>
        /// <param name="Speed"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="blendState"></param>
        /// <param name="Scale"></param>
        public static void DrawRiftBall(Vector2 Center, float Speed, SpriteBatch spriteBatch, BlendState blendState, List<Vector2> tail, float Scale = 1f)
        {
            if (tail == null || tail.Count < 2)
            {
                //Main.NewText("DrawRiftBall: Tail is null or too short. Aborted draw.", Color.Red);
                return;
            }


            Texture2D Ball = DTAssetLib.FeatheredCircle.Value;
            float Rot = 0f;
            Rot += 0.08f;
            float bottomscale = Scale * 1.2f;

            Opus.StartSpriteBatchWithBlending(spriteBatch, blendState, SpriteSortMode.Immediate);

            for (int i = 0; i < tail.Count; i++)
            {
                float progress = i / (float)(tail.Count - 1);
                float scale = MathHelper.Lerp(bottomscale, 0.0005f, progress);
                Color color = ColorLib.Rift;

                Main.EntitySpriteDraw(
                    DTAssetLib.FeatheredCircle.Value,
                    tail[i] - Main.screenPosition,
                    null,
                    color,
                    Rot,
                    Ball.Size() / 2f,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

            spriteBatch.Draw(Ball, Center - Main.screenPosition, null, ColorLib.Rift, Rot, Ball.Size() / 2, bottomscale, SpriteEffects.None, 1f);
            
            Opus.ReturnToDefaultDrawing(spriteBatch);

            for (int i = 0; i < tail.Count; i++)
			{
				float progress = i / (float)(tail.Count - 1);
				float scale = MathHelper.Lerp(Scale, 0.0001f, progress);
				Color color = Color.Black;

				Main.EntitySpriteDraw(
					DTAssetLib.FeatheredCircle.Value,
					tail[i] - Main.screenPosition,
					null,
					color,
					Rot,
					Ball.Size() / 2f,
					scale,
					SpriteEffects.None,
					0
				);
			}

            spriteBatch.Draw(Ball, Center - Main.screenPosition, null, Color.Black, Rot, Ball.Size() / 2, Scale, SpriteEffects.None, 0f);

            
        }

        /// <summary>
        /// Draws a ball with a trail composed of an upper layer and a lower layer.
        /// Unlike the rift ball, the scaling of this is a lot more fine tuned due to the differences in the sizes of the textures that compose it.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="Center"></param>
        /// <param name="colorIN"></param>
        /// <param name="colorOUT"></param>
        /// <param name="TrailPositions"></param>
        /// <param name="TextureRotationOffset"></param>
        /// <param name="Projectile"></param>
        /// <param name="TrailLength"></param>
        public static void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center, Color colorIN, Color colorOUT, float TextureRotationOffset, float Scale = 1f)
        {
            DTUtils Utility = new DTUtils();
            float OuterScale = Scale * 0.12f;

            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                colorOUT with { A = 0 },
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
                colorIN with { A = 0 },
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                Scale,
                SpriteEffects.None,
                1f
            );
        }

        /// <summary>
        /// Draws a ball with a trail composed of an upper layer and a lower layer.
        /// Unlike the rift ball, the scaling of this is a lot more fine tuned due to the differences in the sizes of the textures that compose it.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="Center"></param>
        /// <param name="colorIN"></param>
        /// <param name="colorOUT"></param>
        /// <param name="TrailPositions"></param>
        /// <param name="TextureRotationOffset"></param>
        /// <param name="Projectile"></param>
        /// <param name="TrailLength"></param>
        public static void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center, Color colorIN, Color colorOUT, List<Vector2> TrailPositions, float TextureRotationOffset, Projectile Projectile, int TrailLength, float Scale = 1f)
        {
            DTUtils Utility = new DTUtils();
            float OuterScale = Scale * 0.1425f;

            for (int i = 0; i < TrailPositions.Count; i++)
            {
                float progress = i / (float)TrailLength;
                float scale = MathHelper.Lerp(OuterScale, 0.0005f, progress);
                Color color = colorOUT;

                Main.EntitySpriteDraw(
                    DTAssetLib.Cyclone(2).Value,
                    TrailPositions[i] - Main.screenPosition,
                    null,
                    color with { A = 0 },
                    TextureRotationOffset,
                    DTAssetLib.Cyclone(2).Value.Size() / 2f,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }
            
            for (int i = 0; i < TrailPositions.Count; i++)
			{
				float progress = i / (float)TrailLength;
				float scale = MathHelper.Lerp(Scale, 0.001f, progress);
				Color color = colorIN;

				Main.EntitySpriteDraw(
					DTAssetLib.FeatheredCircle.Value,
					TrailPositions[i] - Main.screenPosition,
					null,
                    color with { A = 0 },
					Projectile.rotation,
					DTAssetLib.FeatheredCircle.Value.Size() / 2f,
					scale,
					SpriteEffects.None,
					0
				);
			}

            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                colorOUT with { A = 0 },
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
                colorIN with { A = 0 },
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                Scale,
                SpriteEffects.None,
                1f
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        public static List<int> RiftSurfaceEnemies = new List<int>
        {
            ModContent.NPCType<PetrifiedWisp1>(),
            ModContent.NPCType<RiftDiggerHead>(),
            ModContent.NPCType<RiftSlime>(),
            ModContent.NPCType<PetrifiedLurker>(),
            ModContent.NPCType<RiftOculus>(),
            ModContent.NPCType<RiftObserver>(),
        };

        public static bool ClassicMode()
        {
            if (Main.expertMode || Main.masterMode)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Dictionary<int, float> TooltipScaleMult = new();

        public static HashSet<int> isSpecialSwingSword = new();


        public static HashSet<int> isDevItem = new();

        public static HashSet<int> NeedsRework = new();

        public static HashSet<int> NoUpgradeStack = new();

        public static Dictionary<int, HashSet<int>> NoEquipWith = new Dictionary<int, HashSet<int>>();

        public static void IncompatibleWith(int itemType, int incompatibleType)
        {
            NoEquipWith.TryAdd(itemType, new HashSet<int>());
            NoEquipWith[itemType].Add(incompatibleType);
        }

        public static int RandomDirection(int Chance)
        {
            return Main.rand.NextBool(Chance) ? 1 : -1;
        }

        public static void PoofOfSmoke(Vector2 position)
        {
            int r = Main.rand.Next(3, 7);
            for (int i = 0; i < r; i++)
            {
                int num2 = Gore.NewGore(Projectile.GetSource_None(), position, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2() * new Vector2(2f, 0.7f) * 0.7f, Main.rand.Next(11, 14));
                Main.gore[num2].scale = 0.7f;
                Main.gore[num2].velocity *= 0.5f;
            }

            for (int j = 0; j < 10; j++)
            {
                Dust obj = Main.dust[Dust.NewDust(position, 14, 14, 16, 0f, 0f, 100, default(Color), 1.5f)];
                obj.position += new Vector2(5f);
                obj.velocity = (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2() * new Vector2(2f, 0.7f) * 0.7f * (0.5f + 0.5f * Main.rand.NextFloat());
            }
        }

        public static Vector2 Spiral(Vector2 center, float angle, float radiusPerRadian)
        {
            float radius = angle * radiusPerRadian;
            return center + angle.ToRotationVector2() * radius;
        }

        public static Vector2 ArchimedeanSpiral(
        Vector2 center,
        float angle,
        float startRadius,
        float spacing)
        {
            float radius = startRadius + spacing * angle;
            return center + angle.ToRotationVector2() * radius;
        }


        public static BezierCurve EasyBezier(Vector2 Start, Vector2 StartDir, Vector2 End, Vector2 EndDir, float CurveModifer = 0.3f, float InterpolationAmount = 0.5f)
        {
            float distance = Vector2.Distance(Start, End);
            float handle = distance * CurveModifer;

            Vector2 c0 = Start + StartDir * handle;
            Vector2 c2 = End - EndDir * handle;

            Vector2 c1 = Vector2.Lerp(c0, c2, InterpolationAmount);

            Vector2[] CurvePoints = new Vector2[]
            {
                Start,
                c0,
                c1,
                c2,
                End
            };

            return new BezierCurve(CurvePoints);
        }

    }

    public class SunlightModification : ModSystem
    {
        public static SunlightModification Instance = ModContent.GetInstance<SunlightModification>();

        public float _SunColorBrightness = 0f;
        public Color _SunlightColor = Color.White;
        public float percent = 0f;
        public float _percent = 0f;


        public static void Sunlight(float SunColorBrightness, Color SunColor, float Percent)
        {
            Instance._SunColorBrightness = SunColorBrightness;
            Instance._SunlightColor = SunColor;
            Instance._percent = Instance.percent = Percent;
        }

        public bool Pulsing = false;
        public int pulseTime = 0;
        public int pulseCounter = 0;
        public float SCB = 0;
        public static void Pulse(float SunColorBrightness, Color SunColor, float Percent, int Time = 120)
        {
            Instance.pulseCounter = 0;
            Instance.SCB = SunColorBrightness;
            Instance._SunlightColor = SunColor;
            Instance.pulseTime = Time;
            Instance._percent = Instance.percent = Percent;
            Instance.Pulsing = true;
        }

        public override void PostUpdateTime()
        {
            if (Pulsing)
            {
                pulseCounter++;

                float progress = MathHelper.Clamp(
                    (float)pulseCounter / pulseTime,
                    0f,
                    1f
                );

                percent = MathHelper.Lerp(_percent, 0f, progress);
                _SunColorBrightness = MathHelper.Lerp(SCB, 0f, progress);

                if (pulseCounter >= pulseTime)
                {
                    Pulsing = false;
                    pulseCounter = 0;
                    percent = 0f;
                }
            }
            else
            {
                pulseCounter = 0;
            }
        }

        public static void Reset()
        {
            Instance._SunColorBrightness = 0f;
            Instance._SunlightColor = Color.White;

            Instance.percent = 0f;
            Instance._percent = 0f;

            Instance.Pulsing = false;
            Instance.pulseCounter = 0;
            Instance.pulseTime = 0;
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            tileColor = tileColor.Darken(_SunColorBrightness);
            backgroundColor = backgroundColor.Darken(_SunColorBrightness);

            if (percent > 0)
            {
                tileColor = Tint(tileColor, _SunlightColor, percent);
                backgroundColor = Tint(backgroundColor, _SunlightColor, percent);
            }
        }

        private static Color Tint(Color original, Color tint, float strength = 0.5f)
        {
            Color multiplied = new Color(
                original.R * tint.R / 255,
                original.G * tint.G / 255,
                original.B * tint.B / 255,
                original.A
            );

            return Color.Lerp(original, multiplied, strength);
        }
    }
    public static class DTStaticUtils
    {
        public static void DefaultToFlask(this Item item, int BuffType, int Rarity, int Value)
		{
			item.UseSound = SoundID.Item3;
			item.useStyle = ItemUseStyleID.DrinkLiquid;
			item.useTurn = true;
			item.useAnimation = 17;
			item.useTime = 17;
			item.maxStack = Item.CommonMaxStack;
			item.consumable = true;
			item.buffType = BuffType;
			item.buffTime = Item.flaskTime;
			item.value = Value;
			item.rare = Rarity;
		}

        public static void DefaultToVial(this Item item, int BuffType, int Rarity, int Value)
		{
			item.UseSound = SoundID.Item3;
			item.useStyle = ItemUseStyleID.DrinkLiquid;
			item.useTurn = true;
			item.useAnimation = 17;
			item.useTime = 17;
			item.maxStack = Item.CommonMaxStack;
			item.consumable = true;
			item.buffType = BuffType;
			item.buffTime = Item.flaskTime;
			item.value = Value;
			item.rare = Rarity;
		}

        public static bool ArmorSetBonusKey(this Player player)
        {
            return DestroyerTestMod.ArmorSetBonusHotKey.JustPressed;
        }

        public static Vector2 Clamp(this Vector2 v, float maxLength)
        {
            float lenSq = v.LengthSquared();
            if (lenSq > maxLength * maxLength)
                return v * (maxLength / MathF.Sqrt(lenSq));

            return v;
        }

        public static Vector2 Clamp(this Vector2 v, float minLength, float maxLength)
        {
            float len = v.Length();
            if (len == 0f)
                return v;

            if (len < minLength)
                return v * (minLength / len);

            if (len > maxLength)
                return v * (maxLength / len);

            return v;
        }

        /// <summary>
        /// Automatically finds the set bonus localization for the given item.
        /// <br/> If none exists, the entry will be created.
        /// <br/> Should only be called in UpdateArmorSet.
        /// <br/> A little tip: player.armor[0] is the easiest thing to pass in the item parameter, since for UpdateArmorSet to run, the item has to be in your head slot anyway.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="item"></param>
        public static void DefaultSetBonusText(this Player player, Item item)
        {
            /*
            string itemInternal = item.Name;
            string Key = $"Mods.DestroyerTest.Items.{itemInternal}.SetBonus";
            
            player.setBonus = Language.GetTextValue(Key);
            */

            if (item.ModItem == null)
            {
                return;
            }

            var modItem = item.ModItem;
            string key = $"Mods.{modItem.Mod.Name}.Items.{modItem.Name}.SetBonus";
            player.setBonus = Language.GetTextValue(key);
        }

        // Helper Method from Fargo's Souls. Added to DTUtils for Blossom Beater's functionality, so that Fargo's is not needed for its ammo override to be disabled.
        public static Item FindAmmoDT(this Player player, int ammoID)
        {
            Item result = new Item();
            bool flag = false;
            if (ammoID == AmmoID.None)
            {
                return result;
            }

            for (int i = 54; i < 58; i++)
            {
                if (player.inventory[i].ammo == ammoID && player.inventory[i].stack > 0)
                {
                    return player.inventory[i];
                }
            }

            if (!flag)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo == ammoID && player.inventory[j].stack > 0)
                    {
                        return player.inventory[j];
                    }
                }
            }

            return result;
        }

        public static void CycleLine(this Line line, Color color, float scroll = 0f, int PointCount = 2, int DustType = DustID.Torch)
        {
            Vector2[] basePoints = line.GetPointsAlongLine(PointCount);
            int len = basePoints.Length;

            scroll += 0.05f;

            int baseIndex = (int)scroll % len;
            float t = scroll % 1f;

            for (int i = 0; i < len; i++)
            {
                int a = (baseIndex + i) % len;
                int b = (a + 1) % len;

                Vector2 pos = Vector2.Lerp(basePoints[a], basePoints[b], t);

                Dust T = Dust.NewDustPerfect(pos, DustType, Vector2.Zero, 0, color, 0.8f);
                T.noGravity = true;
            }
        }

        public static void SpecialColorInnerOuter(this DrawableTooltipLine line, Color strokeColor, Color textColor)
        { 
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Vector2 position = new Vector2(line.X, line.Y);

            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(0, 1.5f), strokeColor, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(1.5f, 1.5f), strokeColor, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(1.5f, 0), strokeColor, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(1.5f, -1.5f), strokeColor, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(0, -1.5f), strokeColor, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(-1.5f, -1.5f), strokeColor, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(0, -1.5f), strokeColor, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(1.5f, -1.5f), strokeColor, 0f, Vector2.Zero, Vector2.One);


            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);
        }

        public static string NoSpace(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace(" ", "");
        }

        public static Dust QuickDust(this Player player, int Type, Vector2 velocity, int alpha = 0)
        {
            return Dust.NewDustDirect(player.position, player.width, player.height, Type, velocity.X, velocity.Y, alpha, default, 1f);
        }

        public static Dust QuickDust(this Player player, int Type, Vector2 velocity, Color color, int alpha = 0)
        {
            return Dust.NewDustDirect(player.position, player.width, player.height, Type, velocity.X, velocity.Y, alpha, color, 1f);
        }

        public static Dust QuickDust(this Player player, int Type, Vector2 velocity, Color color, float Scale = 1f, int alpha = 0)
        {
            return Dust.NewDustDirect(player.position, player.width, player.height, Type, velocity.X, velocity.Y, alpha, color, Scale);
        }

        public static Asset<Texture2D> GetMasoTexture(this NPC npc, string Directory, string Name)
        {
            return ModContent.Request<Texture2D>($"{Directory}/Maso_{Name}", AssetRequestMode.AsyncLoad);
        }

        public static Asset<Texture2D> GetMasoGlowTexture(this NPC npc, string Directory, string Name)
        {
            return ModContent.Request<Texture2D>($"{Directory}/Maso_{Name}_Glow", AssetRequestMode.AsyncLoad);
        }

        public static Asset<Texture2D> GetGlowTexture(this Entity entity, string Directory, string Name)
        {
            return ModContent.Request<Texture2D>($"{Directory}/{Name}_Glow", AssetRequestMode.AsyncLoad);
        }

        public static void SetSpecialMeleeStats(this Item item)
        {
            item.useTime = 60;
            item.useAnimation = 60;
            item.useTurn = true;
        }

        /// <summary>
        /// Returns true if the timer controlling this projectile's homing is greater than or equal to the saftey window in which it cannot home in.
        /// </summary>
        /// <param name="ImmunityTime"></param>
        /// <param name="ImmunityTimer"></param>
        /// <returns></returns>
        public static bool HomingTimerCheck(this Projectile proj, int ImmunityTime, int ImmunityTimer)
        {
            return ImmunityTimer >= ImmunityTime;
        }

        public static bool ManualCanHitFriendly(this Projectile proj, NPC npc)
        {
            return !OpusNPCDropHelper.IgnoreEnemies.Contains(npc.type) && !npc.friendly && !npc.dontTakeDamage;
        }

        public static Vector2 ShoulderPosition(this Player player)
        {
            return player.MountedCenter + new Vector2(-8f * player.direction, 4f);
        }

        public static Vector2[] OldCenter(this Projectile projectile)
        {
            Vector2[] Positions = new Vector2[ProjectileID.Sets.TrailCacheLength[projectile.type]];

            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Positions[i] = projectile.oldPos[i] + new Vector2(projectile.width * 0.5f, projectile.height * 0.5f);
            }

            return Positions;
        }

        public static Vector2[] OldCenter(this NPC npc)
        {
            Vector2[] Positions = new Vector2[NPCID.Sets.TrailCacheLength[npc.type]];

            for (int i = 0; i < npc.oldPos.Length; i++)
            {
                Positions[i] = npc.oldPos[i] + new Vector2(npc.width * 0.5f, npc.height * 0.5f);
            }

            return Positions;
        }

        /// <summary>
        /// Draws a basic afterimage trail using oldPos.
        /// Call inside PreDraw().
        /// </summary>
        public static void DrawAfterimages(this Projectile projectile, SpriteBatch spriteBatch, Texture2D texture, Color color, float scaleMultiplier = 1f, bool useProjectileRotation = true, bool fadeOpacity = true, bool shrink = false, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Vector2 origin = texture.Size() * 0.5f;

            int cacheLength = projectile.oldPos.Length;

            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * projectile.frame,
                texture.Width,
                frameHeight
            );


            for (int i = 0; i < cacheLength; i++)
            {
                // 0 -> 1 progress through trail
                float progress = i / (float)cacheLength;

                // OldCenter extension assumed
                Vector2 drawPos = projectile.OldCenter()[i] - Main.screenPosition;

                // Fade out toward end of trail
                float opacity = fadeOpacity
                    ? (1f - progress)
                    : 1f;

                // Optional shrinking
                float scale = projectile.scale * scaleMultiplier;

                if (shrink)
                    scale *= (1f - progress);

                Color drawColor = color * opacity;

                spriteBatch.Draw(
                    texture,
                    drawPos,
                    frame,
                    drawColor,
                    useProjectileRotation ? projectile.oldRot[i] : projectile.rotation,
                    origin,
                    scale,
                    spriteEffects,
                    0f
                );
            }
        }

        /// <summary>
        /// Convenience overload using projectile texture automatically.
        /// </summary>
        public static void DrawAfterimages(this Projectile projectile, SpriteBatch spriteBatch, Color color, float scaleMultiplier = 1f, bool useProjectileRotation = true, bool fadeOpacity = true, bool shrink = false, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

            projectile.DrawAfterimages(
                spriteBatch,
                texture,
                color,
                scaleMultiplier,
                useProjectileRotation,
                fadeOpacity,
                shrink,
                spriteEffects
            );
        }

        public static void DrawAfterimagesWithRotOffset(this Projectile projectile, SpriteBatch spriteBatch, Color color, float scaleMultiplier = 1f, bool useProjectileRotation = true, float RotOffset = 0f, bool fadeOpacity = true, bool shrink = false, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

            Vector2 origin = texture.Size() * 0.5f;

            int cacheLength = projectile.oldPos.Length;

            for (int i = 0; i < cacheLength; i++)
            {
                // 0 -> 1 progress through trail
                float progress = i / (float)cacheLength;

                // OldCenter extension assumed
                Vector2 drawPos = projectile.OldCenter()[i] - Main.screenPosition;

                // Fade out toward end of trail
                float opacity = fadeOpacity
                    ? (1f - progress)
                    : 1f;

                // Optional shrinking
                float scale = projectile.scale * scaleMultiplier;

                if (shrink)
                    scale *= (1f - progress);

                Color drawColor = color * opacity;

                spriteBatch.Draw(
                    texture,
                    drawPos,
                    null,
                    drawColor,
                    useProjectileRotation ? projectile.oldRot[i] + RotOffset : projectile.rotation + RotOffset,
                    origin,
                    scale,
                    spriteEffects,
                    0f
                );
            }
        }

        public static void DrawDirectionalAfterimages(this Projectile projectile, SpriteBatch spriteBatch, Texture2D texture, Color color, SpriteEffects[] oldSpriteEffects, float[] oldRotationOffsets, float scaleMultiplier = 1f, bool fadeOpacity = true, bool shrink = false)
        {
            Vector2 origin = texture.Size() * 0.5f;

            int cacheLength = projectile.oldPos.Length;

            for (int i = 0; i < cacheLength; i++)
            {
                if (i >= oldSpriteEffects.Length ||
                    i >= oldRotationOffsets.Length)
                {
                    break;
                }

                float progress = i / (float)cacheLength;

                Vector2 drawPos =
                    projectile.OldCenter()[i] - Main.screenPosition;

                float opacity = fadeOpacity
                    ? (1f - progress)
                    : 1f;

                float scale = projectile.scale * scaleMultiplier;

                if (shrink)
                    scale *= (1f - progress);

                Color drawColor = color * opacity;

                spriteBatch.Draw(
                    texture,
                    drawPos,
                    null,
                    drawColor,

                    // Stored historical rotation
                    projectile.oldRot[i] + oldRotationOffsets[i] + (i == 0 ? -MathHelper.PiOver4 : 0f),

                    origin,
                    scale,

                    // Stored historical flip state
                    oldSpriteEffects[i],

                    0f
                );
            }
        }

        public static void ResetExcessTrailPoints(this Projectile projectile)
        {
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero)
                {
                    projectile.oldPos[i] = projectile.Center;
                }
            }
        }

        public static Shield GetActiveShield(this Player player, string Name)
        {
            return ShieldManager.ActiveShields[player.whoAmI].FirstOrDefault(x => x.InternalName == Name);
        }

        public static void SmoothMoveToPoint(this NPC npc, Vector2 targetPosition, float maxSpeed)
        {
            Vector2 offset = targetPosition - npc.Center;
            float distance = offset.Length();

            if (distance <= 0.001f)
            {
                npc.velocity = Vector2.Zero;
                return;
            }

            offset.Normalize();

            // Progress from 0 (at target) to 1 (far away)
            float progress = MathHelper.Clamp(distance / maxSpeed, 0f, 1f);

            float speed = MathHelper.SmoothStep(0f, maxSpeed, progress);

            npc.velocity = offset * speed;
        }

        public static void SmoothMoveToPoint(this Projectile projectile, Vector2 targetPosition, float maxSpeed)
        {
            Vector2 offset = targetPosition - projectile.Center;
            float distance = offset.Length();

            if (distance <= 0.001f)
            {
                projectile.velocity = Vector2.Zero;
                return;
            }

            offset.Normalize();

            // Progress from 0 (at target) to 1 (far away)
            float progress = MathHelper.Clamp(distance / maxSpeed, 0f, 1f);

            float speed = MathHelper.SmoothStep(0f, maxSpeed, progress);

            projectile.velocity = offset * speed;
        }

        public static ScepterClassStats ScepterClass(this Player player)
        {
            if (player.TryGetModPlayer<ScepterClassStats>(out var stats))
            {
                return stats;
            }
            else
            {
                return null;
            }
        }

        public static int AutoTarget(this Projectile projectile)
        {
            Player Owner = Main.player[projectile.owner];
            int chosen = -1;

            // #1 — Player whip target
            if (Owner.MinionAttackTargetNPC >= 0 &&
                Owner.MinionAttackTargetNPC < Main.maxNPCs)
            {
                NPC whipTarget = Main.npc[Owner.MinionAttackTargetNPC];
                if (whipTarget.CanBeChasedBy())
                {
                    chosen = Owner.MinionAttackTargetNPC;
                }
            }

            // #2 — Bosses (if no whip target)
            if (chosen == -1)
            {
                float bossDist = float.MaxValue;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy() && npc.boss)
                    {
                        float dist = Vector2.DistanceSquared(npc.Center, Owner.Center);
                        if (dist < bossDist)
                        {
                            bossDist = dist;
                            chosen = i;
                        }
                    }
                }
            }

            // #3 — Closest to player
            if (chosen == -1)
            {
                float closestDist = float.MaxValue;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float dist = Vector2.DistanceSquared(npc.Center, Owner.Center);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            chosen = i;
                        }
                    }
                }
            }

            return chosen;
        }

        /// <summary>
        /// 
        ///
        /// NOTE: This method can only be used if you know the radius, in pixels, or the ring in your texture.
        /// </summary>
        /// <param name="RadiusToCompare"> The ideal radius that the ring in the texture should be. </param>
        /// <param name="DistanceFromTextureCenter"> The original radius, in pixels, of the ring in the texture. </param>
        /// <returns></returns>
        public static float ScaleRingTextureToMatchRadius(this Texture2D Texture, float RadiusToCompare, int DistanceFromTextureCenter = 0)
        {
            return RadiusToCompare / (float)DistanceFromTextureCenter;
        }
    }

    public class DTPlayerUtil : ModPlayer
    {
        public override void ResetEffects()
        {
            DTFlags.NodeCharmEquipped = false;
        }

    }

    public static class DTColorUtils
    {
        /// <summary>
        /// Returns a copy of the color with a different alpha.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <param name="alpha">Alpha as a float from 0f–1f.</param>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.R, color.G, color.B, (byte)(MathHelper.Clamp(alpha, 0f, 1f) * 255));
        }

        /// <summary>
        /// Returns a copy of the color with a different alpha.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <param name="alpha">Alpha as a byte (0–255).</param>
        public static Color WithAlpha(this Color color, byte alpha)
        {
            return new Color(color.R, color.G, color.B, alpha);
        }

        /// <summary>
        /// Returns the input color that is tinted <i>percentage</i>% white, with 1 being fully white.
        /// </summary>
        /// <param name="inputColor"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static Color Pastel(this Color inputColor, float percentage)
        {
            percentage = MathHelper.Clamp(percentage, 0f, 1f);

            return new Color(
                (byte)MathHelper.Lerp(inputColor.R, 255, percentage),
                (byte)MathHelper.Lerp(inputColor.G, 255, percentage),
                (byte)MathHelper.Lerp(inputColor.B, 255, percentage),
                inputColor.A
            );
        }

        /// <summary>
        /// Returns the input color that is tinted <i>percentage</i>% black, with 1 being fully black.
        /// </summary>
        /// <param name="inputColor"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static Color Darken(this Color inputColor, float percentage)
        {
            percentage = MathHelper.Clamp(percentage, 0f, 1f);

            return new Color(
                (byte)MathHelper.Lerp(inputColor.R, 0, percentage),
                (byte)MathHelper.Lerp(inputColor.G, 0, percentage),
                (byte)MathHelper.Lerp(inputColor.B, 0, percentage),
                inputColor.A
            );
        }

        public static Color MultiLerp(float progress, params Color[] colors)
        {
            if (colors == null || colors.Length == 0)
                return Color.White;

            if (colors.Length == 1)
                return colors[0];

            progress = MathHelper.Clamp(progress, 0f, 1f);

            int segmentCount = colors.Length - 1;
            float scaled = progress * segmentCount;

            int index = (int)scaled;

            if (index >= segmentCount)
                return colors[^1];

            float localProgress = scaled - index;

            return Color.Lerp(
                colors[index],
                colors[index + 1],
                localProgress
            );
        }

        public static Color FromHex(string hex)
        {
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(hex);
            return new Color(color.R, color.G, color.B, color.A);
        }

        
    }

    public class DTUtilLoading : ModSystem
    {
        public override void Load()
        {
            DTUtils.TenebrisBuffImmunities = new int[]
            {
                ModContent.BuffType<ShimmeringFlames>(),
                ModContent.BuffType<HaepiensInferno>(),
                BuffID.OnFire,
                BuffID.OnFire3,
                BuffID.CursedInferno,
                BuffID.Frostburn,
                BuffID.Frostburn2,
                BuffID.Bleeding,
                BuffID.Dazed,
                BuffID.Electrified,
                BuffID.Frozen,
                BuffID.Oiled,
                BuffID.ShadowFlame,
                BuffID.Slimed,
                BuffID.SoulDrain
            };


        }
    }

    public class DTWorldUpdating : ModSystem
    {
        public override void PostUpdatePlayers()
        {
            DTFlags.StellarGogglesEquipped = false;
        }
    }

    public class ColorPalette
    {
        public string Name { get; }
        public Color[] Colors { get; }

        public ColorPalette(string name, params Color[] colors)
        {
            if (colors.Length != 5)
                throw new ArgumentException("A palette must contain exactly 5 colors.");

            Name = name;
            Colors = colors;
        }

        public Color GetColor(int index)
        {
            if (index < 0 || index >= Colors.Length)
                throw new IndexOutOfRangeException("Palette index out of range.");

            return Colors[index];
        }
    }






    /// <summary>
    /// A static class containing a library of colors used in the mod in order to avoid having to manually enter the RGB values for them when drawing.
    /// </summary>
    public static class ColorLib
    {
        /// <summary>
        /// The deepest color used in Living Shadows and other sprites using glow from Living Shadows.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color Electric = new Color(113, 251, 255);
        /// <summary>
        /// The deepest color used in Living Shadows and other sprites using glow from Living Shadows.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color DarkRift1 = new Color(51, 31, 0);
        /// <summary>
        /// The 2nd deepest color used in Living Shadows and other sprites using glow from Living Shadows.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color DarkRift2 = new Color(102, 61, 0);
        /// <summary>
        /// The 3rd deepest color used in Living Shadows and other sprites using glow from Living Shadows.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color DarkRift3 = new Color(153, 92, 0);
        /// <summary>
        /// The 4th deepest color used in Living Shadows and other sprites using glow from Living Shadows.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color DarkRift4 = new Color(204, 122, 0);
        /// <summary>
        /// The main color used in Living Shadows and other sprites using glow from Living Shadows. All other Rift Glow colors derive from this.
        /// </summary>
        public static Color Rift = new Color(255, 153, 0);
        /// <summary>
        /// The 4th brightest color used in Living Shadows and other sprites using glow from Living Shadows.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color LightRift1 = new Color(255, 173, 51);
        /// <summary>
        /// The 3rd brightest color used in Living Shadows and other sprites using glow from Living Shadows.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color LightRift2 = new Color(255, 194, 102);
        /// <summary>
        /// The 2nd brightest color used in Living Shadows and other sprites using glow from Living Shadows.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color LightRift3 = new Color(255, 214, 153);
        /// <summary>
        /// The brightest color used in Living Shadows and other sprites using glow from Living Shadows, aside from White.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color LightRift4 = new Color(255, 235, 204);

        /// <summary>
        /// The brightest possible color used in Living Shadows and other sprites using glow from Living Shadows. This is already an available color as White in the XNA framwwork, but ColorLib is a mirror of the entirety of every palette in the mod, so if a palette has white on it, it will end up here.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color RiftWhite = new Color(255, 255, 255);

        /// <summary>
        /// The standard beige in the tenebrous palette.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color TenebrisBeige = new Color(216, 185, 133);

        /// <summary>
        /// The standard magenta in the tenebrous palette.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color TenebrisMagenta = new Color(202, 40, 212);

        /// <summary>
        /// The standard blue in the tenebrous palette.
        /// <para/> ColorLib colors are numbered from darkest to lightest in a series.
        /// </summary>
        public static Color TenebrisBlue = new Color(87, 99, 186);

        /// <summary>
        /// An average color you will see in a cursed flame projectile.
        /// <para/> Note: This color is one of four and is the second lightest. 
        /// </summary>
        public static Color CursedFlames = new Color(179, 252, 0);

        /// <summary>
        /// An average color you will see in an Ichor projectile.
        /// <para/> Note: This color is one of five and is the third lightest. 
        /// </summary>
        public static Color Ichor = new Color(254, 202, 80);

        /// <summary>
        /// An All-Purpose Neon Gradient cycling through all the colors of the rainbow.
        /// </summary>
        public static Color RainbowGradient => new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));

        /// <summary>
        /// The main color used in Soul related things. All other Soul colors derive from this.
        /// </summary>
        public static Color Soul = new Color(255, 235, 113);

        /// <summary>
        /// The main color used in Soul related things. All other Soul colors derive from this.
        /// </summary>
        public static Color Soul2 = new Color(197, 142, 31);

        /// <summary>
        /// The main color used in Soul related things. All other Soul colors derive from this.
        /// </summary>
        public static Color Soul3 = new Color(154, 99, 27);

        /// <summary>
        /// Used for all things Hellfire!
        /// </summary>
        public static Color HellFire = new Color(254, 121, 2);

        public static Color SoulOfLightColor = new Color(220, 29, 183);
        public static Color SoulOfNightColor = new Color(123, 29, 120);

        public static Color PossessedScepterColor = new Color(60, 121, 164);

        /// <summary>
        /// The color used for drawing the aura and hit effects of the Metallurgy System Javelins.
        /// </summary>
        public static Color JavelinEnergy
        {
            get
            {
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                return Color.Lerp(Color.Gray, new Color(246, 192, 116), lerpAmount);
            }
        }

        public static Color StellarMagenta = new Color(143, 39, 120);
        public static Color StellarYellow = new Color(247, 233, 141);
        public static Color StellarColor
        {
            get
            {
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                return Color.Lerp(StellarYellow, StellarMagenta, lerpAmount);
            }
        }

        public static Color StellarRarityColor
        {
            get
            {
                float time = (Main.GlobalTimeWrappedHourly % 4f);

                if (time < 1f)
                    return Color.Lerp(Color.Black, StellarYellow, time);
                else if (time < 2f)
                    return Color.Lerp(StellarYellow, Color.Black, time - 1f);
                else if (time < 3f)
                    return Color.Lerp(Color.Black, StellarMagenta, time - 2f);
                else
                    return Color.Lerp(StellarMagenta, Color.Black, time - 3f);
            }
        }

        public static Color TenebrisGradient
        {
            get
            {
                float time = (Main.GlobalTimeWrappedHourly % 3f);

                if (time < 1f)
                    return Color.Lerp(TenebrisBeige, TenebrisMagenta, time);
                else if (time < 2f)
                    return Color.Lerp(TenebrisMagenta, TenebrisBlue, time - 1f);
                else
                    return Color.Lerp(TenebrisBlue, TenebrisBeige, time - 2f);
            }
        }

        public static Color Stardust = new Color(0, 174, 238);
        public static Color Vortex = new Color(0, 242, 170);
        public static Color Solar = new Color(254, 158, 35);
        public static Color Nebula = new Color(190, 30, 209);
        public static Color CelestialGradient
        {
            get
            {
                float time = (Main.GlobalTimeWrappedHourly % 4f);

                if (time < 1f)
                    return Color.Lerp(Stardust, Vortex, time);
                else if (time < 2f)
                    return Color.Lerp(Vortex, Solar, time - 1f);
                else if (time < 3f)
                    return Color.Lerp(Solar, Nebula, time - 2f);
                else
                    return Color.Lerp(Nebula, Stardust, time - 3f);
            }
        }

        public static Color IchorCrystal1 = new Color(129, 64, 0);
        public static Color IchorCrystal2 = new Color(169, 101, 0);
        public static Color IchorCrystal3 = new Color(197, 165, 13);
        public static Color IchorCrystal4 = new Color(255, 205, 90);

        public static Color[] IchorCrystalColorMap = new Color[4]
        {
            IchorCrystal1,
            IchorCrystal2,
            IchorCrystal3,
            IchorCrystal4
        };

        public static Color IchorCrystalGradient
        {
            get
            {
                float time = (Main.GlobalTimeWrappedHourly % 6f);

                if (time < 1f)
                    return Color.Lerp(IchorCrystal1, IchorCrystal2, time);
                else if (time < 2f)
                    return Color.Lerp(IchorCrystal2, IchorCrystal3, time - 1f);
                else if (time < 3f)
                    return Color.Lerp(IchorCrystal3, IchorCrystal4, time - 2f);
                else if (time < 4f)
                    return Color.Lerp(IchorCrystal4, IchorCrystal3, time - 3f);
                else if (time < 5f)
                    return Color.Lerp(IchorCrystal3, IchorCrystal2, time - 4f);
                else
                    return Color.Lerp(IchorCrystal2, IchorCrystal1, time - 5f);
            }
        }

        public static Color HoleCatFireBeige = new Color(241, 140, 72);
        public static Color HoleCatFireOrange = new Color(245, 102, 4);
        public static Color HoleCatFireRed = new Color(197, 9, 26);
        public static Color HoleCatFireMaroon = new Color(164, 0, 59);
        public static Color HoleCatFireDeepRed = new Color(106, 0, 0);

        public static Color[] HoleCatFireColormap = new Color[5]
        {
            HoleCatFireBeige,
            HoleCatFireOrange,
            HoleCatFireRed,
            HoleCatFireMaroon,
            HoleCatFireDeepRed
        };

        public static Color HoleCatFireGradient
        {
            get
            {
                float time = (Main.GlobalTimeWrappedHourly % 8f);

                if (time < 1f)
                    return Color.Lerp(HoleCatFireBeige, HoleCatFireOrange, time);
                else if (time < 2f)
                    return Color.Lerp(HoleCatFireOrange, HoleCatFireRed, time - 1f);
                else if (time < 3f)
                    return Color.Lerp(HoleCatFireRed, HoleCatFireMaroon, time - 2f);
                else if (time < 4f)
                    return Color.Lerp(HoleCatFireMaroon, HoleCatFireDeepRed, time - 3f);
                else if (time < 5f)
                    return Color.Lerp(HoleCatFireDeepRed, HoleCatFireMaroon, time - 4f);
                else if (time < 6f)
                    return Color.Lerp(HoleCatFireMaroon, HoleCatFireRed, time - 5f);
                else if (time < 7f)
                    return Color.Lerp(HoleCatFireRed, HoleCatFireOrange, time - 6f);
                else
                    return Color.Lerp(HoleCatFireOrange, HoleCatFireBeige, time - 7f);
            }
        }

        public static Color StellarFire1 = new Color(247, 233, 141);
        public static Color StellarFire2 = new Color(207, 120, 90);
        public static Color StellarFire3 = new Color(183, 61, 114);
        public static Color StellarFire4 = new Color(143, 39, 120);
        public static Color StellarFire5 = new Color(80, 38, 91);
        public static Color StellarFire6 = new Color(33, 36, 37);
        public static Color StellarFire7 = new Color(25, 33, 38);
        public static Color StellarFire8 = new Color(18, 23, 24);

        public static Color[] StellarFireColormap = new Color[9]
        {
            Color.White,
            StellarFire1,
            StellarFire2,
            StellarFire3,
            StellarFire4,
            StellarFire5,
            StellarFire6,
            StellarFire7,
            StellarFire8
        };

        public static Color StellarFireGradient(float t)
        {
            return DTColorUtils.MultiLerp(t, StellarFireColormap);
        }

        public static Color StellarFireGradientLooping()
        {
            float time = (Main.GlobalTimeWrappedHourly % 14f);

            if (time < 1f)
                return Color.Lerp(StellarFire1, StellarFire2, time);
            else if (time < 2f)
                return Color.Lerp(StellarFire2, StellarFire3, time - 1f);
            else if (time < 3f)
                return Color.Lerp(StellarFire3, StellarFire4, time - 2f);
            else if (time < 4f)
                return Color.Lerp(StellarFire4, StellarFire5, time - 3f);
            else if (time < 5f)
                return Color.Lerp(StellarFire5, StellarFire6, time - 4f);
            else if (time < 6f)
                return Color.Lerp(StellarFire6, StellarFire7, time - 5f);
            else if (time < 7f)
                return Color.Lerp(StellarFire7, StellarFire8, time - 6f);

            else if (time < 8f)
                return Color.Lerp(StellarFire8, StellarFire7, time - 7f);
            else if (time < 9f)
                return Color.Lerp(StellarFire7, StellarFire6, time - 8f);
            else if (time < 10f)
                return Color.Lerp(StellarFire6, StellarFire5, time - 9f);
            else if (time < 11f)
                return Color.Lerp(StellarFire5, StellarFire4, time - 10f);
            else if (time < 12f)
                return Color.Lerp(StellarFire4, StellarFire3, time - 11f);
            else if (time < 13f)
                return Color.Lerp(StellarFire3, StellarFire2, time - 12f);
            else
                return Color.Lerp(StellarFire2, StellarFire1, time - 13f);
        }


        private static Color SpiritFire1 = new Color(255, 245, 198);
        private static Color SpiritFire2 = new Color(244, 173, 255);
        private static Color SpiritFire3 = new Color(236, 107, 255);
        private static Color SpiritFire4 = new Color(184, 37, 253);
        private static Color SpiritFire5 = new Color(124, 0, 202);

        public static Color SpiritFireGradient(float t)
        {
            
            t = MathHelper.Clamp(t, 0f, 3f);

            if (t < 1f)
                return Color.Lerp(SpiritFire1, SpiritFire2, t);
            else if (t < 2f)
                return Color.Lerp(SpiritFire2, SpiritFire3, t - 1f);
            else if (t < 3f)
                return Color.Lerp(SpiritFire3, SpiritFire4, t - 2f);
            else
                return Color.Lerp(SpiritFire4, SpiritFire5, t - 3f);
        }

        public static Color Wretched1 = new Color(218, 253, 9);
        public static Color Wretched2 = new Color(179, 252, 0);
        public static Color Wretched3 = new Color(95, 248, 2);
        public static Color Wretched4 = new Color(55, 200, 26);
        public static Color Wretched5 = new Color(8, 129, 81);
        public static Color Wretched6 = new Color(3, 89, 96);
        public static Color Wretched7 = new Color(0, 0, 0);

        public static Color[] WretchedColorMap = new Color[7]
        {
            Wretched1,
            Wretched2,
            Wretched3,
            Wretched4,
            Wretched5,
            Wretched6,
            Wretched7
        };

        public static Color WretchedGradient()
        {
            float time = (Main.GlobalTimeWrappedHourly % 12f);

            if (time < 1f)
                return Color.Lerp(Wretched1, Wretched2, time);
            else if (time < 2f)
                return Color.Lerp(Wretched2, Wretched3, time - 1f);
            else if (time < 3f)
                return Color.Lerp(Wretched3, Wretched4, time - 2f);
            else if (time < 4f)
                return Color.Lerp(Wretched4, Wretched5, time - 3f);
            else if (time < 5f)
                return Color.Lerp(Wretched5, Wretched6, time - 4f);
            else if (time < 6f)
                return Color.Lerp(Wretched6, Wretched7, time - 5f);

            // turning point (no skip now)
            else if (time < 7f)
                return Color.Lerp(Wretched7, Wretched6, time - 6f);
            else if (time < 8f)
                return Color.Lerp(Wretched6, Wretched5, time - 7f);
            else if (time < 9f)
                return Color.Lerp(Wretched5, Wretched4, time - 8f);
            else if (time < 10f)
                return Color.Lerp(Wretched4, Wretched3, time - 9f);
            else if (time < 11f)
                return Color.Lerp(Wretched3, Wretched2, time - 10f);
            else
                return Color.Lerp(Wretched2, Wretched1, time - 11f);
        }

        public static Color InfectedGradient = Opus.Sine(ColorLib.CursedFlames, ColorLib.Ichor);

        public static Color LifeEcho = new Color(204, 243, 255);
    }

    


    public static class Polar
    {
        public static Vector2[] GenerateStar(int pointCount, int step, float radius, Vector2 center)
        {
            if (pointCount < 3)
                throw new ArgumentException("pointCount must be >= 3");

            if (step <= 0 || step >= pointCount)
                throw new ArgumentException("step must be between 1 and pointCount - 1");

            Vector2[] basePoints = new Vector2[pointCount];

            // Precompute the circle points
            for (int i = 0; i < pointCount; i++)
            {
                float angle = MathF.Tau * i / pointCount; // Tau = 2π
                basePoints[i] = center + new Vector2(
                    MathF.Cos(angle),
                    MathF.Sin(angle)
                ) * radius;
            }

            List<Vector2> starPath = new List<Vector2>();
            bool[] visited = new bool[pointCount];

            int current = 0;

            while (!visited[current])
            {
                visited[current] = true;
                starPath.Add(basePoints[current]);
                current = (current + step) % pointCount;
            }

            return starPath.ToArray();
        }

        public static List<Vector2> GenerateCurvedStar(int pointCount, int step, float radius, Vector2 center, int samplesPerEdge = 20, float inwardPull = 0.35f, bool randomOffset = false)
        {
            if (pointCount < 3)
            {
                throw new ArgumentException("pointCount must be >= 3");
            }

            if (step <= 0 || step >= pointCount)
            {
                throw new ArgumentException("step must be between 1 and pointCount - 1");
            }

            float[] angles = new float[pointCount];
            float globalOffset = randomOffset ? Main.rand.NextFloat(MathHelper.TwoPi) : 0f;

            for (int i = 0; i < pointCount; i++)
            {
                angles[i] = MathF.Tau * i / pointCount + globalOffset;
            }

            List<Vector2> points = new List<Vector2>();
            bool[] visited = new bool[pointCount];

            int current = 0;

            while (!visited[current])
            {
                visited[current] = true;

                int next = (current + step) % pointCount;

                float a0 = angles[current];
                float a1 = angles[next];

                float delta = MathHelper.WrapAngle(a1 - a0);

                for (int i = 0; i <= samplesPerEdge; i++)
                {
                    float t = i / (float)samplesPerEdge;

                    float bow = MathF.Sin(t * MathF.PI);

                    float angle = a0 + delta * t;
                    float r = radius * (1f - inwardPull * bow);

                    Vector2 pos = center + new Vector2(
                        MathF.Cos(angle),
                        MathF.Sin(angle)
                    ) * r;

                    points.Add(pos);
                }
                current = next;
            }

            return points;
        }

        public static List<Vector2> GenerateCurvedStar(int pointCount, int step, float radius, Vector2 center, int samplesPerEdge = 20, float inwardPull = 0.35f, float offset = 0)
        {
            if (pointCount < 3)
            {
                throw new ArgumentException("pointCount must be >= 3");
            }

            if (step <= 0 || step >= pointCount)
            {
                throw new ArgumentException("step must be between 1 and pointCount - 1");
            }

            float[] angles = new float[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                angles[i] = MathF.Tau * i / pointCount + offset;
            }

            List<Vector2> points = new List<Vector2>();
            bool[] visited = new bool[pointCount];

            int current = 0;

            while (!visited[current])
            {
                visited[current] = true;

                int next = (current + step) % pointCount;

                float a0 = angles[current];
                float a1 = angles[next];

                float delta = MathHelper.WrapAngle(a1 - a0);
                
                for (int i = 0; i <= samplesPerEdge; i++)
                {
                    float t = i / (float)samplesPerEdge;

                    float bow = MathF.Sin(t * MathF.PI);

                    float angle = a0 + delta * t;
                    float r = radius * (1f - inwardPull * bow);

                    Vector2 pos = center + new Vector2(
                        MathF.Cos(angle),
                        MathF.Sin(angle)
                    ) * r;

                    points.Add(pos);
                }
                current = next;
            }

            return points;
        }
    }

    public class DTTrail : ModSystem
    {
        public static void DrawTrail(SpriteBatch spriteBatch, Texture2D TrailTex, List<Vector2> Positions, List<float> Rotations, float Amplitude, Color color, float Scroll, float TaperRange = 0f)
        {
            DTOptimizationsConfig OptCfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (!OptCfg.DisableExcessTrails)
            {
                Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            

                if (Positions.Count > 1)
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    float a = 0;

                    for (int i = Positions.Count - 1; i > 0; i--)
                    {
                        float t = 1f - (i / (float)Positions.Count);
                        float taper = MathHelper.Lerp(0f, 1f, t);
                        float AdjAmplitude = Amplitude * taper;

                        Color b = color * t;


                        //Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
                        Vector2 curr = Positions[i];
                        Vector2 prev = Positions[i - 1];
                        Vector2 next = i < Positions.Count - 1 ? Positions[i + 1] : curr;

                        Vector2 dirPrev = curr - prev;
                        Vector2 dirNext = next - curr;

                        if (dirPrev != Vector2.Zero) dirPrev.Normalize();
                        if (dirNext != Vector2.Zero) dirNext.Normalize();

                        if (dirPrev == Vector2.Zero) dirPrev = dirNext;
                        if (dirNext == Vector2.Zero) dirNext = dirPrev;

                        Vector2 dir = dirPrev + dirNext;
                        if (dir != Vector2.Zero)
                            dir.Normalize();
                        else
                            dir = dirPrev;

                        Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * AdjAmplitude;
                        Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * AdjAmplitude;

                        DTUtils.AddStrips(ve, Positions, i, offset, offset2, t, b, Scroll);
                       
                    }


                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    if (ve.Count >= 3)
                    {
                        gd.Textures[0] = TrailTex;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }

                Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            }

        }

        public static void DrawTrail(SpriteBatch spriteBatch, BlendState blendState, Texture2D TrailTex, List<Vector2> Positions, List<float> Rotations, float Amplitude, Color color, float Scroll, float TaperRange = 0f)
        {
            DTOptimizationsConfig OptCfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (!OptCfg.DisableExcessTrails)
            {
                Opus.StartSpriteBatchForTrails(spriteBatch, blendState, SpriteSortMode.Immediate);

             

                if (Positions.Count > 1)
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    float a = 0;

                    for (int i = Positions.Count - 1; i > 0; i--)
                    {
                        float t = 1f - (i / (float)Positions.Count);
                        float taper = MathHelper.Lerp(1f, 0f, t);
                        float AdjAmplitude = Amplitude * 1;
                       
                        Color b = color * t;


                        //Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
                        Vector2 curr = Positions[i];
                        Vector2 prev = Positions[i - 1];
                        Vector2 next = i < Positions.Count - 1 ? Positions[i + 1] : curr;

                        Vector2 dirPrev = curr - prev;
                        Vector2 dirNext = next - curr;

                        if (dirPrev != Vector2.Zero) dirPrev.Normalize();
                        if (dirNext != Vector2.Zero) dirNext.Normalize();

                        if (dirPrev == Vector2.Zero) dirPrev = dirNext;
                        if (dirNext == Vector2.Zero) dirNext = dirPrev;

                        Vector2 dir = dirPrev + dirNext;
                        if (dir != Vector2.Zero)
                            dir.Normalize();
                        else
                            dir = dirPrev;

                        Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * AdjAmplitude;
                        Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * AdjAmplitude;

                        DTUtils.AddStrips(ve, Positions, i, offset, offset2, t, b, Scroll);

                    }


                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    if (ve.Count >= 3)
                    {
                        gd.Textures[0] = TrailTex;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }

                Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            }

        }




        public static void DrawTrailPixelated(SpriteBatch spriteBatch, BlendState blendState, Texture2D TrailTex, List<Vector2> Positions, List<float> Rotations, float Amplitude, Color color, float Scroll, float TaperRange = 20f)
        {
            DTOptimizationsConfig OptCfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (!OptCfg.DisableExcessTrails)
            {

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, PixelationSystem.PixelationMatrix);


                if (Positions.Count > 1)
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    float a = 0;

                    for (int i = Positions.Count - 1; i > 0; i--)
                    {
                        float t = 1f - (i / (float)Positions.Count);
                        float taper = MathHelper.Lerp(1f, 0f, t);
                        float AdjAmplitude = Amplitude * 1;

                        Color b = color * t;

                        //Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
                        Vector2 curr = Positions[i];
                        Vector2 prev = Positions[i - 1];
                        Vector2 next = i < Positions.Count - 1 ? Positions[i + 1] : curr;

                        Vector2 dirPrev = curr - prev;
                        Vector2 dirNext = next - curr;

                        if (dirPrev != Vector2.Zero) dirPrev.Normalize();
                        if (dirNext != Vector2.Zero) dirNext.Normalize();

                        if (dirPrev == Vector2.Zero) dirPrev = dirNext;
                        if (dirNext == Vector2.Zero) dirNext = dirPrev;

                        Vector2 dir = dirPrev + dirNext;
                        if (dir != Vector2.Zero)
                            dir.Normalize();
                        else
                            dir = dirPrev;

                        Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * AdjAmplitude;
                        Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * AdjAmplitude;

                        DTUtils.AddStrips(ve, Positions, i, offset, offset2, t, b, Scroll);

                    }


                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    if (ve.Count >= 3)
                    {
                        gd.Textures[0] = TrailTex;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }

                Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            }

        }
    }

    public class DTGenUtils
    {
        public static void GenRoom(Room room)
        {
            if (room == null)
            {
                throw new Exception("DTGenUtils.GenRoom: The Room to be generated is null.");
            }

            

            WorldUtils.Gen(room.Position, new GenShapeActionPair(new Shapes.Rectangle(room.Bounds.Width, room.Bounds.Height), new Actions.SetTile(room.TileType, false, true)));
            WorldUtils.Gen(room.Interior.Location, new GenShapeActionPair(new Shapes.Rectangle(room.Interior.Width, room.Interior.Height), new Actions.ClearTile(true)));

            WorldUtils.Gen(room.Position, new GenShapeActionPair(new Shapes.Rectangle(room.Bounds.Width, room.Bounds.Height), new Actions.PlaceWall(room.WallType)));
        }

        public static void GenHallway(Hallway hallway)
        {
            if (hallway == null)
            {
                throw new Exception("DTGenUtils.GenHallway: The Hallway to be generated is null.");
            }

            


            WorldUtils.Gen(hallway.Position, new GenShapeActionPair(new Shapes.Rectangle(hallway.Bounds.Width, hallway.Bounds.Height), new Actions.SetTile(hallway.TileType, false, true)));
            WorldUtils.Gen(hallway.Interior.Location, new GenShapeActionPair(new Shapes.Rectangle(hallway.Interior.Width, hallway.Interior.Height), new Actions.ClearTile(true)));

            WorldUtils.Gen(hallway.Position, new GenShapeActionPair(new Shapes.Rectangle(hallway.Bounds.Width, hallway.Bounds.Height), new Actions.PlaceWall(hallway.WallType)));
        }

        public static void GenChute(Chute chute)
        {
            if (chute == null)
            {
                throw new Exception("DTGenUtils.GenChute: The Chute to be generated is null.");
            }

            

            WorldUtils.Gen(chute.Position, new GenShapeActionPair(new Shapes.Rectangle(chute.Bounds.Width, chute.Bounds.Height), new Actions.SetTile(chute.TileType, false, true)));
            WorldUtils.Gen(chute.Interior.Location, new GenShapeActionPair(new Shapes.Rectangle(chute.Interior.Width, chute.Interior.Height), new Actions.ClearTile(true)));

            WorldUtils.Gen(chute.Position, new GenShapeActionPair(new Shapes.Rectangle(chute.Bounds.Width, chute.Bounds.Height), new Actions.PlaceWall(chute.WallType)));
        }

        /*

        public static void GenRoom(int i, int j, int Width, int Height, int WallWidth, int CeilingWidth, bool Wall, ushort TileType, ushort WallType)
        {
            //Tile Frames
            WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.SetTile(TileType, false, true)));
            WorldUtils.Gen(new Point(i + WallWidth, j + CeilingWidth), new GenShapeActionPair(new Shapes.Rectangle(Width - WallWidth * 2, Height - CeilingWidth * 2), new Actions.ClearTile(true)));

            if (Wall)
            {
                //Walls
                WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.PlaceWall(WallType)));
            }
        }

        public static void GenRoomWithDoors(int i, int j, int Width, int Height, int WallWidth, int CeilingWidth, bool Wall, ushort TileType, ushort WallType)
        {
            //To preface all this, you shoud know that i and j are the tile coordinates of the top left tile.
            //this is the case with points and vectors in terraria, but in case you didnt know.


            //Tile Frames
            
            //This Generates the base of the room. A block of tiles.
            WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.SetTile(TileType, false, true)));

            //This hollows out the tiles, leaving walls that are equal in thickness on all sides.
            WorldUtils.Gen(new Point(i + WallWidth, j + CeilingWidth), new GenShapeActionPair(new Shapes.Rectangle(Width - WallWidth * 2, Height - CeilingWidth * 2), new Actions.ClearTile(true)));

            //Clear Spots for doors

            //i with no offset is the furthest possible left it can get, so that works as the X for the first door.
            //But for the second door, the furthest we can go is the width - 1, since i + the full width is 1 tile to the right of the right edge of the structure.

            //as for j, adding the height puts us 1 block below the bottom side of the rectangle, out of bounds.
            //so, the offset we need to get the door anchorage is as follows
            //the ceiling width (it applies to the floor too), which puts us at the floor level of the room
            //and the three-tile height of the door we want to place.
            WorldUtils.Gen(new Point(i, (j + Height) - (3 + CeilingWidth)), new GenShapeActionPair(new Shapes.Rectangle(WallWidth, 3), new Actions.ClearTile(true)));
            WorldUtils.Gen(new Point(i + Width - WallWidth, (j + Height) - (3 + CeilingWidth)), new GenShapeActionPair(new Shapes.Rectangle(WallWidth, 3), new Actions.ClearTile(true)));

            //Place doors

            //once again, i with no offset is good for our first door x, and just like earlier, we want to use width - 1 to keep the left point in bounds.
            //and for j, its a little different. While we had to do a 3 tile offset to get the space for the door starting from the top left, the origin for a door tile is the bottom left.
            //as such, we only need to offset the floor level by 1 to put our door on top of the floor and slot it into place with the three tile height.

            //We also guard against breaking existing doors if the room generates twice.
            if (!Framing.GetTileSafely(i, (j + Height) - (CeilingWidth + 1)).HasTile)
            {
                WorldGen.PlaceObject(i, (j + Height) - (CeilingWidth + 1), TileID.ClosedDoor, true, 1);
            }

            if (!Framing.GetTileSafely(i + (Width - 1), (j + Height) - (CeilingWidth + 1)).HasTile)
            {
                WorldGen.PlaceObject(i + (Width - 1), (j + Height) - (CeilingWidth + 1), TileID.ClosedDoor, true, 1);
            }

            if (Wall)
            {
                //Walls
                WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.PlaceWall(WallType)));
            }
        }

        public static void GenLitRoomWithDoors(int i, int j, int Width, int Height, int WallWidth, int CeilingWidth, bool Wall, ushort TileType, ushort WallType)
        {
            //To get more in-depth explanations for the door setup, see GenRoomWithDoors

            //Tile Frames

            //This Generates the base of the room. A block of tiles.
            WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.SetTile(TileType, false, true)));

            //This hollows out the tiles, leaving walls that are equal in thickness on all sides.
            WorldUtils.Gen(new Point(i + WallWidth, j + CeilingWidth), new GenShapeActionPair(new Shapes.Rectangle(Width - WallWidth * 2, Height - CeilingWidth * 2), new Actions.ClearTile(true)));

            //Clear Spots for doors
            WorldUtils.Gen(new Point(i, (j + Height) - (3 + CeilingWidth)), new GenShapeActionPair(new Shapes.Rectangle(WallWidth, 3), new Actions.ClearTile(true)));
            WorldUtils.Gen(new Point(i + Width - WallWidth, (j + Height) - (3 + CeilingWidth)), new GenShapeActionPair(new Shapes.Rectangle(WallWidth, 3), new Actions.ClearTile(true)));

            //Place doors
            if (!Framing.GetTileSafely(i, (j + Height) - (CeilingWidth + 1)).HasTile)
            {
                WorldGen.PlaceObject(i, (j + Height) - (CeilingWidth + 1), TileID.ClosedDoor, true, 1);
            }

            if (!Framing.GetTileSafely(i + (Width - 1), (j + Height) - (CeilingWidth + 1)).HasTile)
            {
                WorldGen.PlaceObject(i + (Width - 1), (j + Height) - (CeilingWidth + 1), TileID.ClosedDoor, true, 1);
            }

            

            //Place torches
            //For a typical room we want to have the torches just above the doors.
            //Thus, we offset j by 4 and the Ceiling Widthh. 1 to get it in bounds, CeilingWidth to get it in the room, and 3 to get over the three-tile door.

            //This vector code here and dust box are for testing the torch position.

            //Vector2 AdjOrig = new Vector2(i * 16, j * 16); //Scale up to world coords
            //Vector2 TestPos = AdjOrig + new Vector2((WallWidth) * 16, (CeilingWidth + 2) * 16);
            //Dust.QuickBox(TestPos, new Vector2(TestPos.X + 16, TestPos.Y + 16), 3, Color.Red, null);

            WorldGen.PlaceObject(i + WallWidth, (j + Height) - (CeilingWidth + 4), TileID.Torches, true, 0);
            WorldGen.PlaceObject(i + Width - (WallWidth + 1), (j + Height) - (CeilingWidth + 4), TileID.Torches, true, 0);

            if (Wall)
            {
                //Walls
                WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.PlaceWall(WallType)));
            }
        }

        public static void GenLitRoomWithDoors(int i, int j, int Width, int Height, int WallWidth, int CeilingWidth, bool Wall, ushort TileType, ushort WallType, int TorchType = 0)
        {
            //To get more in-depth explanations for the door setup, see GenRoomWithDoors

            //Tile Frames

            //This Generates the base of the room. A block of tiles.
            WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.SetTile(TileType, false, true)));

            //This hollows out the tiles, leaving walls that are equal in thickness on all sides.
            WorldUtils.Gen(new Point(i + WallWidth, j + CeilingWidth), new GenShapeActionPair(new Shapes.Rectangle(Width - WallWidth * 2, Height - CeilingWidth * 2), new Actions.ClearTile(true)));

            //Clear Spots for doors
            WorldUtils.Gen(new Point(i, (j + Height) - (3 + CeilingWidth)), new GenShapeActionPair(new Shapes.Rectangle(WallWidth, 3), new Actions.ClearTile(true)));
            WorldUtils.Gen(new Point(i + Width - WallWidth, (j + Height) - (3 + CeilingWidth)), new GenShapeActionPair(new Shapes.Rectangle(WallWidth, 3), new Actions.ClearTile(true)));

            //Place doors
            if (!Framing.GetTileSafely(i, (j + Height) - (CeilingWidth + 1)).HasTile)
            {
                WorldGen.PlaceObject(i, (j + Height) - (CeilingWidth + 1), TileID.ClosedDoor, true, 1);
            }

            if (!Framing.GetTileSafely(i + (Width - 1), (j + Height) - (CeilingWidth + 1)).HasTile)
            {
                WorldGen.PlaceObject(i + (Width - 1), (j + Height) - (CeilingWidth + 1), TileID.ClosedDoor, true, 1);
            }



            //Place torches
            //For a typical room we want to have the torches just above the doors.
            //Thus, we offset j by 4 and the Ceiling Widthh. 1 to get it in bounds, CeilingWidth to get it in the room, and 3 to get over the three-tile door.

            //This vector code here and dust box are for testing the torch position.

            //Vector2 AdjOrig = new Vector2(i * 16, j * 16); //Scale up to world coords
            //Vector2 TestPos = AdjOrig + new Vector2((WallWidth) * 16, (CeilingWidth + 2) * 16);
            //Dust.QuickBox(TestPos, new Vector2(TestPos.X + 16, TestPos.Y + 16), 3, Color.Red, null);

            WorldGen.PlaceObject(i + WallWidth, (j + Height) - (CeilingWidth + 4), TileID.Torches, true, TorchType);
            WorldGen.PlaceObject(i + Width - (WallWidth + 1), (j + Height) - (CeilingWidth + 4), TileID.Torches, true, TorchType);

            if (Wall)
            {
                //Walls
                WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.PlaceWall(WallType)));
            }
        }

        public static void GenHallway(int i, int j, int Width, int Height, int CeilingWidth, bool Wall, ushort TileType, ushort WallType)
        {
            //There are some things to note about this method.
            //For one, you will notice that there is no wall width options. This is intentional, as the hallway should be spanned between two generated rooms.


            //This Generates the base of the room. A block of tiles.
            WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.SetTile(TileType, false, true)));

            //This hollows out the tiles, leaving walls that are equal in thickness on all sides.
            WorldUtils.Gen(new Point(i, j + CeilingWidth), new GenShapeActionPair(new Shapes.Rectangle(Width, Height - (CeilingWidth * 2)), new Actions.ClearTile(true)));

            WorldGen.PlaceObject(i, (j + Height) - (CeilingWidth + 4), TileID.Torches, true, 0);
            WorldGen.PlaceObject(i + Width - 1, (j + Height) - (CeilingWidth + 4), TileID.Torches, true, 0);

            if (Wall)
            {
                //Walls
                WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.PlaceWall(WallType)));
            }

        }

        public static void GenHallway(int i, int j, int Width, int Height, int CeilingWidth, bool Wall, ushort TileType, ushort WallType, int TorchType = 0)
        {
            //There are some things to note about this method.
            //For one, you will notice that there is no wall width options. This is intentional, as the hallway should be spanned between two generated rooms.


            //This Generates the base of the room. A block of tiles.
            WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.SetTile(TileType, false, true)));

            //This hollows out the tiles, leaving walls that are equal in thickness on all sides.
            WorldUtils.Gen(new Point(i, j + CeilingWidth), new GenShapeActionPair(new Shapes.Rectangle(Width, Height - (CeilingWidth * 2)), new Actions.ClearTile(true)));

            WorldGen.PlaceObject(i, (j + Height) - (CeilingWidth + 4), TileID.Torches, true, TorchType);
            WorldGen.PlaceObject(i + Width - 1, (j + Height) - (CeilingWidth + 4), TileID.Torches, true, TorchType);

            if (Wall)
            {
                //Walls
                WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.PlaceWall(WallType)));
            }

        }

        public static void GenChute(int i, int j, int Width, int Height, int WallWidth, bool Wall, bool Rope, ushort TileType, ushort WallType)
        {
            if (Width < 2 || Height < 2) return;
            if (Width % 2 == 0 && Rope)
            {
                throw new InvalidOperationException("Ropes cannot be generated if the width is an even number, since it can't be centered.");
            }
            //This Generates the base of the room. A block of tiles.
            WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.SetTile(TileType, false, true)));

            //This hollows out the tiles, leaving walls that are equal in thickness on all sides.
            WorldUtils.Gen(new Point(i + WallWidth, j), new GenShapeActionPair(new Shapes.Rectangle(Width - (WallWidth * 2), Height), new Actions.ClearTile(true)));

            if (Wall)
            {
                //Walls
                WorldUtils.Gen(new Point(i, j), new GenShapeActionPair(new Shapes.Rectangle(Width, Height), new Actions.PlaceWall(WallType)));
            }

            if (Rope)
            {
                WorldUtils.Gen(new Point(i + Width / 2, j), new GenShapeActionPair(new Shapes.Rectangle(1, Height), new Actions.PlaceTile(TileID.Rope)));
            }
        }

        */

        //Doors

        private static void CarveDoor(int x, int y, int direction)
        {
            // direction:
            // -1 = carve left
            //  1 = carve right

            int currentX = x;

            // Keep carving until we hit open air/interior
            while (WorldGen.SolidTile(currentX, y))
            {
                for (int i = 0; i < 3; i++)
                {
                    WorldGen.KillTile(currentX, y - i);
                }

                currentX += direction;
            }

            // Place the actual door at the original position
            WorldGen.PlaceObject(x, y, TileID.ClosedDoor, style: 13);
        }

        public static void MakeDoor(Room room, RoomSide side)
        {
            if (side == RoomSide.Left || side == RoomSide.Both)
            {
                int x = room.Bounds.Left;
                int y = room.Interior.Bottom - 1;

                CarveDoor(x, y, 1);

               


            }

            if (side == RoomSide.Right || side == RoomSide.Both)
            {
                int x = room.Bounds.Right - 1;
                int y = room.Interior.Bottom - 1;

                CarveDoor(x, y, -1);

              


            }
        }

        public static void MakeHatch(Room room, HatchSide side, int Width, int x)
        {
            int hatchWidth = Width;

            int endX = x + hatchWidth;

            int yTop = room.Bounds.Top;
            int yBottom = room.Bounds.Bottom - 1;

            for (int k = x; k <= endX; k++)
            {
                for (int i = 0; i < room.Ceiling; i++)
                {
                    int y = side switch
                    {
                        HatchSide.Top => yTop + i,
                        HatchSide.Bottom => yBottom - i,
                        _ => throw new Exception("Invalid HatchSide")
                    };

                    WorldGen.KillTile(k, y);
                }
            }
        }
    }

}