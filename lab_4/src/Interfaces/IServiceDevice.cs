namespace lab_4.Interfaces
{
    public interface IServiceDevice
    {
        bool IsBusy { get; }
        double ServiceTime { get; set; }
        double ServiceCompletionTime { get; } 
        void StartService(IRequest request, double currentTime);
        IRequest? CompleteService(double currentTime);
    }
}