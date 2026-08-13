using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Common
{
    public class DTConfig : ModConfig
    {
        public static DTConfig instance = ModContent.GetInstance<DTConfig>();
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool MinionExtrasToggle { get; set; }

        [DefaultValue(false)]
        public bool EnableDebugMessages { get; set; }

        [DefaultValue(true)]
        public bool DragCamera { get; set; }

        [DefaultValue(true)]
        public bool MinionAmmoReplace { get; set; }

        [DefaultValue(true)]
        public bool WeaponKickback { get; set; }

        [DefaultValue(true)]
        public bool BlessingVFX { get; set; }

        [DefaultValue(true)]
        public bool ScreenshakeEffects { get; set; }

        /// <summary>
        /// The cooldown timer for triggering effects for Thrown Scepter tile collisions.
        /// <br/> Has a minimum of half a second and a maximum of 20 seconds.
        /// </summary>
        [Range(30, 1200)]
        [DefaultValue(30f)]
        public int ScepterTileCollsionsCooldown { get; set; }

        /// <summary>
        /// The cooldown timer for triggering effects for Thrown Scepter tile collisions.
        /// <br/> Has a minimum of half a second and a maximum of 20 seconds.
        /// </summary>
        [Range(120, 1200)]
        [DefaultValue(120f)]
        public int ScrollEffectsCooldown { get; set; }
        
    }

    public class DTOptimizationsConfig : ModConfig
    {
        public static DTOptimizationsConfig instance = ModContent.GetInstance<DTOptimizationsConfig>();
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [DefaultValue(false)]
        public bool OptimizeGame { get; set; }
        [DefaultValue(false)]
        public bool DisableExcessDusts { get; set; }
        [DefaultValue(false)]
        public bool DisableExcessParticles { get; set; }
        [DefaultValue(false)]
        public bool DisableExcessTrails { get; set; }
    }

    public class DTMusicConfig : ModConfig
    {
        public static DTMusicConfig instance = ModContent.GetInstance<DTMusicConfig>();
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool NodeIdleMusic { get; set; }

        [DefaultValue(true)]
        public bool EternityMusic { get; set; }

        [DefaultValue(true)]
        public bool ReplaceVanillaTracks { get; set; }
    }

    public class DTUIConfig : ModConfig
    {
        public static DTUIConfig instance = ModContent.GetInstance<DTUIConfig>();
        public override ConfigScope Mode => ConfigScope.ClientSide;
        /// <summary>
        /// An additive value to the default position. Is not absolute, so setting this to a negative will move it to the left.
        /// </summary>
        [Range(-1800f, 1800f)]
        [DefaultValue(0f)]
        public float RiftBarXPos { get; set; }

        /// <summary>
        /// An additive value to the default position. Is not absolute, so setting this to a negative will move it up.
        /// </summary>
        [Range(-1800f, 1800f)]
        [DefaultValue(0f)]
        public float RiftBarYPos { get; set; }

        /// <summary>
        /// An additive value to the default position. Is not absolute, so setting this to a negative will move it to the left.
        /// </summary>
        [Range(-1800f, 1800f)]
        [DefaultValue(0f)]
        public float NightBarXPos { get; set; }
        /// <summary>
        /// An additive value to the default position. Is not absolute, so setting this to a negative will move it up.
        /// </summary>

        [Range(-1800f, 1800f)]
        [DefaultValue(0f)]
        public float NightBarYPos { get; set; }

        /// <summary>
        /// A multiplier on the scale of custom boss bars.
        /// </summary>

        [Range(1f, 10f)]
        [DefaultValue(2.5f)]
        public float CustomBossBarScaleModifier { get; set; }
    }
    
    public class DTConfigDataModifications : ModSystem
    {
        public DTConfig genConfig = ModContent.GetInstance<DTConfig>();
        public DTOptimizationsConfig optConfig = ModContent.GetInstance<DTOptimizationsConfig>();
        public DTMusicConfig musConfig = ModContent.GetInstance<DTMusicConfig>();
        public DTUIConfig UIConfig = ModContent.GetInstance<DTUIConfig>();
        public override void PreUpdateTime()
        {
        }


    }
}
