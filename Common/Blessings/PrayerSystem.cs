using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;

namespace DestroyerTest.Common.Blessings
{
    public class PrayerSystem : ModSystem
    {
        
    }

    public class PrayerPlayer : ModPlayer
    {
        public Blessing? CurrentBlessing = null;

        public override void PostUpdateMiscEffects()
        {
            DTBlessings B = ModContent.GetInstance<DTBlessings>();
            if (!IsABlessingActive())
            {
                return;
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
        public static Blessing RadiantHeart = new Blessing(PrayerID.Regen, HerbID.Daybloom, ItemID.LifeCrystal, Language.GetText($"{CommonBM_Key}.RadiantHeart.Message"));
    }

    public class Blessing
    {
        public int ItemType = 0;
        public int PrayerType = PrayerID.None;
        public int HerbType = HerbID.None;
        public string BlessingMessage = "";
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
    }
}