using FargowiltasSouls.Content.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace DestroyerTest.Common
{

    public class Room
    {
        public enum RoomSide
        {
            Both = 0,
            Left = -1,
            Right = 1
        }

        public enum HatchSide
        {
            Top = -1,
            Bottom = 1
        }


        public Rectangle Bounds;

        public ushort TileType = 0;
        public ushort WallType = 0;

        public int LeftWall = 2;
        public int RightWall = 2;
        public int Ceiling = 2;
        public int Floor = 2;

        public Room(int x, int y, int width, int height, ushort tileType, ushort wallType)
        {
            Bounds = new Rectangle(x, y, width, height);

            TileType = tileType;
            WallType = wallType;
        }

        public Room(int x, int y, int width, int height, int leftWallWidth, int rightWallWidth, int floorWidth, int ceilingWidth, ushort tileType, ushort wallType)
        {
            Bounds = new Rectangle(x, y, width, height);

            TileType = tileType;
            WallType = wallType;

            LeftWall = leftWallWidth;
            RightWall = rightWallWidth;
            Floor = floorWidth;
            Ceiling = ceilingWidth;
        }

        public static Room CenteredRoom(int x, int y, int width, int height, ushort tileType, ushort wallType, int leftWallWidth = 2, int rightWallWidth = 2, int floorWidth = 2, int ceilingWidth = 2)
        {
            int XOff = x - (width / 2);
            int YOff = y - (height / 2);
            return new Room(XOff, YOff, width, height, leftWallWidth, rightWallWidth, floorWidth, ceilingWidth, tileType, wallType);
        }

        public Point Position => Bounds.Location;
        public int FloorLevel => Position.Y + (Bounds.Height - Floor);

        public Point Center =>
            new Point(
                Bounds.Center.X,
                Bounds.Center.Y
            );

        public Rectangle Interior =>
            new Rectangle(
                Bounds.Location.X + LeftWall,
                Bounds.Location.Y + Ceiling,
                Bounds.Width - LeftWall - RightWall,
                Bounds.Height - Ceiling - Floor
            );


        //Doors

        private static void CarveDoor(int x, int y)
        {
            for (int i = 0; i < 3; i++)
            {
                WorldGen.KillTile(x, y - i);
            }

            WorldGen.PlaceObject(x, y, TileID.ClosedDoor);
        }

        public static void MakeDoor(Room room, RoomSide side)
        {
            if (side == RoomSide.Left || side == RoomSide.Both)
            {
                int x = room.Bounds.Left + (room.LeftWall / 2);
                int y = room.Interior.Bottom - 1;

                CarveDoor(x, y);
            }

            if (side == RoomSide.Right || side == RoomSide.Both)
            {
                int x = room.Bounds.Right - 1 - (room.RightWall / 2);
                int y = room.Interior.Bottom - 1;

                CarveDoor(x, y);
            }
        }

        public static void MakeHatch(Room room, HatchSide side, int Width)
        {
            int minX = room.Interior.Left;
            int maxX = room.Interior.Right - 1;

            int hatchWidth = Width;
            hatchWidth = Math.Min(hatchWidth, maxX - minX + 1);

            int startX = minX + (maxX - minX - hatchWidth) / 2;
            int endX = startX + hatchWidth;

            int yTop = room.Interior.Top;
            int yBottom = room.Interior.Bottom - 1;

            int y()
            {
                if (side == HatchSide.Top)
                {
                    return yTop;
                }
                if (side == HatchSide.Bottom)
                {
                    return yBottom;
                }

                return 0;
            }

            for (int x = startX; x <= endX; x++)
            {
                for (int i = 0; i < room.Ceiling; i++)
                {
                    WorldGen.KillTile(x, y() - i);
                }
            }

        }
    }

    public class Hallway
    {
        Room Start;
        Room End;

        public Rectangle Bounds;

        public ushort TileType = 1;
        public ushort WallType = 1;

        int Ceiling;
        int Floor;

        public Hallway(Room start, Room end, int height, ushort tileType, ushort wallType, int ceilingWidth = 2, int floorWidth = 2)
        {
            int x = start.Bounds.Right;
            int y = start.Bounds.Bottom - height;

            int width = end.Bounds.Left - x;

            Bounds = new Rectangle(x, y, width, height);

            Start = start;
            End = end;
            Ceiling = ceilingWidth;
            Floor = floorWidth;
            TileType = tileType;
            WallType = wallType;
        }

        public Point Position => Bounds.Location;

        public Point Center =>
            new Point(
                Bounds.Center.X,
                Bounds.Center.Y
            );

        public Rectangle Interior =>
            new Rectangle(
                Bounds.Location.X,
                Bounds.Location.Y + Ceiling,
                Bounds.Width,
                Bounds.Height - Ceiling - Floor
            );
    }

    public class Chute
    {
        Room Start;
        Room End;

        public Rectangle Bounds;

        public ushort TileType = 1;
        public ushort WallType = 1;

        int LeftWall;
        int RightWall;

        public Chute(Room start, Room end, int x, int Width, ushort tileType, ushort wallType, int leftWidth = 2, int rightWidth = 2)
        {
            int top = Math.Min(start.Bounds.Bottom, end.Bounds.Top);
            int bottom = Math.Max(start.Bounds.Bottom, end.Bounds.Top);

            int height = bottom - top;

            int maxWidth = Math.Min(start.Bounds.Width, end.Bounds.Width);
            Width = Math.Min(Width, maxWidth);

            Bounds = new Rectangle(x, top, Width, height);

            Start = start;
            End = end;
            TileType = tileType;
            WallType = wallType;
            LeftWall = leftWidth;
            RightWall = rightWidth;
        }
        public Point Position => Bounds.Location;

        public Point Center =>
            new Point(
                Bounds.Center.X,
                Bounds.Center.Y
            );

        public Rectangle Interior =>
            new Rectangle(
                Bounds.Location.X + LeftWall,
                Bounds.Location.Y,
                Bounds.Width - LeftWall - RightWall,
                Bounds.Height
            );
    }
}
