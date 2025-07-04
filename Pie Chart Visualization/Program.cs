using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging; 

public class TimeEntry
{
    public string Id { get; set; }
    public string EmployeeName { get; set; }
    public DateTime StarTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public string EntryNotes { get; set; }
    public DateTime? DeletedOn { get; set; }

    public double TimeWorkedHours => (EndTimeUtc - StarTimeUtc).TotalHours;
}

public class EmployeeSummary
{
    public string Name { get; set; }
    public double TotalTimeWorked { get; set; }
    public double PercentageOfTotal { get; set; }
}

public class PieChartGenerator
{
    private static readonly string ApiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Fetching employee time entries for pie chart...");
        List<TimeEntry> timeEntries = await GetTimeEntriesAsync();

        if (timeEntries == null || !timeEntries.Any())
        {
            Console.WriteLine("No time entries found or failed to fetch data.");
            return;
        }

        Console.WriteLine("Processing data for pie chart...");
        var activeTimeEntries = timeEntries.Where(e => e.DeletedOn == null).ToList();

        double overallTotalHours = activeTimeEntries.Sum(e => e.TimeWorkedHours);

        if (overallTotalHours <= 0)
        {
            Console.WriteLine("Overall total time worked is zero or negative, cannot generate pie chart.");
            return;
        }

        var employeeSummaries = activeTimeEntries
            .GroupBy(e => e.EmployeeName)
            .Select(g => new EmployeeSummary
            {
                Name = g.Key,
                TotalTimeWorked = g.Sum(e => e.TimeWorkedHours),
                PercentageOfTotal = (g.Sum(e => e.TimeWorkedHours) / overallTotalHours) * 100
            })
            .OrderByDescending(es => es.PercentageOfTotal)
            .ToList();

        string filePath = "EmployeeTimePieChart.png";
        try
        {
            Console.WriteLine("Generating pie chart image...");
            GeneratePieChart(employeeSummaries, filePath);
            Console.WriteLine($"Pie chart generated successfully at: {Path.GetFullPath(filePath)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating pie chart: {ex.Message}");
            Console.WriteLine("Ensure 'System.Drawing.Common' NuGet package is installed if running on .NET Core/.NET 5+.");
        }
    }

    private static async Task<List<TimeEntry>> GetTimeEntriesAsync()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<TimeEntry>>(jsonString, options);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"HTTP Request Error: {e.Message}");
            return null;
        }
        catch (JsonException e)
        {
            Console.WriteLine($"JSON Deserialization Error: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unexpected error occurred: {e.Message}");
            return null;
        }
    }

    private static void GeneratePieChart(List<EmployeeSummary> employeeSummaries, string outputPath)
    {
        int width = 800;
        int height = 600;
        int pieDiameter = Math.Min(width, height) - 100; 
        int pieX = (width - pieDiameter) / 2;
        int pieY = (height - pieDiameter) / 2 - 50; 
        using (Bitmap bitmap = new Bitmap(width, height))
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.FillRectangle(Brushes.White, 0, 0, width, height);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // For smoother edges

            float currentAngle = 0;
            Random rand = new Random(); 

            List<Color> colors = new List<Color>
            {
                Color.FromArgb(66, 133, 274), 
                Color.FromArgb(234, 67, 63),  
                Color.FromArgb(251, 188, 7), 
                Color.FromArgb(52, 168, 82),  
                Color.FromArgb(171, 71, 188), 
                Color.FromArgb(0, 150, 136),  
                Color.FromArgb(255, 87, 34),  
                Color.FromArgb(103, 58, 183), 
                Color.FromArgb(205, 220, 57), 
                Color.FromArgb(121, 85, 72)   
            };
            int colorIndex = 0;

            foreach (var employee in employeeSummaries)
            {
                float sweepAngle = (float)(employee.PercentageOfTotal / 100 * 360);

                Color sliceColor = colors[colorIndex % colors.Count];
                colorIndex++;

                using (Brush brush = new SolidBrush(sliceColor))
                {
                    graphics.FillPie(brush, pieX, pieY, pieDiameter, pieDiameter, currentAngle, sweepAngle);
                   
                    graphics.DrawPie(Pens.DarkGray, pieX, pieY, pieDiameter, pieDiameter, currentAngle, sweepAngle);
                }

                currentAngle += sweepAngle;
            }

            using (Font titleFont = new Font("Arial", 24, FontStyle.Bold))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                graphics.DrawString("Employee Time Distribution", titleFont, Brushes.Black, new Rectangle(0, 20, width, 50), sf);
            }

            int legendX = 50;
            int legendY = pieY + pieDiameter + 30; 
            int legendColorBoxSize = 20;
            int legendLineHeight = 25;

            using (Font legendFont = new Font("Arial", 12))
            {
                graphics.DrawString("Legend:", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, legendX, legendY - 20);

                colorIndex = 0; 
                foreach (var employee in employeeSummaries)
                {
                    Color sliceColor = colors[colorIndex % colors.Count];
                    colorIndex++;

                    using (Brush brush = new SolidBrush(sliceColor))
                    {
                        graphics.FillRectangle(brush, legendX, legendY, legendColorBoxSize, legendColorBoxSize);
                        graphics.DrawRectangle(Pens.Black, legendX, legendY, legendColorBoxSize, legendColorBoxSize);
                    }

                    string legendText = $"{employee.Name}: {employee.PercentageOfTotal:F2}% ({employee.TotalTimeWorked:F2} hours)";
                    graphics.DrawString(legendText, legendFont, Brushes.Black, legendX + legendColorBoxSize + 10, legendY);

                    legendY += legendLineHeight;
                }
            }

            bitmap.Save(outputPath, ImageFormat.Png);
        }
    }
}
