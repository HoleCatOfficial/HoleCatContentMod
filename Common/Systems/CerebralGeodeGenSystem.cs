using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.Cards;
using DestroyerTest.Content.Lorebooks;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RoseGarden;
using DestroyerTest.Content.Tiles.RoseGarden.Flowers;
using DestroyerTest.Content.Tiles.Walls;
using DestroyerTest.Content.Tools;
using Microsoft.Xna.Framework;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace DestroyerTest.Common.Systems
{
    public enum CerebralGeodeType
    {
        ThermosGlove,
        ZyplonRing,
        Providence,
        TheCircle,
        Heavenbleed

    }

    public class CerebralGeodeGenSystem : ModSystem
    {
        public static LocalizedText GeodePassMessage { get; private set; }
        public Vector2 ThermosGeodePosition;
        public Vector2 ZyplonGeodePosition;
        public Vector2 ProvidenceGeodePosition;
        public Vector2 CircleGeodePosition;
        public Vector2 HeavenbleedGeodePosition;

        public override void Load()
        {
            Tuple<int, int, float>[] StandardLoot = [new(ItemID.Extractinator, 1, 0.05f), new(ItemID.Bomb, 10, 0.3333f)];

            for (int i = 0; i < StandardLoot.Length; i++)
            {
                ChestLootSystem.RegisterChestLoot(new ChestID(ModContent.TileType<Tile_CerebralChest>(), 0), StandardLoot[i].Item1, stack: StandardLoot[i].Item2, rarity: StandardLoot[i].Item3);

                
            }

            ChestLootSystem.RegisterChestLoot(new ChestID(ModContent.TileType<Tile_CerebralChest>(), 0), Opus.CommonPotion, 0.6667f);

        }

        

        public override void SetStaticDefaults()
        {
            GeodePassMessage = Language.GetText("Mods.DestroyerTest.WorldGen.CerebralGeodes");

            if (DTCrossMod.CalamityIsLoaded)
            {
                if (DTCrossMod.CalamityMod.TryFind("AbyssGravel", out ModTile AbyssGravel))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(AbyssGravel.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("PlantyMush", out ModTile PlantyMush))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(PlantyMush.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("PyreMantle", out ModTile PyreMantle))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(PyreMantle.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("PyreMantleMolten", out ModTile PyreMantleMolten))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(PyreMantleMolten.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("SulphurousShale", out ModTile SulphurousShale))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(SulphurousShale.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("Voidstone", out ModTile Voidstone))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(Voidstone.Type);
                }



                if (DTCrossMod.CalamityMod.TryFind("HazardChevronPanels", out ModTile HazardChevronPanels))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(HazardChevronPanels.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("LaboratoryPanels", out ModTile LaboratoryPanels))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(LaboratoryPanels.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("LaboratoryPipePlating", out ModTile LaboratoryPipePlating))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(LaboratoryPipePlating.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("LaboratoryPlating", out ModTile LaboratoryPlating))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(LaboratoryPlating.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("LaboratoryShelf", out ModTile LaboratoryShelf))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(PyreMantle.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("RustedPipes", out ModTile RustedPipes))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(RustedPipes.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("RustedPlating", out ModTile RustedPlating))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(RustedPlating.Type);
                }

                if (DTCrossMod.CalamityMod.TryFind("RustedShelf", out ModTile RustedShelf))
                {
                    CerebralGeodePass.InvalidGenTiles.Add(RustedShelf.Type);
                }
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(p => p.Name == "Stalac");
            if (index != -1)
            {
                tasks.Insert(index + 1, new CerebralGeodePass("Cerebral Geodes", 100f));
            }
        }


        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("ThermosGeodePosition", ThermosGeodePosition);
            tag.Add("ZyplonGeodePosition", ZyplonGeodePosition);
            tag.Add("ProvidenceGeodePosition", ProvidenceGeodePosition);
            tag.Add("CircleGeodePosition", CircleGeodePosition);
            tag.Add("HeavenbleedGeodePosition", HeavenbleedGeodePosition);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("ThermosGeodePosition"))
            {
                ThermosGeodePosition = tag.Get<Vector2>("ThermosGeodePosition");
            }

            if (tag.ContainsKey("ZyplonGeodePosition"))
            {
                ZyplonGeodePosition = tag.Get<Vector2>("ZyplonGeodePosition");
            }

            if (tag.ContainsKey("ProvidenceGeodePosition"))
            {
                ProvidenceGeodePosition = tag.Get<Vector2>("ProvidenceGeodePosition");
            }

            if (tag.ContainsKey("CircleGeodePosition"))
            {
                CircleGeodePosition = tag.Get<Vector2>("CircleGeodePosition");
            }

            if (tag.ContainsKey("HeavenbleedGeodePosition"))
            {
                HeavenbleedGeodePosition = tag.Get<Vector2>("HeavenbleedGeodePosition");
            }
        }

    }

    public class CerebralGeodePass : GenPass
    {
        public CerebralGeodePass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        public void MainGen(int x, int y, CerebralGeodeType geodeType)
        {
            Point origin = new Point(x, y);

            GenShapeActionPair BackWallGen = new GenShapeActionPair(new Shapes.Circle(6), new Actions.PlaceWall((ushort)ModContent.WallType<Wall_DreamstoneWall>()));
            WorldUtils.Gen(new Point(x, y), BackWallGen);


            GenShapeActionPair DreamstoneGen = new GenShapeActionPair(new Shapes.Circle(7), new Actions.SetTileKeepWall((ushort)ModContent.TileType<Tile_Dreamstone>(), true, true));
            WorldUtils.Gen(new Point(x, y), DreamstoneGen);

            GenShapeActionPair Cavity = new GenShapeActionPair(new Shapes.Circle(5), new Actions.ClearTile(true));
            WorldUtils.Gen(new Point(x, y), Cavity);

            GenShapeActionPair Mound = new GenShapeActionPair(new Shapes.Mound(5, 3), new Actions.SetTileKeepWall((ushort)ModContent.TileType<Tile_Dreamstone>(), true, true));
            WorldUtils.Gen(new Point(x, y + 5), Mound);

            Point ChestPoint = new Point(x + (Main.rand.NextBool() ? 0 : -1), y + 2);

            CerebralGeodeGenSystem Instance = ModContent.GetInstance<CerebralGeodeGenSystem>();
            switch (geodeType)
            {
                case CerebralGeodeType.ThermosGlove:
                    {
                        Instance.ThermosGeodePosition = ChestPoint.ToWorldCoordinates();
                        break;
                    }
                case CerebralGeodeType.ZyplonRing:
                    {
                        Instance.ZyplonGeodePosition = ChestPoint.ToWorldCoordinates();
                        break;
                    }
                case CerebralGeodeType.Providence:
                    {
                        Instance.ProvidenceGeodePosition = ChestPoint.ToWorldCoordinates();
                        break;
                    }
                case CerebralGeodeType.TheCircle:
                    {
                        Instance.CircleGeodePosition = ChestPoint.ToWorldCoordinates();
                        break;
                    }
                case CerebralGeodeType.Heavenbleed:
                    {
                        Instance.HeavenbleedGeodePosition = ChestPoint.ToWorldCoordinates();
                        break;
                    }
            }
            int Loot;

            if (!Framing.GetTileSafely(ChestPoint).HasTile)
            {
                Loot = WorldGen.PlaceChest(ChestPoint.X, ChestPoint.Y, (ushort)ModContent.TileType<Tile_CerebralChest>());

                var chest1 = Main.chest[Loot];

                
                int SpecialItem()
                {
                    switch (geodeType)
                    {
                        case CerebralGeodeType.ThermosGlove:
                        {
                            return ModContent.ItemType<ThermosGlove>();
                        }
                        case CerebralGeodeType.ZyplonRing:
                        {
                            return ModContent.ItemType<ZyplonRing>();
                        }
                        case CerebralGeodeType.Providence:
                        {
                            return ModContent.ItemType<Providence>();
                        }
                        case CerebralGeodeType.TheCircle:
                        {
                            return ModContent.ItemType<TheCircle>();
                        }
                        case CerebralGeodeType.Heavenbleed:
                        {
                            return ModContent.ItemType<Heavenbleed>();
                        }
                    }

                    return -1;
                }

                chest1.item[Main.rand.Next(chest1.item.Length)].SetDefaults(SpecialItem());

                for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                {
                    if (chest1.item[inventoryIndex].type == ItemID.None)
                    {
                        



                    }
                }
            }
        }

        public static List<int> InvalidGenTiles = new List<int>
        {
            TileID.BlueDungeonBrick,
            TileID.PinkDungeonBrick,
            TileID.GreenDungeonBrick,
            TileID.LihzahrdBrick,
            TileID.Ash,
        };

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.DestroyerTest.WorldGen.CerebralGeodes.PassMessage");



            int attempts = (int)(Main.maxTilesX * Main.maxTilesY * 6E-05);

            List<CerebralGeodeType> geodes = Enum.GetValues<CerebralGeodeType>().ToList();

            for (int k = 0; k < attempts && geodes.Count > 0; k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);

                int y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200);

                Point sample = new Point(x, y);

                Tile tile = Framing.GetTileSafely(x, y);

                Rectangle Checkframe = Utils.CenteredRectangle(sample.ToWorldCoordinates(), new Vector2(18 * 16, 18 * 16));


                bool valid()
                {
                    for (int i = Checkframe.Left; i < Checkframe.Right; i++)
                    {
                        for (int j = Checkframe.Top; j < Checkframe.Bottom; j++)
                        {
                            Tile tile = Framing.GetTileSafely(
                                i / 16,
                                j / 16
                            );

                            if (InvalidGenTiles.Contains(tile.TileType))
                                return false;
                        }
                    }

                    return true;
                }


                if (valid())
                {

                    int index = WorldGen.genRand.Next(geodes.Count);
                    CerebralGeodeType type = geodes[index];

                    MainGen(x, y, type);

                    geodes.RemoveAt(index);
                    
                   
                }



            }
        }

    }
}
