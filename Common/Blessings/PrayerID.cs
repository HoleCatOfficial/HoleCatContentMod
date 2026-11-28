namespace DestroyerTest.Common.Blessings
{
    /// <summary>
    /// OBSOLETE
    /// Due to frustrations with how blessings were organized and accessed, PrayerIDs were introduced.
    /// PrayerIDs as a concept are entirely AI, and when I reinvent this system into something else, such a thing will not exist.
    /// K1
    /// </summary>
    public class HerbID
    {
        public const short None = 0;
        public const short Daybloom = 1;
        public const short Moonglow = 2;
        public const short Blinkroot = 3;
        public const short Deathweed = 5;
        public const short Waterleaf = 6;
        public const short Fireblossom = 7;
        public const short Shiverthorn = 8;
    }
    public class PrayerID
    {
        public const short None = 0;
        public const short Regen = 1;
        public const short Miscellaneous = 2;
        public const short DefenseOrDamageReduction = 3;
        public const short DamageOutput = 4;
        public const short DebuffImmunities = 5;
        public const short MoveSpeed = 6;
        public const short DamageReduction = 7;
    }
}