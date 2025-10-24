
using System;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using rail;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
        public bool TenebrisCanSpawnInWorldEvilBiome = DownedBossSystem.downedCultistBoss;
        public bool TenebrisCanSpawnInShimmerBiome = DownedBossSystem.downedCultistBoss;
        public void DustsWhileItemIsInWorld(Rectangle itemRect, int DustType = -1, int ChancePerTick = 3, float DustScale = 1f, float DustVelX = 0f, float DustVelY = 0f, Color DustColor = default)
        {
            if (DustType == -1)
                DustType = DustID.TintableDustLighted;


            if (itemRect.Width <= 0 || itemRect.Height <= 0)
                return;

            if (Main.rand.NextBool(ChancePerTick))
            {
                Dust.NewDust(new Vector2(itemRect.Width / 2, itemRect.Height / 2), itemRect.Width, itemRect.Height, DustType, 0f, 0f, 100, DustColor, DustScale);
            }
        }

        public void RadialSpreadProjectile(int ID, int Amount, Vector2 CTR, int Dmg = 0, int KB = 0, int Speed = 2, float AI0 = 0, float AI1 = 0, float AI2 = 0)
        {
            float rotationStep = MathHelper.TwoPi / Amount;

            for (int i = 0; i < Amount; i++)
            {
                Vector2 velocity = new Vector2(Speed, 0f).RotatedBy(rotationStep * i);
                Projectile.NewProjectile(
                    Projectile.GetSource_None(),
                    CTR,
                    velocity,
                    ID,
                    Dmg,
                    KB,
                    ai0: AI0,
                    ai1: AI1,
                    ai2: AI2
                );
            }
        }

        public void RadialProjectileRandomDir(int ID, int Amount, Vector2 CTR, int Dmg = 0, int KB = 0, float Speed = 2f, float AI0 = 0, float AI1 = 0, float AI2 = 0)
        {
            for (int i = 0; i < Amount; i++)
            {
                Vector2 velocity = new Vector2(Speed, 0f).RotatedByRandom(MathHelper.TwoPi);
                Projectile.NewProjectile(
                    Projectile.GetSource_None(),
                    CTR,
                    velocity,
                    ID,
                    Dmg,
                    KB,
                    ai0: AI0,
                    ai1: AI1,
                    ai2: AI2
                );
            }
        }


        /// <summary>
        /// Easy-to-call method for drawing a point glow over the center of a projectile.
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="color"></param>
        /// <param name="RotateWithProj"></param>
        /// <param name="Rot"></param>
        public void DrawGlowOnProj(Projectile projectile, Color color, bool RotateWithProj, float Rot = 0)
        {
            if (RotateWithProj)
            {
                Rot = projectile.rotation;
            }

            Main.EntitySpriteDraw(
                DTAssetLib.PointGlow.Value,
                projectile.Center - Main.screenPosition,
                null,
                color,
                Rot,
                DTAssetLib.PointGlow.Value.Size() / 2,
                projectile.scale,
                SpriteEffects.None,
                0
            );
        }

        /// <summary>
        /// Easy-to-call method for drawing any texture over the center of a projectile.
        /// </summary>
        /// <param name="Tex"></param>
        /// <param name="projectile"></param>
        /// <param name="color"></param>
        /// <param name="RotateWithProj"></param>
        /// <param name="Rot"></param>
        public void DrawTextureOnProj(Asset<Texture2D> Tex, Projectile projectile, Color color, bool RotateWithProj, float Rot = 0, float ScaleX = 1f, float ScaleY = 1f)
        {
            if (RotateWithProj)
            {
                Rot = projectile.rotation;
            }

            Main.EntitySpriteDraw(
                Tex.Value,
                projectile.Center - Main.screenPosition,
                null,
                color,
                Rot,
                Tex.Value.Size() / 2,
                new Vector2(ScaleX, ScaleY),
                SpriteEffects.None,
                0
            );
        }

        public static bool BossNearby()
        {
            foreach (NPC boss in Main.npc)
            {
                if (boss.active && boss.boss)
                {
                    return true;
                }
            }
            return false;
        }

        public void StartSpriteBatchWithBlending(SpriteBatch spriteBatch, BlendState blendState, SpriteSortMode ssm)
        {
            spriteBatch.End();
            spriteBatch.Begin(ssm, blendState, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void ReturnToDefaultDrawing(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void BurstParticle(int type, Vector2 Center, Color color, float Scale = 1f)
        {
            PRTLoader.NewParticle(type, Center, Vector2.Zero, color, Scale);
        }

        /// <summary>
        /// Contrary to what the name suggests, this code was first used in the Hollow Star code, and the name comes from this effect only being used for projectiles used by Constitution.
        /// </summary>
        /// <param name="projectile"></param>
        public static void ConstitutionStarExplosionEffects(Projectile projectile)
        {
            int points = 10; // 5 outer + 5 inner
            float outerRadius = 16f;
            float innerRadius = outerRadius * 0.4f;
            float rotationOffset = projectile.rotation; // could also add MathHelper.PiOver2 if the sprite is rotated visually

            for (int i = 0; i < points; i++)
            {
                float angle = MathHelper.TwoPi * i / points + rotationOffset;
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;

                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 spawnPos = projectile.Center + direction * radius;
                Vector2 velocity = direction * 3f;

                Dust dust = Dust.NewDustPerfect(spawnPos, DustID.TintableDustLighted, velocity, 0, ColorLib.StellarColor, 2f);
                Dust dust1 = Dust.NewDustPerfect(spawnPos, DustID.TintableDustLighted, Vector2.Zero, 0, ColorLib.StellarColor, 2f);
                dust.noGravity = true;
                dust1.noGravity = true;
            }
            PRTLoader.NewParticle(PRTLoader.GetParticleID<FlatStar>(), projectile.Center, Vector2.Zero, ColorLib.StellarColor, 0.15f);
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

        public static int[] ElectricArcs = new int[]
        {
            PRTLoader.GetParticleID<Arc1>(),
            PRTLoader.GetParticleID<Arc2>(),
            PRTLoader.GetParticleID<Arc3>()
        };

        public static int[] Fire =
        {
            PRTLoader.GetParticleID<ColoredFire1>(),
            PRTLoader.GetParticleID<ColoredFire2>(),
            PRTLoader.GetParticleID<ColoredFire3>(),
            PRTLoader.GetParticleID<ColoredFire4>(),
            PRTLoader.GetParticleID<ColoredFire5>(),
            PRTLoader.GetParticleID<ColoredFire6>(),
            PRTLoader.GetParticleID<ColoredFire7>()
        };
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

        public static Color StellarColor
        {
            get
            {
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                return Color.Lerp(new Color(247, 233, 141), new Color(143, 39, 120), lerpAmount);
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
        //
        //Practical, Every-Day VFX Textures
        //
        public static Asset<Texture2D> Square = TextureAssets.MagicPixel;
        public static Asset<Texture2D> PointGlow = ModContent.Request<Texture2D>($"{ParticlePath}/SimpleParticle");
        public static Asset<Texture2D> AreaGlow = ModContent.Request<Texture2D>($"{ParticlePath}/Glow");
        public static Asset<Texture2D> BloomRing = ModContent.Request<Texture2D>($"{ParticlePath}/BloomRing");
        public static Asset<Texture2D> BloomRingSharp = ModContent.Request<Texture2D>($"{ParticlePath}/BloomRingSharp_FullScale");
        public static Asset<Texture2D> FeatheredCircle = ModContent.Request<Texture2D>($"{ParticlePath}/GlowCircle");
        public static Asset<Texture2D> Vingette = ModContent.Request<Texture2D>($"{ExtrasPath}/BigVingette");
        public static Asset<Texture2D> FadeLine = ModContent.Request<Texture2D>($"{ExtrasPath}/FadeLine");
        public static Asset<Texture2D> StarAura = ModContent.Request<Texture2D>($"{ExtrasPath}/StarWrathAura");
        public static Asset<Texture2D> Swirl = ModContent.Request<Texture2D>($"{ParticlePath}/Swirl");
        public static Asset<Texture2D> FireRing = ModContent.Request<Texture2D>($"{ParticlePath}/Boom2");
        public static Asset<Texture2D> Sparkle(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/Shine{Variant}");
        }

        public static Asset<Texture2D> Star(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/Star{Variant}");
        }

        public static Asset<Texture2D> Cyclone(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/Cyclone{Variant}");
        }
        public static Asset<Texture2D> FlameTelegraph = ModContent.Request<Texture2D>($"{ParticlePath}/CursedFlamesTelegraph");
        public static Asset<Texture2D> ArrowTelegraph = ModContent.Request<Texture2D>($"{ExtrasPath}/DashTelegraphArrow");
        public static Asset<Texture2D> ArrowTelegraphCont = ModContent.Request<Texture2D>($"{ExtrasPath}/DashTelegraphArrowContinuous");
        public static Asset<Texture2D> Warning = ModContent.Request<Texture2D>($"{ParticlePath}/WarningTriangle");
        public static Asset<Texture2D> Trail(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/Trail{Variant}");
        }
        public static Asset<Texture2D> Line(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ExtrasPath}/Line{Variant}");
        }
        public static Asset<Texture2D> TilableNoise(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ExtrasPath}/Noise{Variant}");
        }
        //
        //Textures with more niche use cases.
        //
        public static Asset<Texture2D> RiftStar = ModContent.Request<Texture2D>($"{ParticlePath}/RiftStar");
        public static Asset<Texture2D> NightmareRoseArenaBorder = ModContent.Request<Texture2D>($"{ParticlePath}/NightmareRoseBarrier");
        public static Asset<Texture2D> ConstitutionBeamGlow = ModContent.Request<Texture2D>($"{ExtrasPath}/ConstitutionBeamGlow");
        public static Asset<Texture2D> GalantineLanceGlow = ModContent.Request<Texture2D>($"{ExtrasPath}/GalantineLanceGlow");
        public static Asset<Texture2D> TenebrousConstructWingLeft = ModContent.Request<Texture2D>($"{ExtrasPath}/TenebrousConstructWingLeft");
        public static Asset<Texture2D> TenebrousConstructWingRight = ModContent.Request<Texture2D>($"{ExtrasPath}/TenebrousConstructWingRight");
        public static Asset<Texture2D> WyvernSoulDash = ModContent.Request<Texture2D>($"{ExtrasPath}/WyvernSoulDash");
        public static Asset<Texture2D> CorruptSigil = ModContent.Request<Texture2D>($"{ExtrasPath}/CorruptSigil");
        public static Asset<Texture2D> CrimsonSigil = ModContent.Request<Texture2D>($"{ExtrasPath}/CrimsonSigil");
        public static Asset<Texture2D> CrimsonBloodRune = ModContent.Request<Texture2D>($"{ExtrasPath}/CrimsonSigil");
        public static Asset<Texture2D> BloodHexHeart = ModContent.Request<Texture2D>($"{ExtrasPath}/BloodHexHeart");
        public static Asset<Texture2D> StarFuryOutline = ModContent.Request<Texture2D>($"{ExtrasPath}/StarfuryCloneOutline");
        public static Asset<Texture2D> NodeBossPikeOutline = ModContent.Request<Texture2D>($"{ExtrasPath}/NodeBossDistendedPikeOutline");
        public static Asset<Texture2D> PossessedToothOutline = ModContent.Request<Texture2D>($"{ExtrasPath}/PossessedToothOutline");
        //
        // Sounds
        //
        public static SoundStyle ChargeBreak = new SoundStyle($"{AudioPath}/ChargeBreak");
        public static SoundStyle CrystalBreak = new SoundStyle($"{AudioPath}/CrystalBreak");
        public static SoundStyle ConstitutionStarKill = new SoundStyle($"{AudioPath}/ConstitutionBoss/ConstitutionStar/Kill", 14) { PitchVariance = 0.2f, Volume = 0.85f, MaxInstances = 0 };
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
}