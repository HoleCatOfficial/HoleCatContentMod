using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            if (!IsAPrayerActive())
            {
                return;
            }

            
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            DTBlessings B = ModContent.GetInstance<DTBlessings>();
            if (!IsAPrayerActive())
            {
                return;
            }
            if (CurrentBlessing == B.RadiantHeart)
            {
                regen *= 1.4f;
            }
        }

        public bool IsAPrayerActive()
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
        public Blessing RadiantHeart = new Blessing(PrayerID.Regen, HerbID.Daybloom, ItemID.LifeCrystal);
    }

    public class Blessing
    {
        public int ItemType = 0;
        public int PrayerType = PrayerID.None;
        public int HerbType = HerbID.None;
        public Blessing(int prayerType, int herbType, int itemType)
        {
            PrayerType = prayerType;
            HerbType = herbType;
            ItemType = itemType;
        }
    }
}