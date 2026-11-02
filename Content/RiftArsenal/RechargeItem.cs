using DestroyerTest.Common;
using Terraria;
using Terraria.ModLoader;



namespace DestroyerTest.Content.RiftArsenal
{
    public interface IRechargeFunctionality
    {
        bool Energized { get; }
    }
    public class RechargeItem : ModItem
    {
        //Husk class to prevent reference errors, since 20 files reference this class

        public override void UpdateInventory(Player player)
        {

        }
    }

    public class Recharge : ModPlayer, IRechargeFunctionality
    {
        public bool Energized { get; private set; }

        public override void PreUpdate()
        {
            var shadow = Player.GetModPlayer<LivingShadowPlayer>();
            Energized = shadow.LivingShadowCurrent > 0;
        }
    }
}
