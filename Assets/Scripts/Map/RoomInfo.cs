using UnityEngine;

namespace Bug.Map
{
    public class RoomInfo
    {
        public RoomInfo()
            => (Id, Position, Size) = (_idRef++, Vector2Int.zero, Vector2Int.one);

        public int Id { private set; get; }

        public Vector2Int Position { private set; get; }
        public Vector2Int Size { private set; get; }

        private static int _idRef = 0;
    }
}
