using System;
using System.Windows;

namespace lab_4
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                // Этот код запускает WPF приложение
                App app = new App();
                app.InitializeComponent(); // Это работает потому что app - экземпляр partial класса
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}