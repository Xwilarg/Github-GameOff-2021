﻿using Bug.SO;
using UnityEngine;

namespace Bug.Map
{
    public class Room
    {
        public Room(Vector2Int size, Vector2 position, RoomType type, RoomInfo info)
            => (Id, Size, Position, Type, Info) = (_idRef++, size, position, type, info);

        public int Id { private set; get; }

        public Vector2 Position { private set; get; }
        public Vector2Int Size { private set; get; }
        public RoomType Type { private set; get; }
        public RoomInfo Info { private set; get; }

        public Room Up { set; get; }
        public Room Down { set; get; }
        public Room Left { set; get; }
        public Room Right { set; get; }

        private static int _idRef = 0;
    }
}