using DestroyerTest.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Asset.Menu.V6
{
    // Thanks to Nycro#0001 <@!262663471189983242> for this null background which cleanly ignores vanilla's parallax mechanics
    // Copied from Calamity Mod Github.
    public class DreamMenuBackgroundStyle : ModSurfaceBackgroundStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => BackgroundTextureLoader.GetBackgroundSlot("DestroyerTest/Assets/Textures/Backgrounds/DreamMenuClose");
        public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot("DestroyerTest/Assets/Textures/Backgrounds/DreamMenuFar");
        public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot("DestroyerTest/Assets/Textures/Backgrounds/DreamMenuMid");
        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch) => false;
    }
}