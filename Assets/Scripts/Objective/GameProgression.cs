namespace Bug.Objective
{
    public class GameProgression
    {
        public GameProgression(int sectorCount)
        {
            _sectorCount = sectorCount;
            _sectorUnlocked = 1;
        }
        private int _sectorCount;
        private int _sectorUnlocked;
    }
}
