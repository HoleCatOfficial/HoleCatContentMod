using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class ColoredDamageTypesSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("ColoredDamageTypes", out Mod coloreddamagetypes))
            {
                //Color version
                coloreddamagetypes.Call("AddDamageType", ModContent.GetInstance<ScepterClass>(), new Color(255, 255, 255), new Color(141, 242, 222), new Color(20, 200, 222));


                coloreddamagetypes.Call("AddDamageType", ModContent.GetInstance<DTTrueMeleeClass>(), new Color(255, 255, 255), new Color(200, 190, 0), new Color(200, 255, 0));
            }
        }
    }
}
