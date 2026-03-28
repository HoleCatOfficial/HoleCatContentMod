
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.fire;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Rarity.Scepter;
using InnoVault.PRT;
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
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Modules;
using Terraria.UI.Chat;

namespace DestroyerTest.Common
{
    public class DTUtils
    {
        public static DTUtils instance = new DTUtils();
        public static bool PromiseEquipped = false;
        public static bool StellarGogglesEquipped = false;
        public static bool NodeCharmEquipped = false;
        public static bool ConsumeWyvernSoul = false;
        public static bool ConsumeRoseSoul = false;
        /// <summary>
        /// An example of a Color Palette. It has a name and the required 5 colors.
        /// <para/> The first color is the blue on the tip of the hood,
        /// <para/> the second color is the shaded portion of the hood fabric,
        /// <para/> the third color is the ruby color,
        /// <para/> the fourth color is the gold color.
        /// </summary>
        public static readonly ColorPalette HoleCatColors = new("HoleCatColors", new Color(105, 161, 182), new Color(220, 200, 200), new Color(192, 67, 67), new Color(203, 179, 73), new Color(255, 255, 255));

        public int[] TenebrisBuffImmunities;
        public bool TenebrisCanSpawnInWorldEvilBiome = (DownedBossSystem.downedCultistBoss && !WorldGen.crimson);
        public bool TenebrisCanSpawnInShimmerBiome = (DownedBossSystem.downedCultistBoss && !WorldGen.crimson);

        public static string GetModNPCLocalizationEntry(ModNPC npc, int variant = 1)
        {
            return Language.GetTextValue($"Mods.DestroyerTest.NPCs.{npc.Name}.BestiaryEntry{variant}");
        }

        public static string NoTexture = "DestroyerTest/Content/Extras/NoTexture";
        

        /// <summary>
        /// Contrary to what the name suggests, this code was first used in the Hollow Star code, and the name comes from this effect only being used for projectiles used by Constitution.
        /// </summary>
        /// <param name="projectile"></param>
        public static void ConstitutionStarExplosionEffects(Projectile projectile)
        {
            List<Vector2> Star2 = Polar.GenerateCurvedStar(5, 4, 10, projectile.Center, inwardPull: 0.5f, randomOffset: true);
            foreach (Vector2 p2 in Star2)
            {
                Vector2 Vel = p2 - projectile.Center;
                PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, projectile.Center, Vel, (Color)default, 1f);
            }
            PRTLoader.NewParticle(StellarParticleIndex.FlatStar, projectile.Center, Vector2.Zero, (Color)default, 0.15f);
        }

        /// <summary>
        /// Draws a laser texture anchored at its center-bottom, with an internal AABBV line 
        /// from (Tex.Width/2, Tex.Height) to (Tex.Width/2, 0).
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with.</param>
        /// <param name="texture">The laser texture to draw.</param>
        /// <param name="center">The world-space center point of the laser.</param>
        /// <param name="color">The color to draw the laser with.</param>
        /// <param name="rotation">Rotation in radians.</param>
        /// <param name="scale">Scale factor (default 1f).</param>
        /// <param name="sourceRect">Optional source rectangle (null for full texture).</param>
        public static void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 center, Color color, float rotation = 0f, float scale = 1f, Rectangle? sourceRect = null)
        {
            if (texture == null)
                return;

            Rectangle source = sourceRect ?? texture.Bounds;

            // Anchor: bottom-center of the texture
            Vector2 origin = new Vector2(source.Width / 2f, source.Height);

            // Convert world-space to screen-space
            Vector2 screenPos = center - Main.screenPosition;

            // Draw the laser texture
            spriteBatch.Draw(
                texture,
                screenPos,
                source,
                color,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        /// <summary>
        /// Returns the AABBV line segment for the laser’s internal direction,
        /// based on its texture dimensions and optional scaling/rotation.
        /// </summary>
        public static (Vector2 Start, Vector2 End) GetLaserLine(Texture2D texture, Vector2 center, float rotation = 0f, float scale = 1f)
        {
            if (texture == null)
                return (center, center);

            // Local positions
            Vector2 localStart = new Vector2(texture.Width / 2f, texture.Height);
            Vector2 localEnd = new Vector2(texture.Width / 2f, 0);

            // Translate local line to world-space relative to the center
            Vector2 offset = localStart - new Vector2(texture.Width / 2f, texture.Height);
            localStart -= offset;
            localEnd -= offset;

            // Apply rotation and scale
            Matrix transform = Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(scale);
            localStart = Vector2.Transform(localStart, transform);
            localEnd = Vector2.Transform(localEnd, transform);

            Vector2 worldStart = center + localStart;
            Vector2 worldEnd = center + localEnd;

            return (worldStart, worldEnd);
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


        public static int[] ElectricArcs = new int[]
        {
            PRTLoader.GetParticleID<Arc1>(),
            PRTLoader.GetParticleID<Arc2>(),
            PRTLoader.GetParticleID<Arc3>()
        };

        public static int[] Fire =
        {
            PRTLoader.GetParticleID<Fire1>(),
            PRTLoader.GetParticleID<Fire2>(),
            PRTLoader.GetParticleID<Fire3>(),
            PRTLoader.GetParticleID<Fire4>(),
            PRTLoader.GetParticleID<Fire5>(),
            PRTLoader.GetParticleID<Fire6>(),
            PRTLoader.GetParticleID<Fire7>()
        };

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

        public static void InfectedScepter_RingProjectileOutwardAlternating(int ID1, int ID2, int Amount, Vector2 CTR, float Radius, int Dmg = 0, int KB = 0, float Speed = 2, float AI0 = 0, float AI1 = 0, float AI2 = 0, bool RandomOffset = false)
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


            Opus.StartSpriteBatchForTrails(spriteBatch, blendState, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture.Value, line.Start - Main.screenPosition, new Rectangle(TexOffset, 0, (int)line.GetLineLength, texture.Value.Height), drawColor, line.GetLineRotation, new Vector2(0, texture.Value.Height) / 2, new Vector2(1, Width), SpriteEffects.None, 0);

            Opus.ReturnToDefaultDrawing(spriteBatch);
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
            int Shine = PRTLoader.GetParticleID<SmallShine>();
            int Shimmer = PRTLoader.GetParticleID<SimpleParticle>();
            PRTLoader.NewParticle(Shine, Center, Vector2.Zero, Color.White, 1f);
            Opus.RadialParticleRandomDir(Shimmer, 6, Center, 1f, Color.White, 0.5f, 1.5f);
        }

        public static DrawData CenteredDraw(Projectile projectile, Color color)
        {
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            return new DrawData(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, texture.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
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
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

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
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            for (int i = 0; i < TrailPositions.Count; i++)
            {
                float progress = i / (float)TrailLength;
                float scale = MathHelper.Lerp(OuterScale, 0.0005f, progress);
                Color color = colorOUT;

                Main.EntitySpriteDraw(
                    DTAssetLib.Cyclone(2).Value,
                    TrailPositions[i] - Main.screenPosition,
                    null,
                    color,
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
					color,
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


        
    }

    public class SunlightModification : ModSystem
    {
        public static float _SunColorBrightness = 0f;

        public static void Sunlight(float SunColorBrightness)
        {
            _SunColorBrightness = SunColorBrightness;
        }

        public static void Reset()
        {
            _SunColorBrightness = 0f;
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            tileColor = tileColor.Darken(_SunColorBrightness);
            backgroundColor = backgroundColor.Darken(_SunColorBrightness);
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

        public static float Inverse(this float Input)
        {
            return 1f - Input;
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
    }

    public class DTPlayerUtil : ModPlayer
    {
        public override void ResetEffects()
        {
            DTUtils.NodeCharmEquipped = false;
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

        public static Asset<Texture2D> GetMasoTexture(this NPC npc, string Directory, string Name)
        {
            return ModContent.Request<Texture2D>($"{Directory}/Maso_{Name}");
        }

        public static Asset<Texture2D> GetMasoGlowTexture(this NPC npc, string Directory, string Name                                                  )
        {
            return ModContent.Request<Texture2D>($"{Directory}/Maso_{Name}_Glow");
        }
    }

    public class DTUtilLoading : ModSystem
    {
        DTUtils Utility = new DTUtils();
        public override void Load()
        {
            Utility.TenebrisBuffImmunities = new int[]
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
            DTUtils.StellarGogglesEquipped = false;
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

        public static Color CelestialGradient
        {
            get
            {
                float time = (Main.GlobalTimeWrappedHourly % 4f);

                if (time < 1f)
                    return Color.Lerp(new Color(0, 174, 238), new Color(0, 242, 170), time);
                else if (time < 2f)
                    return Color.Lerp(new Color(0, 242, 170), new Color(254, 158, 35), time - 1f);
                else if (time < 3f)
                    return Color.Lerp(new Color(254, 158, 35), new Color(190, 30, 209), time - 2f);
                else
                    return Color.Lerp(new Color(190, 30, 209), new Color(0, 174, 238), time - 3f);
            }
        }

        public static Color IchorCrystal1 = new Color(129, 64, 0);
        public static Color IchorCrystal2 = new Color(169, 101, 0);
        public static Color IchorCrystal3 = new Color(197, 165, 13);
        public static Color IchorCrystal4 = new Color(255, 205, 90);

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

        public static Color StellarFireGradient(float t)
        {
            
            t = MathHelper.Clamp(t, 0f, 8f);

            if (t < 1f)
                return Color.Lerp(StellarFire1, StellarFire2, t);
            else if (t < 2f)
                return Color.Lerp(StellarFire2, StellarFire3, t - 1f);
            else if (t < 3f)
                return Color.Lerp(StellarFire3, StellarFire4, t - 2f);
            else if (t < 4f)
                return Color.Lerp(StellarFire4, StellarFire5, t - 3f);
            else if (t < 5f)
                return Color.Lerp(StellarFire5, StellarFire6, t - 4f);
            else if (t < 6f)
                return Color.Lerp(StellarFire6, StellarFire7, t - 5f);
            else
                return Color.Lerp(StellarFire7, StellarFire8, t - 6f);
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
    }

    /// <summary>
    /// The central repository from which most drawn textures in the mod are sourced. If a texture appears more than once in the mod, it will likely have its place here.
    /// <para/> By sharing assets from AssetLib instead of loading them individually, draw calls can be optimised.
    /// </summary>
    public class DTAssetLib
    {
        public const string ParticlePath = "DestroyerTest/Content/Particles";
        public const string ExtrasPath = "DestroyerTest/Content/Extras";
        public const string AudioPath = "DestroyerTest/Assets/Audio";
        public const string EffectPath = "DestroyerTest/Assets/Effects";
        //
        //Practical, Every-Day VFX Textures
        //
        public static Asset<Texture2D> Square = TextureAssets.MagicPixel;
        public static Asset<Texture2D> PointGlow = ModContent.Request<Texture2D>($"{ParticlePath}/SimpleParticle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> AreaGlow = ModContent.Request<Texture2D>($"{ParticlePath}/Glow", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> BloomRing = ModContent.Request<Texture2D>($"{ParticlePath}/BloomRing", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> BloomRingSharp = ModContent.Request<Texture2D>($"{ParticlePath}/BloomRingSharp_FullScale", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FeatheredCircle = ModContent.Request<Texture2D>($"{ExtrasPath}/FeatheredCircle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Vingette = ModContent.Request<Texture2D>($"{ExtrasPath}/BigVingette", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FadeLine = ModContent.Request<Texture2D>($"{ExtrasPath}/FadeLine", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> StarAura = ModContent.Request<Texture2D>($"{ExtrasPath}/StarWrathAura", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ColorlessStar = ModContent.Request<Texture2D>($"{ExtrasPath}/ColorlessStar", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Swirl = ModContent.Request<Texture2D>($"{ParticlePath}/Swirl", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FireRing = ModContent.Request<Texture2D>($"{ParticlePath}/Boom2", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> SwingFX = ModContent.Request<Texture2D>($"{ExtrasPath}/CircularSlash", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Slash144 = ModContent.Request<Texture2D>($"{ExtrasPath}/144Slash", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ZapTrail = ModContent.Request<Texture2D>($"{ExtrasPath}/ZapTrail", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> SoulStreak = ModContent.Request<Texture2D>($"{ExtrasPath}/SoulStreak", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Barback = ModContent.Request<Texture2D>($"{ExtrasPath}/GenericBarBack", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Barfront = ModContent.Request<Texture2D>($"{ExtrasPath}/GenericBarFront", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> WyvernCorpseSky = ModContent.Request<Texture2D>($"{ExtrasPath}/WyvernCorpseSky", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> GlowCone = ModContent.Request<Texture2D>($"{ExtrasPath}/GlowCone", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> MiscSparkle144 = ModContent.Request<Texture2D>($"{ExtrasPath}/144MiscSparkle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> SwordSlash = ModContent.Request<Texture2D>($"{ExtrasPath}/SwordTrail2", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FireSwing = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FireSwingHighlight = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3Highlight", AssetRequestMode.AsyncLoad);

        public static Asset<Texture2D> Sparkle(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/Shine{Variant}", AssetRequestMode.AsyncLoad);
        }

        public static Asset<Texture2D> Streak(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ExtrasPath}/Streak{Variant}", AssetRequestMode.AsyncLoad);
        }

        public static Asset<Texture2D> SwordTrail(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ExtrasPath}/SwordTrail{Variant}", AssetRequestMode.AsyncLoad);
        }

        public static Asset<Texture2D> Star(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/Star{Variant}", AssetRequestMode.AsyncLoad);
        }

        public static Asset<Texture2D> Cyclone(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/Cyclone{Variant}", AssetRequestMode.AsyncLoad);
        }
        public static Asset<Texture2D> FlameTelegraph = ModContent.Request<Texture2D>($"{ParticlePath}/CursedFlamesTelegraph", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ArrowTelegraph = ModContent.Request<Texture2D>($"{ExtrasPath}/DashTelegraphArrow", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ArrowTelegraphCont = ModContent.Request<Texture2D>($"{ExtrasPath}/DashTelegraphArrowContinuous", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Warning = ModContent.Request<Texture2D>($"{ParticlePath}/WarningTriangle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Trail(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/Trail{Variant}", AssetRequestMode.AsyncLoad);
        }
        public static Asset<Texture2D> Line(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ExtrasPath}/Line{Variant}", AssetRequestMode.AsyncLoad);
        }
        public static Asset<Texture2D> TilableNoise(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ExtrasPath}/Noise{Variant}", AssetRequestMode.AsyncLoad);
        }
        //
        //Textures with more niche use cases.
        //
        public static Asset<Texture2D> RiftStar = ModContent.Request<Texture2D>($"{ParticlePath}/RiftStar", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> NightmareRoseArenaBorder = ModContent.Request<Texture2D>($"{ParticlePath}/NightmareRoseBarrier", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ConstitutionBeamGlow = ModContent.Request<Texture2D>($"{ExtrasPath}/ConstitutionBeamGlow", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> GalantineLanceGlow = ModContent.Request<Texture2D>($"{ExtrasPath}/GalantineLanceGlow", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> TenebrousConstructWingLeft = ModContent.Request<Texture2D>($"{ExtrasPath}/TenebrousConstructWingLeft", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> TenebrousConstructWingRight = ModContent.Request<Texture2D>($"{ExtrasPath}/TenebrousConstructWingRight", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> WyvernSoulDash = ModContent.Request<Texture2D>($"{ExtrasPath}/WyvernSoulDash", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> RuneCircle = ModContent.Request<Texture2D>($"{ParticlePath}/RuneCircle1", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> CorruptSigil = ModContent.Request<Texture2D>($"{ExtrasPath}/CorruptSigil", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> CrimsonSigil = ModContent.Request<Texture2D>($"{ExtrasPath}/CrimsonSigil", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> CrimsonBloodRune = ModContent.Request<Texture2D>($"{ExtrasPath}/CrimsonSigil", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> BloodHexHeart = ModContent.Request<Texture2D>($"{ExtrasPath}/BloodHexHeart", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> MobilityHexDoll = ModContent.Request<Texture2D>($"{ExtrasPath}/MobilityHexDoll", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> StarFuryOutline = ModContent.Request<Texture2D>($"{ExtrasPath}/StarfuryCloneOutline", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> NodeBossPikeOutline = ModContent.Request<Texture2D>($"{ExtrasPath}/NodeBossDistendedPikeOutline", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> PossessedToothOutline = ModContent.Request<Texture2D>($"{ExtrasPath}/PossessedToothOutline", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> HaepienCircleBottom = ModContent.Request<Texture2D>($"{ExtrasPath}/HaepienSigilBottom", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> HaepienCircleTop = ModContent.Request<Texture2D>($"{ExtrasPath}/HaepienSigilTop", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FlatStar = ModContent.Request<Texture2D>($"{ParticlePath}/FlatStar", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ShieldRing = ModContent.Request<Texture2D>($"{ParticlePath}/ShieldRing", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> QuixotismPowerAura = ModContent.Request<Texture2D>($"{ExtrasPath}/QuixotismPowerAura", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> CursorLanternTexture = ModContent.Request<Texture2D>($"{ExtrasPath}/CursorLantern", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ManifestStar = ModContent.Request<Texture2D>($"{ExtrasPath}/ManifestHoldoutStar", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> BlossomBeaterRope = ModContent.Request<Texture2D>($"{ExtrasPath}/BlossomBeaterRope", AssetRequestMode.AsyncLoad);

        public struct HallowedBar
        {
            public static Asset<Texture2D> Back = ModContent.Request<Texture2D>($"{ExtrasPath}/HallowedBarBack", AssetRequestMode.AsyncLoad);
            public static Asset<Texture2D> Front = ModContent.Request<Texture2D>($"{ExtrasPath}/HallowedBarFront", AssetRequestMode.AsyncLoad);
            public static Asset<Texture2D> Frame = ModContent.Request<Texture2D>($"{ExtrasPath}/HallowedBarFrame", AssetRequestMode.AsyncLoad);
        }
        public static Asset<Texture2D> MiniRoseFragment(int Variant)
        {
            return ModContent.Request<Texture2D>($"{ExtrasPath}/MiniRoseFragment{Variant}", AssetRequestMode.AsyncLoad);
        }
        //
        // Sounds
        //
        public static SoundStyle ChargeBreak = new SoundStyle($"{AudioPath}/ChargeBreak");
        public static SoundStyle CrystalBreak = new SoundStyle($"{AudioPath}/CrystalBreak");
        public static SoundStyle FlailSpin = new SoundStyle($"{AudioPath}/FlailSpin");
        public static SoundStyle FlailThrow = new SoundStyle($"{AudioPath}/FlailThrow");
        public static SoundStyle ConstitutionStarKill = new SoundStyle($"{AudioPath}/ConstitutionBoss/ConstitutionStar/Kill", 14) { PitchVariance = 0.2f, Volume = 0.85f, MaxInstances = 0 };
        public static SoundStyle EnergyWoosh = new SoundStyle($"{AudioPath}/EnergyWoosh", 3);
        public static SoundStyle RiftExplosion = new SoundStyle($"{AudioPath}/RiftMaker_Boom");
        public static SoundStyle Zap = new SoundStyle($"{AudioPath}/Zap", 3);
        public struct ScholarShieldSounds
        {
            public static string Path = $"{AudioPath}/Scholar";
            public static SoundStyle Hit = new SoundStyle($"{Path}/ShieldHit", 3);
            public static SoundStyle Activate = new SoundStyle($"{Path}/ShieldActivate", 3);
            public static SoundStyle Break = new SoundStyle($"{Path}/ShieldBreak");
        }
        public struct Impacts
        {
            public static string Path = $"{AudioPath}/Impacts";
            public static SoundStyle AmbitionChargeBurst = new SoundStyle($"{Path}/AmbitionChargeBurst", 5);
            public static SoundStyle BrightBell = new SoundStyle($"{Path}/BrightBell");
            public static SoundStyle DarkMagicImpact = new SoundStyle($"{Path}/DarkMagicImpact", 3);
            public static SoundStyle DarkShot = new SoundStyle($"{Path}/DarkShot", 3);
            public static SoundStyle DreamHit = new SoundStyle($"{Path}/DreamHit", 4);
            public static SoundStyle EnergyBounce = new SoundStyle($"{Path}/EnergyBounce", 3);
            public static SoundStyle ExplosiveImpactSmall = new SoundStyle($"{Path}/ExplosiveImpactSmall");
            public static SoundStyle ExplosiveImpactBig = new SoundStyle($"{Path}/ExplosiveImpactBig");
            public static SoundStyle FlameImpact = new SoundStyle($"{Path}/FlameImpact", 4);
            public static SoundStyle FleshHit = new SoundStyle($"{Path}/FleshHit", 5);
            public static SoundStyle HellWeaponImpact = new SoundStyle($"{Path}/HellWeaponImpact");
            public static SoundStyle IceImpact = new SoundStyle($"{Path}/IceImpact", 3);
            public static SoundStyle IceMagicImpact = new SoundStyle($"{Path}/IceMagicImpact", 3);
            public static SoundStyle Malevolence = new SoundStyle($"{Path}/MalevolenceHit");
            public static SoundStyle MagicBeep = new SoundStyle($"{Path}/MagicBeep", 3);
            public static SoundStyle MetalImpact = new SoundStyle($"{Path}/MetalImpact", 3);
            public static SoundStyle ShortShine = new SoundStyle($"{Path}/ShortShine", 3);
            public static SoundStyle StellarFox = new SoundStyle($"{Path}/StellarFoxImpact", 5);
            public static SoundStyle SpiritOfJusticeParry = new SoundStyle($"{Path}/SpiritOfJusticeParry");
            public static SoundStyle Void = new SoundStyle($"{Path}/VoidImpact", 3);
            
        }

        public struct SwordSounds
        {
            public static string Path = $"{AudioPath}/SwordSounds";
            public static SoundStyle BigBasicSwing = new SoundStyle($"{Path}/BigBasicSwing", 3);
            public static SoundStyle ColdSword = new SoundStyle($"{Path}/ColdSword", 3);
            public static SoundStyle Woosh = new SoundStyle($"{Path}/DefaultWoosh");
            public static SoundStyle EvilSwing = new SoundStyle($"{Path}/EvilSwing", 3);
            public static SoundStyle HeavySwing = new SoundStyle($"{Path}/HeavySwing", 3);
            public static SoundStyle HellSword = new SoundStyle($"{Path}/HellSword", 3);
            public static SoundStyle MagicSwing = new SoundStyle($"{Path}/MagicSwing", 3);
            public static SoundStyle MediumSwing = new SoundStyle($"{Path}/MediumSwing", 3);
            public static SoundStyle MediumHeavySwing = new SoundStyle($"{Path}/MediumHeavySwing", 3);
            public static SoundStyle MemoriamSwing = new SoundStyle($"{Path}/MemoriamSwing");
            public static SoundStyle MetalSwing = new SoundStyle($"{Path}/MetalSwing", 4);
            public static SoundStyle QuickSwing = new SoundStyle($"{Path}/QuickSwing", 4);
            public static SoundStyle SwiftSwing = new SoundStyle($"{Path}/SwiftSwing1");
            public static SoundStyle Slam = new SoundStyle($"{Path}/Slam", 2);
            public static SoundStyle TenebrisSwing = new SoundStyle($"{Path}/TenebrisSwing", 3);
            public static SoundStyle StandardSwing = new SoundStyle($"{Path}/StandardSwing");
            public static SoundStyle SpiritOfJusticeSwing = new SoundStyle($"{Path}/SpiritOfJusticeSwing");
        }

        public static SoundStyle IdriGreatswordSlice(bool Gore)
        {
            if (Gore)
            {
                return new SoundStyle($"{SwordSounds.Path}/IdriGreatswordGoreSlice", 2);
            }
            else
            {
                return new SoundStyle($"{SwordSounds.Path}/TenebrisSwing", 3);
            }
        } 

        public struct Djinn
        {
            public static string Path = $"{AudioPath}/Djinn";
            public static SoundStyle Laugh = new SoundStyle($"{Path}Laugh");
            public static SoundStyle Hit = new SoundStyle($"{Path}Hit");
            public static SoundStyle Kill = new SoundStyle($"{Path}Kill");
        }

        public struct Charge
        {
            public static string Path = $"{AudioPath}/Charge";
            public static SoundStyle RiftFlailTick = new SoundStyle($"{Path}/RiftFlailTick");
            public static SoundStyle RiftFlailBurst = new SoundStyle($"{Path}/RiftFlailBurst");
            public static SoundStyle Anvil = new SoundStyle($"{Path}/Anvil");
            public static SoundStyle FlatTick = new SoundStyle($"{Path}/FlatTick");
            public static SoundStyle MetalTinkLight = new SoundStyle($"{Path}/MetalTinkLight", 3);
            public static SoundStyle WoodyTick = new SoundStyle($"{Path}/WoodyTick", 6);

            public static SoundStyle Quixotism = new SoundStyle($"{Path}/QuixotismCharge");
        }

        public struct StellarBow
        {
            public static string Path = $"{AudioPath}/StellarBow";
            public static SoundStyle ArrowImpact = new SoundStyle($"{Path}/StellarBowArrowImpact", 4);
            public static SoundStyle Shoot = new SoundStyle($"{Path}/StellarBowShoot", 3);
            public static SoundStyle EmpoweredShoot = new SoundStyle($"{Path}/StellarBowEmpoweredShoot", 3);
        }
    }

    public class AssetVerifierSystem : ModSystem
    {
        public override void OnModLoad()
        {


            var fields = typeof(DTAssetLib).GetFields(
                BindingFlags.Public | BindingFlags.Static
            );

            foreach (var field in fields)
            {
                if (field.FieldType.IsGenericType &&
                    field.FieldType.GetGenericTypeDefinition() == typeof(Asset<>))
                {
                    try
                    {
                        var value = field.GetValue(null);
                        if (value is Asset<Texture2D> tex)
                        {
                            _ = tex.Value; // force load
                        }
                    }
                    catch (Exception ex)
                    {
                        /*
                        string filePath = @"DestroyerTest/Assets/Music/DTAssetLibError.wav";

                        #if WINDOWS
                                                using (SoundPlayer player = new SoundPlayer(filePath))
                                                {
                                                    player.Play();
                                                    Console.WriteLine("Do not panic. This sound was triggered by a mod. Not by a virus.");
                                                    Console.ReadLine();
                                                }
                        #else
                                                Console.WriteLine("Asset verification failed and sound notification is only available on Windows.");
                        #endif
                        */
                        throw new Exception(
                            $"Asset verification failed for {field.Name} in DTAssetLib. Path may be invalid.",
                            ex
                        );
                    }
                }
            }

            TestMethodAssets();
        }

        private void TestSoundAssets()
        {

        }

        private void TestMethodAssets()
        {
            _ = DTAssetLib.BloomRing.Value;
            _ = DTAssetLib.Sparkle(1).Value;
            _ = DTAssetLib.Star(1).Value;
            _ = DTAssetLib.Cyclone(1).Value;
            _ = DTAssetLib.Trail(1).Value;
            _ = DTAssetLib.Line(1).Value;
            _ = DTAssetLib.TilableNoise(1).Value;
        }
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
        public static void DrawTrail(SpriteBatch spriteBatch, Texture2D TrailTex, List<Vector2> Positions, List<float> Rotations, float Amplitude, Color color, float Scroll, float TaperRange = 20f)
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
                        float taper = MathHelper.Clamp(i / TaperRange, 0f, 1f);

                        // optional smoothing (feels nicer than linear)
                        taper = taper * taper; // quadratic ease-in

                        float AdjAmplitude = Amplitude * taper;
                        float t = 1f - (i / (float)Positions.Count); // fade toward tail
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
            }

        }
    }

}