using System;

namespace MessagePublisher.Utility
{
    public class RandomGenerator
    {
        public static RandomGenerator Instance = new RandomGenerator();
        private Random _rnd;

        private RandomGenerator()
        {
            _rnd = new Random();
        }

        public int Next(int minValue, int maxValue)
        {
            lock (_rnd)
            {
                return _rnd.Next(minValue, maxValue);
            }
        }
    }
}
