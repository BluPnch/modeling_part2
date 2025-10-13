using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RandomNumberLab
{
    public partial class Lab1 : Form
    {
        private Random random;
        private List<TextBox> userInputBoxes;
        private const int NumbersCount = 10;
        private TableLayoutPanel tablePanel;
        private Label randomResultLabel;
        private Label userResultLabel;
        
        public Lab1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Main form
            this.Text = "Генерация псевдослучайных чисел - Лабораторная работа №1";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Initialize random and collections
            random = new Random();
            userInputBoxes = new List<TextBox>();
            
            CreateTable();
            CreateButtons();
            
            this.ResumeLayout();
            GenerateNewNumbers();
        }

        private void CreateTable()
        {
            tablePanel = new TableLayoutPanel();
            tablePanel.Dock = DockStyle.Fill;
            tablePanel.ColumnCount = 8; // 8 колонок как в таблице
            tablePanel.RowCount = NumbersCount + 2;
            tablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tablePanel.AutoScroll = true;
            
            // Устанавливаем ширину колонок
            tablePanel.ColumnStyles.Clear();
            for (int i = 0; i < 8; i++)
            {
                tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }
            
            // Заголовки таблицы
            string[] headers = { "№", "Табличный способ", "", "", "Алгоритмический способ", "", "", "Рудаков" };
            for (int i = 0; i < headers.Length; i++)
            {
                tablePanel.Controls.Add(new Label { 
                    Text = headers[i], 
                    Font = new Font("Arial", 10, FontStyle.Bold), 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    AutoSize = true
                }, i, 0);
            }
            
            // Подзаголовки для табличного и алгоритмического способов
            string[] subHeaders = { "", "1", "2", "3", "1", "2", "3", "" };
            for (int i = 0; i < subHeaders.Length; i++)
            {
                tablePanel.Controls.Add(new Label { 
                    Text = subHeaders[i], 
                    Font = new Font("Arial", 9, FontStyle.Bold), 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    AutoSize = true
                }, i, 1);
            }
            
            // Заполняем таблицу данными
            for (int i = 0; i < NumbersCount; i++)
            {
                // Номер строки
                tablePanel.Controls.Add(new Label { 
                    Text = (i + 1).ToString(), 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    AutoSize = true
                }, 0, i + 2);
                
                // Табличный способ (3 колонки)
                for (int j = 0; j < 3; j++)
                {
                    tablePanel.Controls.Add(new Label { 
                        Text = random.Next(1, 200).ToString(), 
                        TextAlign = ContentAlignment.MiddleCenter, 
                        Dock = DockStyle.Fill,
                        AutoSize = true
                    }, j + 1, i + 2);
                }
                
                // Алгоритмический способ (3 колонки)
                for (int j = 0; j < 3; j++)
                {
                    tablePanel.Controls.Add(new Label { 
                        Text = random.Next(1, 200).ToString(), 
                        TextAlign = ContentAlignment.MiddleCenter, 
                        Dock = DockStyle.Fill,
                        AutoSize = true
                    }, j + 4, i + 2);
                }
                
                // Колонка Рудаков (ввод пользователя)
                TextBox inputBox = new TextBox { 
                    MaxLength = 1,
                    TextAlign = HorizontalAlignment.Center,
                    Dock = DockStyle.Fill,
                    Width = 50
                };
                inputBox.KeyPress += ValidateDigitInput;
                userInputBoxes.Add(inputBox);
                tablePanel.Controls.Add(inputBox, 7, i + 2);
            }
            
            // Строка результатов
            tablePanel.Controls.Add(new Label { 
                Text = "Результат:", 
                Font = new Font("Arial", 10, FontStyle.Bold), 
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Fill,
                AutoSize = true
            }, 0, NumbersCount + 2);
            
            // Результаты для табличного способа
            for (int i = 1; i <= 3; i++)
            {
                List<int> tableNumbers = new List<int>();
                for (int row = 2; row < NumbersCount + 2; row++)
                {
                    Label numberLabel = tablePanel.GetControlFromPosition(i, row) as Label;
                    if (numberLabel != null && int.TryParse(numberLabel.Text, out int num))
                    {
                        tableNumbers.Add(num);
                    }
                }
    
                double tableScore = CalculateCustomRandomness(tableNumbers);
                string resultText = tableScore > 50 ? $"Случайно ({tableScore:F1}%)" : $"Не случайно ({tableScore:F1}%)";
    
                tablePanel.Controls.Add(new Label { 
                    Text = resultText, 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    ForeColor = tableScore > 50 ? Color.Green : Color.Red,
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    AutoSize = true
                }, i, NumbersCount + 2);
            }

// Результаты для алгоритмического способа
            for (int i = 4; i <= 6; i++)
            {
                List<int> algoNumbers = new List<int>();
                for (int row = 2; row < NumbersCount + 2; row++)
                {
                    Label numberLabel = tablePanel.GetControlFromPosition(i, row) as Label;
                    if (numberLabel != null && int.TryParse(numberLabel.Text, out int num))
                    {
                        algoNumbers.Add(num);
                    }
                }
    
                double algoScore = CalculateCustomRandomness(algoNumbers);
                string resultText = algoScore > 50 ? $"Случайно ({algoScore:F1}%)" : $"Не случайно ({algoScore:F1}%)";
    
                tablePanel.Controls.Add(new Label { 
                    Text = resultText, 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    ForeColor = algoScore > 50 ? Color.Green : Color.Red,
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    AutoSize = true
                }, i, NumbersCount + 2);
            }
            
            // Результат для колонки Рудаков
            userResultLabel = new Label { 
                Text = "Не случайно", 
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Fill, 
                ForeColor = Color.Red, 
                Font = new Font("Arial", 9, FontStyle.Bold),
                AutoSize = true
            };
            tablePanel.Controls.Add(userResultLabel, 7, NumbersCount + 2);
            
            this.Controls.Add(tablePanel);
        }

        private void CreateButtons()
        {
            Panel buttonPanel = new Panel { 
                Dock = DockStyle.Bottom, 
                Height = 80,
                BackColor = Color.LightGray
            };
            
            Button generateBtn = new Button { 
                Text = "Сгенерировать новые числа", 
                Size = new Size(180, 35), 
                Location = new Point(10, 20),
                BackColor = Color.White
            };
            generateBtn.Click += (s, e) => GenerateNewNumbers();
            
            Button checkBtn = new Button { 
                Text = "Проверить случайность", 
                Size = new Size(180, 35), 
                Location = new Point(200, 20),
                BackColor = Color.White
            };
            checkBtn.Click += (s, e) => CheckRandomness();
            
            buttonPanel.Controls.Add(generateBtn);
            buttonPanel.Controls.Add(checkBtn);
            
            this.Controls.Add(buttonPanel);
        }

        private void ValidateDigitInput(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void GenerateNewNumbers()
        {
            // Обновляем числа в табличном способе
            for (int i = 0; i < NumbersCount; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    Label numberLabel = tablePanel.GetControlFromPosition(j, i + 2) as Label;
                    if (numberLabel != null)
                    {
                        numberLabel.Text = random.Next(1, 200).ToString();
                    }
                }
            }
            
            // Обновляем числа в алгоритмическом способе
            for (int i = 0; i < NumbersCount; i++)
            {
                for (int j = 4; j <= 6; j++)
                {
                    Label numberLabel = tablePanel.GetControlFromPosition(j, i + 2) as Label;
                    if (numberLabel != null)
                    {
                        numberLabel.Text = random.Next(1, 200).ToString();
                    }
                }
            }
            
            // Очищаем ввод пользователя
            foreach (TextBox inputBox in userInputBoxes)
            {
                inputBox.Text = "";
            }
            
            userResultLabel.Text = "Не случайно";
            userResultLabel.ForeColor = Color.Red;
        }

        private void CheckRandomness()
        {
            // Получаем цифры пользователя
            List<int> userDigits = new List<int>();
            foreach (TextBox textBox in userInputBoxes)
            {
                if (int.TryParse(textBox.Text, out int digit))
                {
                    userDigits.Add(digit);
                }
            }

            // Проверяем случайность цифр пользователя
            double userScore = userDigits.Count >= 3 ? CalculateCustomRandomness(userDigits) : -1;

            if (userDigits.Count >= 3)
            {
                if (userScore > 50)
                {
                    userResultLabel.Text = $"Случайно ({userScore:F1}%)";
                    userResultLabel.ForeColor = Color.Green;
                }
                else
                {
                    userResultLabel.Text = $"Не случайно ({userScore:F1}%)";
                    userResultLabel.ForeColor = Color.Red;
                }
            }
            else
            {
                userResultLabel.Text = "Введите ≥3 цифр";
                userResultLabel.ForeColor = Color.Orange;
            }
        }

        // СОБСТВЕННЫЙ КРИТЕРИЙ СЛУЧАЙНОСТИ: "КРИТЕРИЙ ЧЕРЕДОВАНИЯ ПАРИТЕТА"
        private double CalculateCustomRandomness(List<int> numbers)
        {
            if (numbers.Count < 3) return 0;

            // 1. Анализ чередования чётности
            int parityChanges = 0;
            for (int i = 1; i < numbers.Count; i++)
            {
                bool currentEven = numbers[i] % 2 == 0;
                bool previousEven = numbers[i - 1] % 2 == 0;
                if (currentEven != previousEven)
                {
                    parityChanges++;
                }
            }

            // 2. Анализ "шагов" - разностей между соседними числами
            int directionChanges = 0;
            for (int i = 2; i < numbers.Count; i++)
            {
                int diff1 = numbers[i - 1] - numbers[i - 2];
                int diff2 = numbers[i] - numbers[i - 1];
                
                if ((diff1 > 0 && diff2 < 0) || (diff1 < 0 && diff2 > 0))
                {
                    directionChanges++;
                }
            }

            // 3. Анализ уникальности чисел
            double uniqueness = (double)numbers.Distinct().Count() / numbers.Count;

            // 4. Комбинированная оценка
            double idealParityChanges = numbers.Count - 1;
            double parityScore = parityChanges / idealParityChanges;

            double idealDirectionChanges = numbers.Count - 2;
            double directionScore = idealDirectionChanges > 0 ? directionChanges / idealDirectionChanges : 0;

            // Итоговый показатель случайности (0-100%)
            double finalScore = (parityScore * 0.4 + directionScore * 0.4 + uniqueness * 0.2) * 100;

            return Math.Min(100, Math.Max(0, finalScore));
        }
    }
}