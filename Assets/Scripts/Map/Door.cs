using System.Collections.Generic;
using UnityEngine;

namespace Bug.Map
{
    public class Door : MonoBehaviour
    {
        public List<int> LinkedZones { get; } = new();
    }
}
