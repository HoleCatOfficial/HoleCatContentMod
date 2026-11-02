
using DestroyerTest.Common;
using Terraria;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class CursedFlameScroll : LateHardmodeScroll
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.TryGetGlobalProjectile<ScrollScepterProj>(out ScrollScepterProj Scptr))
                {
                    Scptr.CursedFlameScroll = true;
                }
            }
        }
    }
}