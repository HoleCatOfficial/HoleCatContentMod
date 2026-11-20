using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DestroyerTest.Content.BorderBiome.Tiles.Rubble
{
    #region 1x1 rubble
    public abstract class HallowShaleRubble1x1Base : ModTile
    {
        public override string Texture => "DestroyerTest/Content/BorderBiome/Tiles/Rubble/Tile_HallowShale1x1Rubble";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;

            DustType = ModContent.DustType<HallowCarbonaceousShaleDust>();
            HitSound = new SoundStyle(DTAssetLib.AudioPath + "/Border/ShaleMine", 3) { PitchVariance = 0.5f, MaxInstances = 0 };
            MineResist = 4f;
            MinPick = 12;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(62, 62, 77));

            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = 1;
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.HallowCountCollection.Add(Type);

            //To
            TileLoader.RegisterSimpleConversion(ModContent.TileType<ShaleRubble1x1Base>(), BiomeConversionID.Hallow, Type);
            //And fro
            TileLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.TileType<ShaleRubble1x1Base>());
        }
    }

    public class Tile_HallowShale1x1RubbleFake : HallowShaleRubble1x1Base
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Item_HallowCarbonaceousShale>(), Type, 0, 1, 2, 3, 4, 5);

            RegisterItemDrop(ModContent.ItemType<Item_HallowCarbonaceousShale>());
        }
    }

    public class Tile_HallowShale1x1RubbleNatural : HallowShaleRubble1x1Base
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;

            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }

        public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance)
        {
            wormChance = 8;
        }
    }
    #endregion

    #region 2x1 rubble

    public abstract class HallowShale2x1RubbleBase : ModTile
    {
        public override string Texture => "DestroyerTest/Content/BorderBiome/Tiles/Rubble/Tile_HallowShale2x1Rubble";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;

            DustType = ModContent.DustType<HallowCarbonaceousShaleDust>();
            HitSound = new SoundStyle(DTAssetLib.AudioPath + "/Border/ShaleMine", 3) { PitchVariance = 0.5f, MaxInstances = 0 };
            MineResist = 4f;
            MinPick = 12;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(62, 77, 76));

            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = 1;
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.HallowCountCollection.Add(Type);

            //To
            TileLoader.RegisterSimpleConversion(ModContent.TileType<Shale2x1RubbleBase>(), BiomeConversionID.Hallow, Type);
            //And fro
            TileLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.TileType<Shale2x1RubbleBase>());
        }
    }

    public class Tile_HallowShale2x1RubbleFake : HallowShale2x1RubbleBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            FlexibleTileWand.RubblePlacementMedium.AddVariations(ModContent.ItemType<Item_HallowCarbonaceousShale>(), Type, 0, 1, 2, 3, 4, 5);

            RegisterItemDrop(ModContent.ItemType<Item_HallowCarbonaceousShale>());
        }
    }

    public class Tile_HallowShale2x1RubbleNatural : HallowShale2x1RubbleBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;

            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }

        public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance)
        {
            wormChance = 6;
        }
    }

    #endregion

    #region 3x2 rubble

    public abstract class HallowShale3x2RubbleBase : ModTile
    {
        public override string Texture => "DestroyerTest/Content/BorderBiome/Tiles/Rubble/Tile_HallowShale3x2Rubble";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;

            DustType = ModContent.DustType<HallowCarbonaceousShaleDust>();
            HitSound = new SoundStyle(DTAssetLib.AudioPath + "/Border/ShaleMine", 3) { PitchVariance = 0.5f, MaxInstances = 0 };
            MineResist = 4f;
            MinPick = 12;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(62, 77, 76));

            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = 1;
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.HallowCountCollection.Add(Type);

            //To
            TileLoader.RegisterSimpleConversion(ModContent.TileType<Shale2x1RubbleBase>(), BiomeConversionID.Hallow, Type);
            //And fro
            TileLoader.RegisterConversion(Type, BiomeConversionID.Purity, ModContent.TileType<Shale2x1RubbleBase>());
        }
    }

    public class Tile_HallowShale3x2RubbleFake : HallowShale3x2RubbleBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<Item_HallowCarbonaceousShale>(), Type, 0, 1, 2, 3, 4, 5);

            RegisterItemDrop(ModContent.ItemType<Item_HallowCarbonaceousShale>());
        }
    }

    public class Tile_HallowShale3x2RubbleNatural : HallowShale3x2RubbleBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;

            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }

        public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance)
        {
            wormChance = 6;
        }
    }

    #endregion

}
