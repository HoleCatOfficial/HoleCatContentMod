using System;
using System.Collections.Generic;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Lorebooks;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Altar;
using DestroyerTest.Content.Tiles.RoseGarden;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SteelSeries.GameSense;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
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
using static DestroyerTest.Content.Tiles.Tile_MemoryPedistal;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;

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



                TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            }


        }

        private void VesperGenTest(int x, int y)
        {
            

            WorldGen.OreRunner(x, y, 5, 2, (ushort)ModContent.TileType<Tile_VesperOre>());

            WorldGen.OreRunner(x, y, 10, 3, (ushort)ModContent.TileType<Tile_Dreamstone>());
        }

        public void SetupPath(int x, int y)
        {


            /*
            WorldGen.TileRunner(x, y, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 10, y, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 20, y, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 30, y, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 40, y, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 50, y, 20, 4, TileID.Ebonstone, true, overRide: true);

            WorldGen.TileRunner(x, y + 10, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 10, y + 10, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 20, y + 10, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 30, y + 10, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 40, y + 10, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 50, y + 10, 20, 4, TileID.Ebonstone, true, overRide: true);

            WorldGen.TileRunner(x, y + 20, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 10, y + 20, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 20, y + 20, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 30, y + 20, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 40, y + 20, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 50, y + 20, 20, 4, TileID.Ebonstone, true, overRide: true);

            WorldGen.TileRunner(x, y + 30, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 10, y + 30, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 20, y + 30, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 30, y + 30, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 40, y + 30, 20, 4, TileID.Ebonstone, true, overRide: true);
            WorldGen.TileRunner(x + 50, y + 30, 20, 4, TileID.Ebonstone, true, overRide: true);


            WorldGen.TileRunner(x - 7, y - 6, 9, 12, TileID.Ebonstone, true, 1, -3, overRide: true);
            WorldGen.TileRunner(x + 53, y - 6, 9, 12, TileID.Ebonstone, true, -1, -3, overRide: true);
            */
        }

        public void TopGen(int x, int y)
        {
            WorldGen.PlaceTile(x, y, TileID.GoldBrick);
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

            Room StatueRoom = new Room(x + 22, y + 24, 16, 16, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(StatueRoom);
            DTGenUtils.MakeDoor(StatueRoom, Room.RoomSide.Both);

            Hallway EntryToStatue = new Hallway(Entry, StatueRoom, 9, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenHallway(EntryToStatue);

            Room SeedBank = new Room(x - 49, y + 25, 31, 15, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(SeedBank);
            DTGenUtils.MakeDoor(SeedBank, Room.RoomSide.Both);

            void furnishEntry()
            {
                WorldGen.PlaceObject(Entry.Interior.Location.X + 8, Entry.Interior.Location.Y + 2, BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 11, Entry.Interior.Location.Y, TileID.BrazierSuspended);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 13, Entry.Interior.Location.Y + 2, BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 16, Entry.Interior.Location.Y, TileID.BrazierSuspended);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 18, Entry.Interior.Location.Y + 2, BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 22, Entry.Interior.Location.Y + 2, ModContent.TileType<Tile_IdriPainting>());

                WorldGen.PlaceObject(Entry.Interior.Location.X + 26, Entry.Interior.Location.Y + 13, TileID.Dressers, style: 1);

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


                    int Loot1 = WorldGen.PlaceChest(X, Y + 1, TileID.Containers, style: 0);
                }



                for (int i = 0; i < 7; i++)
                {
                    int X = (SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 3) + i;
                    int Y = SeedBank.Interior.Location.Y + 4;

                    WorldGen.PlaceObject(X, Y, TileID.Platforms, style: 1);
                }

                WorldGen.PlaceObject((SeedBank.Interior.Location.X + (SeedBank.Interior.Width / 2) - 3), SeedBank.Interior.Location.Y + 3, (ushort)ModContent.TileType<NeglectedRegardsDisplay>());


                WorldGen.PlaceObject(SeedBank.Interior.Location.X + 7, SeedBank.Interior.Location.Y, TalismanVariants[Main.rand.Next(TalismanVariants.Length)]);
                WorldGen.PlaceObject(SeedBank.Interior.Location.X + SeedBank.Interior.Width - 8, SeedBank.Interior.Location.Y, TalismanVariants[Main.rand.Next(TalismanVariants.Length)]);

            }



            furnishEntry();
            furnishSeedBank();
      
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
            int L7 = ModContent.ItemType<MalachiteKnives>();

            int Loot3= WorldGen.PlaceChest(origin.X + 39, origin.Y + 26, (ushort)ModContent.TileType<Tile_NightmareChest>());

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

            SetupPath(x, y);

            ClearTopSpace(x, y);

            TopFloor(x, y);

            BottomFloor(x, y);

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

            furnishChestTopFloor();



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

            Room StatueRoom = new Room(x + 22, y + 24, 16, 16, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenRoom(StatueRoom);
            DTGenUtils.MakeDoor(StatueRoom, Room.RoomSide.Both);

            Hallway EntryToStatue = new Hallway(Entry, StatueRoom, 9, TileID.EbonstoneBrick, WallID.EbonstoneBrick);
            DTGenUtils.GenHallway(EntryToStatue);

            void furnishEntry()
            {
                WorldGen.PlaceObject(Entry.Interior.Location.X + 8, Entry.Interior.Location.Y + 2, BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 11, Entry.Interior.Location.Y, TileID.BrazierSuspended);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 13, Entry.Interior.Location.Y + 2, BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 16, Entry.Interior.Location.Y, TileID.BrazierSuspended);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 18, Entry.Interior.Location.Y + 2, BookcaseVariants[Main.rand.Next(BookcaseVariants.Length)]);

                WorldGen.PlaceObject(Entry.Interior.Location.X + 22, Entry.Interior.Location.Y + 2, ModContent.TileType<Tile_IdriPainting>());

                WorldGen.PlaceObject(Entry.Interior.Location.X + 26, Entry.Interior.Location.Y + 13, TileID.Dressers, style: 1);

                Point ChestPoint = new(Entry.Interior.Location.X + 11, Entry.Interior.Location.Y + 13);
                int Loot1;
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
            }



            furnishEntry();


            //DTGenUtils.GenLitRoomWithDoors(origin.X - 10, origin.Y - 6, 20, 12, 2, 2, true, TileID.EbonstoneBrick, WallID.EbonstoneBrick, 18);

            //DTGenUtils.GenLitRoomWithDoors(origin.X + 20, origin.Y, 25, 14, 2, 2, true, TileID.EbonstoneBrick, WallID.EbonstoneBrick, 18);


            //Setup Hallways

            //DTGenUtils.GenHallway(origin.X + 10, origin.Y + 6, 10, 8, 2, true, TileID.EbonstoneBrick, WallID.EbonstoneBrick, 18);
        }


        public void MainGen(int x, int y)
        {
            Point origin = new Point(x, y);
            ClearTopSpace(x, y);

            TopFloor(x, y);
            BottomFloor(x, y);

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