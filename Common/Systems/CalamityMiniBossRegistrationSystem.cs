using DestroyerTest.Content.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class CalamityMiniBossRegistrationSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            if (DTCrossMod.CalamityIsLoaded)
            {
                DTCrossMod.CalamityMod.Call("DeclareMiniboss", ModContent.NPCType<CursedFlameNodeMB>());
                DTCrossMod.CalamityMod.Call("DeclareMiniboss", ModContent.NPCType<IchorNodeMB>());
                DTCrossMod.CalamityMod.Call("DeclareMiniboss", ModContent.NPCType<BlessedNodeMB>());


            }
        }
    }
}
