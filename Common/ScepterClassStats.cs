using System.Collections.Generic;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    /// <summary>
	/// All the stats that are editable for the <b>Scepter Damage</b> class.
	/// These only apply to items with the <b>Scepter Damage</b> class.
    /// 
    /// <para/> <b>DamageModifier</b> - A value that can be operated on to alter the amount of damage dealt by the projectiles. This does not change the damage dealt by the weapons.
    /// <para/> <b>Range</b> - An integer value that can be altered to increase or decrease the range of a thrown projectile (how far it goes before returning to the player).  
    /// <para/> <b>Note:</b> Because the thrown projectiles run on a timer, range value measures in <b>In-Game ticks</b>. Also, it is recommended to use addition and subtraction for range. It defaults to <b>0</b>, so if you try to divide or multiply, it will cause problems.
    /// <para/> <b>SizeModifier</b> - This float controls the size of the thrown projectiles. This value can tend to be finicky due to the scepters' tile-bouncing AI stopping them from properly reaching the player past a certain size.
    /// <para/> <b>ManaBurstPower</b> - This integer is for a not-yet-implemented mechanic involving the player's mana. This currently does nothing.
	/// </summary>
	public class ScepterClassStats
    {   
        /// <summary>
        /// A value that can be operated on to alter the amount of damage dealt by the projectiles. This does not change the damage dealt by the weapons. Be careful with this value though, as most damage in terrar
        /// </summary>
        public static float DamageModifier { get; set; } = 1.0f;

        /// <summary>
        /// An integer modifier affecting how far a scepter will fly before automatically returning to the player.
        /// <para/> The formula is as follows: 1f + (ScepterClassStats.Range * 0.01f). As you can observe, it is impossible to decrease the range, not that it would be useful to. For debuffs you can just decrease the effectiveness of the range multiplier.
        /// <para/><b>Note:</b> Because the thrown projectiles run on a timer, range value measures in <b>In-Game ticks</b>. 
        /// </summary>
        public static int Range { get; set; } = 0;

        /// <summary>
        /// This float controls the size of the thrown projectiles. This value can tend to be finicky due to the scepters' tile-bouncing AI stopping them from properly reaching the player past a certain size.
        /// <para/><b>Note:</b> This value is used best as an additive or subtractive value, due to being an integer. If you want a precise multiplication, use SizeMultiplier, SizeModifier's younger cousin.
        /// </summary>
        public static int SizeModifier { get; set; } = 1;

        public static float SizeMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// This integer is for a not-yet-implemented mechanic involving the player's mana. This currently does nothing.
        /// </summary>
        public static int ManaBurstPower { get; set; } = 0;

        /// <summary>
        /// The scepter class has boolean values for certain accessories to more easily determine associated behavior. Some Accessories only alter stats, while others introduce new behaviors and projectiles.
        /// <para/> This boolean is for the Blood Vial accessory.
        /// </summary>
        public static bool BloodVialItem { get; set; } = false;

        /// <summary>
        /// The scepter class has boolean values for certain accessories to more easily determine associated behavior. Some Accessories only alter stats, while others introduce new behaviors and projectiles.
        /// <para/> This boolean is for the Vile Cyst accessory.
        /// </summary>
        public static bool VileCystItem { get; set; } = false;
        /// <summary>
        /// A multiplicative modifer that affects how fast thrown scepters travel.
        /// </summary>
        public static float ThrowSpeedModifier { get; set; } = 1f;

        /// <summary>
        /// An additive modifier that increases or decreases the amount of times a shot will bounce, if it is set to bounce.
        /// </summary>
        public static int ShotBounceModifier { get; set; } = 0;
    }

    public static class ScepterRegistry
    {
        /// <summary>
        /// Some Thrown Scepters do not inherit from ThrownScepter. For effects on thrown scepters, we can add projectiles to the list to extend the effects to them as well.
        /// <br/> Compiled during loading.
        /// </summary>
        public static List<Projectile> AllThrownScepters = new List<Projectile>
        {
            
        };

        /// <summary>
        /// Some Thrown Scepters do not inherit from ThrownScepter. For effects on thrown scepters, we can add projectiles to the list to extend the effects to them as well.
        /// <br/> Compiled during loading.
        /// </summary>
        public static List<Projectile> AllScepterShots = new List<Projectile>
        {
            
        };

        /// <summary>
        /// This is for effects that exclusively apply to the ThrownScepter class and its children.
        /// <br/> Scepters that do not inherit will not be affected.
        /// <br/> Compiled during loading.
        /// </summary>
        public static List<Projectile> DirectInheritanceThrownScepters = new List<Projectile>
        {

        };

        /// <summary>
        /// This is for effects that exclusively apply to the ThrownScepter class and its children.
        /// <br/> Scepters that do not inherit will not be affected.
        /// <br/> Compiled during loading.
        /// </summary>
        public static List<Projectile> DirectInheritanceScepterShots = new List<Projectile>
        {

        };
    }

    public class ScepterClassStatResetPlayer : ModPlayer
    {
        public override void ResetEffects()
        {
            ScepterClassStats.Range = 0;
            ScepterClassStats.SizeModifier = 1;
            ScepterClassStats.SizeMultiplier = 1;
            ScepterClassStats.DamageModifier = 1;
            ScepterClassStats.ManaBurstPower = 0;
            ScepterClassStats.ThrowSpeedModifier = 1f;
            ScepterClassStats.BloodVialItem = false;
            ScepterClassStats.VileCystItem = false;
        }

    }

    
}
