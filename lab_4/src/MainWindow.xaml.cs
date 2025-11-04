using System;
using System.Windows;
using System.Windows.Controls;
using lab_4.Distributions;
using lab_4.Interfaces;
using lab_4.Models;


namespace lab_4
{
    public partial class MainWindow : Window
    {
        private SmoSystem? _smoSystem;
        private IRequestGenerator? _generator;
        private IQueue? _queue;
        private IServiceDevice? _serviceDevice;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                InitializeSmoSystem();
                txtStatus.Text = "Система готова к работе";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeSmoSystem()
        {
            try
            {
                int queueSize = GetIntValue(txtQueueSize.Text, 5);
                double meanTime = GetDoubleValue(txtMeanGenerationTime.Text, 2.0);
                double serviceTime = GetDoubleValue(txtServiceTime.Text, 1.5);

                var generatorDistribution = new ExponentialDistribution(1.0 / meanTime);
                var serviceDistribution = new ExponentialDistribution(1.0 / serviceTime);

                _generator = new RequestGenerator(generatorDistribution);
                _queue = new Queue(queueSize);
                _serviceDevice = new ServiceDevice(serviceDistribution);
                _smoSystem = new SmoSystem(_generator, _queue, _serviceDevice);

                txtStatus.Text = $"Система инициализирована (очередь: {queueSize})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации СМО: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double GetDoubleValue(string text, double defaultValue)
        {
            if (double.TryParse(text, out double result))
                return result;
            return defaultValue;
        }

        private int GetIntValue(string text, int defaultValue)
        {
            if (int.TryParse(text, out int result))
                return result;
            return defaultValue;
        }

        private void BtnStepByStep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeSmoSystem();
                if (_smoSystem == null) return;

                double simulationTime = GetDoubleValue("50.0", 50.0);
                _smoSystem.StepByStep(0.1, simulationTime);
                
                DisplayResults();
                UpdateStatistics();
                txtStatus.Text = "Пошаговое моделирование завершено";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка пошагового моделирования: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEventBased_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeSmoSystem();
                if (_smoSystem == null) return;

                double simulationTime = GetDoubleValue("50.0", 50.0);
                _smoSystem.EventBased(simulationTime);
                
                DisplayResults();
                UpdateStatistics();
                txtStatus.Text = "Событийное моделирование завершено";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка событийного моделирования: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFindOptimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double simulationTime = GetDoubleValue("50.0", 50.0);
                double meanTime = GetDoubleValue(txtMeanGenerationTime.Text, 2.0);
                double serviceTime = GetDoubleValue(txtServiceTime.Text, 1.5);

                InitializeSmoSystem();
                if (_smoSystem == null) return;

                _smoSystem.FindOptimalQueueSize(simulationTime, meanTime, serviceTime);
                
                DisplayResults();
                txtStatus.Text = "Поиск оптимальной очереди завершен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска оптимальной очереди: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
            _smoSystem?.Reset();
            UpdateStatistics();
            txtStatus.Text = "Результаты очищены";
        }

        private void DisplayResults()
        {
            if (_smoSystem == null) return;

            txtLog.Clear();
            foreach (var logEntry in _smoSystem.Log)
            {
                txtLog.AppendText(logEntry + "\n");
            }
            txtLog.ScrollToEnd();
        }

        private void UpdateStatistics()
        {
            try
            {
                if (_smoSystem == null) return;

                // Базовая статистика - можно добавить позже
                txtStatus.Text = $"Заявок: {_smoSystem.TotalRequests}, Обслужено: {_smoSystem.ServedRequests}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateStatistics: {ex.Message}");
            }
        }
    }
}