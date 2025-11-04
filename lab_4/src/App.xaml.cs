using System;
using System.Windows;

namespace lab_4
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Минимальная обработка ошибок
            this.DispatcherUnhandledException += (s, args) => 
            {
                MessageBox.Show($"Ошибка: {args.Exception.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}