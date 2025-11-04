using lab_4.Distributions;

namespace lab_4.Interfaces
{
    public interface IRequestGenerator
    {
        IRequest? GenerateRequest(double currentTime);
        double GetNextGenerationTime();
        IDistribution Distribution { get; set; }
    }
}