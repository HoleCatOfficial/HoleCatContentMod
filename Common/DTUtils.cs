
using System;
using System.Runtime.CompilerServices;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using rail;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class DTUtils
    {
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
        public void DrawTextureOnProj(Asset<Texture2D> Tex, Projectile projectile, Color color, bool RotateWithProj, float Rot = 0)
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
                projectile.scale,
                SpriteEffects.None,
                0
            );
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
                float time = (Main.GlobalTimeWrappedHourly % 3f);

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
    }

    /// <summary>
    /// The central repository from which most drawn textures in the mod are sourced. If a texture appears more than once in the mod, it will likely have its place here.
    /// <para/> By sharing assets from AssetLib instead of loading them individually, draw calls can be optimised.
    /// </summary>
    public class DTAssetLib
    {
        public const string ParticlePath = "DestroyerTest/Content/Particles";
        public const string ExtrasPath = "DestroyerTest/Content/Extras";
        //
        //Practical, Every-Day VFX Textures
        //
        public static Asset<Texture2D> Square = TextureAssets.MagicPixel;
        public static Asset<Texture2D> PointGlow = ModContent.Request<Texture2D>($"{ParticlePath}/SimpleParticle");
        public static Asset<Texture2D> AreaGlow = ModContent.Request<Texture2D>($"{ParticlePath}/Glow");
        public static Asset<Texture2D> BloomRing(int Variant)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            return ModContent.Request<Texture2D>($"{ParticlePath}/BloomRing{Variant}");
        }
        public static Asset<Texture2D> FeatheredCircle = ModContent.Request<Texture2D>($"{ParticlePath}/GlowCircle");
        public static Asset<Texture2D> Vingette = ModContent.Request<Texture2D>($"{ExtrasPath}/BigVingette");
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
        public static Asset<Texture2D> ArrowTelegraph = ModContent.Request<Texture2D>($"{ParticlePath}/ArrowTelegraph");
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
        public static Asset<Texture2D> NightmareRoseArenaBorder = ModContent.Request<Texture2D>($"{ParticlePath}/NightmareRoseArenaBorder");
        public static Asset<Texture2D> ConstitutionBeamGlow = ModContent.Request<Texture2D>($"{ExtrasPath}/ConstitutionBeamGlow");
        public static Asset<Texture2D> GalantineLanceGlow = ModContent.Request<Texture2D>($"{ExtrasPath}/GalantineLanceGlow");
        public static Asset<Texture2D> TenebrousConstructWingLeft = ModContent.Request<Texture2D>($"{ExtrasPath}/TenebrousConstructWingLeft");
        public static Asset<Texture2D> TenebrousConstructWingRight = ModContent.Request<Texture2D>($"{ExtrasPath}/TenebrousConstructWingRight");
        public static Asset<Texture2D> WyvernSoulDash = ModContent.Request<Texture2D>($"{ExtrasPath}/WyvernSoulDash");
        public static Asset<Texture2D> CorruptSigil = ModContent.Request<Texture2D>($"{ExtrasPath}/CorruptSigil");
    }
}