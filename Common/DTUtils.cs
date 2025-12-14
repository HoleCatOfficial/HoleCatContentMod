
using System;
using System.Collections.Generic;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.fire;
using DestroyerTest.Rarity.Scepter;
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
using Terraria.Localization;
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

        public static void AddStrips(List<ColoredVertex> List, List<Vector2> Vex, int Index, Vector2 off1, Vector2 off2, float Fade, Color CLR, float stripMotion = 0f)
        {
            List.Add(new ColoredVertex(Vex[Index] - Main.screenPosition + off1, new Vector3(Fade - stripMotion, 1, 1), CLR));
            List.Add(new ColoredVertex(Vex[Index] - Main.screenPosition + off2, new Vector3(Fade - stripMotion, 0, 1), CLR));
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

        private static Color StellarFire1 = new Color(247, 233, 141);
        private static Color StellarFire2 = new Color(207, 120, 90);
        private static Color StellarFire3 = new Color(183, 61, 114);
        private static Color StellarFire4 = new Color(143, 39, 120);
        private static Color StellarFire5 = new Color(80, 38, 91);
        private static Color StellarFire6 = new Color(33, 36, 37);
        private static Color StellarFire7 = new Color(25, 33, 38);
        private static Color StellarFire8 = new Color(18, 23, 24);

        public static Color StellarFireGradient(float t)
        {
            
            t = MathHelper.Clamp(t, 0f, 3f);

            if (t < 1f)
                return Color.Lerp(StellarFire1, StellarFire2, t);
            else if (t < 2f)
                return Color.Lerp(StellarFire3, StellarFire4, t - 1f);
            else if (t < 3f)
                return Color.Lerp(StellarFire5, StellarFire6, t - 2f);
            else
                    return Color.Lerp(StellarFire7, StellarFire8, t - 3f);
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
        public const string EffectPath = "DestroyerTest/Assets/Effects";
        //
        //Practical, Every-Day VFX Textures
        //
        public static Asset<Texture2D> Square = TextureAssets.MagicPixel;
        public static Asset<Texture2D> PointGlow = ModContent.Request<Texture2D>($"{ParticlePath}/SimpleParticle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> AreaGlow = ModContent.Request<Texture2D>($"{ParticlePath}/Glow", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> BloomRing = ModContent.Request<Texture2D>($"{ParticlePath}/BloomRing", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> BloomRingSharp = ModContent.Request<Texture2D>($"{ParticlePath}/BloomRingSharp_FullScale", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FeatheredCircle = ModContent.Request<Texture2D>($"{ParticlePath}/GlowCircle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Vingette = ModContent.Request<Texture2D>($"{ExtrasPath}/BigVingette", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FadeLine = ModContent.Request<Texture2D>($"{ExtrasPath}/FadeLine", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> StarAura = ModContent.Request<Texture2D>($"{ExtrasPath}/StarWrathAura", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Swirl = ModContent.Request<Texture2D>($"{ParticlePath}/Swirl", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FireRing = ModContent.Request<Texture2D>($"{ParticlePath}/Boom2", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> SwingFX = ModContent.Request<Texture2D>($"{ExtrasPath}/CircularSlash", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Slash144 = ModContent.Request<Texture2D>($"{ExtrasPath}/144Slash", AssetRequestMode.AsyncLoad);
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
        //
        // Sounds
        //
        public static SoundStyle ChargeBreak = new SoundStyle($"{AudioPath}/ChargeBreak");
        public static SoundStyle CrystalBreak = new SoundStyle($"{AudioPath}/CrystalBreak");
        public static SoundStyle ConstitutionStarKill = new SoundStyle($"{AudioPath}/ConstitutionBoss/ConstitutionStar/Kill", 14) { PitchVariance = 0.2f, Volume = 0.85f, MaxInstances = 0 };
    
        //
        // Effects
        //

        public static Asset<Effect> TrailScroller = ModContent.Request<Effect>($"{EffectPath}/TrailScroll", AssetRequestMode.AsyncLoad);
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