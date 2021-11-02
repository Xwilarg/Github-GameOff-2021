using UnityEngine;

namespace Bug.Map
{
    public class Room
    {
        public Room(Vector2Int size, Vector2Int position)
            => (Id, Position, Size) = (_idRef++, size, position);

        public int Id { private set; get; }

        public Vector2Int Position { private set; get; }
        public Vector2Int Size { private set; get; }

        private static int _idRef = 0;
    }
}
