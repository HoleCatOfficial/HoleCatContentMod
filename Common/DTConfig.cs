using System.ComponentModel;
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

        [DefaultValue(true)]
        public bool NodeIdleMusic { get; set; }

        [DefaultValue(false)]
        public bool OptimizeGame { get; set; }
        [DefaultValue(false)]
        public bool EternityMusic { get; set; }
    }
}
