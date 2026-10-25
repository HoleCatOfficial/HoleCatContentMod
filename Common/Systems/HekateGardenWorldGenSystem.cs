using System;
using System.Collections.Generic;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.Cards;
using DestroyerTest.Content.Lorebooks;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Altar;
using DestroyerTest.Content.Tiles.RoseGarden;
using DestroyerTest.Content.Tiles.RoseGarden.Flowers;
using DestroyerTest.Content.Tiles.RoseGarden.Walls;
using DestroyerTest.Content.Tiles.Walls;
using DestroyerTest.Content.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpusLib;
using ReLogic.Content;
using ReLogic.Utilities;
using SteelSeries.GameSense;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Generation;
using Terraria.GameContent.Tile_Entities;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace DestroyerTest.Common.Systems
{
    public class HekateGardenWorldGenSystem : ModSystem
    {
        public static LocalizedText GardenPassMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            GardenPassMessage = Language.GetText("Mods.DestroyerTest.WorldGen.OminousGarden");
        }

        public static bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }

        public override void PostUpdateWorld()
        {
            
            if (JustPressed(Keys.F))
            {
                //VesperGenTest((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);

                //CerebralGeodeGenTest((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);


                TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);

                //HeliciteGenTest((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);

                Main.NewText(new Point((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16), Color.Green);
            }


        }
        
        void HeliciteGenTest(int x, int y)
        {
            int outerRadius = Main.rand.Next(8, 12);
            ShapeData shapeData = new ShapeData();
            WorldUtils.Gen(new Point(x, y), new Shapes.Circle(outerRadius), new Actions.Blank().Output(shapeData));

            GenShapeActionPair OuterRing = new GenShapeActionPair(new ModShapes.InnerOutline(shapeData, true), new Actions.SetTile((ushort)ModContent.TileType<Tile_HeliciteCrystal>(), true, true));
            WorldUtils.Gen(new Point(x, y), OuterRing);

            ShapeData shapeData2 = new ShapeData();
            WorldUtils.Gen(new Point(x, y), new Shapes.Circle(outerRadius - 1), new Actions.Blank().Output(shapeData2));

            GenShapeActionPair OuterRing2 = new GenShapeActionPair(new ModShapes.InnerOutline(shapeData2, true), new Actions.SetTile((ushort)ModContent.TileType<Tile_HeliciteCrystal>(), true, true));
            WorldUtils.Gen(new Point(x, y), OuterRing2);

            WorldUtils.Gen(new Point(x, y), new Shapes.Circle(Main.rand.Next(2, 5)), new Actions.SetTile((ushort)ModContent.TileType<Tile_HeliciteCrystal>(), true, true));
        }
        void CerebralGeodeGenTest(int x, int y)
        {
            GenShapeActionPair BackWallGen = new GenShapeActionPair(new Shapes.Circle(6), new Actions.PlaceWall((ushort)ModContent.WallType<Wall_DreamstoneWall>()));
            WorldUtils.Gen(new Point(x, y), BackWallGen);


            GenShapeActionPair DreamstoneGen = new GenShapeActionPair(new Shapes.Circle(7), new Actions.SetTileKeepWall((ushort)ModContent.TileType<Tile_Dreamstone>(), true, true));
            WorldUtils.Gen(new Point(x, y), DreamstoneGen);

            GenShapeActionPair Cavity = new GenShapeActionPair(new Shapes.Circle(5), new Actions.ClearTile(true));
            WorldUtils.Gen(new Point(x, y), Cavity);

            GenShapeActionPair Mound = new GenShapeActionPair(new Shapes.Mound(5, 3), new Actions.SetTileKeepWall((ushort)ModContent.TileType<Tile_Dreamstone>(), true, true));
            WorldUtils.Gen(new Point(x , y + 5), Mound);

            Point ChestPoint = new Point(x + (Main.rand.NextBool() ? 0 : -1), y + 2);
            int Loot1;
            
            if (!Framing.GetTileSafely(ChestPoint).HasTile)
            {
                Loot1 = WorldGen.PlaceChest(ChestPoint.X, ChestPoint.Y, (ushort)ModContent.TileType<Tile_CerebralChest>());

                var chest1 = Main.chest[Loot1];

                int[] SpecialItemTable = [ModContent.ItemType<ThermosGlove>(), ModContent.ItemType<ZyplonRing>(), ModContent.ItemType<Providence>(), ModContent.ItemType<TheCircle>(), ModContent.ItemType<Heavenbleed>()];




                for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                {
                    if (chest1.item[inventoryIndex].type == ItemID.None)
                    {
                        

                    }
                }
            }

        


    }

    private void VesperGenTest(int x, int y)
    {


        GenShapeActionPair DreamstoneGen = new GenShapeActionPair(new Shapes.Circle(Main.rand.Next(5, 11)), new Actions.SetTile((ushort)ModContent.TileType<Tile_Dreamstone>(), true, true));
        WorldUtils.Gen(new Point(x, y), DreamstoneGen);

        GenShapeActionPair VesperGen = new GenShapeActionPair(new ShapeRoot((double)Main.rand.NextFloat(MathHelper.TwoPi)), new Actions.SetTile((ushort)ModContent.TileType<Tile_VesperOre>(), true, true));
        WorldUtils.Gen(new Point(x, y), VesperGen);



        //WorldGen.OreRunner(x, y, 10, 19, (ushort)ModContent.TileType<Tile_Dreamstone>());
    }

        public void FirstFloor(int x, int y)
        {
            ushort TT = (ushort)ModContent.TileType<Tile_SanctifiedBrick>();
            ushort WT = (ushort)ModContent.WallType<Wall_SanctifiedBrickWall>();
            int[] Variants = new int[3]
            {
                ModContent.TileType<Tile_HekateTalisman1>(),
                ModContent.TileType<Tile_HekateTalisman2>(),
                ModContent.TileType<Tile_HekateTalisman3>(),
            };

            ushort BT = (ushort)ModContent.TileType<Tile_SanctifiedBrazier>();

            //Setup Rooms
            Room Central = new Room(x, y, 21, 15, TT, WT);
            DTGenUtils.GenRoom(Central);
            DTGenUtils.MakeDoor(Central, Room.RoomSide.Both);

            

            

            Room ChestRoom1 = new Room(x + 32, y + 3, 16, 12, TT, WT);
            DTGenUtils.GenRoom(ChestRoom1);
            DTGenUtils.MakeDoor(ChestRoom1, Room.RoomSide.Left);

            Room FirstToSecondFloorChute = new Room(Central.Bounds.Left - 18, y + 3, 10, 12, TT, WT);
            DTGenUtils.GenRoom(FirstToSecondFloorChute);
            DTGenUtils.MakeDoor(FirstToSecondFloorChute, Room.RoomSide.Right);
            DTGenUtils.MakeHatch(FirstToSecondFloorChute, Room.HatchSide.Bottom, 4, FirstToSecondFloorChute.Position.X + 2);

            Hallway CenToChest1 = new Hallway(Central, ChestRoom1, 9, TT, WT);
            DTGenUtils.GenHallway(CenToChest1);

            Hallway CenToChute = new Hallway(FirstToSecondFloorChute, Central, 9, TT, WT);
            DTGenUtils.GenHallway(CenToChute);
            
            Room Dummy = new Room(x - 20, y + 22, 35, 18, TileID.Dirt, WallID.Dirt);
             
            Chute FloorConnector = new Chute(FirstToSecondFloorChute, Dummy, x - 18, 9, TT, WT);

            

            DTGenUtils.GenChute(FloorConnector);

            


            //Deco

            void furnishCentral()
            {
                WorldGen.PlaceObject(Central.Interior.Location.X + 1, Central.Interior.Location.Y, Variants[Main.rand.Next(Variants.Length)]);
                WorldGen.PlaceObject(Central.Interior.Location.X + Central.Interior.Width - 2, Central.Interior.Location.Y, Variants[Main.rand.Next(Variants.Length)]);

                WorldGen.PlaceObject(Central.Interior.Location.X + (Central.Interior.Width / 2), Central.Interior.Y, BT);

                for (int i = 0; i < 2; i++)
                {
                    int x = Central.Interior.Left + Main.rand.Next(1, Central.Interior.Width - 2);
                    int y = Central.Interior.Y - (Central.Floor + 2);
                    int style = Main.rand.Next(31, 34);

                    WorldGen.PlaceObject(x, y, TileID.Pots, style: style);
                }
            }

            furnishCentral();

            void furnishChestTopFloor()
            {
                WorldGen.PlaceObject(ChestRoom1.Interior.Location.X + 1, ChestRoom1.Interior.Location.Y, Variants[Main.rand.Next(Variants.Length)]);
                WorldGen.PlaceObject(ChestRoom1.Interior.Location.X + ChestRoom1.Interior.Width - 2, ChestRoom1.Interior.Location.Y, Variants[Main.rand.Next(Variants.Length)]);

                //WorldGen.PlaceObject(ChestRoom1.Interior.Location.X + ChestRoom1.Interior.Width - 1, ChestRoom1.Interior.Location.Y + 2, TileID.Torches);

                
                int Loot1 = WorldGen.PlaceChest(ChestRoom1.Interior.Location.X + 6, ChestRoom1.Interior.Location.Y + 7, (ushort)ModContent.TileType<Tile_NightmareChest>());

                var chest1 = Main.chest[Loot1];

                int L1 = ModContent.ItemType<HekateBookPrologue>();
                int L2 = ModContent.ItemType<Dyrn>();

                for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                {
                    if (chest1.item[inventoryIndex].type == ItemID.None)
                    {
                        chest1.item[0].SetDefaults(L1);
                        chest1.item[1].SetDefaults(L2);
                        chest1.item[1].stack = Main.rand.Next(10, 21);
                    }
                }
                
            }

            //furnishChestTopFloor();

            for (int i = 0; i < 26; i++)
            {
                WorldGen.PlaceObject(FloorConnector.Center.X, FloorConnector.Interior.Location.Y + 4 + i, TileID.Dirt);
            }
        }

        
        public void SecondFloor(int x, int y)
        {
            ushort TT = (ushort)ModContent.TileType<Tile_SanctifiedBrick>();
            ushort WT = (ushort)ModContent.WallType<Wall_SanctifiedBrickWall>();

            ushort BT = (ushort)ModContent.TileType<Tile_SanctifiedBrazier>();

            int[] TalismanVariants = new int[3]
            {
                ModContent.TileType<Tile_HekateTalisman1>(),
                ModContent.TileType<Tile_HekateTalisman2>(),
                ModContent.TileType<Tile_HekateTalisman3>(),
            };

            int[] BookcaseVariants = new int[3]
            {
                ModContent.TileType<Tile_TallBookcase1>(),
                ModContent.TileType<Tile_TallBookcase2>(),
                ModContent.TileType<Tile_TallBookcase3>(),
            };

            Point origin = new Point(x, y);

            Room Entry = new Room(x - 20, y + 22, 35, 18, TT, WT);
            DTGenUtils.GenRoom(Entry);
            DTGenUtils.MakeHatch(Entry, Room.HatchSide.Top, 4, Entry.Position.X + 4);
            DTGenUtils.MakeDoor(Entry, Room.RoomSide.Both);

            Room StatueRoom = new Room(x + 22, y + 20, 21, 20, TT, WT);
            DTGenUtils.GenRoom(StatueRoom);
            DTGenUtils.MakeDoor(StatueRoom, Room.RoomSide.Both);

            Hallway EntryToStatue = new Hallway(Entry, StatueRoom, 9, TT, WT);
            DTGenUtils.GenHallway(EntryToStatue);


            Room PotionRoom = new Room(x + 41, y + 27, 24, 13, TT, WT);
            DTGenUtils.GenRoom(PotionRoom);
            DTGenUtils.MakeDoor(PotionRoom, Room.RoomSide.Left);


            Room SeedBank = new Room(x - 49, y + 25, 31, 15, TT, WT);
            DTGenUtils.GenRoom(SeedBank);
            DTGenUtils.MakeDoor(SeedBank, Room.RoomSide.Both);


            Room Foyer = new Room(x - 56, y + 30, 9, 10, TT, WT);
            DTGenUtils.GenRoom(Foyer);
            DTGenUtils.MakeDoor(Foyer, Room.RoomSide.Right);
            DTGenUtils.MakeHatch(Foyer, Room.HatchSide.Bottom, 4, Foyer.Interior.Left);

            Room BottomFoyer = new Room(x - 59, y + 85, 15, 16, TT, WT);
            DTGenUtils.GenRoom(BottomFoyer);
            DTGenUtils.MakeDoor(BottomFoyer, Room.RoomSide.Right);
            DTGenUtils.MakeHatch(BottomFoyer, Room.HatchSide.Top, 4, Foyer.Interior.Left);

            Chute ToFourthFloor = new Chute(Foyer, BottomFoyer, Foyer.Center.X - 4, 9, TT, WT);
            DTGenUtils.GenChute(ToFourthFloor);

            void furnishEntry()
            {
                WorldGen.PlaceObject(Entry.Interior.Location.X + 8, Entry.Interior.Location.Y + (Entry.Interior.Height - 1), BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 11, Entry.Interior.Location.Y, BT);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 13, Entry.Interior.Location.Y + (Entry.Interior.Height - 1), BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 16, Entry.Interior.Location.Y, BT);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 18, Entry.Interior.Location.Y + (Entry.Interior.Height - 1), BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);
            }

            void FurnishPaintingRoom()
            {

                WorldGen.PlaceObject(StatueRoom.Interior.Location.X + 5, StatueRoom.Interior.Location.Y + 1, ModContent.TileType<Tile_IdriPainting>());
                WorldGen.PlaceObject(StatueRoom.Interior.Location.X + 1, StatueRoom.Interior.Location.Y, BT);
                WorldGen.PlaceObject(StatueRoom.Interior.Location.X + (StatueRoom.Interior.Width - 3), StatueRoom.Interior.Location.Y, BT);

                WorldGen.PlaceObject(StatueRoom.Interior.Center.X - 2, StatueRoom.Interior.Bottom, ModContent.TileType<Tile_WallDoor>());

            }

            void furnishSeedBank()
            {
                WorldGen.PlaceObject(SeedBank.Interior.Left + 2, SeedBank.Interior.Top, BT);
                WorldGen.PlaceObject(SeedBank.Interior.Left + (SeedBank.Interior.Width - 4), SeedBank.Interior.Top, BT);

                int[] HerbTypes = [ItemID.Daybloom, ItemID.Moonglow, ItemID.Blinkroot, ItemID.Deathweed, ItemID.Fireblossom];
                int[] SeedTypes = [ItemID.DaybloomSeeds, ItemID.MoonglowSeeds, ItemID.BlinkrootSeeds, ItemID.DeathweedSeeds, ItemID.FireblossomSeeds];

                void Chest1()
                {
                    int ChestX = SeedBank.Interior.Left + 2;
                    int ChestY  = SeedBank.Interior.Bottom - 1;

                    WorldUtils.ClearChestLocation(ChestX, ChestY);

                    int Loot = WorldGen.PlaceChest(ChestX, ChestY);


                    // Don't index Main.chest if placement failed.
                    if (Loot >= 0)
                    {

                        var Chest = Main.chest[Loot];

                        if (Chest?.item != null || Chest.item.Length > 0)
                        {
                            for (int i = 0; i < 16; i++)
                            {

                                int slot = Main.rand.Next(Chest.item.Length);
                                int selection = Main.rand.Next(HerbTypes.Length);

                                Chest.item[slot].SetDefaults(Main.rand.NextBool() ? HerbTypes[selection] : SeedTypes[selection]);
                            }
                        }
                    }
                }

                void Chest2()
                {
                    int ChestX = SeedBank.Interior.Left + 5;
                    int ChestY = SeedBank.Interior.Bottom - 1;

                    WorldUtils.ClearChestLocation(ChestX, ChestY);

                    int Loot = WorldGen.PlaceChest(ChestX, ChestY);

                    // Don't index Main.chest if placement failed.
                    if (Loot >= 0)
                    {

                        var Chest = Main.chest[Loot];

                        if (Chest?.item != null || Chest.item.Length > 0)
                        {
                            for (int i = 0; i < 16; i++)
                            {

                                int slot = Main.rand.Next(Chest.item.Length);
                                int selection = Main.rand.Next(HerbTypes.Length);

                                Chest.item[slot].SetDefaults(Main.rand.NextBool() ? HerbTypes[selection] : SeedTypes[selection]);
                            }
                        }
                    }
                }


                Chest1();
                Chest2();

                for (int i = 0; i < 7; i++)
                {
                    int X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 3) + i;
                    int Y = SeedBank.Interior.Location.Y + 4;

                    WorldGen.PlaceObject(X, Y, TileID.Platforms, style: 1);
                }

                
                WorldGen.PlaceObject((SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 3), SeedBank.Interior.Location.Y + 3, (ushort)ModContent.TileType<NeglectedRegardsDisplay>());

                WorldGen.PlaceObject((SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 2), SeedBank.Interior.Location.Y + 10, (ushort)ModContent.TileType<Tile_HoleisBed>());

                WorldGen.PlaceObject(SeedBank.Interior.Location.X + 7, SeedBank.Interior.Location.Y, TalismanVariants[Main.rand.Next(TalismanVariants.Length)]);
                WorldGen.PlaceObject(SeedBank.Interior.Location.X + SeedBank.Interior.Width - 8, SeedBank.Interior.Location.Y, TalismanVariants[Main.rand.Next(TalismanVariants.Length)]);

            }

            void furnishPotionRoom()
            {
                WorldGen.PlaceObject(PotionRoom.Interior.Location.X + 4, PotionRoom.Interior.Location.Y, BT);

                WorldGen.PlaceObject(PotionRoom.Interior.Location.X + PotionRoom.Interior.Width - 4, PotionRoom.Interior.Location.Y, BT);

                for (int i = 0; i < 2; i++)
                {
                    int Y = PotionRoom.Interior.Location.Y
                        + (PotionRoom.Interior.Height - (i == 0 ? 2 : 6));

                    // Two tiles of platform.
                    for (int j = 0; j < 2; j++)
                    {
                        int X = PotionRoom.Interior.Location.X + 1 + j;
                        WorldGen.PlaceObject(X, Y, TileID.Platforms, style: 1);
                    }

                    // Place one chest on each platform.
                    int ChestX = PotionRoom.Interior.Location.X + 1;
                    int ChestY = Y - 1;

                    WorldUtils.ClearChestLocation(ChestX, ChestY);

                    int Loot = WorldGen.PlaceChest(ChestX, ChestY);

                    int attempts = 0;
                    while (Loot < 0 && attempts < 10)
                    {
                        WorldUtils.ClearChestLocation(ChestX, ChestY);
                        Loot = WorldGen.PlaceChest(ChestX, ChestY);
                        attempts++;
                    }

                    // Don't index Main.chest if placement failed.
                    if (Loot < 0)
                        continue;

                    var Chest = Main.chest[Loot];

                    if (Chest?.item == null || Chest.item.Length == 0)
                        continue;

                    switch (i)
                    {
                        case 0:
                            for (int t = 0; t < 9; t++)
                            {
                                int slot = Main.rand.Next(Chest.item.Length);
                                int potion = Main.rand.Next(Opus.CommonPotion.Count);

                                Chest.item[slot].SetDefaults(
                                    Opus.CommonPotion[potion].ItemType);
                            }
                            break;

                        case 1:
                            int idx = Main.rand.Next(Chest.item.Length);
                            Chest.item[idx].SetDefaults(ModContent.ItemType<Dyrn>());
                            Chest.item[idx].stack = Main.rand.Next(20, 30);
                            break;
                    }
                }


                WorldGen.PlaceObject(PotionRoom.Interior.Location.X + 5, PotionRoom.Interior.Location.Y + PotionRoom.Interior.Height - 1, ModContent.TileType<Tile_PotionDesk>());

                WorldGen.PlaceObject(PotionRoom.Interior.Location.X + 11, PotionRoom.Interior.Location.Y + PotionRoom.Interior.Height - 7, TileID.AmethystGemspark);

                WorldGen.PlaceObject(PotionRoom.Interior.Left + 14, PotionRoom.Interior.Bottom - 1, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(PotionRoom.Interior.Left + 16, PotionRoom.Interior.Bottom - 1, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(PotionRoom.Interior.Left + 18, PotionRoom.Interior.Bottom - 1, ModContent.TileType<Tile_DecoCrate>());

                WorldGen.PlaceObject(PotionRoom.Interior.Left + 17, PotionRoom.Interior.Bottom - 3, ModContent.TileType<Tile_DecoCrate>());
            }



            furnishEntry();
            FurnishPaintingRoom();
            furnishSeedBank();
            furnishPotionRoom();
      
        }

        public void ThirdFloor(int x, int y)
        {
            ushort TT = (ushort)ModContent.TileType<Tile_SanctifiedBrick>();
            ushort WT = (ushort)ModContent.WallType<Wall_SanctifiedBrickWall>();

            ushort BT = (ushort)ModContent.TileType<Tile_SanctifiedBrazier>();

            int[] TalismanVariants = new int[3]
            {
                ModContent.TileType<Tile_HekateTalisman1>(),
                ModContent.TileType<Tile_HekateTalisman2>(),
                ModContent.TileType<Tile_HekateTalisman3>(),
            };

            Room ForestRoom = new Room(x - 33, y + 38, 25, 40, TT, WT);
            DTGenUtils.GenRoom(ForestRoom);

            Room Entry = new Room(x - 10, y + 38, 25, 13, TT, WT);
            DTGenUtils.GenRoom(Entry);
            DTGenUtils.MakeHatch(Entry, Room.HatchSide.Top, 4, Entry.Position.X + 16);
            DTGenUtils.MakeDoor(Entry, Room.RoomSide.Both);

            Room CritterCollection = new Room(x + 22, y + 38, 27, 13, TT, WT);
            DTGenUtils.GenRoom(CritterCollection);
            DTGenUtils.MakeDoor(CritterCollection, Room.RoomSide.Left);

            Hallway ToCritters = new Hallway(Entry, CritterCollection, 9, TT, WT);
            DTGenUtils.GenHallway(ToCritters);


            Rectangle ForestFloor = new(ForestRoom.Interior.X, ForestRoom.Interior.Y + (ForestRoom.Interior.Height - 3), ForestRoom.Interior.Width, 3);
            WorldUtils.Gen(ForestFloor.Location, new GenShapeActionPair(new Shapes.Rectangle(ForestFloor.Width, ForestFloor.Height), new Actions.SetTile(TileID.Dirt)));
            for (int i = 0; i < ForestFloor.Width; i++)
            {
                int X = ForestFloor.Location.X + i;
                int Y = ForestFloor.Location.Y;
                WorldGen.PlaceTile(X, Y, TileID.CorruptGrass);
            }

            

            WorldGen.PlaceObject(ForestRoom.Interior.Location.X + 7, ForestRoom.Interior.Location.Y + ForestRoom.Interior.Height - 4, (ushort)ModContent.TileType<Tile_ForeignTree>());


            void furnishCritterRoom()
            {
                
                WorldGen.PlaceObject(CritterCollection.Interior.Left + 4, CritterCollection.Interior.Top + (CritterCollection.Interior.Height - 1), TileID.AmberBunnyCage);
                WorldGen.PlaceObject(CritterCollection.Interior.Left + 4, CritterCollection.Interior.Top + (CritterCollection.Interior.Height - 4), TileID.CageGrubby);

                WorldGen.PlaceObject(CritterCollection.Interior.Left + 10, CritterCollection.Interior.Top + (CritterCollection.Interior.Height - 1), (ushort)ModContent.TileType<Tile_Terrarium>());
                WorldGen.PlaceObject(CritterCollection.Interior.Left + 10, CritterCollection.Interior.Top + (CritterCollection.Interior.Height - 3), (ushort)ModContent.TileType<Tile_Terrarium>());
            }

            furnishCritterRoom();
        }

        public void FourthFloor(int X, int Y)
        {
            ushort TT = (ushort)ModContent.TileType<Tile_SanctifiedBrick>();
            ushort WT = (ushort)ModContent.WallType<Wall_SanctifiedBrickWall>();

            ushort BT = (ushort)ModContent.TileType<Tile_SanctifiedBrazier>();

            int[] TalismanVariants = new int[3]
            {
                ModContent.TileType<Tile_HekateTalisman1>(),
                ModContent.TileType<Tile_HekateTalisman2>(),
                ModContent.TileType<Tile_HekateTalisman3>(),
            };

            int[] BookcaseVariants = new int[3]
            {
                ModContent.TileType<Tile_TallBookcase1>(),
                ModContent.TileType<Tile_TallBookcase2>(),
                ModContent.TileType<Tile_TallBookcase3>(),
            };

            Room Entry = new Room(X - 38, Y + 80, 40, 21, TT, WT);
            DTGenUtils.GenRoom(Entry);
            DTGenUtils.MakeDoor(Entry, Room.RoomSide.Both);

            Room BottomFoyer = new Room(X - 59, Y + 85, 15, 16, TT, WT);
            DTGenUtils.GenRoom(BottomFoyer);
            DTGenUtils.MakeDoor(BottomFoyer, Room.RoomSide.Right);
            DTGenUtils.MakeHatch(BottomFoyer, Room.HatchSide.Top, 4, X - 54);

            Hallway connector1 = new Hallway(BottomFoyer, Entry, 9, TT, WT);
            DTGenUtils.GenHallway(connector1);


            Room RightRoom = new Room(X + 70, Y + 85, 20, 16, TT, WT);
            DTGenUtils.GenRoom(RightRoom);
            DTGenUtils.MakeDoor(RightRoom, Room.RoomSide.Left);
            DTGenUtils.MakeHatch(RightRoom, Room.HatchSide.Bottom, 3, X + 82);

            Hallway connector2 = new Hallway(Entry, RightRoom, 9, TT, WT);
            DTGenUtils.GenHallway(connector2);

            Room PreBoss = new Room(X + 70, Y + 140, 20, 16, TT, WT);
            DTGenUtils.GenRoom(PreBoss);
            DTGenUtils.MakeDoor(PreBoss, Room.RoomSide.Left);
            DTGenUtils.MakeHatch(PreBoss, Room.HatchSide.Top, 3, X + 82);

            Chute connector3 = new(RightRoom, PreBoss, RightRoom.Center.X, 8, TT, WT);
            DTGenUtils.GenChute(connector3);


            //Walls
            ShapeData shapeData = new ShapeData();
            WorldUtils.Gen(new Point(X, Y + 145), new Shapes.Mound(60, 30), Actions.Chain(new GenAction[]
            {
                new Actions.Blank().Output(shapeData),

                new Actions.ClearTile(true),
                new Actions.Clear(),

                new Actions.PlaceWall(WT)
            }));

            
            WorldUtils.Gen(new Point(X - 120, Y + 145), new Shapes.Rectangle((60 * 2) - 1, 10), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(- 60, 0),
                new Actions.Blank().Output(shapeData),


                new Modifiers.Offset(120, 0),
                new Actions.ClearTile(true),
                new Actions.Clear(),
                new Actions.PlaceWall(WT)
            }));
            WorldUtils.Gen(new Point(X, Y + 145), new ModShapes.InnerOutline(shapeData, true), new Actions.SetTile(TT, true));

            
            ShapeData shapeData2 = new ShapeData();
            WorldUtils.Gen(new Point(X, Y + 146), new Shapes.Mound(61, 30), new Actions.Blank().Output(shapeData2));
            WorldUtils.Gen(new Point(X - 120, Y + 146), new Shapes.Rectangle((61 * 2) - 1, 10), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(- 61, 0),
                new Actions.Blank().Output(shapeData2)
            }));
            WorldUtils.Gen(new Point(X, Y + 146), new ModShapes.InnerOutline(shapeData2, true), new Actions.SetTile(TT, true));



            WorldUtils.Gen(new Point(X + 58, Y + 146), new Shapes.Rectangle(12, 10), new Actions.SetTile(TT, true));
            WorldUtils.Gen(new Point(X + 58, Y + 146), new Shapes.Rectangle(12, 10), new Actions.PlaceWall(WT, true));
            WorldUtils.Gen(new Point(X + 58, Y + 148), new Shapes.Rectangle(12, 6), new Actions.ClearTile(true));


            void furnishEntry()
            {
                WorldGen.PlaceObject(Entry.Interior.Left + 2, Entry.Interior.Bottom - 1, BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Left + 5, Entry.Interior.Bottom - 1, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(Entry.Interior.Left + 7, Entry.Interior.Bottom - 1, ModContent.TileType<Tile_DecoCrate>());

                WorldGen.PlaceObject(Entry.Interior.Left + 6, Entry.Interior.Bottom - 3, ModContent.TileType<Tile_DecoCrate>());

                for (int i = 0; i < 9; i++)
                {
                    WorldGen.PlaceObject(Entry.Interior.Left + 6 + i, Entry.Interior.Top + 5, TileID.Platforms, style: 1);

                }

                for (int i = 0; i < 4; i++)
                {
                    WorldGen.PlaceObject(Entry.Interior.Left + 6, Entry.Interior.Top + i, TileID.Chain);

                    WorldGen.PlaceObject(Entry.Interior.Left + 15, Entry.Interior.Top + i, TileID.Chain);
                }

                WorldGen.PlaceObject(Entry.Interior.Left + 7, Entry.Interior.Top + 4, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(Entry.Interior.Left + 9, Entry.Interior.Top + 4, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(Entry.Interior.Left + 11, Entry.Interior.Top + 4, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(Entry.Interior.Left + 8, Entry.Interior.Top + 2, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(Entry.Interior.Left + 10, Entry.Interior.Top + 2, ModContent.TileType<Tile_DecoCrate>());

                WorldGen.PlaceObject(Entry.Interior.Left + 17, Entry.Interior.Top, ModContent.TileType<Tile_SanctifiedBrazier>());

                WorldUtils.ClearChestLocation(Entry.Interior.Left + 10, Entry.Interior.Bottom - 1);
                int WandIndex = WorldGen.PlaceChest(Entry.Interior.Left + 10, Entry.Interior.Bottom - 1, TileID.Containers);
                Chest WandChest = Main.chest[WandIndex];
                WandChest.item[Main.rand.Next(WandChest.item.Length - 1)].SetDefaults(ModContent.ItemType<HekatesMystique>());

                WorldGen.PlaceObject(Entry.Interior.Left + 20, Entry.Interior.Bottom - 1, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(Entry.Interior.Left + 22, Entry.Interior.Bottom - 1, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(Entry.Interior.Left + 24, Entry.Interior.Bottom - 1, ModContent.TileType<Tile_DecoCrate>());
                WorldGen.PlaceObject(Entry.Interior.Left + 21, Entry.Interior.Bottom - 3, ModContent.TileType<Tile_DecoCrate>());
            }

            furnishEntry();
        }




        private void TestMethod(int x, int y)
        {
            //Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

            // Code to test placed here:
            //WorldGen.TileRunner(x - 1, y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(2, 8), TileID.Cobweb);
            SoundEngine.PlaySound(SoundID.Camera);
            SoundEngine.PlaySound(SoundID.Item14);

            //Prep Area

            //WorldGen.digTunnel(x, y, 0, 1, 50, 6, false);

            Point origin = new Point(x, y);
            //WorldUtils.Gen(origin, new GenShapeActionPair(new Shapes.Rectangle(100, 100), new Actions.ClearTile(true)));
            //WorldUtils.Gen(origin, new GenShapeActionPair(new Shapes.Rectangle(100, 100), new Actions.PlaceWall(WallID.DirtUnsafe)));

            FirstFloor(x, y);

            SecondFloor(x, y);

            ThirdFloor(x, y);

            FourthFloor(x, y);

            WorldGen.PlaceObject(x + 12, x + 6, ModContent.TileType<Tile_RoseGardenEffectSource>());
            WorldGen.PlaceObject(x + 12, x + 6, ModContent.TileType<Tile_RoseGardenEffectSource>());
            WorldGen.PlaceObject(x + 12, x + 6, ModContent.TileType<Tile_RoseGardenEffectSource>());
            WorldGen.PlaceObject(x + 12, x + 6, ModContent.TileType<Tile_RoseGardenEffectSource>());

            //ConnectingChute(x + 41, y + 12);

            //PlaceAndFillChests(x, y);

            Vector2 OrigVec = origin.ToWorldCoordinates();
            Rectangle Rect = new Rectangle((int)OrigVec.X, (int)OrigVec.Y, 60, 43);
            if (DTCrossMod.FargosMutantIsLoaded)
            {
                DTCrossMod.FargosMutantMod.Call("AddIndestructibleRectangle", Rect);
            }


        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(p => p.Name == "Larva");
            if (index != -1)
            {
                tasks.Insert(index + 1, new HekateGardenPass("Ominous Garden", 100f));
            }
        }
    }

    public class HekateGardenPass : GenPass
    {
        public HekateGardenPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        public void ClearTopSpace(int x, int y)
        {
            Point origin = new Point(x, y);
            Point surfacePoint;
            // Search up to 1000 tiles above for an area 50 tiles tall and 1 tile wide without a single solid tile. Basically find the surface.
            bool flag = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out surfacePoint);
            // Search from the orgin up to the surface and make sure no sand is between origin and surface
            if (WorldUtils.Find(origin, Searches.Chain(new Searches.Up(origin.Y - surfacePoint.Y), new Conditions.IsTile(TileID.Sand)), out Point _))
                return;

            if (!flag)
                return;

            // Remove tiles to create shaft to surface. Convert Sand tiles along shaft to hardened sand tiles.
            ShapeData shaftShapeData = new ShapeData();
            WorldUtils.Gen(new Point(origin.X, surfacePoint.Y + 10), new Shapes.Circle(1, origin.Y - surfacePoint.Y - 9), Actions.Chain(new Modifiers.Blotches(2, 0.2), new Actions.ClearTile().Output(shaftShapeData), new Modifiers.Expand(1), new Modifiers.OnlyTiles(TileID.Sand), new Actions.SetTile(TileID.HardenedSand).Output(shaftShapeData)));
            WorldUtils.Gen(new Point(origin.X, surfacePoint.Y + 10), new ModShapes.All(shaftShapeData), new Actions.SetFrames(frameNeighbors: true));

            //Leave these in

            WorldUtils.Gen(new Point(x, y - 5), new GenShapeActionPair(new Shapes.Mound(28, Main.rand.Next(24, 30)), new Actions.ClearTile()));

            WorldUtils.Gen(new Point(x, y - 3), new GenShapeActionPair(new Shapes.Slime(7, 4, (double)Main.rand.NextFloat(1.03f, 1.5f)), new Actions.PlaceTile((ushort)ModContent.TileType<Tile_RootedDirt>())));

            //WorldGen.TileRunner(x, y - 4, 6, 8, ModContent.TileType<Tile_RootedDirt>(), true, 2, 1, overRide: true);
        }
        public void TopFloor(int x, int y)
        {

            int[] Variants = new int[3]
            {
                ModContent.TileType<Tile_HekateTalisman1>(),
                ModContent.TileType<Tile_HekateTalisman2>(),
                ModContent.TileType<Tile_HekateTalisman3>(),
            };

            //Setup Rooms
            Room Central = new Room(x, y, 21, 15, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(Central);
            DTGenUtils.MakeDoor(Central, Room.RoomSide.Both);





            Room ChestRoom1 = new Room(x + 32, y + 3, 16, 12, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(ChestRoom1);
            DTGenUtils.MakeDoor(ChestRoom1, Room.RoomSide.Left);

            Room FirstToSecondFloorChute = new Room(Central.Bounds.Left - 18, y + 3, 10, 12, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(FirstToSecondFloorChute);
            DTGenUtils.MakeDoor(FirstToSecondFloorChute, Room.RoomSide.Right);
            DTGenUtils.MakeHatch(FirstToSecondFloorChute, Room.HatchSide.Bottom, 4, FirstToSecondFloorChute.Position.X + 2);

            Hallway CenToChest1 = new Hallway(Central, ChestRoom1, 9, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenHallway(CenToChest1);

            Hallway CenToChute = new Hallway(FirstToSecondFloorChute, Central, 9, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenHallway(CenToChute);

            Room Dummy = new Room(x - 20, y + 22, 35, 18, TileID.Dirt, WallID.Dirt);

            Chute FloorConnector = new Chute(FirstToSecondFloorChute, Dummy, x - 18, 9, TileID.EbonstoneBrick, WallID.EbonstoneBrick);



            DTGenUtils.GenChute(FloorConnector);




            //Deco

            void furnishCentral()
            {
                WorldGen.PlaceObject(Central.Interior.Location.X + 1, Central.Interior.Location.Y, Variants[Main.rand.Next(Variants.Length)]);
                WorldGen.PlaceObject(Central.Interior.Location.X + Central.Interior.Width - 2, Central.Interior.Location.Y, Variants[Main.rand.Next(Variants.Length)]);

                WorldGen.PlaceObject(Central.Interior.Location.X + (Central.Interior.Width / 2), Central.Interior.Y, TileID.BrazierSuspended);

                for (int i = 0; i < 2; i++)
                {
                    int x = Central.Interior.Left + Main.rand.Next(1, Central.Interior.Width - 2);
                    int y = Central.Interior.Y - (Central.Floor + 2);
                    int style = Main.rand.Next(31, 34);

                    WorldGen.PlaceObject(x, y, TileID.Pots, style: style);
                }
            }

            furnishCentral();

            void furnishChestTopFloor()
            {
                WorldGen.PlaceObject(ChestRoom1.Interior.Location.X + 1, ChestRoom1.Interior.Location.Y, Variants[Main.rand.Next(Variants.Length)]);
                WorldGen.PlaceObject(ChestRoom1.Interior.Location.X + ChestRoom1.Interior.Width - 2, ChestRoom1.Interior.Location.Y, Variants[Main.rand.Next(Variants.Length)]);

                WorldGen.PlaceObject(ChestRoom1.Interior.Location.X + ChestRoom1.Interior.Width - 1, ChestRoom1.Interior.Location.Y + 2, TileID.Torches);

                /*
                int Loot1 = WorldGen.PlaceChest(ChestRoom1.Interior.Location.X + 6, ChestRoom1.Interior.Location.Y + 7, (ushort)ModContent.TileType<Tile_NightmareChest>());

                var chest1 = Main.chest[Loot1];

                int L1 = ModContent.ItemType<HekateBookPrologue>();
                int L2 = ModContent.ItemType<Dyrn>();

                for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                {
                    if (chest1.item[inventoryIndex].type == ItemID.None)
                    {
                        chest1.item[0].SetDefaults(L1);
                        chest1.item[1].SetDefaults(L2);
                        chest1.item[1].stack = Main.rand.Next(10, 21);
                    }
                }
                '*/
            }

            furnishChestTopFloor();

            for (int i = 0; i < 26; i++)
            {
                WorldGen.PlaceObject(FloorConnector.Center.X, FloorConnector.Interior.Location.Y + 4 + i, TileID.Dirt);
            }

            //Setup Hallways

            //DTGenUtils.GenHallway(origin.X + 20, origin.Y + 4, 15, 10, 2, true, TileID.EbonstoneBrick, WallID.EbonstoneBrick, 18);
        }


        public void BottomFloor(int x, int y)
        {
            int[] TalismanVariants = new int[3]
            {
                ModContent.TileType<Tile_HekateTalisman1>(),
                ModContent.TileType<Tile_HekateTalisman2>(),
                ModContent.TileType<Tile_HekateTalisman3>(),
            };

            int[] BookcaseVariants = new int[3]
            {
                ModContent.TileType<Tile_TallBookcase1>(),
                ModContent.TileType<Tile_TallBookcase2>(),
                ModContent.TileType<Tile_TallBookcase3>(),
            };

            Point origin = new Point(x, y);

            Room Entry = new Room(x - 20, y + 22, 35, 18, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(Entry);
            DTGenUtils.MakeHatch(Entry, Room.HatchSide.Top, 4, Entry.Position.X + 4);
            DTGenUtils.MakeDoor(Entry, Room.RoomSide.Both);

            Room StatueRoom = new Room(x + 22, y + 20, 21, 20, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(StatueRoom);
            DTGenUtils.MakeDoor(StatueRoom, Room.RoomSide.Both);

            Hallway EntryToStatue = new Hallway(Entry, StatueRoom, 9, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenHallway(EntryToStatue);


            Room PotionRoom = new Room(x + 41, y + 22, 30, 18, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(PotionRoom);
            DTGenUtils.MakeDoor(PotionRoom, Room.RoomSide.Left);


            Room SeedBank = new Room(x - 49, y + 25, 31, 15, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(SeedBank);
            DTGenUtils.MakeDoor(SeedBank, Room.RoomSide.Both);

            void furnishEntry()
            {
                WorldGen.PlaceObject(Entry.Interior.Location.X + 8, Entry.Interior.Location.Y + (Entry.Interior.Height - 1), BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 11, Entry.Interior.Location.Y, TileID.BrazierSuspended);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 13, Entry.Interior.Location.Y + (Entry.Interior.Height - 1), BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 16, Entry.Interior.Location.Y, TileID.BrazierSuspended);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 18, Entry.Interior.Location.Y + (Entry.Interior.Height - 1), BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                Point ChestPoint = new(Entry.Interior.Location.X + 11, Entry.Interior.Location.Y + 13);
                int Loot1;
                /*
                if (!Framing.GetTileSafely(ChestPoint).HasTile)
                {
                    Loot1 = WorldGen.PlaceChest(ChestPoint.X, ChestPoint.Y, TileID.Containers, style: 1);

                    var chest1 = Main.chest[Loot1];

                    int L1 = ModContent.ItemType<HekateBook1>();
                    int L2 = ModContent.ItemType<HekateBook2>();
                    int L3 = ModContent.ItemType<MalachiteKnives>();

                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {
                        if (chest1.item[inventoryIndex].type == ItemID.None)
                        {
                            chest1.item[0].SetDefaults(L1);
                            chest1.item[1].SetDefaults(L2);
                            chest1.item[2].SetDefaults(L3);
                        }
                    }
                }
                else
                {
                    WorldGen.KillTile(ChestPoint.X, ChestPoint.Y);
                }
                */
            }

            void FurnishPaintingRoom()
            {

                WorldGen.PlaceObject(StatueRoom.Interior.Location.X + 5, StatueRoom.Interior.Location.Y + 1, ModContent.TileType<Tile_IdriPainting>());
                WorldGen.PlaceObject(StatueRoom.Interior.Location.X + 1, StatueRoom.Interior.Location.Y, TileID.BrazierSuspended);
                WorldGen.PlaceObject(StatueRoom.Interior.Location.X + (StatueRoom.Interior.Width - 3), StatueRoom.Interior.Location.Y, TileID.BrazierSuspended);

            }

            void furnishSeedBank()
            {
                WorldGen.PlaceObject(SeedBank.Interior.Location.X + 2, SeedBank.Interior.Location.Y, TileID.BrazierSuspended);
                WorldGen.PlaceObject(SeedBank.Interior.Location.X + (SeedBank.Interior.Width - 4), SeedBank.Interior.Location.Y, TileID.BrazierSuspended);

                for (int i = 0; i < 4; i++)
                {

                    for (int j = 0; j < 4; j++)
                    {
                        int X = 0;
                        int Y = 0;
                        switch (i)
                        {
                            case 0:
                                {
                                    X = (SeedBank.Interior.Location.X + 2) + j;
                                    Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 2);
                                    break;
                                }
                            case 1:
                                {
                                    X = (SeedBank.Interior.Location.X + 2) + j;
                                    Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 6);
                                    break;
                                }
                            case 2:
                                {
                                    X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width - 6)) + j;
                                    Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 2);
                                    break;
                                }
                            case 3:
                                {
                                    X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width - 6)) + j;
                                    Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 6);
                                    break;
                                }
                        }


                        WorldGen.PlaceObject(X, Y, TileID.Platforms, style: 1);
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    int X = 0;
                    int Y = 0;
                    switch (i)
                    {
                        case 0:
                            {
                                X = (SeedBank.Interior.Location.X + 2);
                                Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 4);
                                break;
                            }
                        case 1:
                            {
                                X = (SeedBank.Interior.Location.X + 2);
                                Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 8);
                                break;
                            }
                        case 2:
                            {
                                X = (SeedBank.Interior.Location.X + 4);
                                Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 4);
                                break;
                            }
                        case 3:
                            {
                                X = (SeedBank.Interior.Location.X + 4);
                                Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 8);
                                break;
                            }

                        case 4:
                            {
                                X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width - 4));
                                Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 4);
                                break;
                            }
                        case 5:
                            {
                                X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width - 4));
                                Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 8);
                                break;
                            }
                        case 6:
                            {
                                X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width - 6));
                                Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 4);
                                break;
                            }
                        case 7:
                            {
                                X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width - 6));
                                Y = SeedBank.Interior.Location.Y + (SeedBank.Interior.Height - 8);
                                break;
                            }
                    }


                    int Loot = WorldGen.PlaceChest(X, Y + 1, TileID.Containers, style: 0);

                }



                for (int i = 0; i < 7; i++)
                {
                    int X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 3) + i;
                    int Y = SeedBank.Interior.Location.Y + 4;

                    WorldGen.PlaceObject(X, Y, TileID.Platforms, style: 1);
                }



                Dust.QuickDust((SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 3), SeedBank.Interior.Location.Y + 3, Color.Red);
                WorldGen.PlaceObject((SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 3), SeedBank.Interior.Location.Y + 3, (ushort)ModContent.TileType<NeglectedRegardsDisplay>());

                Dust.QuickDust((SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 2), SeedBank.Interior.Location.Y + 10, Color.Orange);
                WorldGen.PlaceObject((SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 2), SeedBank.Interior.Location.Y + 10, (ushort)ModContent.TileType<Tile_HoleisBed>());


                WorldGen.PlaceObject(SeedBank.Interior.Location.X + 7, SeedBank.Interior.Location.Y, TalismanVariants[Main.rand.Next(TalismanVariants.Length)]);
                WorldGen.PlaceObject(SeedBank.Interior.Location.X + SeedBank.Interior.Width - 8, SeedBank.Interior.Location.Y, TalismanVariants[Main.rand.Next(TalismanVariants.Length)]);

            }

            void furnishPotionRoom()
            {
                WorldGen.PlaceObject(PotionRoom.Interior.Location.X + 2, PotionRoom.Interior.Location.Y, TileID.BrazierSuspended);

                WorldGen.PlaceObject(PotionRoom.Interior.Location.X + PotionRoom.Interior.Width - 4, PotionRoom.Interior.Location.Y, TileID.BrazierSuspended);

                for (int i = 0; i < 2; i++)
                {

                    for (int j = 0; j < 2; j++)
                    {
                        int X = 0;
                        int Y = 0;
                        switch (i)
                        {
                            case 0:
                                {
                                    X = (PotionRoom.Interior.Location.X + 2) + j;
                                    Y = PotionRoom.Interior.Location.Y + (PotionRoom.Interior.Height - 2);
                                    break;
                                }
                            case 1:
                                {
                                    X = (PotionRoom.Interior.Location.X + 2) + j;
                                    Y = PotionRoom.Interior.Location.Y + (PotionRoom.Interior.Height - 6);
                                    break;
                                }
                        }


                        WorldGen.PlaceObject(X, Y, TileID.Platforms, style: 1);

                        int Loot = WorldGen.PlaceChest((PotionRoom.Interior.Location.X + 2), Y - 1);
                    }
                }


                WorldGen.PlaceObject(PotionRoom.Interior.Location.X + 8, PotionRoom.Interior.Location.Y + PotionRoom.Interior.Height - 1, ModContent.TileType<Tile_PotionDesk>());

                WorldGen.PlaceObject(PotionRoom.Interior.Location.X + 11, PotionRoom.Interior.Location.Y + PotionRoom.Interior.Height - 7, TileID.AmethystGemspark);


            }



            furnishEntry();
            FurnishPaintingRoom();
            furnishSeedBank();
            furnishPotionRoom();

        }

        public void ThirdFloor(int x, int y)
        {
            int[] TalismanVariants = new int[3]
            {
                ModContent.TileType<Tile_HekateTalisman1>(),
                ModContent.TileType<Tile_HekateTalisman2>(),
                ModContent.TileType<Tile_HekateTalisman3>(),
            };

            Room ForestRoom = new Room(x - 33, y + 38, 25, 40, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(ForestRoom);

            Room Entry = new Room(x - 10, y + 38, 25, 13, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(Entry);
            DTGenUtils.MakeHatch(Entry, Room.HatchSide.Top, 4, Entry.Position.X + 16);
            DTGenUtils.MakeDoor(Entry, Room.RoomSide.Both);

            Rectangle ForestFloor = new(ForestRoom.Interior.X, ForestRoom.Interior.Y + (ForestRoom.Interior.Height - 3), ForestRoom.Interior.Width, 3);
            WorldUtils.Gen(ForestFloor.Location, new GenShapeActionPair(new Shapes.Rectangle(ForestFloor.Width, ForestFloor.Height), new Actions.SetTile(TileID.Dirt)));
            for (int i = 0; i < ForestFloor.Width; i++)
            {
                int X = ForestFloor.Location.X + i;
                int Y = ForestFloor.Location.Y;
                WorldGen.PlaceTile(X, Y, TileID.CorruptGrass);
            }

            WorldGen.PlaceObject(ForestRoom.Interior.Location.X + 7, ForestRoom.Interior.Location.Y + ForestRoom.Interior.Height - 4, (ushort)ModContent.TileType<Tile_ForeignTree>());

        }

        public void ConnectingChute(int x, int y)
        {
            Point origin = new Point(x, y);


            //DTGenUtils.GenChute(x, y, 9, 5, 2, true, true, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
        }

        public void PlaceAndFillChests(int x, int y)
        {
            Point origin = new Point(x, y);

            int Loot1 = WorldGen.PlaceChest(origin.X + 6, origin.Y + 11, (ushort)ModContent.TileType<Tile_NightmareChest>());

            var chest1 = Main.chest[Loot1];

            int L1 = ModContent.ItemType<HekateBookPrologue>();
            int L2 = ModContent.ItemType<HekateBook1>();
            int L3 = ModContent.ItemType<HekateBook2>();

            int Loot2 = WorldGen.PlaceChest(origin.X + 28, origin.Y + 11, (ushort)ModContent.TileType<Tile_NightmareChest>());

            var chest2 = Main.chest[Loot2];

            int L4 = ModContent.ItemType<HekatesMystique>();
            int L5 = ModContent.ItemType<HekateBook3>();
            int L6 = ModContent.ItemType<HekateBook4>();
            int L7 = ModContent.ItemType<UnstableSeedPouch>();

            int Loot3 = WorldGen.PlaceChest(origin.X + 39, origin.Y + 26, (ushort)ModContent.TileType<Tile_NightmareChest>());

            var chest3 = Main.chest[Loot3];

            int L9 = ModContent.ItemType<Dyrn>();
            int L10 = ModContent.ItemType<IdriPotion>();

            for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
            {
                if (chest1.item[inventoryIndex].type == ItemID.None)
                {
                    // Place the item
                    chest1.item[0].SetDefaults(L1);
                    chest1.item[1].SetDefaults(L2);
                    chest1.item[2].SetDefaults(L9);
                    chest1.item[2].stack = Main.rand.Next(10, 21);
                    chest1.item[3].SetDefaults(L3);
                }

                if (chest2.item[inventoryIndex].type == ItemID.None)
                {
                    // Place the item
                    chest2.item[0].SetDefaults(L4);
                    chest2.item[1].SetDefaults(L5);
                    chest2.item[2].SetDefaults(L6);
                    chest2.item[3].SetDefaults(L7);
                    chest2.item[4].SetDefaults(L9);
                    chest2.item[4].stack = Main.rand.Next(10, 21);

                }

                if (chest3.item[inventoryIndex].type == ItemID.None)
                {
                    // Place the item
                    chest3.item[0].SetDefaults(L4);
                    chest3.item[1].SetDefaults(L5);
                    chest3.item[2].SetDefaults(L6);
                    chest3.item[3].SetDefaults(L7);
                    chest3.item[4].SetDefaults(L10);
                }
            }


        }



        public void MainGen(int x, int y)
        {
            Point origin = new Point(x, y);
            if (!ModLoader.HasMod("Remnants"))
            {
                ClearTopSpace(x, y);
            }

            TopFloor(x, y);
            BottomFloor(x, y);
            ThirdFloor(x, y);



            Vector2 OrigVec = origin.ToWorldCoordinates();
            Rectangle Rect = new Rectangle((int)OrigVec.X, (int)OrigVec.Y, 60, 43);
            if (DTCrossMod.FargosMutantIsLoaded)
            {
                DTCrossMod.FargosMutantMod.Call("AddIndestructibleRectangle", Rect);
            }
        }

        public List<int> InvalidGenTiles = new List<int>
        {
            TileID.BlueDungeonBrick,
            TileID.PinkDungeonBrick,
            TileID.GreenDungeonBrick,
            TileID.Mud,
            TileID.LihzahrdBrick,
            TileID.Crimstone,
        };

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.DestroyerTest.WorldGen.OminousGarden.PassMessage");

            bool structurePlaced = false;

            int attempts = (int)(Main.maxTilesX * Main.maxTilesY * 6E-05);
            for (int k = 0; k < attempts; k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);

                int y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200);

                Point sample = new Point(x, y);

                Tile tile = Framing.GetTileSafely(x, y);

                Rectangle Checkframe = new Rectangle(x, y, 60, 70);


                bool valid()
                {
                    if (Checkframe.Contains(sample))
                    {
                        if (InvalidGenTiles.Contains(tile.TileType))
                        {
                            return false;
                        }
                    }
                    return true;
                }

                
                if (tile.TileType == TileID.Ebonstone && valid())
                {
                    if (!structurePlaced)
                    {
                        MainGen(x, y);
                        structurePlaced = true;
                    }
                    if (DTCrossMod.FargosMutantIsLoaded)
                    {
                        DTCrossMod.FargosMutantMod.Call("AddIndestructibleRectangle", Checkframe);
                    }
                }

                if (structurePlaced)
                    break;

            }
        }

    }
}