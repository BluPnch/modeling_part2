using lab_3.Interfaces;

namespace lab_3
{
    public class SmoSystem
    {
        private readonly IRequestGenerator _generator;
        private readonly IQueue _queue;
        private readonly IServiceDevice _serviceDevice;

        public double CurrentTime { get; private set; }
        public int TotalRequests { get; private set; }
        public int ServedRequests { get; private set; }
        public List<string> Log { get; private set; }

        public SmoSystem(IRequestGenerator generator, IQueue queue, IServiceDevice serviceDevice)
        {
            _generator = generator;
            _queue = queue;
            _serviceDevice = serviceDevice;
            CurrentTime = 0;
            TotalRequests = 0;
            ServedRequests = 0;
            Log = new List<string>();
        }

        public void Reset()
        {
            CurrentTime = 0;
            TotalRequests = 0;
            ServedRequests = 0;
            Log.Clear();
        }

        // Пошаговый метод
        public void StepByStep(double timeStep, double simulationTime)
        {
            Reset();
            double nextGenerationTime = _generator.GetNextGenerationTime();

            while (CurrentTime <= simulationTime)
            {
                // Генерация заявок
                if (CurrentTime >= nextGenerationTime)
                {
                    var request = _generator.GenerateRequest(CurrentTime);
                    if (request != null)
                    {
                        TotalRequests++;
                        _queue.Enqueue(request);
                        Log.Add($"[{CurrentTime:F2}] Generated: {request}");
                    }
                    nextGenerationTime = CurrentTime + _generator.GetNextGenerationTime();
                }

                // Обслуживание заявок
                if (!_serviceDevice.IsBusy && _queue.Count > 0)
                {
                    var request = _queue.Dequeue();
                    if (request != null)
                    {
                        _serviceDevice.StartService(request, CurrentTime);
                        Log.Add($"[{CurrentTime:F2}] Started service: {request}");
                    }
                }

                // Завершение обслуживания
                var completedRequest = _serviceDevice.CompleteService(CurrentTime);
                if (completedRequest != null)
                {
                    ServedRequests++;
                    Log.Add($"[{CurrentTime:F2}] Completed service: {completedRequest}");
                }

                CurrentTime += timeStep;
            }
        }

        // Событийный метод
        public void EventBased(double simulationTime)
        {
            Reset();
            double nextGenerationTime = _generator.GetNextGenerationTime();
            double nextServiceCompletionTime = double.MaxValue;

            while (CurrentTime <= simulationTime)
            {
                // Определяем следующее событие
                double nextEventTime = Math.Min(nextGenerationTime, nextServiceCompletionTime);

                if (nextEventTime > simulationTime) break;

                CurrentTime = nextEventTime;

                // Обработка генерации заявки
                if (CurrentTime == nextGenerationTime)
                {
                    var request = _generator.GenerateRequest(CurrentTime);
                    if (request != null)
                    {
                        TotalRequests++;
                        _queue.Enqueue(request);
                        Log.Add($"[{CurrentTime:F2}] Generated: {request}");
                    }
                    nextGenerationTime = CurrentTime + _generator.GetNextGenerationTime();
                }

                // Обработка завершения обслуживания
                if (CurrentTime == nextServiceCompletionTime)
                {
                    var completedRequest = _serviceDevice.CompleteService(CurrentTime);
                    if (completedRequest != null)
                    {
                        ServedRequests++;
                        Log.Add($"[{CurrentTime:F2}] Completed service: {completedRequest}");
                    }
                    nextServiceCompletionTime = double.MaxValue;
                }

                // Начало нового обслуживания
                if (!_serviceDevice.IsBusy && _queue.Count > 0)
                {
                    var request = _queue.Dequeue();
                    if (request != null)
                    {
                        _serviceDevice.StartService(request, CurrentTime);
                        nextServiceCompletionTime = _serviceDevice.ServiceCompletionTime; // Теперь это работает
                        Log.Add($"[{CurrentTime:F2}] Started service: {request}");
                    }
                }
            }
        }

        public void FindOptimalQueueSize(double simulationTime, double meanGenerationTime, double serviceTime)
        {
            Log.Add("=== Поиск оптимального размера очереди ===");
            
            int optimalSize = 0;
            int minLostRequests = int.MaxValue;

            for (int queueSize = 1; queueSize <= 20; queueSize++)
            {
                // Создаем тестовые компоненты с правильными интерфейсами
                var testQueue = new Queue(queueSize); // Убедитесь, что Queue реализует IQueue
                var testGenerator = new RequestGenerator(meanGenerationTime); // Убедитесь, что реализует IRequestGenerator
                var testDevice = new ServiceDevice(serviceTime); // Убедитесь, что реализует IServiceDevice
                var testSmo = new SmoSystem(testGenerator, testQueue, testDevice);

                testSmo.EventBased(simulationTime);

                Log.Add($"Размер очереди: {queueSize}, Потеряно заявок: {testQueue.LostRequests}");

                if (testQueue.LostRequests < minLostRequests)
                {
                    minLostRequests = testQueue.LostRequests;
                    optimalSize = queueSize;
                }

                if (testQueue.LostRequests == 0)
                {
                    break; // Нашли размер без потерь
                }
            }

            Log.Add($"Оптимальный размер очереди: {optimalSize} (потерь: {minLostRequests})");
        }
    }
}