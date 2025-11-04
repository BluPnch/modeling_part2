using lab_4.Interfaces;
using lab_4.Distributions;

namespace lab_4.Models
{
    public class ServiceDevice : IServiceDevice
    {
        private IRequest? _currentRequest;
        private double _serviceStartTime;
        public IDistribution Distribution { get; set; }

        public bool IsBusy => _currentRequest != null;
        public double ServiceTime { get; set; }
        public double ServiceCompletionTime { get; private set; }

        public ServiceDevice(IDistribution distribution)
        {
            Distribution = distribution;
            ServiceTime = Distribution.GenerateSample();
        }

        public void StartService(IRequest request, double currentTime)
        {
            _currentRequest = request;
            _serviceStartTime = currentTime;
            ServiceTime = Distribution.GenerateSample(); // Генерируем новое время обслуживания
            ServiceCompletionTime = currentTime + ServiceTime;
        }

        public IRequest? CompleteService(double currentTime)
        {
            if (_currentRequest != null && currentTime >= ServiceCompletionTime)
            {
                var completedRequest = _currentRequest;
                _currentRequest = null;
                return completedRequest;
            }
            return null;
        }
    }
}