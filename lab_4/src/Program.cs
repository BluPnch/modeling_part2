namespace lab_3;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Система Массового Обслуживания (СМО) ===");

        // Параметры системы
        double meanGenerationTime = 2.0; // Среднее время между заявками
        double serviceTime = 1.5;        // Время обслуживания
        int initialQueueSize = 5;        // Начальный размер очереди
        double simulationTime = 50.0;    // Время моделирования
        double timeStep = 0.1;           // Шаг для пошагового метода

        // Создание компонентов СМО
        var generator = new RequestGenerator(meanGenerationTime);
        var queue = new Queue(initialQueueSize);
        var serviceDevice = new ServiceDevice(serviceTime);
        var smoSystem = new SmoSystem(generator, queue, serviceDevice);

        while (true)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1 - Пошаговый метод");
            Console.WriteLine("2 - Событийный метод");
            Console.WriteLine("3 - Найти оптимальный размер очереди");
            Console.WriteLine("4 - Изменить параметры");
            Console.WriteLine("0 - Выход");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    RunStepByStep(smoSystem, timeStep, simulationTime);
                    break;
                case "2":
                    RunEventBased(smoSystem, simulationTime);
                    break;
                case "3":
                    FindOptimalQueueSize(smoSystem, simulationTime, meanGenerationTime, serviceTime);
                    break;
                case "4":
                    ChangeParameters(ref meanGenerationTime, ref serviceTime, ref simulationTime, ref timeStep);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }
    }

    static void RunStepByStep(SmoSystem smoSystem, double timeStep, double simulationTime)
    {
        Console.WriteLine($"\n=== Пошаговый метод (шаг: {timeStep}, время: {simulationTime}) ===");
        
        smoSystem.StepByStep(timeStep, simulationTime);
        
        DisplayResults(smoSystem);
    }

    static void RunEventBased(SmoSystem smoSystem, double simulationTime)
    {
        Console.WriteLine($"\n=== Событийный метод (время: {simulationTime}) ===");
        
        smoSystem.EventBased(simulationTime);
        
        DisplayResults(smoSystem);
    }

    static void FindOptimalQueueSize(SmoSystem smoSystem, double simulationTime, double meanGenerationTime, double serviceTime)
    {
        Console.WriteLine($"\n=== Поиск оптимального размера очереди ===");
        Console.WriteLine($"Среднее время между заявками: {meanGenerationTime}");
        Console.WriteLine($"Время обслуживания: {serviceTime}");
        
        smoSystem.FindOptimalQueueSize(simulationTime, meanGenerationTime, serviceTime);
        
        // Выводим только результаты поиска
        foreach (var logEntry in smoSystem.Log)
        {
            if (logEntry.Contains("Размер очереди") || logEntry.Contains("Оптимальный"))
            {
                Console.WriteLine(logEntry);
            }
        }
    }

    static void ChangeParameters(ref double meanGenerationTime, ref double serviceTime, 
        ref double simulationTime, ref double timeStep)
    {
        Console.Write($"Среднее время между заявками ({meanGenerationTime}): ");
        if (double.TryParse(Console.ReadLine(), out double newGenTime) && newGenTime > 0)
            meanGenerationTime = newGenTime;

        Console.Write($"Время обслуживания ({serviceTime}): ");
        if (double.TryParse(Console.ReadLine(), out double newServTime) && newServTime > 0)
            serviceTime = newServTime;

        Console.Write($"Время моделирования ({simulationTime}): ");
        if (double.TryParse(Console.ReadLine(), out double newSimTime) && newSimTime > 0)
            simulationTime = newSimTime;

        Console.Write($"Шаг для пошагового метода ({timeStep}): ");
        if (double.TryParse(Console.ReadLine(), out double newStep) && newStep > 0)
            timeStep = newStep;
    }

    static void DisplayResults(SmoSystem smoSystem)
    {
        Console.WriteLine("\n=== Результаты моделирования ===");
        Console.WriteLine($"Общее время: {smoSystem.CurrentTime:F2}");
        Console.WriteLine($"Всего заявок: {smoSystem.TotalRequests}");
        Console.WriteLine($"Обслужено заявок: {smoSystem.ServedRequests}");
        
        if (smoSystem.TotalRequests > 0)
        {
            double serviceRate = (double)smoSystem.ServedRequests / smoSystem.TotalRequests * 100;
            Console.WriteLine($"Процент обслуженных заявок: {serviceRate:F2}%");
        }

        Console.WriteLine("\n=== Лог событий (первые 20 записей) ===");
        for (int i = 0; i < Math.Min(20, smoSystem.Log.Count); i++)
        {
            Console.WriteLine(smoSystem.Log[i]);
        }

        if (smoSystem.Log.Count > 20)
        {
            Console.WriteLine("... и еще " + (smoSystem.Log.Count - 20) + " записей");
        }
    }
}