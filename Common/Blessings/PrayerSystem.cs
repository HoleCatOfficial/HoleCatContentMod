using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using InnoVault.PRT;
using OpusLib;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Resources;
using System;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.MeleeWeapons;

namespace DestroyerTest.Common.Blessings
{
    public class PrayerSystem : ModSystem
    {
        
    }

    public class PrayerPlayer : ModPlayer
    {
        public Blessing? CurrentBlessing = null;

        public bool VFX => DTConfig.instance.BlessingVFX;

        public void ApplyBlessing(Blessing IncomingBlessing)
        {
            if (IncomingBlessing == null)
                return;

            CurrentBlessing = IncomingBlessing;

            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BlessingParticle>(), Player.Center, Vector2.Zero, ColorLib.Soul, 0.01f, 2f);
            Opus.RadialParticleRandomDir(PRTLoader.GetParticleID<HallowedPallStar>(), 10, Player.Center, 1, ColorLib.Soul, 1f, 2.75f);

            CombatText.NewText(Player.Hitbox, ColorLib.Soul, IncomingBlessing.BlessingMessage, true, false);
        }

        public void RejectOffer()
        {
            
        }

        public void DisplayBlessingMessage()
        {

        }

        public override void PostUpdateMiscEffects()
        {
            DTBlessings B = ModContent.GetInstance<DTBlessings>();
            if (!IsABlessingActive())
            {
                Player.ClearBuff(ModContent.BuffType<BlessingBuff>());
                return;
            }
            else
            {
                Player.AddBuff(ModContent.BuffType<BlessingBuff>(), 20);
            }

            if (VFX)
            {
                switch (CurrentBlessing.ID)
                {
                    case (BlessingID.RadiantHeart):
                        {
                            if (Main.rand.NextBool(14))
                            {
                                Dust D = Player.QuickDust(DustID.LifeDrain, Main.rand.NextVector2Circular(2f, 2f), 100);
                                D.noGravity = true;

                            }
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
                            Player.flowerBoots = true;
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
                }
            }

            UpdateMana();
        }

        public void UpdateMana()
        {
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

        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (CurrentBlessing == DTBlessings.OozingAffection)
            {
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item97, Player.Center);
                    Opus.RadialProjectileRandomDir(ProjectileID.Bee, 7, Player.Center, 10, 10, 5, friendly: true);
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
                    Opus.RadialProjectileRandomDir(ProjectileID.Bee, 7, Player.Center, 10, 10, 5, friendly: true);
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

    public class DTBlessings
    {
        public const string CommonBM_Key = "Mods.DestroyerTest.Blessings";

        //Regen Blessings
        public static Blessing RadiantHeart = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.LifeCrystal, BlessingID.RadiantHeart, Language.GetText($"{CommonBM_Key}.RadiantHeart.Message"), Language.GetText($"{CommonBM_Key}.RadiantHeart.Bonus"), Language.GetText($"{CommonBM_Key}.RadiantHeart.Name"));
        public static Blessing Enchanted = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.ManaCrystal, BlessingID.Enchanted, Language.GetText($"{CommonBM_Key}.Enchanted.Message"), Language.GetText($"{CommonBM_Key}.Enchanted.Bonus"), Language.GetText($"{CommonBM_Key}.Enchanted.Name"));
        public static Blessing Attuned = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.NaturesGift, BlessingID.Attuned, Language.GetText($"{CommonBM_Key}.Attuned.Message"), Language.GetText($"{CommonBM_Key}.Attuned.Bonus"), Language.GetText($"{CommonBM_Key}.Attuned.Name"));
        public static Blessing OozingAffection = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.BottledHoney, BlessingID.OozingAffection, Language.GetText($"{CommonBM_Key}.OozingAffection.Message"), Language.GetText($"{CommonBM_Key}.OozingAffection.Bonus"), Language.GetText($"{CommonBM_Key}.OozingAffection.Name"));
        public static Blessing Serenity = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.Starfish, BlessingID.Serenity, Language.GetText($"{CommonBM_Key}.Serenity.Message"), Language.GetText($"{CommonBM_Key}.Serenity.Bonus"), Language.GetText($"{CommonBM_Key}.Serenity.Name"));
        public static Blessing ThrivingDarknessCorr = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.ShadowScale, BlessingID.ThrivingDarknessCorr, Language.GetText($"{CommonBM_Key}.EvilVitality.Message"), Language.GetText($"{CommonBM_Key}.EvilVitality.Bonus"), Language.GetText($"{CommonBM_Key}.EvilVitality.Name"));
        public static Blessing ThrivingDarknessCrim = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.TissueSample, BlessingID.ThrivingDarknessCrim, Language.GetText($"{CommonBM_Key}.EvilVitality.Message"), Language.GetText($"{CommonBM_Key}.EvilVitality.Bonus"), Language.GetText($"{CommonBM_Key}.EvilVitality.Name"));
        public static Blessing Decadence = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.Ambrosia, BlessingID.Decadence, Language.GetText($"{CommonBM_Key}.Decadence.Message"), Language.GetText($"{CommonBM_Key}.Decadence.Bonus"), Language.GetText($"{CommonBM_Key}.Decadence.Name"));
        public static Blessing Overgrown = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.JungleSpores, BlessingID.Overgrown, Language.GetText($"{CommonBM_Key}.Overgrown.Message"), Language.GetText($"{CommonBM_Key}.Overgrown.Bonus"), Language.GetText($"{CommonBM_Key}.Overgrown.Name"));
        public static Blessing MilkywayStride = new Blessing(PrayerID.Regen, ItemID.Daybloom, ModContent.ItemType<StellarMatter>(), BlessingID.MilkywayStride, Language.GetText($"{CommonBM_Key}.MilkywayStride.Message"), Language.GetText($"{CommonBM_Key}.MilkywayStride.Bonus"), Language.GetText($"{CommonBM_Key}.MilkywayStride.Name"));
        public static Blessing RejuvenatingWarmth = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.HellstoneBar, BlessingID.RejuvenatingWarmth, Language.GetText($"{CommonBM_Key}.RejuvenatingWarmth.Message"), Language.GetText($"{CommonBM_Key}.RejuvenatingWarmth.Bonus"), Language.GetText($"{CommonBM_Key}.RejuvenatingWarmth.Name"));

        //Damage Blessings
        
    
    }

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
        public Blessing(int prayerType, int herbType, int itemType, short id)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            ID = id;
        }

        public Blessing(int prayerType, int herbType, int itemType, short id, LocalizedText blessingMessage)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            BlessingMessage = blessingMessage.Value;
            ID = id;
        }

        public Blessing(int prayerType, int herbType, int itemType, short id, LocalizedText blessingMessage, LocalizedText blessingBonus)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            BlessingMessage = blessingMessage.Value;
            BlessingBonus = blessingBonus.Value;
            ID = id;
        }

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