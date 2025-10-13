using System;
using System.Windows.Forms;

namespace RandomNumberLab
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Lab1.Lab1());
        }
    }
}