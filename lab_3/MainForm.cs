using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace DistributionAnalysis
{
    public partial class MainForm : Form
    {
        private ComboBox cmbDistribution;
        private Panel pnlParameters;
        private Button btnGenerate;
        private PictureBox picFunctions;
        private Label lblStatus;
        private Label lblStatistics;
        
        private GraphPlotter plotter;
        private IDistribution currentDistribution;

        public MainForm()
        {
            InitializeComponent();
            plotter = new GraphPlotter();
            SetupDistributionParameters();
        }

        private void InitializeComponent()
        {
            this.Text = "Анализ распределений";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ComboBox для выбора распределения
            var lblDistribution = new Label
            {
                Text = "Выберите распределение:",
                Location = new Point(20, 20),
                Size = new Size(180, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            cmbDistribution = new ComboBox
            {
                Location = new Point(20, 45),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };

            cmbDistribution.Items.AddRange(new object[]
            {
                "Равномерное распределение",
                "Распределение Пуассона", 
                "Экспоненциальное распределение",
                "Нормальное распределение",
                "Распределение Эрланга"
            });
            cmbDistribution.SelectedIndex = 0;
            cmbDistribution.SelectedIndexChanged += CmbDistribution_SelectedIndexChanged;

            // Панель параметров
            pnlParameters = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(300, 200),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопка генерации
            btnGenerate = new Button
            {
                Text = "Построить графики",
                Location = new Point(20, 310),
                Size = new Size(300, 40),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightBlue
            };
            btnGenerate.Click += BtnGenerate_Click;

            // Метка статуса
            lblStatus = new Label
            {
                Location = new Point(20, 360),
                Size = new Size(300, 40),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkGreen
            };

            // Метка статистики
            var lblStatisticsTitle = new Label
            {
                Text = "Статистика",
                Location = new Point(20, 420),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            lblStatistics = new Label
            {
                Location = new Point(20, 450),
                Size = new Size(350, 200),
                Font = new Font("Arial", 10),
                ForeColor = Color.Black,
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };

            // PictureBox для функций
            var lblFunctions = new Label
            {
                Text = "Функции плотности и распределения",
                Location = new Point(400, 20),
                Size = new Size(300, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            picFunctions = new PictureBox
            {
                Location = new Point(400, 45),
                Size = new Size(950, 800),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Добавляем контролы на форму
            this.Controls.AddRange(new Control[]
            {
                lblDistribution,
                cmbDistribution,
                pnlParameters,
                btnGenerate,
                lblStatus,
                lblStatisticsTitle,
                lblStatistics,
                lblFunctions,
                picFunctions
            });
        }

        private void CmbDistribution_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupDistributionParameters();
        }

        private void SetupDistributionParameters()
        {
            pnlParameters.Controls.Clear();
            
            switch (cmbDistribution.SelectedIndex)
            {
                case 0: // Равномерное
                    CreateParameterControl("Параметр a (мин):", "0", 0);
                    CreateParameterControl("Параметр b (макс):", "10", 1);
                    break;
                case 1: // Пуассон
                    CreateParameterControl("Лямбда (λ):", "3", 0);
                    break;
                case 2: // Экспоненциальное
                    CreateParameterControl("Лямбда (λ):", "0.5", 0);
                    break;
                case 3: // Нормальное
                    CreateParameterControl("Среднее (μ):", "5", 0);
                    CreateParameterControl("Стд. отклонение (σ):", "2", 1);
                    break;
                case 4: // Эрланга
                    CreateParameterControl("Форма (k):", "3", 0);
                    CreateParameterControl("Интенсивность (λ):", "0.5", 1);
                    break;
            }
        }

        private void CreateParameterControl(string labelText, string defaultValue, int row)
        {
            int yPos = 10 + row * 40;

            var label = new Label
            {
                Text = labelText,
                Location = new Point(10, yPos),
                Size = new Size(130, 20),
                Font = new Font("Arial", 9)
            };

            var textBox = new TextBox
            {
                Text = defaultValue,
                Location = new Point(150, yPos),
                Size = new Size(80, 20),
                Font = new Font("Arial", 9)
            };

            pnlParameters.Controls.Add(label);
            pnlParameters.Controls.Add(textBox);
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Генерация графиков...";
                Application.DoEvents();

                currentDistribution = CreateSelectedDistribution();
                
                if (currentDistribution == null)
                {
                    lblStatus.Text = "Ошибка: неверные параметры";
                    return;
                }

                // Создаем временный файл для графика функций
                string tempDir = Path.GetTempPath();
                string functionsFile = Path.Combine(tempDir, "functions_temp.png");

                // Генерируем графики
                double minX, maxX;
                GetPlotRange(currentDistribution, out minX, out maxX);

                plotter.PlotDistribution(currentDistribution, minX, maxX, functionsFile);

                // Загружаем изображение в PictureBox
                LoadImageToPictureBox(functionsFile, picFunctions);

                // Обновляем статистику в Label
                UpdateStatisticsLabel();

                lblStatus.Text = "Графики построены успешно!";

                // Очищаем временный файл с задержкой
                Timer cleanupTimer = new Timer();
                cleanupTimer.Interval = 5000;
                cleanupTimer.Tick += (s, args) =>
                {
                    cleanupTimer.Stop();
                    TryDeleteFile(functionsFile);
                    cleanupTimer.Dispose();
                };
                cleanupTimer.Start();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Ошибка при построении графиков: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatisticsLabel()
        {
            if (currentDistribution != null)
            {
                double mean = currentDistribution.Mean();
                double variance = currentDistribution.Variance();
                double stdDev = Math.Sqrt(variance);
                double cv = stdDev / mean;

                lblStatistics.Text = $"Математическое ожидание: {mean:F6}\n" +
                                   $"Дисперсия: {variance:F6}\n" +
                                   $"Стандартное отклонение: {stdDev:F6}\n" +
                                   $"Коэффициент вариации: {cv:F6}\n" +
                                   $"Диапазон графика: [{GetPlotRangeMin(currentDistribution):F3}, {GetPlotRangeMax(currentDistribution):F3}]";
            }
        }

        private double GetPlotRangeMin(IDistribution dist)
        {
            double mean = dist.Mean();
            double stdDev = Math.Sqrt(dist.Variance());
            
            if (dist is UniformDistribution) return mean - 2 * stdDev;
            if (dist is PoissonDistribution) return Math.Max(0, mean - 3 * stdDev);
            if (dist is ExponentialDistribution || dist is ErlangDistribution) return 0;
            return mean - 3 * stdDev;
        }

        private double GetPlotRangeMax(IDistribution dist)
        {
            double mean = dist.Mean();
            double stdDev = Math.Sqrt(dist.Variance());
            
            if (dist is UniformDistribution) return mean + 2 * stdDev;
            if (dist is PoissonDistribution) return mean + 3 * stdDev;
            if (dist is ExponentialDistribution) return mean + 4 * stdDev;
            return mean + 3 * stdDev;
        }

        private void LoadImageToPictureBox(string filePath, PictureBox pictureBox)
        {
            if (File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    pictureBox.Image = Image.FromStream(stream);
                }
            }
        }

        private void TryDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch { }
        }

        private IDistribution CreateSelectedDistribution()
        {
            try
            {
                var parameters = pnlParameters.Controls;
                
                switch (cmbDistribution.SelectedIndex)
                {
                    case 0: // Равномерное
                        if (parameters.Count >= 2)
                        {
                            double a = double.Parse(((TextBox)parameters[1]).Text);
                            double b = double.Parse(((TextBox)parameters[3]).Text);
                            return new UniformDistribution(a, b);
                        }
                        break;
                        
                    case 1: // Пуассон
                        if (parameters.Count >= 1)
                        {
                            double lambda = double.Parse(((TextBox)parameters[1]).Text);
                            return new PoissonDistribution(lambda);
                        }
                        break;
                        
                    case 2: // Экспоненциальное
                        if (parameters.Count >= 1)
                        {
                            double lambda = double.Parse(((TextBox)parameters[1]).Text);
                            return new ExponentialDistribution(lambda);
                        }
                        break;
                        
                    case 3: // Нормальное
                        if (parameters.Count >= 2)
                        {
                            double mu = double.Parse(((TextBox)parameters[1]).Text);
                            double sigma = double.Parse(((TextBox)parameters[3]).Text);
                            return new NormalDistribution(mu, sigma);
                        }
                        break;
                        
                    case 4: // Эрланга
                        if (parameters.Count >= 2)
                        {
                            int k = int.Parse(((TextBox)parameters[1]).Text);
                            double lambda = double.Parse(((TextBox)parameters[3]).Text);
                            return new ErlangDistribution(k, lambda);
                        }
                        break;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые параметры", "Неверный ввод", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания распределения: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return null;
        }

        private void GetPlotRange(IDistribution dist, out double minX, out double maxX)
        {
            double mean = dist.Mean();
            double stdDev = Math.Sqrt(dist.Variance());
            
            if (dist is UniformDistribution)
            {
                minX = mean - 2 * stdDev;
                maxX = mean + 2 * stdDev;
            }
            else if (dist is PoissonDistribution)
            {
                minX = Math.Max(0, mean - 3 * stdDev);
                maxX = mean + 3 * stdDev;
            }
            else if (dist is ExponentialDistribution)
            {
                minX = 0;
                maxX = mean + 4 * stdDev;
            }
            else if (dist is NormalDistribution)
            {
                minX = mean - 3 * stdDev;
                maxX = mean + 3 * stdDev;
            }
            else if (dist is ErlangDistribution)
            {
                minX = 0;
                maxX = mean + 3 * stdDev;
            }
            else
            {
                minX = mean - 3 * stdDev;
                maxX = mean + 3 * stdDev;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Очищаем ресурсы
            picFunctions.Image?.Dispose();
            base.OnFormClosed(e);
        }
    }
}