using System;
using System.Windows;

namespace lab_4
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Обработка необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += (s, args) => 
            {
                MessageBox.Show($"Необработанная ошибка: {((Exception)args.ExceptionObject).Message}", 
                    "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            
            this.DispatcherUnhandledException += (s, args) => 
            {
                MessageBox.Show($"Ошибка в UI: {args.Exception.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
            
            // УДАЛИТЕ эту строку: txtStatus.Text = "Приложение запущено успешно!";
        }
    }
}