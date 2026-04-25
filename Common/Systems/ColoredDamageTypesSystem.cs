using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            }
        }
    }
}
