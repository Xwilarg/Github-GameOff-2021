using UnityEngine;

namespace Bug.Map
{
    public class Room : MonoBehaviour
    {
        public void Configure(Vector2Int size, Vector2 position, RoomState state, RoomInfo info, int distance)
            => (Id, Size, Position, State, Info, Distance, ZoneId)
            = (_idRef++, size, position, state, info, distance, state == RoomState.LOCKED ? -1 : 0);

        /// <summary>
        /// Unique id of the room
        /// </summary>
        public int Id { private set; get; }

        /// <summary>
        /// Position of the room
        /// </summary>
        public Vector2 Position { private set; get; }
        /// <summary>
        /// Size of the room, must be an even number
        /// </summary>
        public Vector2Int Size { private set; get; }
        /// <summary>
        /// Type of the room
        /// </summary>
        public RoomState State { private set; get; }
        /// <summary>
        /// Prefab information about the room
        /// </summary>
        public RoomInfo Info { private set; get; }
        /// <summary>
        /// Distance with the center (in number of generation steps)
        /// The lowest it is, the more steps away it'll be
        /// </summary>
        public int Distance { private set; get; }

        /// <summary>
        /// Rooms are grouped by zones, all zones are separated by closed doors
        /// </summary>
        public int ZoneId { set; get; }

        /// <summary>
        /// Room on top of this one
        /// </summary>
        public Room Up { set; get; }
        /// <summary>
        /// Room at the bottom of this one
        /// </summary>
        public Room Down { set; get; }
        /// <summary>
        /// Room at the left of this one
        /// </summary>
        public Room Left { set; get; }
        /// <summary>
        /// Room at the right of this one
        /// </summary>
        public Room Right { set; get; }

        private static int _idRef = 0;
    }
}
