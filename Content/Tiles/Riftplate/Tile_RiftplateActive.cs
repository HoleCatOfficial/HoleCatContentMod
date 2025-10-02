using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.Riftplate
{
    public class Tile_RiftplateActive : ModTile
    {
        public override void SetStaticDefaults() {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            HitSound = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Hit", 5)
            {
                PitchVariance = 0.2f,
                MaxInstances = 0
            };
            DustType = ModContent.DustType<RiftDust>();

            AddMapEntry(new Color(255, 155, 0));
        }
            public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
            {
                int itemType = ModContent.ItemType<Item_Riftplate>();
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, itemType);
            }
    }
}
