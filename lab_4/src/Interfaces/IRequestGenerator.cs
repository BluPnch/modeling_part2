
namespace lab_4.Interfaces
{
    public interface IRequestGenerator
    {
        IRequest? GenerateRequest(double currentTime);
        double GetNextGenerationTime();
    }
}