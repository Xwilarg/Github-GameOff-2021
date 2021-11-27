using System.Collections.Generic;

namespace Bug.Objective
{
    public class GameProgression
    {
        public GameProgression(int sectorCount)
        {
            _sectorCount = sectorCount;
            _sectorUnlocked = new()
            {
                0
            };
        }
        private int _sectorCount;
        private List<int> _sectorUnlocked;
    }
}
