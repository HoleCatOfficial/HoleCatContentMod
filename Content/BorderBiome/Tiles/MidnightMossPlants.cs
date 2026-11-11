using System.Linq;
using DestroyerTest.Content.BorderBiome.Tiles;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome.Tiles
{
    public class MidnightMossPlants : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.TileCutIgnore.Regrowth[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;
            TileID.Sets.SlowlyDiesInWater[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;

            HitSound = SoundID.Grass;
            DustType = ModContent.DustType<MidnightMossDust>();

            AddMapEntry(new Color(172, 23, 232));
        }

        public override void SickleDropHay(int i, int j, ref int hayStack)
        {
            hayStack = Main.rand.Next(1, 3);
        }

        public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance)
        {
            // This critter drop chance logic matches normal grass.
            wormChance = 400;
            grassHopperChance = 100;

            // If the player is wearing Flower Boots, drop rates are decreased drastically for "flower" styles, likely to prevent players from farming critters with Flower Boots.
            int playerIndex = Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16);
            if (Main.player[playerIndex].flowerBoots && Main.tile[i, j].TileFrameX >= 6 * 18)
            {
                wormChance *= 10000;
                grassHopperChance *= 10000;
            }
        }

        // When the tile below ExamplePlant changes to another tile, we might need to adjust it by using TileFrame.
        // If the tile below is mined or changed to a non-grass tile, we kill this plant.
        // If the tile below changes to a grass tile, we will convert to the corresponding vanilla plant tile.
        // ExampleSolutionConversion.ConvertGrass handles the opposite conversion, converting vanilla plants to ExamplePlants.
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            Tile below = Main.tile[i, j + 1];

            // Verify that the tile below is a valid placement for this tile
            int belowTileType = -1;
            if (below.HasUnactuatedTile && !below.IsHalfBlock && below.Slope == SlopeType.Solid)
                belowTileType = below.TileType;

            int[] validGrassTypes = [ModContent.TileType<MidnightMoss>()];
            if (validGrassTypes.Contains(belowTileType))
                return true;


            return true;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            height = 20;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}