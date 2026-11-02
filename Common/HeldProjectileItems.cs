using Terraria.ID;
using Terraria.Localization;

namespace DestroyerTest.Common
{
    public class HeldProjectileItemManager
    {
        public static string HeldProjectileItemKey => Language.GetTextValue("Mods.DestroyerTest.Sets.IsHeldProjectileItemKey");
        public static bool[] IsHeldProjectileItem = ItemID.Sets.Factory.CreateNamedSet(HeldProjectileItemKey)
            .Description("Uses a held projectile and is not a tool such as a drill.")
            .RegisterBoolSet();



    }
}
