using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems
{
    public class CustomSwordMinion : SwordMinionTemplate
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public CustomSwordMinion()
        {
            ThemeColor = Color.Red;
            TintColor = Color.Gold;
            IdleDustType = DustID.Gold;
            DashDustType = DustID.Torch;
            TeleDustType = DustID.Silver;
            TeleSound = SoundID.Item4;
            DashSound = SoundID.Item1;
            AfterImageColorless = false;
            AfterImageTinted = true;
            AfterImage = true;
            DefaultDraw = true;
            TickSpeed = 3;
            UsesParticleOrchestratorOnTele = false;
            TeleDist = 1500;
            Range = 1500;
            Style = IdleStyle.Defensive;
        }
    }
}
