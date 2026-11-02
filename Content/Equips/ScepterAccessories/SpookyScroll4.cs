using Terraria;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class SpookyScroll4 : LateHardmodeScroll
    {

        public override void SetDefaults()
        {
            Item.width = 32; // Width of the item
            Item.height = 30; // Height of the item
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.TryGetGlobalProjectile<ScrollScepterProj>(out ScrollScepterProj Scptr))
                {
                    Scptr.SpookyScroll4 = true;
                }
            }
        }
    }
}
