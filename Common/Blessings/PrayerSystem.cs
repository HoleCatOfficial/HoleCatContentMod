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

namespace DestroyerTest.Common.Blessings
{
    public class PrayerSystem : ModSystem
    {
        
    }

    public class PrayerPlayer : ModPlayer
    {
        public Blessing? CurrentBlessing = null;

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
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BlessingParticle>(), Player.Center, Vector2.Zero, Color.Violet, 0.01f, 2f);
            CombatText.NewText(Player.Hitbox, Color.Violet, Language.GetTextValue("Mods.DestroyerTest.Blessings.RejectedMessage"), true, false);
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

            UpdateMana();
        }

        public void UpdateMana()
        {
            if (CurrentBlessing == DTBlessings.Enchanted)
            {
                Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.ManaRegeneration, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.1f, 0.1f), 50, default, 0.75f);
                Player.manaRegenBonus = 30;
            }
        }

        public override void PostUpdateRunSpeeds()
        {
            if (CurrentBlessing == DTBlessings.Persistence)
            {
                Player.moveSpeed += 0.15f;
            }
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            DTBlessings B = ModContent.GetInstance<DTBlessings>();
            if (!IsABlessingActive())
            {
                return;
            }
            if (CurrentBlessing == DTBlessings.RadiantHeart)
            {
                regen *= 1.4f;
            }
            if (CurrentBlessing == DTBlessings.OozingAffection)
            {
                if (Player.velocity.Length() < 1.5f)
                {
                    Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Honey, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.1f, 0.1f), 50, default, 0.75f);
                    regen *= 1.5f;
                }
            }
            if (CurrentBlessing == DTBlessings.Persistence)
            {
                if (Player.velocity.Length() > 5)
                {
                    Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<ColorableNeonDust>(), Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.1f, 0.1f), 50, Color.SkyBlue, 0.75f);
                    regen *= 1.2f;
                    Player.statDefense -= 10;
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
        public static string CommonBM_Key = "Mods.DestroyerTest.Blessings";

        //Regen Blessings
        public static Blessing RadiantHeart = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.LifeCrystal, Language.GetText($"{CommonBM_Key}.RadiantHeart.Message"), Language.GetText($"{CommonBM_Key}.RadiantHeart.Bonus"), Language.GetText($"{CommonBM_Key}.RadiantHeart.Name"));
        public static Blessing OozingAffection = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.BottledHoney, Language.GetText($"{CommonBM_Key}.OozingAffection.Message"), Language.GetText($"{CommonBM_Key}.OozingAffection.Bonus"), Language.GetText($"{CommonBM_Key}.OozingAffection.Name"));
        public static Blessing Persistence = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.HermesBoots, Language.GetText($"{CommonBM_Key}.Persistence.Message"), Language.GetText($"{CommonBM_Key}.Persistence.Bonus"), Language.GetText($"{CommonBM_Key}.Persistence.Name"));
        public static Blessing Enchanted = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.ManaCrystal, Language.GetText($"{CommonBM_Key}.Enchanted.Message"), Language.GetText($"{CommonBM_Key}.Enchanted.Bonus"), Language.GetText($"{CommonBM_Key}.Enchanted.Name"));

        //Damage Blessings
        public static Blessing Temperance = new Blessing(PrayerID.DamageOutput, ItemID.Deathweed, ItemID.IronHammer, Language.GetText($"{CommonBM_Key}.Temperance.Message"), Language.GetText($"{CommonBM_Key}.Temperance.Bonus"), Language.GetText($"{CommonBM_Key}.Temperance.Name"));
    
    }

    public class Blessing
    {
        public int ItemType = 0;
        public int PrayerType = PrayerID.None;
        public int HerbType = 0;
        public string BlessingName = "";
        public string BlessingMessage = "";
        public string BlessingBonus = "";
        public Blessing(int prayerType, int herbType, int itemType)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
        }

        public Blessing(int prayerType, int herbType, int itemType, LocalizedText blessingMessage)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            BlessingMessage = blessingMessage.Value;
        }

        public Blessing(int prayerType, int herbType, int itemType, LocalizedText blessingMessage, LocalizedText blessingBonus)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            BlessingMessage = blessingMessage.Value;
            BlessingBonus = blessingBonus.Value;
        }

        public Blessing(int prayerType, int herbType, int itemType, LocalizedText blessingMessage, LocalizedText blessingBonus, LocalizedText blessingName)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
            BlessingMessage = blessingMessage.Value;
            BlessingBonus = blessingBonus.Value;
            BlessingName = blessingName.Value;
        }
    }
}