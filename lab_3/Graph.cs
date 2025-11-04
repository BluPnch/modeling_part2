using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

public class GraphPlotter
{
    private const int ChartWidth = 800;
    private const int ChartHeight = 400;
    private const int Margin = 50;
    
    public void PlotDistribution(IDistribution distribution, double minX, double maxX, string outputPath)
{
    // Увеличиваем размер графика
    const int ChartWidth = 1200;
    const int ChartHeight = 600;
    const int Margin = 60;
    
    // Создаем bitmap для графика
    using (var bitmap = new Bitmap(ChartWidth, ChartHeight * 2 + Margin))
    using (var graphics = Graphics.FromImage(bitmap))
    {
        graphics.Clear(Color.White);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        
        // Рисуем PDF
        PlotPDF(graphics, distribution, minX, maxX, 0, ChartWidth, ChartHeight, Margin);
        
        // Рисуем CDF
        PlotCDF(graphics, distribution, minX, maxX, ChartHeight + Margin, ChartWidth, ChartHeight, Margin);
        
        // Сохраняем изображение
        bitmap.Save(outputPath, ImageFormat.Png);
    }
}

private void PlotPDF(Graphics graphics, IDistribution distribution, double minX, double maxX, int yOffset, int chartWidth, int chartHeight, int margin)
{
    // Подготовка данных
    int pointsCount = 300; // Увеличили количество точек для более гладкого графика
    var points = new List<PointF>();
    
    double maxY = 0;
    for (int i = 0; i <= pointsCount; i++)
    {
        double x = minX + (maxX - minX) * i / pointsCount;
        double y = distribution.PDF(x);
        if (y > maxY) maxY = y;
        
        points.Add(new PointF(i, (float)y));
    }
    
    // Масштабирование
    var scaledPoints = points.Select(p => 
        new PointF(
            margin + (chartWidth - 2 * margin) * p.X / pointsCount,
            yOffset + chartHeight - margin - (chartHeight - 2 * margin) * (float)(p.Y / maxY)
        )).ToArray();
    
    // Рисуем оси
    DrawAxes(graphics, minX, maxX, 0, maxY, yOffset, "PDF", chartWidth, chartHeight, margin);
    
    // Рисуем график
    using (var pen = new Pen(Color.Blue, 3)) // Увеличили толщину линии
    {
        graphics.DrawLines(pen, scaledPoints);
    }
    
    // Заголовок
    using (var font = new Font("Arial", 14, FontStyle.Bold)) // Увеличили шрифт
    using (var brush = new SolidBrush(Color.Black))
    {
        graphics.DrawString($"{distribution.Name} - Probability Density Function", 
            font, brush, margin, yOffset + 15);
    }
}

private void PlotCDF(Graphics graphics, IDistribution distribution, double minX, double maxX, int yOffset, int chartWidth, int chartHeight, int margin)
{
    // Подготовка данных
    int pointsCount = 300;
    var points = new List<PointF>();
    
    for (int i = 0; i <= pointsCount; i++)
    {
        double x = minX + (maxX - minX) * i / pointsCount;
        double y = distribution.CDF(x);
        points.Add(new PointF(i, (float)y));
    }
    
    // Масштабирование
    var scaledPoints = points.Select(p => 
        new PointF(
            margin + (chartWidth - 2 * margin) * p.X / pointsCount,
            yOffset + chartHeight - margin - (chartHeight - 2 * margin) * p.Y
        )).ToArray();
    
    // Рисуем оси
    DrawAxes(graphics, minX, maxX, 0, 1, yOffset, "CDF", chartWidth, chartHeight, margin);
    
    // Рисуем график
    using (var pen = new Pen(Color.Red, 3)) // Увеличили толщину линии
    {
        graphics.DrawLines(pen, scaledPoints);
    }
    
    // Заголовок
    using (var font = new Font("Arial", 14, FontStyle.Bold)) // Увеличили шрифт
    using (var brush = new SolidBrush(Color.Black))
    {
        graphics.DrawString($"{distribution.Name} - Cumulative Distribution Function", 
            font, brush, margin, yOffset + 15);
    }
}

private void DrawAxes(Graphics graphics, double minX, double maxX, double minY, double maxY, int yOffset, string yLabel, int chartWidth, int chartHeight, int margin)
{
    using (var pen = new Pen(Color.Black, 2)) // Увеличили толщину осей
    using (var font = new Font("Arial", 10)) // Увеличили шрифт подписей
    using (var brush = new SolidBrush(Color.Black))
    {
        // Ось X
        graphics.DrawLine(pen, margin, yOffset + chartHeight - margin, 
            chartWidth - margin, yOffset + chartHeight - margin);
        
        // Ось Y
        graphics.DrawLine(pen, margin, yOffset + margin, 
            margin, yOffset + chartHeight - margin);
        
        // Подписи оси X
        for (double x = minX; x <= maxX; x += (maxX - minX) / 5)
        {
            float xPos = margin + (chartWidth - 2 * margin) * (float)((x - minX) / (maxX - minX));
            graphics.DrawLine(pen, xPos, yOffset + chartHeight - margin - 8, 
                xPos, yOffset + chartHeight - margin + 8);
            graphics.DrawString(x.ToString("F1"), font, brush, xPos - 15, yOffset + chartHeight - margin + 12);
        }
        
        // Подписи оси Y
        for (double y = minY; y <= maxY; y += (maxY - minY) / 5)
        {
            float yPos = yOffset + chartHeight - margin - (chartHeight - 2 * margin) * (float)((y - minY) / (maxY - minY));
            graphics.DrawLine(pen, margin - 8, yPos, margin + 8, yPos);
            graphics.DrawString(y.ToString("F2"), font, brush, 5, yPos - 8);
        }
        
        // Метки осей
        using (var labelFont = new Font("Arial", 12, FontStyle.Bold))
        {
            graphics.DrawString("x", labelFont, brush, chartWidth - margin + 15, yOffset + chartHeight - margin - 15);
            graphics.DrawString(yLabel, labelFont, brush, margin - 40, yOffset + margin - 25);
        }
    }
}

// public void ShowStatistics(IDistribution distribution, string outputPath)
// {
//     // Увеличили размер изображения статистики
//     using (var bitmap = new Bitmap(400, 250))
//     using (var graphics = Graphics.FromImage(bitmap))
//     {
//         graphics.Clear(Color.LightGray); // Изменили фон для лучшей видимости
//         
//         using (var font = new Font("Arial", 11))
//         using (var boldFont = new Font("Arial", 13, FontStyle.Bold))
//         using (var brush = new SolidBrush(Color.Black))
//         {
//             graphics.DrawString($"Statistics: {distribution.Name}", boldFont, brush, 20, 15);
//             graphics.DrawString($"Mean: {distribution.Mean():F6}", font, brush, 20, 50);
//             graphics.DrawString($"Variance: {distribution.Variance():F6}", font, brush, 20, 80);
//             graphics.DrawString($"Std Deviation: {Math.Sqrt(distribution.Variance()):F6}", font, brush, 20, 110);
//             graphics.DrawString($"Coefficient of Variation: {Math.Sqrt(distribution.Variance()) / distribution.Mean():F6}", font, brush, 20, 140);
//             graphics.DrawString($"Range: [{GetPlotRangeMin(distribution):F3}, {GetPlotRangeMax(distribution):F3}]", font, brush, 20, 170);
//         }
//         
//         bitmap.Save(outputPath, ImageFormat.Png);
//     }
// }

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
}