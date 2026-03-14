using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace DestroyerTest.Common
{
    public class BannerModification : ModSystem
    {
        public override void Load()
        {
            
        }

        public delegate void orig_UIPanel_DrawPanel(UIPanel self, SpriteBatch spriteBatch, Texture2D texture, Color color);

        /*
        public static void UIPanel_DrawPanel(orig_UIPanel_DrawPanel orig, UIPanel self, SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            if (self is UIModItem uiModItem)
            {
                if (uiModItem.ModName == "Your Mod Name")
                {
                    CalculatedStyle dimensions = self.GetDimensions(); //<- you can get X, Y, width and height out of this
                    return;
                }
            }
            orig(self, spriteBatch, texture, color);
        }
        */
    }
}
