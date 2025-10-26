using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome
{
    public class RiftDesertUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
        {
            public override void FillTextureArray(int[] textureSlots)
            {
                textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RiftDesertUnderground0");
                textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RiftDesertUnderground1");
                textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RiftDesertUnderground2");
                textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RiftDesertUnderground3");
            }
        }
}