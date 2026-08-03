using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Rarity.Scepter;
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


namespace DestroyerTest.Common
{
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
        public const string FontPath = "DestroyerTest/Assets/Fonts";

        public static Asset<SpriteFont> Arial = ModContent.Request<SpriteFont>(FontPath + "/arial", AssetRequestMode.AsyncLoad);
        public static Asset<SpriteFont> Doxent = ModContent.Request<SpriteFont>(FontPath + "/doxent", AssetRequestMode.AsyncLoad);

        

        //
        //Practical, Every-Day VFX Textures
        //
        public static Asset<Texture2D> Square = TextureAssets.MagicPixel;
        public static Asset<Texture2D> Circle = ModContent.Request<Texture2D>($"{ExtrasPath}/CrispCircle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> PointGlow = ModContent.Request<Texture2D>($"{ParticlePath}/SimpleParticle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> PointGlowPreMultiplied = ModContent.Request<Texture2D>($"{ExtrasPath}/PreMultiplied/PointGlow", AssetRequestMode.AsyncLoad);
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
        public static Asset<Texture2D> CircularSwing = ModContent.Request<Texture2D>($"{ExtrasPath}/CircularSlash", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> CircularSwingThin = ModContent.Request<Texture2D>($"{ExtrasPath}/CircularSlash2", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FireSwing = ModContent.Request<Texture2D>($"{ExtrasPath}/CircularSlash3", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FireSwingHighlight = ModContent.Request<Texture2D>($"{ExtrasPath}/CircularSlash3Highlight", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> CutSwing = ModContent.Request<Texture2D>($"{ExtrasPath}/CircularSlashCut", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> Laser = ModContent.Request<Texture2D>($"{ExtrasPath}/LongLaser", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> LaserRepeating(bool PreMultiplied)
        {
            string P = PreMultiplied ? "/PreMultiplied" : "";
            return ModContent.Request<Texture2D>($"{ExtrasPath}{P}/LaserRepeating", AssetRequestMode.AsyncLoad);
        }

        public static Asset<Texture2D> AuraRing = ModContent.Request<Texture2D>($"{ParticlePath}/AuraRing", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> FaintGlow = ModContent.Request<Texture2D>($"{ExtrasPath}/FaintGlow", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> CurseSigilRing = ModContent.Request<Texture2D>($"{ExtrasPath}/CurseSigilRing", AssetRequestMode.AsyncLoad);

        public static Asset<Texture2D> SparkDefault = ModContent.Request<Texture2D>($"{ExtrasPath}/144MiscSparkle", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> SparkSmooth = ModContent.Request<Texture2D>($"{ExtrasPath}/MiscSparkle2", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> SparkSmoothThin = ModContent.Request<Texture2D>($"{ExtrasPath}/MiscSparkle3", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> TinyBloom = ModContent.Request<Texture2D>($"{ParticlePath}/TinyBloom", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> BarrierRing(bool PreMultiplied)
        {
            string P = PreMultiplied ? "/PreMultiplied" : "";
            return ModContent.Request<Texture2D>($"{ExtrasPath}{P}/BarrierRing", AssetRequestMode.AsyncLoad);
        }
        public static Asset<Texture2D> Sparkle(int Variant, bool PreMultiplied = false)
        {

            if (Variant <= 0)
            {
                Variant = 1;
            }
            if (!PreMultiplied)
            {
                return ModContent.Request<Texture2D>($"{ParticlePath}/Shine{Variant}", AssetRequestMode.AsyncLoad);
            }
            else
            {
                return ModContent.Request<Texture2D>($"{ExtrasPath}/PreMultiplied/Shine{Variant}", AssetRequestMode.AsyncLoad);
            }
        }

        public static Asset<Texture2D> Streak(int Variant, bool PreMultiplied = false)
        {
            if (Variant <= 0)
            {
                Variant = 1;
            }
            if (!PreMultiplied)
            {
                return ModContent.Request<Texture2D>($"{ExtrasPath}/Streak{Variant}", AssetRequestMode.AsyncLoad);
            }
            else
            {
                return ModContent.Request<Texture2D>($"{ExtrasPath}/PreMultiplied/Streak{Variant}", AssetRequestMode.AsyncLoad);
            }
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

        public static Asset<Texture2D> ShadeSigil = ModContent.Request<Texture2D>($"{ExtrasPath}/ShadeSigil", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ShadeSigilLine = ModContent.Request<Texture2D>($"{ExtrasPath}/ShadeSigilLine", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ShadeSigilLine2 = ModContent.Request<Texture2D>($"{ExtrasPath}/ShadeSigilLine2", AssetRequestMode.AsyncLoad);

        public static Asset<Texture2D> HallowedSigil = ModContent.Request<Texture2D>($"{ExtrasPath}/HallowedSigil", AssetRequestMode.AsyncLoad);
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
        public static Asset<Texture2D> ConstitutionLanceWarning = ModContent.Request<Texture2D>($"{ExtrasPath}/ConstitutionEternityLanceWarning", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> DreamDiscHighlight = ModContent.Request<Texture2D>($"{ExtrasPath}/DreamDiscMainBlades", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> BlessedNodeLaserTelegraph = ModContent.Request<Texture2D>($"{ExtrasPath}/BlessedNodeLaserTelegraph", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> ConstitutionStarTrail = ModContent.Request<Texture2D>($"{ExtrasPath}/ConstitutionStarTrail", AssetRequestMode.AsyncLoad);
        public static Asset<Texture2D> SparkFrostCleaverMask = ModContent.Request<Texture2D>($"{ExtrasPath}/SparkFrostCleaverMask", AssetRequestMode.AsyncLoad);


        public static Asset<Texture2D> TenebrisCorruptionWorldIcon = ModContent.Request<Texture2D>($"{ExtrasPath}/TenebrisWorldIcon", AssetRequestMode.AsyncLoad);
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
        public struct AudioFolder
        {
            public static string Loops = AudioPath + "/AuraLoop";
            public static string BlessingSounds = AudioPath + "/Blessing";
            public static string ChargeSounds = AudioPath + "/Charge";
            public static string ConstitutionBossSounds = AudioPath + "/ConstitutionBoss";
            public static string ConstitutionStarSounds = AudioPath + "/ConstitutionBoss/ConstitutionStar";
            public static string Corpse = AudioPath + "/Corpse";
            public static string DarkGreatsword = AudioPath + "/DarkGreatSword";
            public static string GhoulishScepter = AudioPath + "/GhoulishScepter";
            public static string Impacts = AudioPath + "/Impacts";
            public static string Malakhim = AudioPath + "/Malakhim";
            public static string NightmareRose = AudioPath + "/NightmareRose";
            public static string Scholar = AudioPath + "/Scholar";
            public static string StellarBow = AudioPath + "/Scholar";
            public static string SwordSounds = AudioPath + "/SwordSounds";
            public static string TenebrisSlinger = AudioPath + "/TenebrisSlinger";
            public static string TenebrisConstruct = AudioPath + "/TenebrisConstruct";
            public static string TenebrousKatana = AudioPath + "/TenebrousKatana";
            public static string TenebrousTrialBossDefeats = AudioPath + "/TenebrousTrialBossDefeats";
        }

        
        
        

        public static SoundStyle ChargeBreak = new SoundStyle($"{AudioPath}/ChargeBreak");
        public static SoundStyle CrystalBreak = new SoundStyle($"{AudioPath}/CrystalBreak");
        public static SoundStyle FlailSpin = new SoundStyle($"{AudioPath}/FlailSpin");
        public static SoundStyle FlailThrow = new SoundStyle($"{AudioPath}/FlailThrow");
        public static SoundStyle ConstitutionStarKill = new SoundStyle($"{AudioPath}/ConstitutionBoss/ConstitutionStar/Kill", 14) { PitchVariance = 0.2f, Volume = 0.85f, MaxInstances = 0 };
        public static SoundStyle EnergyWoosh = new SoundStyle($"{AudioPath}/EnergyWoosh", 3);
        public static SoundStyle RiftExplosion = new SoundStyle($"{AudioPath}/RiftMaker_Boom");
        public static SoundStyle Zap = new SoundStyle($"{AudioPath}/Zap", 3);

        public struct LoopedSounds
        {
            public static string Path = AudioFolder.Loops;
            public static SoundStyle Electric1 = new SoundStyle($"{Path}/ElectricLoop1");
            public static SoundStyle Electric2 = new SoundStyle($"{Path}/ElectricLoop2");
            public static SoundStyle Electric3 = new SoundStyle($"{Path}/ElectricLoop3");
            public static SoundStyle GenericLaser = new SoundStyle($"{Path}/LaserLoop1");
            public static SoundStyle Corona = new SoundStyle($"{Path}/RiftYoyoT3Loop");
            public static SoundStyle ShadowflameAura = new SoundStyle($"{Path}/ShadowflameAuraLoop");
            public static SoundStyle Spirit = new SoundStyle($"{Path}/SpiritAura", 4);
            public static SoundStyle ShadesRevenge = new SoundStyle($"{Path}/TenebrisLoop");
        }

        public static SoundStyle ElectricLoopSound(int variant)
        {
            switch (variant)
            {
                case 1:
                    {
                        return LoopedSounds.Electric1;
                    }
                case 2:
                    {
                        return LoopedSounds.Electric2;
                    }
                case 3:
                    {
                        return LoopedSounds.Electric3;
                    }
                default:
                    {
                        return new SoundStyle();
                    }
            }
        }

        public struct Blessing
        {
            public static string Path = AudioFolder.BlessingSounds;
            public static SoundStyle Accepted = new SoundStyle($"{Path}/AcceptedBlessing");
            public static SoundStyle Rejected = new SoundStyle($"{Path}/RejectedBlessing");
        }
        

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
            public static SoundStyle DarkShatter = new SoundStyle($"{Path}/DarkShatter");
            public static SoundStyle Deflect = new SoundStyle($"{Path}/Deflect");
            public static SoundStyle DreamHit = new SoundStyle($"{Path}/DreamHit", 4);
            public static SoundStyle EnergyBounce = new SoundStyle($"{Path}/EnergyBounce", 3);
            public static SoundStyle ExplosiveImpactSmall = new SoundStyle($"{Path}/ExplosiveImpactSmall");
            public static SoundStyle ExplosiveImpactBig = new SoundStyle($"{Path}/ExplosiveImpactBig");
            public static SoundStyle FlameImpact = new SoundStyle($"{Path}/FlameImpact", 4);
            public static SoundStyle FleshHit = new SoundStyle($"{Path}/FleshHit", 5);
            public static SoundStyle HellWeaponImpact = new SoundStyle($"{Path}/HellWeaponImpact");
            public static SoundStyle HeatseekerSilohSlam = new SoundStyle($"{Path}/HeatseekerSilohSlam");
            public static SoundStyle IceImpact = new SoundStyle($"{Path}/IceImpact", 3);
            public static SoundStyle IceMagicImpact = new SoundStyle($"{Path}/IceMagicImpact", 3);
            public static SoundStyle KCrystalConsume = new SoundStyle($"{Path}/KCrystalConsume");
            public static SoundStyle LightMetalHit = new SoundStyle($"{Path}/LightMetalHit", 4);
            public static SoundStyle Malevolence = new SoundStyle($"{Path}/MalevolenceHit");
            public static SoundStyle MagicBeep = new SoundStyle($"{Path}/MagicBeep", 3);
            public static SoundStyle MagicHit = new SoundStyle($"{Path}/MagicHit", 3);
            public static SoundStyle MetalImpact = new SoundStyle($"{Path}/MetalImpactV1_", 3);
            public static SoundStyle ShortShine = new SoundStyle($"{Path}/ShortShine", 3);
            public static SoundStyle StellarFox = new SoundStyle($"{Path}/StellarFoxImpact", 5);
            public static SoundStyle SpiritOfJusticeParry = new SoundStyle($"{Path}/SpiritOfJusticeParry");
            public static SoundStyle Void = new SoundStyle($"{Path}/VoidImpact", 3);

        }

        public struct SwordSounds
        {
            public static string Path = $"{AudioPath}/SwordSounds";
            public static SoundStyle BigBasicSwing = new SoundStyle($"{Path}/BigBasicSwing", 3);
            public static SoundStyle ConSwing = new SoundStyle($"{Path}/Constitution/ConSwing", 6);
            public static SoundStyle ColdSword = new SoundStyle($"{Path}/ColdSword", 3);
            public static SoundStyle Woosh = new SoundStyle($"{Path}/DefaultWoosh");
            public static SoundStyle EvilSwing = new SoundStyle($"{Path}/EvilSwing", 3);
            public static SoundStyle HeavySwing = new SoundStyle($"{Path}/HeavySwing", 3);
            public static SoundStyle HellSword = new SoundStyle($"{Path}/HellSword", 3);
            public static SoundStyle LightGoreCut = new SoundStyle($"{Path}/LightGoreCut", 4);
            public static SoundStyle LightSnap = new SoundStyle($"{Path}/LightSnap");
            public static SoundStyle MagicSwing = new SoundStyle($"{Path}/MagicSwing", 3);
            public static SoundStyle MediumSwing = new SoundStyle($"{Path}/MediumSwing", 3);
            public static SoundStyle MediumHeavySwing = new SoundStyle($"{Path}/MediumHeavySwing", 3);
            public static SoundStyle MemoriamSwing = new SoundStyle($"{Path}/MemoriamSwing");
            public static SoundStyle MetalSwing = new SoundStyle($"{Path}/MetalSwing", 4);
            public static SoundStyle QuickSwing = new SoundStyle($"{Path}/QuickSwing", 4);
            public static SoundStyle SwiftSwing = new SoundStyle($"{Path}/SwiftSwing1");
            public static SoundStyle Slam = new SoundStyle($"{Path}/Slam", 2);
            public static SoundStyle SpinWave = new SoundStyle($"{Path}/SpinWave");
            public static SoundStyle TenebrisSwing = new SoundStyle($"{Path}/TenebrisSwing", 3);
            public static SoundStyle ThinSlice = new SoundStyle($"{Path}/ThinSlice", 5);
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

        public struct FrigidFenzim
        {
            public static string Path = $"{AudioPath}/FrigidFenzim";
            public static SoundStyle TileHit = new SoundStyle($"{Path}/TileImpact");
            public static SoundStyle Hit = new SoundStyle($"{Path}/Impact");
            public static SoundStyle Crit = new SoundStyle($"{Path}/CritImpact");
        }

        public struct StellarBow
        {
            public static string Path = $"{AudioPath}/StellarBow";
            public static SoundStyle ArrowImpact = new SoundStyle($"{Path}/StellarBowArrowImpact", 4);
            public static SoundStyle Shoot = new SoundStyle($"{Path}/StellarBowShoot", 3);
            public static SoundStyle EmpoweredShoot = new SoundStyle($"{Path}/StellarBowEmpoweredShoot", 3);
        }

        public struct TileMine
        {
            public static SoundStyle Altar = new SoundStyle($"{AudioPath}/AltarMine", 3);
            public static SoundStyle AltarBrick = new SoundStyle($"{AudioPath}/AltarStoneMine", 3);
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
}
