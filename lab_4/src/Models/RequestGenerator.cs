using System;
using lab_4.Interfaces;
using lab_4.Distributions;

namespace lab_4.Models
{
    public class RequestGenerator : IRequestGenerator
    {
        private readonly Random _random;
        public IDistribution Distribution { get; set; }

        public RequestGenerator(IDistribution distribution, int seed = 0)
        {
            Distribution = distribution;
            _random = seed == 0 ? new Random() : new Random(seed);
        }

        public IRequest? GenerateRequest(double currentTime)
        {
            return new Request(currentTime);
        }

        public double GetNextGenerationTime()
        {
            // Используем выбранное распределение для генерации времени
            return Distribution.GenerateSample();
        }
    }
}