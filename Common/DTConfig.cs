using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Common
{
    public class DTConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool MinionExtrasToggle { get; set; }

        [DefaultValue(false)]
        public bool UnnerfTenebrousKatana { get; set; }

        [DefaultValue(false)]
        public bool EnableDebugMessages { get; set; }

        [DefaultValue(true)]
        public bool ShowBugCommandMessage { get; set; }
        
    }

    public class DTOptimizationsConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [DefaultValue(false)]
        public bool OptimizeGame { get; set; }
        [DefaultValue(false)]
        public bool DisableExcessDusts { get; set; }
        [DefaultValue(false)]
        public bool DisableExcessParticles { get; set; }
    }

    public class DTMusicConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool NodeIdleMusic { get; set; }

        [DefaultValue(false)]
        public bool EternityMusic { get; set; }
    }

    public class DTUIConfig : ModConfig
    {
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
