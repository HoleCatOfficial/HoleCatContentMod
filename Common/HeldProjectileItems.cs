using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
