using Terraria;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class SnowSurgeScroll : EarlyHardmodeScroll
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
            {
                Scptr.FrigidScroll2 = true;
            }
        }
    }
}