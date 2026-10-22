using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.RiftBiome.RiftSurfaceResources
{
    public abstract class RiftStoneRubble1x1Base : ModTile
    {
        public override string Texture => "DestroyerTest/Content/RiftBiome/RiftSurfaceResources/Tile_RiftStoneRubble1x1";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;

            DustType = DustID.Wraith;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(75, 75, 75));
        }
    }

    // This is the fake tile that will be placed by the Rubblemaker.
    public class Tile_RiftStoneRubble1x1Fake : RiftStoneRubble1x1Base
    {
        public override void SetStaticDefaults()
        {
            // Call to base SetStaticDefaults. Must inherit static defaults from base type 
            base.SetStaticDefaults();

            // Add rubble variant, all existing styles, to Rubblemaker, allowing to place this tile by consuming ExampleBlock
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Item_RiftStone>(), Type, 0, 1, 2, 3, 4, 5);

            // Tiles placed by Rubblemaker drop the item used to place them.
            RegisterItemDrop(ModContent.ItemType<Item_RiftStone>());
        }
    }

    // This is the natural tile, this version is placed during world generation in the RubbleWorldGen class.
    public class Tile_RiftStoneRubble1x1Natural : RiftStoneRubble1x1Base
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // Only the natural version is broken automatically by placing other tiles over it or block swapping under it.
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;

            // By default, the TileObjectData.Style1x1 tile we copied in Example1x1RubbleBase has LavaDeath = true. Natural rubble tiles don't have this behavior, so we want to be immune to lava.
            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }

        public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance)
        {
            wormChance = 8;
        }
    }
}