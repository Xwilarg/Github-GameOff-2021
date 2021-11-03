using UnityEngine;

namespace Bug.Map
{
    public class Room
    {
        public Room(Vector2Int size, Vector2 position)
            => (Id, Size, Position) = (_idRef++, size, position);

        public int Id { private set; get; }

        public Vector2 Position { private set; get; }
        public Vector2Int Size { private set; get; }

        private static int _idRef = 0;
    }
}
