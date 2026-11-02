using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class RGBSlash : LinearSlash
    {
        public override string Texture => "DestroyerTest/Content/Extras/144Slash";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Blending = true;
            themeColor = Main.DiscoColor;
            DustType = DustID.FireworksRGB;
            DustUsesColor = true;
            DustScale = 3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 600);
        }
    }
}