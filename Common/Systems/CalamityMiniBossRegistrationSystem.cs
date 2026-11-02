using DestroyerTest.Content.Entities;
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
