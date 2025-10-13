using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace Lab1
{
    public partial class Lab1 : Form
    {
        private Random random;
        private List<TextBox> userInputBoxes;
        private const int NumbersCount = 10;
        private TableLayoutPanel tablePanel;
        private Label randomResultLabel;
        private Label userResultLabel;
        
        private double? cachedTemperature = null;
        private DateTime lastTemperatureUpdate = DateTime.MinValue;
        
        private enum RandomnessMode { Normal, Lunar, Temp, Mixed }
        private RandomnessMode currentMode = RandomnessMode.Normal;
        
        public class WeatherResponse
        {
            public MainData main { get; set; }
        }

        public class MainData
        {
            public double temp { get; set; }
        }
        
        
        public Lab1()
        {
            InitializeComponent();
        }
        

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            random = new Random();
            userInputBoxes = new List<TextBox>();
            
            CreateTable();
            CreateButtons();
            
            this.ResumeLayout();
            GenerateNewNumbers();
        }

        private void GenerateNewNumbers()
        {
            // Обновляем числа в табличном способе с разными диапазонами
            for (int i = 0; i < NumbersCount; i++)
            {
                // Первый столбец - однозначные числа (1-9)
                Label numberLabel1 = tablePanel.GetControlFromPosition(1, i + 2) as Label;
                if (numberLabel1 != null)
                {
                    numberLabel1.Text = random.Next(1, 10).ToString(); // 1-9
                }

                // Второй столбец - двузначные числа (10-99)
                Label numberLabel2 = tablePanel.GetControlFromPosition(2, i + 2) as Label;
                if (numberLabel2 != null)
                {
                    numberLabel2.Text = random.Next(10, 100).ToString(); // 10-99
                }

                // Третий столбец - трехзначные числа (100-199)
                Label numberLabel3 = tablePanel.GetControlFromPosition(3, i + 2) as Label;
                if (numberLabel3 != null)
                {
                    numberLabel3.Text = random.Next(100, 200).ToString(); // 100-199
                }
            }

            // Обновляем числа в алгоритмическом способе (как было)
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

            // ОБНОВЛЯЕМ результаты случайности для всех столбцов
            UpdateRandomnessResults();

            userResultLabel.Text = "Не случайно";
            userResultLabel.ForeColor = Color.Red;
        }

        private void CreateTable()
        {
            tablePanel = new TableLayoutPanel();
            tablePanel.Dock = DockStyle.Fill;
            tablePanel.ColumnCount = 8; 
            tablePanel.RowCount = NumbersCount + 2;
            tablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tablePanel.AutoScroll = true;
            
            tablePanel.ColumnStyles.Clear();
            for (int i = 0; i < 8; i++)
            {
                tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }
            
            // Заголовки
            tablePanel.Controls.Add(new Label { 
                Text = "№", 
                Font = new Font("Arial", 10, FontStyle.Bold), 
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Fill,
                AutoSize = true
            }, 0, 0);
            
            // Табличный способ
            tablePanel.Controls.Add(new Label { 
                Text = "Табличный способ", 
                Font = new Font("Arial", 10, FontStyle.Bold), 
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Fill,
                AutoSize = true
            }, 1, 0);
            tablePanel.SetColumnSpan(tablePanel.GetControlFromPosition(1, 0), 3);
            
            // Алгоритмический способ
            tablePanel.Controls.Add(new Label { 
                Text = "Алгоритмический способ", 
                Font = new Font("Arial", 10, FontStyle.Bold), 
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Fill,
                AutoSize = true
            }, 4, 0);
            tablePanel.SetColumnSpan(tablePanel.GetControlFromPosition(4, 0), 3);
            
            // Рудаков
            tablePanel.Controls.Add(new Label { 
                Text = "Пользовательское", 
                Font = new Font("Arial", 10, FontStyle.Bold), 
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Fill,
                AutoSize = true
            }, 7, 0);
            
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
                tablePanel.Controls.Add(new Label { 
                    Text = (i + 1).ToString(), 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    AutoSize = true
                }, 0, i + 2);
                
                // Табличный способ с разными диапазонами
                // Первый столбец - однозначные числа
                tablePanel.Controls.Add(new Label { 
                    Text = random.Next(1, 10).ToString(),
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    AutoSize = true
                }, 1, i + 2);
                
                // Второй столбец - двузначные числа
                tablePanel.Controls.Add(new Label { 
                    Text = random.Next(10, 100).ToString(),
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    AutoSize = true
                }, 2, i + 2);
                
                // Третий столбец - трехзначные числа
                tablePanel.Controls.Add(new Label { 
                    Text = random.Next(100, 200).ToString(), 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Fill,
                    AutoSize = true
                }, 3, i + 2);
                
                // Алгоритмический способ (как было)
                for (int j = 0; j < 3; j++)
                {
                    tablePanel.Controls.Add(new Label { 
                        Text = random.Next(1, 200).ToString(), 
                        TextAlign = ContentAlignment.MiddleCenter, 
                        Dock = DockStyle.Fill,
                        AutoSize = true
                    }, j + 4, i + 2);
                }
                
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
            
            // Остальной код CreateTable остается без изменений...
            tablePanel.Controls.Add(new Label { 
                Text = "Результат:", 
                Font = new Font("Arial", 10, FontStyle.Bold), 
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Fill,
                AutoSize = true
            }, 0, NumbersCount + 2);
            
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
                Height = 100,  // Увеличили высоту для нового элемента
                BackColor = Color.LightGray
            };
    
            // ДОБАВИТЬ ВЫПАДАЮЩИЙ СПИСОК ДЛЯ РЕЖИМОВ
            Label modeLabel = new Label { 
                Text = "Режим оценки:", 
                Size = new Size(100, 20), 
                Location = new Point(10, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };
    
            ComboBox modeComboBox = new ComboBox { 
                Size = new Size(120, 25), 
                Location = new Point(115, 12),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            modeComboBox.Items.AddRange(new object[] { "Нормальный", "Лунный", "Забавный", "Смешанный" });
            modeComboBox.SelectedIndex = 0;
            modeComboBox.SelectedIndexChanged += (s, e) => 
            {
                currentMode = (RandomnessMode)modeComboBox.SelectedIndex;
                UpdateRandomnessResults(); // Обновляем результаты при смене режима
            };
    
            Button generateBtn = new Button { 
                Text = "Сгенерировать новые числа", 
                Size = new Size(180, 35), 
                Location = new Point(250, 10),
                BackColor = Color.White
            };
            generateBtn.Click += (s, e) => GenerateNewNumbers();
    
            Button checkBtn = new Button { 
                Text = "Проверить случайность", 
                Size = new Size(180, 35), 
                Location = new Point(440, 10),
                BackColor = Color.White
            };
            checkBtn.Click += (s, e) => CheckRandomness();
    
            buttonPanel.Controls.Add(modeLabel);
            buttonPanel.Controls.Add(modeComboBox);
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
        
        private void UpdateRandomnessResults()
        {
            // Обновляем результаты для табличного способа
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
        
                // Обновляем существующую метку
                Label resultLabel = tablePanel.GetControlFromPosition(i, NumbersCount + 2) as Label;
                if (resultLabel != null)
                {
                    resultLabel.Text = resultText;
                    resultLabel.ForeColor = tableScore > 50 ? Color.Green : Color.Red;
                }
            }

            // Обновляем результаты для алгоритмического способа
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
        
                // Обновляем существующую метку
                Label resultLabel = tablePanel.GetControlFromPosition(i, NumbersCount + 2) as Label;
                if (resultLabel != null)
                {
                    resultLabel.Text = resultText;
                    resultLabel.ForeColor = algoScore > 50 ? Color.Green : Color.Red;
                }
            }
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
            
            UpdateRandomnessResults();
        }

        private double CalculateCustomRandomness(List<int> numbers)
        {
            if (numbers.Count < 3) return 0;

            switch (currentMode)
            {
                case RandomnessMode.Lunar:
                    return CalculateLunarRandomness(numbers); 
                // case RandomnessMode.Normal:
                //     return CalculateNormalRandomness(numbers);
                // case RandomnessMode.Temp:
                //     return CalculateTemperatureRandomness(numbers);
                // case RandomnessMode.Mixed:
                //     return CalculateMixedRandomness(numbers);
                default:
                    return CalculateNormalRandomness(numbers);
            }
        }

        private double CalculateNormalRandomness(List<int> numbers)
        {
            double parityScore = CalculateParityScore(numbers);
            double directionScore = CalculateDirectionScore(numbers);
            double uniquenessScore = CalculateUniquenessScore(numbers);

            double finalScore = (
                parityScore * 0.4 + 
                directionScore * 0.4 + 
                uniquenessScore * 0.2
            ) * 100;

            return Math.Min(100, Math.Max(0, finalScore));
        }

        private double CalculateLunarRandomness(List<int> numbers)
        {
            double moonPhaseScore = CalculateMoonPhaseScore(numbers);
        
            double finalScore = moonPhaseScore * 100;
        
            return Math.Min(100, Math.Max(0, finalScore));
        }

        // private double CalculateTemperatureRandomness(List<int> numbers)
        // {
        //     double temperatureScore = CalculateTemperatureScore(numbers);
        //
        //     double finalScore = temperatureScore * 100;
        //
        //     return Math.Min(100, Math.Max(0, finalScore));
        // }

        
        
        private double CalculateMixedRandomness(List<int> numbers)
        {
            double parityScore = CalculateParityScore(numbers);
            double directionScore = CalculateDirectionScore(numbers);
            double uniquenessScore = CalculateUniquenessScore(numbers);
            // double temperatureScore = CalculateTemperatureScore(numbers);
            double moonPhaseScore = CalculateMoonPhaseScore(numbers);

            // double finalScore = (
            //     parityScore * 0.25 + 
            //     directionScore * 0.25 + 
            //     uniquenessScore * 0.2 +
            //     temperatureScore * 0.15 +
            //     moonPhaseScore * 0.15
            // ) * 100;
            
            double finalScore = (
                parityScore * 0.25 + 
                directionScore * 0.25 + 
                uniquenessScore * 0.25 +
                moonPhaseScore * 0.25
            ) * 100;

            return Math.Min(100, Math.Max(0, finalScore));
        }

        // Анализ чередования чётности
        private double CalculateParityScore(List<int> numbers)
        {
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

            double idealParityChanges = numbers.Count - 1;
            return idealParityChanges > 0 ? parityChanges / idealParityChanges : 0;
        }

        // Анализ изменения направления
        private double CalculateDirectionScore(List<int> numbers)
        {
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

            double idealDirectionChanges = numbers.Count - 2;
            return idealDirectionChanges > 0 ? directionChanges / idealDirectionChanges : 0;
        }

        // Анализ уникальности чисел
        private double CalculateUniquenessScore(List<int> numbers)
        {
            return (double)numbers.Distinct().Count() / numbers.Count;
        }

        // Критерий Луна
        // Критерий Луна
        // Критерий Луна - ИСПРАВЛЕННАЯ ВЕРСИЯ
        private double CalculateMoonPhaseScore(List<int> numbers)
        {
            int dayOfMonth = DateTime.Now.Day;
            
            // Определяем фазу луны на основе дня месяца с учетом длительности фаз
            (string moonPhaseName, int moonPhaseNumber, int phaseDuration) = GetMoonPhase(dayOfMonth);
            
            // ИСПРАВЛЕНИЕ: ищем числа, КРАТНЫЕ номеру фазы, а не с остатком
            int moonMultiples = numbers.Count(n => n != 0 && n % moonPhaseNumber == 0);
            
            // Идеальное соотношение учитывает длительность фазы
            double idealRatio = phaseDuration / 29.5; // 29.5 - средняя длительность лунного цикла
            double actualRatio = (double)moonMultiples / numbers.Count;
            
            // Чем БОЛЬШЕ кратных чисел - тем МЕНЕЕ случайна последовательность
            double deviation = Math.Max(0, actualRatio - idealRatio); // Только превышение
            
            // Базовый счет: 1.0 - нет превышения, 0.0 - сильное превышение
            double baseScore = Math.Max(0, 1.0 - (deviation));
            
            // Вывод в консоль
            Console.WriteLine($"=== MOON PHASE CRITERION ===");
            Console.WriteLine($"Day of month: {dayOfMonth}");
            Console.WriteLine($"Current Moon Phase: {moonPhaseName} (Phase number: {moonPhaseNumber})");
            Console.WriteLine($"Looking for numbers MULTIPLE of {moonPhaseNumber}");
            Console.WriteLine($"Numbers checked: {string.Join(", ", numbers)}");
            
            var multiples = numbers.Where(n => n != 0 && n % moonPhaseNumber == 0).ToList();
            Console.WriteLine($"Multiples of {moonPhaseNumber}: {(multiples.Any() ? string.Join(", ", multiples) : "None")}");
            
            Console.WriteLine($"Numbers multiple of moon phase: {moonMultiples}");
            Console.WriteLine($"Ideal ratio: {idealRatio:P1} (based on {phaseDuration} days duration)");
            Console.WriteLine($"Actual ratio: {actualRatio:P2}");
            Console.WriteLine($"Deviation (excess only): {deviation:P2}, Base score: {baseScore:P2}");
            
            // Дополнительная информация для понимания логики
            if (moonMultiples == 0)
            {
                Console.WriteLine($"NO multiples - MOST RANDOM (score: {baseScore:P2})");
            }
            else if (actualRatio <= idealRatio)
            {
                Console.WriteLine($"Within acceptable range - RANDOM (score: {baseScore:P2})");
            }
            else
            {
                Console.WriteLine($"TOO MANY multiples - NOT RANDOM (score: {baseScore:P2})");
            }
            Console.WriteLine($"=============================");
            
            return baseScore;
        }

        private (string phaseName, int phaseNumber, int phaseDuration) GetMoonPhase(int dayOfMonth)
        {
            // Реальная приблизительная длительность фаз Луны в днях
            // Лунный цикл ~29.5 дней, распределяем по фазам
            int dayInCycle = ((dayOfMonth - 1) % 29) + 1; // День в лунном цикле (1-29)
            
            if (dayInCycle <= 1)
                return ("New Moon", 1, 1); // Новолуние - 1 день
            else if (dayInCycle <= 6)
                return ("Waxing Crescent", 2, 5); // Молодая Луна - 5 дней
            else if (dayInCycle <= 8)
                return ("First Quarter", 3, 2); // Первая четверть - 2 дня
            else if (dayInCycle <= 13)
                return ("Waxing Gibbous", 4, 5); // Растущая Луна - 5 дней
            else if (dayInCycle <= 15)
                return ("Full Moon", 5, 2); // Полнолуние - 2 дня
            else if (dayInCycle <= 20)
                return ("Waning Gibbous", 6, 5); // Убывающая Луна - 5 дней
            else if (dayInCycle <= 22)
                return ("Last Quarter", 7, 2); // Последняя четверть - 2 дня
            else
                return ("Waning Crescent", 8, 7); // Старая Луна - 7 дней
        }
        
        // private double CalculateTemperatureScore(List<int> numbers)
        // {
        //     try
        //     {
        //         double currentTemperature = GetHistoricalTemperatureFromGismeteo();
        //         
        //         int temperatureMultiples = numbers.Count(n => n != 0 && 
        //             currentTemperature != 0 && 
        //             Math.Abs(currentTemperature) % Math.Abs(n) == 0);
        //         
        //         double undesirableRatio = (double)temperatureMultiples / numbers.Count;
        //         
        //         // Для отладки
        //         Console.WriteLine($"Температура с Gismeteo: {currentTemperature}°C, Кратные: {temperatureMultiples}");
        //         
        //         return 1.0 - undesirableRatio;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Ошибка в температурном критерии: {ex.Message}");
        //         return CalculateTemperatureScoreFallback(numbers);
        //     }
        // }
        //
        // // МЕТОД ДЛЯ ПОЛУЧЕНИЯ ТЕМПЕРАТУРЫ С GISMETEO
        // private double GetHistoricalTemperatureFromGismeteo()
        // {
        //     try
        //     {
        //         // Кэширование на 6 часов
        //         if (cachedTemperature.HasValue && 
        //             DateTime.Now - lastTemperatureUpdate < TimeSpan.FromHours(6))
        //         {
        //             return cachedTemperature.Value;
        //         }
        //         
        //         DateTime lastYearDate = DateTime.Now.AddYears(-1);
        //         
        //         // URL архива Gismeteo для Москвы
        //         string url = $"https://www.gismeteo.ru/diary/4368/{lastYearDate.Year}/{lastYearDate.Month}/";
        //         
        //         using (HttpClient client = new HttpClient())
        //         {
        //             // Устанавливаем заголовки как у браузера
        //             client.DefaultRequestHeaders.Add("User-Agent", 
        //                 "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        //             client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
        //             client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
        //             
        //             client.Timeout = TimeSpan.FromSeconds(15);
        //             
        //             HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
        //             
        //             if (response.IsSuccessStatusCode)
        //             {
        //                 string html = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //                 double temperature = ParseGismeteoTemperature(html, lastYearDate.Day);
        //                 
        //                 cachedTemperature = temperature;
        //                 lastTemperatureUpdate = DateTime.Now;
        //                 
        //                 Console.WriteLine($"Успешно получена температура: {temperature}°C");
        //                 return temperature;
        //             }
        //             else
        //             {
        //                 throw new Exception($"HTTP error: {response.StatusCode}");
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Ошибка получения температуры с Gismeteo: {ex.Message}");
        //         return GetHistoricalTemperatureFallback();
        //     }
        // }
        //
        // // ПАРСИНГ HTML С HTML AGILITY PACK
        // private double ParseGismeteoTemperature(string html, int targetDay)
        // {
        //     var htmlDoc = new HtmlDocument();
        //     htmlDoc.LoadHtml(html);
        //     
        //     // Ищем таблицу с погодой по классу
        //     var table = htmlDoc.DocumentNode.SelectSingleNode("//table[contains(@class, 'gtable')]");
        //     
        //     if (table == null)
        //     {
        //         // Пробуем найти любую таблицу
        //         table = htmlDoc.DocumentNode.SelectSingleNode("//table");
        //         if (table == null)
        //         {
        //             throw new Exception("Таблица с погодой не найдена на странице");
        //         }
        //     }
        //     
        //     // Ищем все строки таблицы, пропускаем заголовок
        //     var rows = table.SelectNodes(".//tr[position()>1]");
        //     
        //     if (rows == null || rows.Count == 0)
        //     {
        //         throw new Exception("Данные о погоде не найдены в таблице");
        //     }
        //     
        //     foreach (var row in rows)
        //     {
        //         var cells = row.SelectNodes(".//td|.//th");
        //         
        //         if (cells != null && cells.Count >= 2)
        //         {
        //             // Первая ячейка - день месяца
        //             string dayText = CleanText(cells[0].InnerText);
        //             
        //             if (int.TryParse(dayText, out int day) && day == targetDay)
        //             {
        //                 // Вторая ячейка - дневная температура
        //                 string tempText = CleanText(cells[1].InnerText);
        //                 
        //                 // Обрабатываем различные форматы минуса
        //                 tempText = tempText.Replace("&minus;", "-")
        //                                   .Replace("−", "-")
        //                                   .Replace("–", "-")
        //                                   .Replace("—", "-")
        //                                   .Replace(" ", "")
        //                                   .Replace("\n", "")
        //                                   .Replace("\t", "")
        //                                   .Replace("&deg;", "")
        //                                   .Replace("°", "");
        //                 
        //                 if (double.TryParse(tempText, out double temperature))
        //                 {
        //                     return temperature;
        //                 }
        //                 else
        //                 {
        //                     // Пробуем извлечь число из текста
        //                     var match = System.Text.RegularExpressions.Regex.Match(tempText, @"(-?\d+[,.]?\d*)");
        //                     if (match.Success)
        //                     {
        //                         string numberStr = match.Value.Replace(",", ".");
        //                         if (double.TryParse(numberStr, out temperature))
        //                         {
        //                             return temperature;
        //                         }
        //                     }
        //                 }
        //                 
        //                 throw new Exception($"Не удалось распознать температуру: '{tempText}'");
        //             }
        //         }
        //     }
        //     
        //     throw new Exception($"Температура для {targetDay} числа не найдена");
        // }
        //
        // // ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ОЧИСТКИ ТЕКСТА
        // private string CleanText(string text)
        // {
        //     if (string.IsNullOrEmpty(text))
        //         return text;
        //         
        //     return System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        // }
        //
        // // FALLBACK МЕТОДЫ
        // private double CalculateTemperatureScoreFallback(List<int> numbers)
        // {
        //     double fallbackTemperature = GetHistoricalTemperatureFallback();
        //     
        //     int temperatureMultiples = numbers.Count(n => n != 0 && 
        //         fallbackTemperature != 0 && 
        //         Math.Abs(fallbackTemperature) % Math.Abs(n) == 0);
        //     
        //     double undesirableRatio = (double)temperatureMultiples / numbers.Count;
        //     return 1.0 - undesirableRatio;
        // }
        //
        // private double GetHistoricalTemperatureFallback()
        // {
        //     DateTime lastYearDate = DateTime.Now.AddYears(-1);
        //     int month = lastYearDate.Month;
        //     
        //     // Средние температуры для Москвы по месяцам
        //     double baseTemp = month switch
        //     {
        //         1 => -8.0, 2 => -7.5, 3 => -2.0, 4 => 6.5,
        //         5 => 13.5, 6 => 17.0, 7 => 19.0, 8 => 17.5,
        //         9 => 11.5, 10 => 5.0, 11 => -1.0, 12 => -5.5,
        //         _ => 10.0
        //     };
        //     
        //     // Случайное отклонение для реалистичности
        //     Random rnd = new Random();
        //     double randomDeviation = (rnd.NextDouble() - 0.5) * 4;
        //     
        //     return baseTemp + randomDeviation;
        // }
    }
}