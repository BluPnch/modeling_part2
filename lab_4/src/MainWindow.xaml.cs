using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using lab_4.Interfaces;
using lab_4.Models;

namespace lab_4
{
    public partial class MainWindow : Window
    {
        private SmoSystem _smoSystem;
        private IRequestGenerator _generator;
        private IQueue _queue;
        private IServiceDevice _serviceDevice;

        public MainWindow()
        {
            InitializeComponent();
            SetDefaultValues();
            InitializeSmoSystem();
        }

        private void InitializeSmoSystem()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMeanGenerationTime.Text) ||
                    string.IsNullOrWhiteSpace(txtServiceTime.Text) || 
                    string.IsNullOrWhiteSpace(txtQueueSize.Text))
                {
                    txtMeanGenerationTime.Text = "2,0";
                    txtServiceTime.Text = "1,5";
                    txtQueueSize.Text = "5";
                }

                double meanGenerationTime = double.Parse(txtMeanGenerationTime.Text);
                double serviceTime = double.Parse(txtServiceTime.Text);
                int queueSize = int.Parse(txtQueueSize.Text);

                // Проверка на корректность значений
                if (meanGenerationTime <= 0 || serviceTime <= 0 || queueSize <= 0)
                {
                    throw new ArgumentException("Все параметры должны быть положительными числами");
                }

                _generator = new RequestGenerator(meanGenerationTime);
                _queue = new Queue(queueSize);
                _serviceDevice = new ServiceDevice(serviceTime);
                _smoSystem = new SmoSystem(_generator, _queue, _serviceDevice);

                txtStatus.Text = "Система инициализирована";
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка формата: введите числовые значения", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                // Установка значений по умолчанию
                SetDefaultValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                SetDefaultValues();
            }
        }

        private void SetDefaultValues()
        {
            txtMeanGenerationTime.Text = "2,0";
            txtServiceTime.Text = "1,5";
            txtQueueSize.Text = "5";
            txtSimulationTime.Text = "50,0";
            
            _generator = new RequestGenerator(2.0);
            _queue = new Queue(5);
            _serviceDevice = new ServiceDevice(1.5);
            _smoSystem = new SmoSystem(_generator, _queue, _serviceDevice);
        }

        private void UpdateParameters()
        {
            try
            {
                double meanGenerationTime = double.Parse(txtMeanGenerationTime.Text);
                double serviceTime = double.Parse(txtServiceTime.Text);
                int queueSize = int.Parse(txtQueueSize.Text);

                _generator = new RequestGenerator(meanGenerationTime);
                _queue = new Queue(queueSize);
                _serviceDevice = new ServiceDevice(serviceTime);
                _smoSystem = new SmoSystem(_generator, _queue, _serviceDevice); // ДОБАВЬТЕ ЭТУ СТРОЧКУ
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в параметрах: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnStepByStep_Click(object sender, RoutedEventArgs e)
        {
            await RunSimulationAsync(async () =>
            {
                double timeStep = 0.1;
                double simulationTime = double.Parse(txtSimulationTime.Text);
                
                await Task.Run(() => _smoSystem.StepByStep(timeStep, simulationTime));
                
                Dispatcher.Invoke(() =>
                {
                    DisplayResults();
                    UpdateStatistics();
                });
            }, "Пошаговое моделирование завершено");
        }

        private async void BtnEventBased_Click(object sender, RoutedEventArgs e)
        {
            await RunSimulationAsync(async () =>
            {
                double simulationTime = double.Parse(txtSimulationTime.Text);
                
                await Task.Run(() => _smoSystem.EventBased(simulationTime));
                
                Dispatcher.Invoke(() =>
                {
                    DisplayResults();
                    UpdateStatistics();
                });
            }, "Событийное моделирование завершено");
        }

        private async void BtnFindOptimal_Click(object sender, RoutedEventArgs e)
        {
            await RunSimulationAsync(async () =>
            {
                double meanGenerationTime = double.Parse(txtMeanGenerationTime.Text);
                double serviceTime = double.Parse(txtServiceTime.Text);
                double simulationTime = double.Parse(txtSimulationTime.Text);
                
                await Task.Run(() => _smoSystem.FindOptimalQueueSize(simulationTime, meanGenerationTime, serviceTime));
                
                Dispatcher.Invoke(() =>
                {
                    DisplayOptimalQueueResults();
                    UpdateStatistics();
                });
            }, "Поиск оптимальной очереди завершен");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
            txtOptimalQueue.Clear();
            _smoSystem?.Reset();
            UpdateStatistics();
            txtStatus.Text = "Результаты очищены";
        }

        private async Task RunSimulationAsync(Func<Task> simulationAction, string completionMessage)
        {
            try
            {
                UpdateParameters();
                SetUiEnabled(false);
                progressBar.Visibility = Visibility.Visible;
                txtStatus.Text = "Моделирование выполняется...";

                await simulationAction();

                txtStatus.Text = completionMessage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка моделирования: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка при моделировании";
            }
            finally
            {
                SetUiEnabled(true);
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void SetUiEnabled(bool enabled)
        {
            btnStepByStep.IsEnabled = enabled;
            btnEventBased.IsEnabled = enabled;
            btnFindOptimal.IsEnabled = enabled;
            btnClear.IsEnabled = enabled;
            txtMeanGenerationTime.IsEnabled = enabled;
            txtServiceTime.IsEnabled = enabled;
            txtQueueSize.IsEnabled = enabled;
            txtSimulationTime.IsEnabled = enabled;
        }

        private void DisplayResults()
        {
            txtLog.Clear();
            foreach (var logEntry in _smoSystem.Log)
            {
                txtLog.AppendText(logEntry + "\n");
            }
            txtLog.ScrollToEnd();
        }

        private void DisplayOptimalQueueResults()
        {
            txtOptimalQueue.Clear();
            foreach (var logEntry in _smoSystem.Log)
            {
                if (logEntry.Contains("Размер очереди") || logEntry.Contains("Оптимальный"))
                {
                    txtOptimalQueue.AppendText(logEntry + "\n");
                }
            }
        }

        private void UpdateStatistics()
        {
            try
            {
                if (_smoSystem == null) return;

                txtTotalRequests.Text = $"Всего заявок: {_smoSystem.TotalRequests}";
                txtServedRequests.Text = $"Обслужено заявок: {_smoSystem.ServedRequests}";
                txtCurrentTime.Text = $"Текущее время: {_smoSystem.CurrentTime:F2}";

                if (_smoSystem.TotalRequests > 0)
                {
                    double serviceRate = (double)_smoSystem.ServedRequests / _smoSystem.TotalRequests * 100;
                    txtServiceRate.Text = $"Процент обслуженных: {serviceRate:F2}%";
                }
                else
                {
                    txtServiceRate.Text = "Процент обслуженных: 0%";
                }

                // Используем LostRequests из SmoSystem или Queue
                int lostRequests = 0;
                if (_queue != null)
                {
                    lostRequests = _queue.LostRequests;
                }
                txtLostRequests.Text = $"Потеряно заявок: {lostRequests}";
            }
            catch (Exception ex)
            {
                // Временно скроем ошибку при обновлении статистики
                Console.WriteLine($"Error in UpdateStatistics: {ex.Message}");
            }
        }

        // Обработчики изменений параметров
        private void TxtParameter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateParameters();
            UpdateStatistics();
        }
    }
}