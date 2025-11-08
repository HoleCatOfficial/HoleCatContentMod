
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class ChristmasCarol2 : LateHardmodeScroll
    {
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
                    Scptr.ChristmasScroll2 = true;
                }
            }
        }
    }
}