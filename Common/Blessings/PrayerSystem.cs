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

            //CombatText.NewText(Player.Hitbox, ColorLib.Soul, IncomingBlessing.BlessingMessage, true, false);
            
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
        public static Blessing RadiantHeart = new Blessing(PrayerID.Regen, ItemID.Daybloom, ItemID.LifeCrystal, Language.GetText($"{CommonBM_Key}.RadiantHeart.Message"), Language.GetText($"{CommonBM_Key}.RadiantHeart.Bonus"));
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