using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
    public class RoseGardenUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
        {
            public override void FillTextureArray(int[] textureSlots)
            {
                textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RoseGardenUnderground0");
                textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RoseGardenUnderground1");
                textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RoseGardenUnderground2");
                textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RoseGardenUnderground3");
            }
        }
}