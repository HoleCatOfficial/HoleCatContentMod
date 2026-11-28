using System;
using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Blessings
{
    public class PrayerSystem : ModSystem
    {

    }

    /*
    __/\\\________/\\\______/\\\_        
     _\/\\\_____/\\\//___/\\\\\\\_       
      _\/\\\__/\\\//_____\/////\\\_      
       _\/\\\\\\//\\\_________\/\\\_     
        _\/\\\//_\//\\\________\/\\\_    
         _\/\\\____\//\\\_______\/\\\_   
          _\/\\\_____\//\\\______\/\\\_  
           _\/\\\______\//\\\_____\/\\\_ 
            _\///________\///______\///_  
    https://symboldb.org/font-generator/big/
    */
    /// <summary>
    /// OBSOLETE
    /// Blessings will be reworked at a later time. This is ugly and convoluted.
    /// </summary>
    public class PrayerPlayer : ModPlayer
    {
        //The current blessing is made nullable (the ? on the end) because the default value of a Blessing should be null.
        //Due to me not knowing if that actually is the case, I can't say whether or not this is a necessary maneuver
        public Blessing? CurrentBlessing = null;

        //If blessing vfx are disabled in the configs, we don't run the related code.
        //It's generally good practice to have a static instance of your config class for ease of access and centralization.
        //Otherwise you'd need to call ModContent.GetInstance<>() every time you needed to access the configs.
        public bool VFX => DTConfig.instance.BlessingVFX;

        public void ApplyBlessing(Blessing IncomingBlessing)
        {
            //Without this check, our PopupText would cause a NullReferenceException.
            //When in doubt, put a null check on it.
            if (IncomingBlessing == null)
            {
                return;
            }

            //The current blessing becomes what we passed into the method.
            CurrentBlessing = IncomingBlessing;

            //A good chance to demonstrate Bakery's particle setup.
            //First, you use the constructor of your particle class.
            BlessingParticle FX = new BlessingParticle();

            //Your particle class should have a method for setting up and spawning the particles.
            //Peek at what Prepare does for a better understanding.
            //Name it whatever youd like. Prepare is conventional for Bakery, but you can use Create or Initialize as well, as youll see in some of the other particles.
            FX.Prepare(Player.Center, Vector2.Zero, ColorLib.Soul, 0.2f, 0.01f, 2.5f, BlendState.Additive);

            //ParticleEngine has 3 pools of particles that it draws with.
            //I don't have the best understanding of it yet, but from what I could see in my investigations:
            //ShaderParticles is for particles that use shaders when drawing. All it does is put them in ParticleRenderGroup.Shader.
            //Particles is where most particles will go.
            //Behind projectiles will draw behind projectiles, as the name implies. Useful for if you don't want to use a different PixelLayer.
            ParticleEngine.ShaderParticles.Add(FX);

            //RadialVectorOutwardRandom is a helper method, and does exactly what it says on the tin.
            Vector2[] Vels = Opus.RadialVectorOutwardRandom(10, Player.Center, 1f);

            for (int i = 0; i < Vels.Length; i++)
            {
                HallowedPallStar Star = new();
                Star.Initialize(Player.Center, Vels[i], ColorLib.Soul, 1f);
                ParticleEngine.ShaderParticles.Add(Star);
            }

            //This is a bit of a jank way to setup a PopupText, but it was the way I found was easiest.
            //Things that are structs, such as AdvancedPopupRequest, TooltipLine, or SoundStyle, can be edited in this way both when constructed and after.
            PopupText.NewText(new AdvancedPopupRequest() { Text = IncomingBlessing.BlessingMessage, Color = ColorLib.Soul, DurationInFrames = 300, Velocity = new Vector2(0, -20) }, Player.Center);
        }

        //I had some kind of plans for effects when an offer was rejected. It seems they never came to pass.
        public void RejectOffer()
        {

        }

        public override void PostUpdateMiscEffects()
        {
            //Keep it centralized. Save yourself the headache.
            DTBlessings B = ModContent.GetInstance<DTBlessings>();

            //IsABlessingActive only checks one thing. Not sure why I made it a method instead of a lambda property (EX: bool day => Main.dayTime), but it ultimately accomplishes the same goal.
            if (!IsABlessingActive())
            {
                //ClearBuff is the easier of the two Buff Removal methods in TMod.
                //The other one, DelBuff requires you to know which index a given buff is at. It has its use cases, but this is not among them.
                Player.ClearBuff(ModContent.BuffType<BlessingBuff>());
                return;
            }
            else
            {
                //BlessingBuff is a centralized buff to keep things lightweight. I don't imagine it'll stay the same, but I have no idea what the future may hold.
                Player.AddBuff(ModContent.BuffType<BlessingBuff>(), 20);
            }

            if (VFX)
            {
                //You've probably seen a few of these before.
                //This is a really good substitute for a long chain of if ( == ) statements.
                switch (CurrentBlessing.ID)
                {
                    case (BlessingID.RadiantHeart):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.LifeDrain, Main.rand.NextVector2Circular(2f, 2f), 100);
                                D.noGravity = true;

                            }
                            //Always break your cases at the end.
                            //Without this, Control spills between two cases, which the compiler can't make sense of,
                            break;
                        }
                    case (BlessingID.Enchanted):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.ManaRegeneration, Main.rand.NextVector2Circular(2f, 2f), 100);
                                D.noGravity = true;
                            }
                            break;
                        }
                    case (BlessingID.Attuned):
                        {
                            if (Main.rand.NextBool(18))
                            {
                                Dust D = Player.QuickDust(DustID.AmberBolt, Main.rand.NextVector2Circular(1f, 3f), 205);
                                D.noGravity = true;
                            }
                            break;
                        }

                    case (BlessingID.OozingAffection):
                        {
                            if (Player.statLife < Player.statLifeMax * 0.25f)
                            {
                                Player.AddBuff(BuffID.Honey, 300);
                            }
                            break;
                        }
                    case (BlessingID.Serenity):
                        {
                            if (Main.windSpeedCurrent < 10f && !Main.IsItRaining)
                            {
                                if (Main.rand.NextBool(18))
                                {
                                    Dust D = Player.QuickDust(DustID.GoldFlame, Main.rand.NextVector2Circular(1f, 3f), 205);
                                    D.noGravity = true;
                                }
                            }
                            break;
                        }
                    case (BlessingID.Overgrown):
                        {
                            if (Main.rand.NextBool(18))
                            {
                                Dust D = Player.QuickDust(DustID.JungleSpore, Main.rand.NextVector2Circular(1f, 1f), 205);
                                D.noGravity = true;
                            }
                            //I'll always recommend finding some way to implement the functionality yourself, as it saves you some wierd interactions with other mods.
                            //Although, if youre doing something simple or are just starting out, setting the player flag is fine.
                            Player.flowerBoots = true;
                            break;
                        }
                    case BlessingID.ThrivingDarknessCorr:
                        {
                            if (Lighting.Brightness((int)Player.Center.X, (int)Player.Center.Y) < 0.5f)
                            {
                                Dust D = Player.QuickDust(DustID.Corruption, Main.rand.NextVector2Circular(1f, 1f), 205);
                                D.noGravity = true;
                            }
                            break;
                        }
                    case BlessingID.ThrivingDarknessCrim:
                        {
                            if (Lighting.Brightness((int)Player.Center.X, (int)Player.Center.Y) < 0.5f)
                            {
                                Dust D = Player.QuickDust(DustID.Crimson, Main.rand.NextVector2Circular(1f, 1f), 205);
                                D.noGravity = true;
                            }
                            break;
                        }
                    case (BlessingID.MilkywayStride):
                        {
                            if (Player.HeldItem.type == ModContent.ItemType<Constitution>() || Player.HeldItem.type == ModContent.ItemType<Committment>())
                            {
                                if (Main.rand.NextBool(18))
                                {
                                    Dust D = Player.QuickDust(DustID.FireworksRGB, Main.rand.NextVector2Circular(1f, 3f), ColorLib.StellarFireGradientLooping(), 0);
                                    D.noGravity = true;
                                }
                            }
                            break;
                        }

                    case (BlessingID.ScarletSurge):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.FireworksRGB, Main.rand.NextVector2Circular(2f, 2f), Color.Red, 70);
                            }
                            break;
                        }
                    case (BlessingID.Clarity):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.FireworksRGB, Main.rand.NextVector2Circular(2f, 2f), Color.Blue, 70);
                            }
                            break;
                        }
                    case (BlessingID.Waywashed):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.FireworksRGB, Main.rand.NextVector2Circular(2f, 2f), Color.DeepSkyBlue, 70);
                            }
                            break;
                        }
                    case (BlessingID.Seething):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.FireworksRGB, Main.rand.NextVector2Circular(2f, 2f), Color.DarkMagenta, 70);
                            }
                            break;
                        }
                    case (BlessingID.LifeStriken):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.FireworksRGB, Main.rand.NextVector2Circular(2f, 2f), Color.Chartreuse, 70);
                            }
                            break;
                        }
                    case (BlessingID.FieryPassion):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.FireworksRGB, Main.rand.NextVector2Circular(2f, 2f), Color.OrangeRed, 70);
                            }
                            break;
                        }
                    case (BlessingID.StarScorn):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.FireworksRGB, Main.rand.NextVector2Circular(2f, 2f), ColorLib.StellarFireGradientLooping(), 70);
                            }
                            break;
                        }
                }
            }

            UpdateMana();
        }

        //There are definitiely better ways of doing this than with separate switch statements.
        //I unfortunately have not found them yet.
        public void UpdateMana()
        {
            //This check is redundant, but better safe than sorry!
            if (!IsABlessingActive())
            {
                return;
            }

            switch (CurrentBlessing.ID)
            {
                case (BlessingID.Enchanted):
                    {
                        Player.manaRegenBonus = 6;
                        break;
                    }
                case (BlessingID.Attuned):
                    {
                        float MaxRegenBonus = 25;
                        float speed = Math.Abs(Player.velocity.X);

                        float RegenBonus = MathHelper.Lerp(10, MaxRegenBonus, speed / 10f);

                        Player.manaRegenBonus = (int)RegenBonus;
                        break;
                    }
                case (BlessingID.Serenity):
                    {
                        if (Main.windSpeedCurrent < 10f && !Main.IsItRaining)
                        {

                            Player.manaRegenBonus = 6;
                        }
                        break;
                    }
            }

        }

        public override void PostUpdateRunSpeeds()
        {
            if (CurrentBlessing == null)
            {
                return;
            }
            else
            {
                switch (CurrentBlessing.ID)
                {
                    case BlessingID.ScarletSurge:
                        {
                            Player.GetDamage(DamageClass.Ranged) += 0.08f;
                            break;
                        }
                    case BlessingID.Clarity:
                        {
                            Player.GetDamage(DamageClass.Magic) += 0.10f;
                            break;
                        }
                    case BlessingID.Tempered:
                        {
                            Player.GetDamage(DamageClass.Generic) += 0.12f;
                            break;
                        }
                    case BlessingID.Alliegance:
                        {
                            Player.GetDamage(DamageClass.Summon) += 0.10f;
                            break;
                        }
                    case BlessingID.Waywashed:
                        {
                            Player.GetDamage<ScepterClass>() += 0.14f;
                            break;
                        }
                    case BlessingID.Seething:
                        {
                            Player.GetDamage(DamageClass.Magic) += 0.14f;
                            break;
                        }
                    case BlessingID.Bulk:
                        {
                            Player.GetDamage(DamageClass.Melee) += 0.16f;
                            break;
                        }
                    case BlessingID.LifeStriken:
                        {
                            Player.GetDamage(DamageClass.Magic) += 0.20f;
                            break;
                        }
                    case BlessingID.FieryPassion:
                        {
                            Player.GetDamage(DamageClass.Throwing) += 0.20f;
                            break;
                        }
                    case BlessingID.StarScorn:
                        {
                            Player.GetDamage<ScepterClass>() += 0.20f;
                            break;
                        }
                }
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (CurrentBlessing == DTBlessings.OozingAffection)
            {
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item97, Player.Center);

                    Opus.RadialSpreadProjectileRandom(ProjectileID.Bee, 7, Player.Center, 10, 10, 5);
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (CurrentBlessing == DTBlessings.OozingAffection)
            {
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item97, Player.Center);
                    Opus.RadialSpreadProjectileRandom(ProjectileID.Bee, 7, Player.Center, 10, 10, 5);
                }
            }
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            DTBlessings B = ModContent.GetInstance<DTBlessings>();
            if (!IsABlessingActive())
            {
                return;
            }

            switch (CurrentBlessing.ID)
            {
                case (BlessingID.RadiantHeart):
                    {
                        regen *= 1.1f;
                        break;
                    }
                case (BlessingID.Attuned):
                    {
                        float MaxRegenBonus = 1.5f;
                        float Speed = Math.Abs(Player.velocity.X);

                        float RegenBonus = MathHelper.Lerp(1f, MaxRegenBonus, Speed / 10f);

                        regen *= RegenBonus;
                        break;
                    }
                case BlessingID.ThrivingDarknessCorr:
                case BlessingID.ThrivingDarknessCrim:
                    {
                        if (Lighting.Brightness((int)Player.Center.X, (int)Player.Center.Y) < 0.5f)
                        {
                            regen *= 1.5f;
                        }
                        break;
                    }
                case (BlessingID.Decadence):
                    {
                        if (Player.HasBuff(BuffID.WellFed))
                        {
                            regen *= 1.08f;
                        }
                        if (Player.HasBuff(BuffID.WellFed2))
                        {
                            regen *= 1.16f;
                        }
                        if (Player.HasBuff(BuffID.WellFed3))
                        {
                            regen *= 1.24f;
                        }
                        break;
                    }
                case (BlessingID.Overgrown):
                    {
                        if (Player.ZoneForest || Player.ZoneJungle)
                        {
                            regen *= 1.65f;
                        }
                        break;
                    }
                case (BlessingID.MilkywayStride):
                    {
                        if (Player.HeldItem.type == ModContent.ItemType<Constitution>() || Player.HeldItem.type == ModContent.ItemType<Committment>())
                        {
                            regen *= 1.8f;
                        }
                        break;
                    }
                case (BlessingID.RejuvenatingWarmth):
                    {
                        if (Player.ZoneRockLayerHeight || Player.ZoneUnderworldHeight)
                        {
                            regen *= 1.6f;
                        }
                        break;
                    }
            }
        }



        public bool IsABlessingActive()
        {
            if (CurrentBlessing == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class DTBlessings : ModSystem
    {
        //Constant values kind of piss me off, but they have their use cases, and TECHNICALLY :nerd:, you SHOULD be making things constant if you don't plan on ever changing their value.
        //I, for a long time, confused the functionality of const and readonly. readonly is slightly different from what it says on the tin.
        public const string CommonBM_Key = "Mods.DestroyerTest.Blessings";

        //Regen Blessings
        public static Blessing RadiantHeart;

        public static Blessing Enchanted;

        public static Blessing Attuned;

        public static Blessing OozingAffection;

        public static Blessing Serenity;

        public static Blessing ThrivingDarknessCorr;

        public static Blessing ThrivingDarknessCrim;

        public static Blessing Decadence;

        public static Blessing Overgrown;

        public static Blessing MilkywayStride;

        public static Blessing RejuvenatingWarmth;


        //Damage Blessings

        public static Blessing ScarletSurge;

        public static Blessing Clarity;

        public static Blessing Tempered;

        public static Blessing Alliegance;

        public static Blessing Waywashed;

        public static Blessing SeethingCrim;

        public static Blessing SeethingCorr;

        public static Blessing Bulk;

        public static Blessing LifeStricken;

        public static Blessing FieryPassion;

        public static Blessing StarScorn;



        public static void RegisterBlessing(Blessing b)
        {
            AllBlessings.Add(b);
        }

        /// AVERT YOUR EYES!!!!!!
        /// CONSTRUCTOOOOOOOOORS!!!!!
        public override void SetStaticDefaults()
        {
            //I GOT A GUUN!!!!
            RadiantHeart = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.LifeCrystal, BlessingID.RadiantHeart, Language.GetText($"{CommonBM_Key}.RadiantHeart.Message"), Language.GetText($"{CommonBM_Key}.RadiantHeart.Bonus"), Language.GetText($"{CommonBM_Key}.RadiantHeart.Name"));

            Enchanted = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.ManaCrystal, BlessingID.Enchanted, Language.GetText($"{CommonBM_Key}.Enchanted.Message"), Language.GetText($"{CommonBM_Key}.Enchanted.Bonus"), Language.GetText($"{CommonBM_Key}.Enchanted.Name"));

            Attuned = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.NaturesGift, BlessingID.Attuned, Language.GetText($"{CommonBM_Key}.Attuned.Message"), Language.GetText($"{CommonBM_Key}.Attuned.Bonus"), Language.GetText($"{CommonBM_Key}.Attuned.Name"));

            OozingAffection = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.BottledHoney, BlessingID.OozingAffection, Language.GetText($"{CommonBM_Key}.OozingAffection.Message"), Language.GetText($"{CommonBM_Key}.OozingAffection.Bonus"), Language.GetText($"{CommonBM_Key}.OozingAffection.Name"));

            Serenity = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.Starfish, BlessingID.Serenity, Language.GetText($"{CommonBM_Key}.Serenity.Message"), Language.GetText($"{CommonBM_Key}.Serenity.Bonus"), Language.GetText($"{CommonBM_Key}.Serenity.Name"));

            ThrivingDarknessCorr = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.ShadowScale, BlessingID.ThrivingDarknessCorr, Language.GetText($"{CommonBM_Key}.ThrivingDarkness.Message"), Language.GetText($"{CommonBM_Key}.ThrivingDarkness.Bonus"), Language.GetText($"{CommonBM_Key}.ThrivingDarkness.Name"));

            ThrivingDarknessCrim = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.TissueSample, BlessingID.ThrivingDarknessCrim, Language.GetText($"{CommonBM_Key}.ThrivingDarkness.Message"), Language.GetText($"{CommonBM_Key}.ThrivingDarkness.Bonus"), Language.GetText($"{CommonBM_Key}.ThrivingDarkness.Name"));

            Decadence = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.Ambrosia, BlessingID.Decadence, Language.GetText($"{CommonBM_Key}.Decadence.Message"), Language.GetText($"{CommonBM_Key}.Decadence.Bonus"), Language.GetText($"{CommonBM_Key}.Decadence.Name"));

            Overgrown = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.JungleSpores, BlessingID.Overgrown, Language.GetText($"{CommonBM_Key}.Overgrown.Message"), Language.GetText($"{CommonBM_Key}.Overgrown.Bonus"), Language.GetText($"{CommonBM_Key}.Overgrown.Name"));

            MilkywayStride = new Blessing(PrayerID.Regen, ItemID.Daybloom, ModContent.ItemType<StellarMatter>(), BlessingID.MilkywayStride, Language.GetText($"{CommonBM_Key}.MilkywayStride.Message"), Language.GetText($"{CommonBM_Key}.MilkywayStride.Bonus"), Language.GetText($"{CommonBM_Key}.MilkywayStride.Name"));

            RejuvenatingWarmth = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.HellstoneBar, BlessingID.RejuvenatingWarmth, Language.GetText($"{CommonBM_Key}.RejuvenatingWarmth.Message"), Language.GetText($"{CommonBM_Key}.RejuvenatingWarmth.Bonus"), Language.GetText($"{CommonBM_Key}.RejuvenatingWarmth.Name"));

            //RELOADIIING!!
            ScarletSurge = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.LifeCrystal, BlessingID.ScarletSurge, Language.GetText($"{CommonBM_Key}.ScarletSurge.Message"), Language.GetText($"{CommonBM_Key}.ScarletSurge.Bonus"), Language.GetText($"{CommonBM_Key}.ScarletSurge.Name"));

            Clarity = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.ManaCrystal, BlessingID.Clarity, Language.GetText($"{CommonBM_Key}.Clarity.Message"), Language.GetText($"{CommonBM_Key}.Clarity.Bonus"), Language.GetText($"{CommonBM_Key}.Clarity.Name"));

            Tempered = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.NaturesGift, BlessingID.Tempered, Language.GetText($"{CommonBM_Key}.Tempered.Message"), Language.GetText($"{CommonBM_Key}.Tempered.Bonus"), Language.GetText($"{CommonBM_Key}.Tempered.Name"));

            Alliegance = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.BottledHoney, BlessingID.Alliegance, Language.GetText($"{CommonBM_Key}.Alliegance.Message"), Language.GetText($"{CommonBM_Key}.Alliegance.Bonus"), Language.GetText($"{CommonBM_Key}.Alliegance.Name"));

            Waywashed = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.Starfish, BlessingID.Waywashed, Language.GetText($"{CommonBM_Key}.Waywashed.Message"), Language.GetText($"{CommonBM_Key}.Waywashed.Bonus"), Language.GetText($"{CommonBM_Key}.Waywashed.Name"));

            SeethingCrim = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.TissueSample, BlessingID.Seething, Language.GetText($"{CommonBM_Key}.Seething.Message"), Language.GetText($"{CommonBM_Key}.Seething.Bonus"), Language.GetText($"{CommonBM_Key}.Seething.Name"));

            SeethingCorr = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.ShadowScale, BlessingID.Seething, Language.GetText($"{CommonBM_Key}.Seething.Message"), Language.GetText($"{CommonBM_Key}.Seething.Bonus"), Language.GetText($"{CommonBM_Key}.Seething.Name"));

            Bulk = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.Ambrosia, BlessingID.Bulk, Language.GetText($"{CommonBM_Key}.Bulk.Message"), Language.GetText($"{CommonBM_Key}.Bulk.Bonus"), Language.GetText($"{CommonBM_Key}.Bulk.Name"));

            LifeStricken = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.JungleSpores, BlessingID.LifeStriken, Language.GetText($"{CommonBM_Key}.LifeStricken.Message"), Language.GetText($"{CommonBM_Key}.LifeStricken.Bonus"), Language.GetText($"{CommonBM_Key}.LifeStricken.Name"));

            FieryPassion = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ItemID.HellstoneBar, BlessingID.LifeStriken, Language.GetText($"{CommonBM_Key}.FieryPassion.Message"), Language.GetText($"{CommonBM_Key}.FieryPassion.Bonus"), Language.GetText($"{CommonBM_Key}.FieryPassion.Name"));

            StarScorn = new Blessing(PrayerID.DamageOutput, ItemID.Moonglow, ModContent.ItemType<StellarMatter>(), BlessingID.StarScorn, Language.GetText($"{CommonBM_Key}.StarScorn.Message"), Language.GetText($"{CommonBM_Key}.StarScorn.Bonus"), Language.GetText($"{CommonBM_Key}.StarScorn.Name"));

            // Register all blessings
            AllBlessings.Clear();
            RegisterBlessing(RadiantHeart);
            RegisterBlessing(Enchanted);
            RegisterBlessing(Attuned);
            RegisterBlessing(OozingAffection);
            RegisterBlessing(Serenity);
            RegisterBlessing(ThrivingDarknessCorr);
            RegisterBlessing(ThrivingDarknessCrim);
            RegisterBlessing(Decadence);
            RegisterBlessing(Overgrown);
            RegisterBlessing(RejuvenatingWarmth);
            RegisterBlessing(MilkywayStride);
            RegisterBlessing(ScarletSurge);
            RegisterBlessing(Clarity);
            RegisterBlessing(Tempered);
            RegisterBlessing(Alliegance);
            RegisterBlessing(Waywashed);
            RegisterBlessing(SeethingCorr);
            RegisterBlessing(SeethingCrim);
            RegisterBlessing(Bulk);
            RegisterBlessing(LifeStricken);
            RegisterBlessing(FieryPassion);
            RegisterBlessing(StarScorn);

            //Are you starting to understand why I hate how the blessings are currently implemented?
        }

        public override void Unload()
        {
            RadiantHeart = null;
            Enchanted = null;
            Attuned = null;
            OozingAffection = null;
            Serenity = null;
            ThrivingDarknessCorr = null;
            ThrivingDarknessCrim = null;
            Decadence = null;
            Overgrown = null;
            RejuvenatingWarmth = null;
            MilkywayStride = null;
            ScarletSurge = null;
            Clarity = null;
            Tempered = null;
            Alliegance = null;
            Waywashed = null;
            SeethingCorr = null;
            SeethingCrim = null;
            Bulk = null;
            LifeStricken = null;
            FieryPassion = null;
            StarScorn = null;

            //Never hurts to clear.
            AllBlessings.Clear();
        }

        public static readonly List<Blessing> AllBlessings = new();
    }

    public class BlessingLoader : ModSystem
    {
        //This class is a one-off, and I only had to do this because the compiler was ignoring these two on account of using ModItems as their components.
        public override void SetStaticDefaults()
        {
            foreach (Blessing blessing in DTBlessings.AllBlessings)
            {
                if (blessing == DTBlessings.MilkywayStride)
                {
                    blessing.ItemType = ModContent.ItemType<StellarMatter>();
                }
                if (blessing == DTBlessings.StarScorn)
                {
                    blessing.ItemType = ModContent.ItemType<StellarMatter>();
                }
            }
        }
    }

    //This is also AI, but I may keep it in a different form in the future.
    //I still am not quite sure how Blessings will be reimplemented.
    public static class BlessingID
    {
        public const short None = -1;

        //REGEN
        public const short RadiantHeart = 0;
        public const short Enchanted = 1;
        public const short Attuned = 2;
        public const short OozingAffection = 3;
        public const short Serenity = 4;
        public const short ThrivingDarknessCorr = 5;
        public const short ThrivingDarknessCrim = 6;
        public const short Decadence = 7;
        public const short Overgrown = 8;
        public const short MilkywayStride = 9;
        public const short RejuvenatingWarmth = 10;

        //DAMAGE
        public const short ScarletSurge = 11;
        public const short Clarity = 12;
        public const short Tempered = 13;
        public const short Alliegance = 14;
        public const short Waywashed = 15;
        public const short Seething = 16;
        public const short Bulk = 17;
        public const short LifeStriken = 18;
        public const short FieryPassion = 19;
        public const short StarScorn = 20;
    }


    public class Blessing
    {
        public int ItemType = 0;
        public int PrayerType = PrayerID.None;
        public int HerbType = 0;
        public string BlessingName = "";
        public string BlessingMessage = "";
        public string BlessingBonus = "";
        public short ID = BlessingID.None;

        /// <summary>
        /// Blessings, currently, consist of the following:
        /// <br/> - An Item, known as the "offering".
        /// <br/> - A second Item, known as the "modifier", which will always be some kind of herb. Hence the parameter is herbType.
        /// <br/> - A PrayerID for the blessing. I hate it too, don't worry.
        /// <br/> - A Message to display on the player when they recieve it.
        /// <br/> - A tooltip to show in the blessing buff.
        /// <br/> - A name to show in the blessing buff and elsewhere.
        /// </summary>
        public Blessing(int prayerType, int herbType, int itemType, short id)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            ID = id;
        }

        /// <summary>
        /// Blessings, currently, consist of the following:
        /// <br/> - An Item, known as the "offering".
        /// <br/> - A second Item, known as the "modifier", which will always be some kind of herb. Hence the parameter is herbType.
        /// <br/> - A PrayerID for the blessing. I hate it too, don't worry.
        /// <br/> - A Message to display on the player when they recieve it.
        /// <br/> - A tooltip to show in the blessing buff.
        /// <br/> - A name to show in the blessing buff and elsewhere.
        /// </summary>
        public Blessing(int prayerType, int herbType, int itemType, short id, LocalizedText blessingMessage)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            BlessingMessage = blessingMessage.Value;
            ID = id;
        }

        /// <summary>
        /// Blessings, currently, consist of the following:
        /// <br/> - An Item, known as the "offering".
        /// <br/> - A second Item, known as the "modifier", which will always be some kind of herb. Hence the parameter is herbType.
        /// <br/> - A PrayerID for the blessing. I hate it too, don't worry.
        /// <br/> - A Message to display on the player when they recieve it.
        /// <br/> - A tooltip to show in the blessing buff.
        /// <br/> - A name to show in the blessing buff and elsewhere.
        /// </summary>
        public Blessing(int prayerType, int herbType, int itemType, short id, LocalizedText blessingMessage, LocalizedText blessingBonus)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            BlessingMessage = blessingMessage.Value;
            BlessingBonus = blessingBonus.Value;
            ID = id;
        }

        /// <summary>
        /// Blessings, currently, consist of the following:
        /// <br/> - An Item, known as the "offering".
        /// <br/> - A second Item, known as the "modifier", which will always be some kind of herb. Hence the parameter is herbType.
        /// <br/> - A PrayerID for the blessing. I hate it too, don't worry.
        /// <br/> - A Message to display on the player when they recieve it.
        /// <br/> - A tooltip to show in the blessing buff.
        /// <br/> - A name to show in the blessing buff and elsewhere.
        /// </summary>
        public Blessing(int prayerType, int herbType, int itemType, short id, LocalizedText blessingMessage, LocalizedText blessingBonus, LocalizedText blessingName)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            BlessingMessage = blessingMessage.Value;
            BlessingBonus = blessingBonus.Value;
            BlessingName = blessingName.Value;
            ID = id;
        }
    }
}