using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace StateSystemAnalyzer
{
    public partial class Lab2 : Form
    {
        private NumericUpDown nudSize;
        private DataGridView dataGridView;
        private Button btnCalculate;
        private TextBox txtResults;
        private Label lblSum;
        private RadioButton radioManual;
        private RadioButton radioPredefined;
        
        public Lab2()
        {
            InitializeComponent();
            InitializePredefinedMatrix(); // По умолчанию используем предопределенную матрицу
        }

        private void InitializeComponent()
        {
            this.Text = "Анализ времени пребывания в состояниях системы";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Радиокнопки для выбора режима
            radioPredefined = new RadioButton
            {
                Text = "Предопределенная матрица 4x4",
                Location = new Point(20, 20),
                Size = new Size(200, 20),
                Checked = true
            };
            radioPredefined.CheckedChanged += (s, e) => 
            { 
                if (radioPredefined.Checked) InitializePredefinedMatrix(); 
            };
            this.Controls.Add(radioPredefined);

            radioManual = new RadioButton
            {
                Text = "Ручной ввод матрицы",
                Location = new Point(220, 20),
                Size = new Size(150, 20)
            };
            radioManual.CheckedChanged += (s, e) => 
            { 
                if (radioManual.Checked) InitializeMatrix(3); 
            };
            this.Controls.Add(radioManual);

            // Поле для выбора размера матрицы (только для ручного режима)
            var lblSize = new Label
            {
                Text = "Размер матрицы (2-5):",
                Location = new Point(20, 45),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblSize);

            nudSize = new NumericUpDown
            {
                Minimum = 2,
                Maximum = 5,
                Value = 3,
                Location = new Point(150, 45),
                Size = new Size(50, 20),
                Enabled = false
            };
            nudSize.ValueChanged += (s, e) => InitializeMatrix((int)nudSize.Value);
            this.Controls.Add(nudSize);

            // DataGridView для матрицы
            dataGridView = new DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(500, 300),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = true,
                ColumnHeadersVisible = true
            };
            this.Controls.Add(dataGridView);

            // Кнопка расчета
            btnCalculate = new Button
            {
                Text = "Рассчитать",
                Location = new Point(20, 390),
                Size = new Size(100, 30)
            };
            btnCalculate.Click += CalculateResults;
            this.Controls.Add(btnCalculate);

            // Поле для результатов
            txtResults = new TextBox
            {
                Location = new Point(550, 80),
                Size = new Size(300, 400),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10),
                ReadOnly = true
            };
            this.Controls.Add(txtResults);

            // Метка для суммы вероятностей
            lblSum = new Label
            {
                Location = new Point(550, 490),
                Size = new Size(300, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblSum);
        }

        private void InitializePredefinedMatrix()
        {
            // Предопределенная матрица 4x4
            double[,] predefinedMatrix = {
                { 0, 2, 1, 0.5 },
                { 1, 0, 1.5, 0.8 },
                { 0.5, 1, 0, 2 },
                { 1.2, 0.7, 0.9, 0 }
            };

            dataGridView.Columns.Clear();
            dataGridView.Rows.Clear();
            nudSize.Enabled = false;

            // Настройка столбцов
            for (int i = 0; i < 4; i++)
            {
                dataGridView.Columns.Add($"S{i + 1}", $"S{i + 1}");
                dataGridView.Columns[i].Width = 50;
            }

            // Настройка строк
            dataGridView.RowCount = 4;

            // Заполнение заголовков строк
            for (int i = 0; i < 4; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = $"S{i + 1}";
            }

            // Заполнение матрицы предопределенными значениями
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    dataGridView.Rows[i].Cells[j].Value = predefinedMatrix[i, j].ToString("F1");
                    dataGridView.Rows[i].Cells[j].ReadOnly = true; // Запрещаем редактирование
                    dataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                }
            }

            // Убираем обработчик редактирования
            dataGridView.CellEndEdit -= DataGridView_CellEndEdit;
        }

                private void InitializeMatrix(int size)
        {
            dataGridView.Columns.Clear();
            dataGridView.Rows.Clear();
            nudSize.Enabled = true;

            // Настройка столбцов
            for (int i = 0; i < size; i++)
            {
                dataGridView.Columns.Add($"S{i + 1}", $"S{i + 1}");
                dataGridView.Columns[i].Width = 50;
            }

            // Настройка строк
            dataGridView.RowCount = size;

            // Заполнение заголовков строк
            for (int i = 0; i < size; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = $"S{i + 1}";
            }

            // Заполнение матрицы нулями
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    dataGridView.Rows[i].Cells[j].Value = "0";
                    dataGridView.Rows[i].Cells[j].ReadOnly = false; // Разрешаем редактирование
                    dataGridView.Rows[i].Cells[j].Style.BackColor = Color.White;
                }
            }

            // Добавляем обработчик изменения ячеек
            dataGridView.CellEndEdit += DataGridView_CellEndEdit;
        }

        private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == e.ColumnIndex)
            {
                // Запрещаем изменение главной диагонали
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
                MessageBox.Show("Нельзя изменять элементы главной диагонали!", "Предупреждение", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Проверяем, что введено число
                var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (!double.TryParse(cell.Value?.ToString(), out double value) || value < 0)
                {
                    cell.Value = "0";
                    MessageBox.Show("Введите неотрицательное число!", "Ошибка", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CalculateResults(object sender, EventArgs e)
        {
            try
            {
                int size = (int)nudSize.Value;
                double[,] intensityMatrix = new double[size, size];

                // Чтение данных из DataGridView
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (double.TryParse(dataGridView.Rows[i].Cells[j].Value?.ToString(), out double value))
                        {
                            intensityMatrix[i, j] = value;
                        }
                        else
                        {
                            intensityMatrix[i, j] = 0;
                        }
                    }
                }

                // Находим установившиеся вероятности
                double[] steadyStateProbabilities = FindSteadyStateProbabilities(intensityMatrix);

                // Вывод результатов
                txtResults.Text = "Результаты:\r\n";
                txtResults.Text += "Состояние\tВремя\t\tВероятность\r\n";
                txtResults.Text += "----------------------------------------\r\n";

                double sumProbabilities = 0;

                for (int i = 0; i < size; i++)
                {
                    // Время пребывания = 1 / сумма интенсивностей выхода из состояния
                    double exitRate = 0;
                    for (int j = 0; j < size; j++)
                    {
                        exitRate += intensityMatrix[i, j];
                    }

                    double timeInState = (exitRate > 0) ? 1.0 / exitRate : 0;

                    txtResults.Text += $"S{i + 1}\t\t{timeInState:F4}\t\t{steadyStateProbabilities[i]:F4}\r\n";
                    sumProbabilities += steadyStateProbabilities[i];
                }

                txtResults.Text += "----------------------------------------\r\n";
                lblSum.Text = $"Сумма вероятностей: {sumProbabilities:F6}";
                lblSum.ForeColor = Math.Abs(sumProbabilities - 1.0) < 0.0001 ? Color.Green : Color.Red;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчетах: {ex.Message}", "Ошибка", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для нахождения установившихся вероятностей
        static double[] FindSteadyStateProbabilities(double[,] intensityMatrix)
        {
            int n = intensityMatrix.GetLength(0);

            // Строим матрицу коэффициентов для системы уравнений
            double[,] coefficients = new double[n, n + 1];

            // Уравнения баланса: для каждого состояния сумма входящих = сумма исходящих
            for (int i = 0; i < n; i++)
            {
                // Диагональные элементы = -сумма исходящих интенсивностей
                double sumOut = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        sumOut += intensityMatrix[i, j];
                        coefficients[i, j] = intensityMatrix[j, i]; // входящие из j в i
                    }
                }
                coefficients[i, i] = -sumOut;
            }

            // Заменяем последнее уравнение на условие нормировки: сумма вероятностей = 1
            for (int i = 0; i < n; i++)
            {
                coefficients[n - 1, i] = 1;
            }
            coefficients[n - 1, n] = 1;

            // Решаем систему уравнений методом Гаусса
            return SolveLinearSystem(coefficients);
        }

        // Решение системы линейных уравнений методом Гаусса
        static double[] SolveLinearSystem(double[,] augmentedMatrix)
        {
            int n = augmentedMatrix.GetLength(0);

            // Прямой ход метода Гаусса
            for (int i = 0; i < n; i++)
            {
                // Поиск главного элемента
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(augmentedMatrix[k, i]) > Math.Abs(augmentedMatrix[maxRow, i]))
                    {
                        maxRow = k;
                    }
                }

                // Перестановка строк
                if (maxRow != i)
                {
                    for (int k = 0; k <= n; k++)
                    {
                        double temp = augmentedMatrix[i, k];
                        augmentedMatrix[i, k] = augmentedMatrix[maxRow, k];
                        augmentedMatrix[maxRow, k] = temp;
                    }
                }

                // Обнуление элементов под главной диагональю
                for (int k = i + 1; k < n; k++)
                {
                    double factor = augmentedMatrix[k, i] / augmentedMatrix[i, i];
                    for (int j = i; j <= n; j++)
                    {
                        augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                    }
                }
            }

            // Обратный ход
            double[] solution = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                solution[i] = augmentedMatrix[i, n] / augmentedMatrix[i, i];
                for (int k = i - 1; k >= 0; k--)
                {
                    augmentedMatrix[k, n] -= augmentedMatrix[k, i] * solution[i];
                }
            }

            return solution;
        }
    }
}