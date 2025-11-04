using lab_4.Interfaces;


namespace lab_4.Models
{
    public class ServiceDevice : IServiceDevice
    {
        private IRequest? _currentRequest;
        private double _serviceStartTime;

        public bool IsBusy => _currentRequest != null;
        public double ServiceTime { get; set; }
        public double ServiceCompletionTime { get; private set; }

        public ServiceDevice(double serviceTime)
        {
            ServiceTime = serviceTime;
        }

        public void StartService(IRequest request, double currentTime)
        {
            _currentRequest = request;
            _serviceStartTime = currentTime;
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