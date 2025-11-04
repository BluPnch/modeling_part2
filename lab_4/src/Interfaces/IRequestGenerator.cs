namespace lab_3.Interfaces
{
    public interface IRequestGenerator
    {
        IRequest? GenerateRequest(double currentTime);
        double GetNextGenerationTime();
    }
}