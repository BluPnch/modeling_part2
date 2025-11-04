using System;
using lab_4.Interfaces;


namespace lab_4.Models
{
    public class RequestGenerator : IRequestGenerator
    {
        private readonly Random _random;
        private readonly double _meanGenerationTime;

        public RequestGenerator(double meanGenerationTime, int seed = 0)
        {
            _meanGenerationTime = meanGenerationTime;
            _random = seed == 0 ? new Random() : new Random(seed);
        }

        public IRequest? GenerateRequest(double currentTime)
        {
            return new Request(currentTime);
        }

        public double GetNextGenerationTime()
        {
            // Экспоненциальное распределение для времени между заявками
            return -_meanGenerationTime * Math.Log(1 - _random.NextDouble());
        }
    }
}