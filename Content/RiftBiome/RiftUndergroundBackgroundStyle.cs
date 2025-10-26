using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome
{
    public class RiftUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
        {
            public override void FillTextureArray(int[] textureSlots)
            {
                textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RiftUnderground0");
                textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RiftUnderground1");
                textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RiftUnderground2");
                textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/RiftUnderground3");
            }
        }
}