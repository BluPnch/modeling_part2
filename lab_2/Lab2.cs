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
        private Button btnShowGraph;
        private PictureBox pictureBoxGraph; 
        
        public Lab2()
        {
            InitializeComponent();
            InitializePredefinedMatrix(); // По умолчанию используем предопределенную матрицу
        }

        private void InitializeComponent()
        {
            this.Text = "Анализ времени пребывания в состояниях системы";
            // this.Size = new Size(900, 870);
            this.Size = new Size(900, 840);
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
                if (radioManual.Checked) InitializeMatrix(4); 
            };
            this.Controls.Add(radioManual);

            // Поле для выбора размера матрицы (только для ручного режима)
            var lblSize = new Label
            {
                Text = "Размер матрицы (2-10):",
                Location = new Point(20, 45),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblSize);

            nudSize = new NumericUpDown
            {
                Minimum = 2,
                Maximum = 10,
                Value = 4,
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
                Size = new Size(400, 300),
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
            
            // Кнопка для показа графа
            btnShowGraph = new Button
            {
                Text = "Показать граф",
                Location = new Point(130, 390),
                Size = new Size(100, 30)
            };
            btnShowGraph.Click += ShowGraph;
            this.Controls.Add(btnShowGraph);

            // PictureBox для отображения графа
            pictureBoxGraph = new PictureBox
            {
                Location = new Point(20, 420),
                Size = new Size(850, 350),
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBoxGraph);

            // Поле для результатов
            txtResults = new TextBox
            {
                Location = new Point(450, 80),
                Size = new Size(400, 300),
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

                // Чтение данных из DataGridView с проверкой
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (i < dataGridView.Rows.Count && j < dataGridView.Columns.Count)
                        {
                            var cellValue = dataGridView.Rows[i].Cells[j].Value?.ToString();
                            if (double.TryParse(cellValue, out double value))
                            {
                                intensityMatrix[i, j] = value;
                            }
                            else
                            {
                                intensityMatrix[i, j] = 0;
                            }
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
                txtResults.Text = "Предельные вероятности состояний:\r\n";
                txtResults.Text += "(установившийся режим)\r\n";
                txtResults.Text += "----------------------------------------\r\n";
                txtResults.Text += "Состояние\tВероятность P\u1d62\tВремя T\u1d62\r\n";
                txtResults.Text += "----------------------------------------\r\n";

                double sumProbabilities = 0;

                for (int i = 0; i < size; i++)
                {
                    // Расчет времени
                    double sumOutgoing = 0;
                    for (int j = 0; j < size; j++)
                    {
                        if (i != j)
                        {
                            sumOutgoing += intensityMatrix[i, j];
                        }
                    }
                    
                    double timeInState = (sumOutgoing > 0) ? steadyStateProbabilities[i] / sumOutgoing : 0;

                    txtResults.Text += $"S{i + 1}\t\t{steadyStateProbabilities[i]:F6}\t{timeInState:F4}\r\n";
                    sumProbabilities += steadyStateProbabilities[i];
                }

                txtResults.Text += "----------------------------------------\r\n";
                lblSum.Text = $"Сумма вероятностей: {sumProbabilities:F8}";
                lblSum.ForeColor = Math.Abs(sumProbabilities - 1.0) < 0.0001 ? Color.Green : Color.Red;

                txtResults.Text += $"\r\nСреднее относительное время пребывания\r\n";
                txtResults.Text += $"в каждом состоянии рассчитывается по формуле:\r\n"; 
                txtResults.Text += $"T\u1d62 = P\u1d62 / Σλ\u1d62\u1d57\r\n";

            }
            catch (Exception ex)
            {
                txtResults.Text = $"Ошибка: {ex.Message}\r\n\r\n";
                txtResults.Text += "Stack Trace:\r\n";
                txtResults.Text += $"{ex.StackTrace}\r\n";
                lblSum.Text = "Ошибка расчета";
                lblSum.ForeColor = Color.Red;
            }
        }
                
        private void ShowGraph(object sender, EventArgs e)
        {
            try
            {
                int size = (int)nudSize.Value;
                double[,] intensityMatrix = new double[size, size];

                // Чтение данных из DataGridView с проверкой границ
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        // Проверяем, что индексы в пределах
                        if (i < dataGridView.Rows.Count && j < dataGridView.Columns.Count)
                        {
                            var cell = dataGridView.Rows[i].Cells[j];
                            if (cell.Value != null && double.TryParse(cell.Value.ToString(), out double value))
                            {
                                intensityMatrix[i, j] = value;
                            }
                            else
                            {
                                intensityMatrix[i, j] = 0;
                            }
                        }
                        else
                        {
                            intensityMatrix[i, j] = 0;
                        }
                    }
                }

                // Создаем изображение графа
                Bitmap graphImage = DrawStateGraph(intensityMatrix);
                pictureBoxGraph.Image = graphImage;
                pictureBoxGraph.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении графа: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Bitmap DrawStateGraph(double[,] intensityMatrix)
        {
            int size = intensityMatrix.GetLength(0);
            
            // Автоматически подбираем размер изображения в зависимости от количества состояний
            int baseSize = Math.Max(400, size * 80);
            int width = baseSize;
            int height = baseSize;
            
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Рассчитываем позиции состояний по кругу
                Point[] positions = new Point[size];
                int centerX = width / 2;
                int centerY = height / 2;
                int radius = Math.Min(width, height) / 3;

                for (int i = 0; i < size; i++)
                {
                    double angle = 2 * Math.PI * i / size;
                    positions[i] = new Point(
                        centerX + (int)(radius * Math.Cos(angle)),
                        centerY + (int)(radius * Math.Sin(angle))
                    );
                }

                // Рисуем стрелки переходов
                Pen arrowPen = new Pen(Color.Blue, 2);
                Font labelFont = new Font("Arial", 8);
                Brush labelBrush = Brushes.Red;

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (intensityMatrix[i, j] > 0 && i != j)
                        {
                            DrawArrow(g, arrowPen, positions[i], positions[j], 
                                     intensityMatrix[i, j].ToString("F1"), labelFont, labelBrush);
                        }
                    }
                }

                // Рисуем состояния (узлы графа)
                int nodeSize = size > 8 ? 30 : 40; // Уменьшаем размер узлов для больших графов
                Font stateFont = new Font("Arial", size > 8 ? 8 : 10, FontStyle.Bold);
                
                for (int i = 0; i < size; i++)
                {
                    g.FillEllipse(Brushes.LightBlue, positions[i].X - nodeSize/2, positions[i].Y - nodeSize/2, nodeSize, nodeSize);
                    g.DrawEllipse(Pens.Black, positions[i].X - nodeSize/2, positions[i].Y - nodeSize/2, nodeSize, nodeSize);
                    
                    // Номер состояния
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    
                    g.DrawString($"S{i + 1}", stateFont, Brushes.Black, 
                                new RectangleF(positions[i].X - nodeSize/2, positions[i].Y - nodeSize/2, nodeSize, nodeSize), 
                                format);
                }

                // Заголовок
                g.DrawString($"Граф состояний системы ({size} состояний)", 
                           new Font("Arial", 12, FontStyle.Bold), 
                           Brushes.DarkBlue, new PointF(10, 10));
            }

            return bitmap;
        }
        
        private void DrawArrow(Graphics g, Pen pen, Point from, Point to, string label, Font font, Brush brush)
        {
            // Вектор направления
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            
            // Нормализуем вектор
            dx /= length;
            dy /= length;
            
            // Укорачиваем линию чтобы стрелка не залезала на круг
            int circleRadius = 20;
            Point start = new Point(from.X + (int)(dx * circleRadius), from.Y + (int)(dy * circleRadius));
            Point end = new Point(to.X - (int)(dx * circleRadius), to.Y - (int)(dy * circleRadius));
            
            // Рисуем линию
            g.DrawLine(pen, start, end);
            
            // Рисуем стрелку
            double arrowSize = 10;
            double angle = Math.Atan2(dy, dx);
            
            Point[] arrowPoints = new Point[3];
            arrowPoints[0] = end;
            arrowPoints[1] = new Point(
                end.X - (int)(arrowSize * Math.Cos(angle - Math.PI / 6)),
                end.Y - (int)(arrowSize * Math.Sin(angle - Math.PI / 6))
            );
            arrowPoints[2] = new Point(
                end.X - (int)(arrowSize * Math.Cos(angle + Math.PI / 6)),
                end.Y - (int)(arrowSize * Math.Sin(angle + Math.PI / 6))
            );
            
            g.FillPolygon(Brushes.Blue, arrowPoints);
            
            // Подпись интенсивности (посередине стрелки)
            Point labelPos = new Point(
                (start.X + end.X) / 2,
                (start.Y + end.Y) / 2
            );
            
            // Смещаем подпись чтобы не накладывалась на стрелку
            labelPos.X += (int)(dy * 15);
            labelPos.Y -= (int)(dx * 15);
            
            g.DrawString(label, font, brush, labelPos);
        }

        // Метод для нахождения установившихся вероятностей
        static double[] FindSteadyStateProbabilities(double[,] intensityMatrix)
        {
            int n = intensityMatrix.GetLength(0);

            // Строим расширенную матрицу для системы уравнений Колмогорова
            // Уравнения: для каждого i: Σ P_j × λ_ji = P_i × Σ λ_ij
            // Плюс условие нормировки: Σ P_i = 1
            
            double[,] coefficients = new double[n + 1, n + 1]; // +1 для условия нормировки
            double[] rightSide = new double[n + 1];

            // Уравнения баланса (первые n уравнений)
            for (int i = 0; i < n; i++)
            {
                double sumOut = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        sumOut += intensityMatrix[i, j];
                        coefficients[i, j] = intensityMatrix[j, i]; // входящие из j в i
                    }
                }
                coefficients[i, i] = -sumOut; // исходящие из i
                rightSide[i] = 0;
            }

            // Условие нормировки (последнее уравнение)
            for (int j = 0; j < n; j++)
            {
                coefficients[n, j] = 1;
            }
            rightSide[n] = 1;

            // Решаем систему методом наименьших квадратов для устойчивости
            return SolveLeastSquares(coefficients, rightSide);
        }

        // Решение системы методом наименьших квадратов
        static double[] SolveLeastSquares(double[,] A, double[] b)
        {
            int m = A.GetLength(0); // число уравнений
            int n = A.GetLength(1); // число переменных

            // A^T × A × x = A^T × b
            double[,] AT = new double[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    AT[i, j] = A[j, i];

            // Вычисляем A^T × A
            double[,] ATA = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < m; k++)
                        ATA[i, j] += AT[i, k] * A[k, j];

            // Вычисляем A^T × b
            double[] ATb = new double[n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    ATb[i] += AT[i, j] * b[j];

            // Решаем систему ATA × x = ATb методом Гаусса
            return SolveLinearSystemWithAugmented(ATA, ATb);
        }

        // Решение системы линейных уравнений с расширенной матрицей
        static double[] SolveLinearSystemWithAugmented(double[,] A, double[] b)
        {
            int n = A.GetLength(0);
            double[,] augmented = new double[n, n + 1];
            
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    augmented[i, j] = A[i, j];
                augmented[i, n] = b[i];
            }

            return SolveLinearSystem(augmented);
        }

        // Решение системы линейных уравнений методом Гаусса (оставить без изменений)
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

                // Проверка на вырожденность
                if (Math.Abs(augmentedMatrix[i, i]) < 1e-10)
                {
                    // Если диагональный элемент почти нулевой, устанавливаем небольшое значение
                    augmentedMatrix[i, i] = 1e-10;
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

            // Нормализуем решение (сумма вероятностей = 1)
            double sum = solution.Sum();
            if (Math.Abs(sum) > 1e-10)
            {
                for (int i = 0; i < n; i++)
                    solution[i] /= sum;
            }

            return solution;
        }
    }
}