using lab_4.Interfaces;


namespace lab_4.Models
{
    public class Request : IRequest
    {
        private static int _nextId = 1;

        public int Id { get; }
        public double GenerationTime { get; }

        public Request(double generationTime)
        {
            Id = _nextId++;
            GenerationTime = generationTime;
        }

        public override string ToString()
        {
            return $"Request #{Id} (generated at {GenerationTime:F2})";
        }
    }
}