using System;
using System.Windows;
using System.Windows.Controls;
using lab_4.Distributions;
using lab_4.Models;


namespace lab_4
{
    public partial class MainWindow : Window
    {
        private SmoSystem? _smoSystem;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                UpdateDistributionLabels();
                txtStatus.Text = "Введите параметры и запустите моделирование";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки окна: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeSmoSystem()
        {
            try
            {
                int queueSize = GetIntValue(txtQueueSize.Text, 5);
                var generatorDistribution = CreateGeneratorDistribution();
                var serviceDistribution = CreateServiceDistribution();

                var generator = new RequestGenerator(generatorDistribution);
                var queue = new Queue(queueSize);
                var serviceDevice = new ServiceDevice(serviceDistribution);
                
                _smoSystem = new SmoSystem(generator, queue, serviceDevice);

                txtStatus.Text = "Система инициализирована";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации СМО: {ex.Message}", "Ошибка", 
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
                
                double param1 = GetDoubleValue(txtGeneratorParam1.Text, 2.0);

                return distributionType switch
                {
                    "Uniform" => new UniformDistribution(0, param1),
                    "Exponential" => new ExponentialDistribution(1.0 / param1),
                    "Normal" => new NormalDistribution(param1, 0.5),
                    "Poisson" => new PoissonDistribution(param1),
                    "Erlang" => new ErlangDistribution(3, 1.0 / param1),
                    _ => new ExponentialDistribution(1.0 / param1)
                };
            }
            catch (Exception)
            {
                return new ExponentialDistribution(0.5);
            }
        }

        private IDistribution CreateServiceDistribution()
        {
            try
            {
                if (cmbServiceDistribution?.SelectedItem == null)
                    return new ExponentialDistribution(0.666);

                var selectedItem = cmbServiceDistribution.SelectedItem as ComboBoxItem;
                string distributionType = selectedItem?.Tag as string ?? "Exponential";
                
                double param1 = GetDoubleValue(txtServiceParam1.Text, 1.5);

                return distributionType switch
                {
                    "Uniform" => new UniformDistribution(0, param1),
                    "Exponential" => new ExponentialDistribution(1.0 / param1),
                    "Normal" => new NormalDistribution(param1, 0.3),
                    "Poisson" => new PoissonDistribution(param1),
                    "Erlang" => new ErlangDistribution(2, 1.0 / param1),
                    _ => new ExponentialDistribution(1.0 / param1)
                };
            }
            catch (Exception)
            {
                return new ExponentialDistribution(0.666);
            }
        }

        private double GetDoubleValue(string text, double defaultValue)
        {
            if (string.IsNullOrWhiteSpace(text))
                return defaultValue;
                
            if (double.TryParse(text, out double result))
                return result;
            return defaultValue;
        }

        private int GetIntValue(string text, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(text))
                return defaultValue;
                
            if (int.TryParse(text, out int result))
                return result;
            return defaultValue;
        }

        private void UpdateDistributionLabels()
        {
            // Простая реализация без сложной логики
            if (txtGeneratorParam1Label != null)
                txtGeneratorParam1Label.Text = "Среднее время между заявками:";
            
            if (txtServiceParam1Label != null)
                txtServiceParam1Label.Text = "Среднее время обслуживания:";
        }
        
        private void Distribution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDistributionLabels();
        }

        private void BtnStepByStep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeSmoSystem();
                if (_smoSystem == null) return;

                double simulationTime = GetDoubleValue(txtSimulationTime.Text, 50.0);
                double timeStep = GetDoubleValue(txtTimeStep.Text, 0.1);

                progressBar.Visibility = Visibility.Visible;
                txtStatus.Text = "Пошаговое моделирование...";

                _smoSystem.StepByStep(timeStep, simulationTime);
                
                DisplayResults();
                txtStatus.Text = "Пошаговое моделирование завершено";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка при моделировании";
            }
            finally
            {
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnEventBased_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeSmoSystem();
                if (_smoSystem == null) return;

                double simulationTime = GetDoubleValue(txtSimulationTime.Text, 50.0);

                progressBar.Visibility = Visibility.Visible;
                txtStatus.Text = "Событийное моделирование...";

                _smoSystem.EventBased(simulationTime);
                
                DisplayResults();
                txtStatus.Text = "Событийное моделирование завершено";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка при моделировании";
            }
            finally
            {
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnFindOptimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double simulationTime = GetDoubleValue(txtSimulationTime.Text, 50.0);
                double genTime = GetDoubleValue(txtGeneratorParam1.Text, 2.0);
                double servTime = GetDoubleValue(txtServiceParam1.Text, 1.5);

                progressBar.Visibility = Visibility.Visible;
                txtStatus.Text = "Поиск оптимальной очереди...";

                // Создаем временную систему для поиска
                var tempSystem = new SmoSystem(
                    new RequestGenerator(new ExponentialDistribution(1.0 / genTime)),
                    new Queue(1),
                    new ServiceDevice(new ExponentialDistribution(1.0 / servTime))
                );

                tempSystem.FindOptimalQueueSize(simulationTime, genTime, servTime);
                
                DisplayOptimalResults(tempSystem);
                txtStatus.Text = "Поиск оптимальной очереди завершен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка при поиске";
            }
            finally
            {
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
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
        }

        private void DisplayOptimalResults(SmoSystem tempSystem)
        {
            if (tempSystem == null) return;

            txtLog.Clear();
            foreach (var logEntry in tempSystem.Log)
            {
                if (logEntry.Contains("Размер очереди") || logEntry.Contains("Оптимальный"))
                {
                    txtLog.AppendText(logEntry + "\n");
                }
            }
        }
    }
}