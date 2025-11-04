using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using lab_4.Interfaces;
using lab_4.Models;
using lab_4.Distributions;
using lab_4.Helpers;

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
            try
            {
                InitializeComponent();
                SetDefaultValues();
            
                // Инициализируем поля значениями по умолчанию
                var generatorDistribution = new ExponentialDistribution(0.5);
                var serviceDistribution = new ExponentialDistribution(0.666);
            
                _generator = new RequestGenerator(generatorDistribution);
                _queue = new Queue(5);
                _serviceDevice = new ServiceDevice(serviceDistribution);
                _smoSystem = new SmoSystem(_generator, _queue, _serviceDevice);
            
                // Подписываемся на событие загрузки
                this.Loaded += MainWindow_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации окна: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbGeneratorDistribution != null && cmbServiceDistribution != null)
                {
                    UpdateDistributionLabels();
                    InitializeSmoSystem();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки окна: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private IDistribution CreateGeneratorDistribution()
        {
            try
            {
                if (cmbGeneratorDistribution?.SelectedItem == null)
                    return new ExponentialDistribution(0.5);

                var selectedItem = cmbGeneratorDistribution.SelectedItem as ComboBoxItem;
                string distributionType = selectedItem?.Tag as string ?? "Exponential";
                
                if (string.IsNullOrEmpty(txtGeneratorParam1?.Text))
                    return new ExponentialDistribution(0.5);

                double param1 = NumberParser.ParseDouble(txtGeneratorParam1.Text);
                double param2 = string.IsNullOrEmpty(txtGeneratorParam2?.Text) ? 0.5 : NumberParser.ParseDouble(txtGeneratorParam2.Text);

                return distributionType switch
                {
                    "Uniform" => new UniformDistribution(0, param1 * 2),
                    "Exponential" => new ExponentialDistribution(1.0 / param1),
                    "Normal" => new NormalDistribution(param1, param2),
                    "Poisson" => new PoissonDistribution(param1),
                    "Erlang" => new ErlangDistribution((int)param2, (int)param2 / param1),
                    _ => new ExponentialDistribution(1.0 / param1)
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в параметрах генератора: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new ExponentialDistribution(0.5);
            }
        }

        private IDistribution CreateServiceDistribution()
        {
            try
            {
                if (cmbServiceDistribution == null || cmbServiceDistribution.SelectedItem == null)
                    return new ExponentialDistribution(0.666);

                var selectedItem = cmbServiceDistribution.SelectedItem as ComboBoxItem;
                string distributionType = selectedItem?.Tag as string ?? "Exponential";
                
                if (string.IsNullOrEmpty(txtServiceParam1?.Text))
                    return new ExponentialDistribution(0.666);

                double param1 = NumberParser.ParseDouble(txtServiceParam1.Text);
                double param2 = string.IsNullOrEmpty(txtServiceParam2?.Text) ? 0.3 : NumberParser.ParseDouble(txtServiceParam2.Text);

                return distributionType switch
                {
                    "Uniform" => new UniformDistribution(0, param1 * 2),
                    "Exponential" => new ExponentialDistribution(1.0 / param1),
                    "Normal" => new NormalDistribution(param1, param2),
                    "Poisson" => new PoissonDistribution(param1),
                    "Erlang" => new ErlangDistribution((int)param2, (int)param2 / param1),
                    _ => new ExponentialDistribution(1.0 / param1)
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в параметрах обслуживания: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new ExponentialDistribution(0.666);
            }
        }

        private void UpdateDistributionLabels()
        {
            UpdateGeneratorDistributionLabels();
            UpdateServiceDistributionLabels();
        }
        
        private void UpdateGeneratorDistributionLabels()
        {
            var selectedItem = cmbGeneratorDistribution.SelectedItem as ComboBoxItem;
            string distributionType = selectedItem?.Tag as string ?? "Exponential";

            switch (distributionType)
            {
                case "Uniform":
                    txtGeneratorParam1Label.Text = "Максимум:";
                    txtGeneratorParam2Label.Visibility = Visibility.Collapsed;
                    txtGeneratorParam2.Visibility = Visibility.Collapsed;
                    txtGeneratorParam1.Text = "4.0"; // 2 * mean
                    break;
                case "Exponential":
                    txtGeneratorParam1Label.Text = "Среднее:";
                    txtGeneratorParam2Label.Visibility = Visibility.Collapsed;
                    txtGeneratorParam2.Visibility = Visibility.Collapsed;
                    txtGeneratorParam1.Text = "2.0";
                    break;
                case "Normal":
                    txtGeneratorParam1Label.Text = "Среднее:";
                    txtGeneratorParam2Label.Text = "Станд. отклонение:";
                    txtGeneratorParam2Label.Visibility = Visibility.Visible;
                    txtGeneratorParam2.Visibility = Visibility.Visible;
                    txtGeneratorParam1.Text = "2.0";
                    txtGeneratorParam2.Text = "0.5";
                    break;
                case "Poisson":
                    txtGeneratorParam1Label.Text = "Лямбда:";
                    txtGeneratorParam2Label.Visibility = Visibility.Collapsed;
                    txtGeneratorParam2.Visibility = Visibility.Collapsed;
                    txtGeneratorParam1.Text = "2.0";
                    break;
                case "Erlang":
                    txtGeneratorParam1Label.Text = "Среднее:";
                    txtGeneratorParam2Label.Text = "Форма (k):";
                    txtGeneratorParam2Label.Visibility = Visibility.Visible;
                    txtGeneratorParam2.Visibility = Visibility.Visible;
                    txtGeneratorParam1.Text = "2.0";
                    txtGeneratorParam2.Text = "3";
                    break;
            }
        }

        private void UpdateServiceDistributionLabels()
        {
            var selectedItem = cmbServiceDistribution.SelectedItem as ComboBoxItem;
            string distributionType = selectedItem?.Tag as string ?? "Exponential";

            switch (distributionType)
            {
                case "Uniform":
                    txtServiceParam1Label.Text = "Максимум:";
                    txtServiceParam2Label.Visibility = Visibility.Collapsed;
                    txtServiceParam2.Visibility = Visibility.Collapsed;
                    txtServiceParam1.Text = "3,0"; // 2 * mean
                    break;
                case "Exponential":
                    txtServiceParam1Label.Text = "Среднее:";
                    txtServiceParam2Label.Visibility = Visibility.Collapsed;
                    txtServiceParam2.Visibility = Visibility.Collapsed;
                    txtServiceParam1.Text = "1,5";
                    break;
                case "Normal":
                    txtServiceParam1Label.Text = "Среднее:";
                    txtServiceParam2Label.Text = "Станд. отклонение:";
                    txtServiceParam2Label.Visibility = Visibility.Visible;
                    txtServiceParam2.Visibility = Visibility.Visible;
                    txtServiceParam1.Text = "1,5";
                    txtServiceParam2.Text = "0,3";
                    break;
                case "Poisson":
                    txtServiceParam1Label.Text = "Лямбда:";
                    txtServiceParam2Label.Visibility = Visibility.Collapsed;
                    txtServiceParam2.Visibility = Visibility.Collapsed;
                    txtServiceParam1.Text = "1,5";
                    break;
                case "Erlang":
                    txtServiceParam1Label.Text = "Среднее:";
                    txtServiceParam2Label.Text = "Форма (k):";
                    txtServiceParam2Label.Visibility = Visibility.Visible;
                    txtServiceParam2.Visibility = Visibility.Visible;
                    txtServiceParam1.Text = "1,5";
                    txtServiceParam2.Text = "2";
                    break;
            }
        }
        
        private void Distribution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDistributionLabels();
            InitializeSmoSystem();
        }

        private void InitializeSmoSystem()
        {
            try
            {
                if (txtQueueSize == null) return;

                int queueSize = 5;
                if (!string.IsNullOrEmpty(txtQueueSize.Text))
                {
                    queueSize = NumberParser.ParseInt(txtQueueSize.Text);
                }

                var generatorDistribution = CreateGeneratorDistribution();
                var serviceDistribution = CreateServiceDistribution();

                _generator = new RequestGenerator(generatorDistribution);
                _queue = new Queue(queueSize);
                _serviceDevice = new ServiceDevice(serviceDistribution);
                _smoSystem = new SmoSystem(_generator, _queue, _serviceDevice);

                txtStatus.Text = $"Система инициализирована. Генератор: {generatorDistribution.Name}, Обслуживание: {serviceDistribution.Name}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации системы: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateParameters()
        {
            try
            {
                int queueSize = NumberParser.ParseInt(txtQueueSize.Text);

                var generatorDistribution = CreateGeneratorDistribution();
                var serviceDistribution = CreateServiceDistribution();

                _generator = new RequestGenerator(generatorDistribution);
                _queue = new Queue(queueSize);
                _serviceDevice = new ServiceDevice(serviceDistribution);
                _smoSystem = new SmoSystem(_generator, _queue, _serviceDevice);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в параметрах: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetDefaultValues()
        {
            txtMeanGenerationTime.Text = "2.0";
            txtServiceTime.Text = "1.5";
            txtQueueSize.Text = "5";
            txtSimulationTime.Text = "50.0";
    
            txtGeneratorParam1.Text = "2.0";
            txtGeneratorParam2.Text = "0.5";
            txtServiceParam1.Text = "1.5";
            txtServiceParam2.Text = "0.3";
        }

        
        private async void BtnStepByStep_Click(object sender, RoutedEventArgs e)
        {
            await RunSimulationAsync(async () =>
            {
                double timeStep = 0.1;
                double simulationTime = NumberParser.ParseDouble(txtSimulationTime.Text);
        
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
                double simulationTime = NumberParser.ParseDouble(txtSimulationTime.Text);
        
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
                double simulationTime = NumberParser.ParseDouble(txtSimulationTime.Text);
                double meanGenerationTime = NumberParser.ParseDouble(txtMeanGenerationTime.Text);
                double serviceTime = NumberParser.ParseDouble(txtServiceTime.Text);

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