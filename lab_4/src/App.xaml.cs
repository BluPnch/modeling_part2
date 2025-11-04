using System.Windows;

namespace lab_4
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Создаем и показываем главное окно
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}