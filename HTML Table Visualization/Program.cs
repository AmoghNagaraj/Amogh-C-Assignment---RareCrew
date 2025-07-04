using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

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
}

public class HtmlTableGenerator
{
    private static readonly string ApiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Fetching employee time entries...");
        List<TimeEntry> timeEntries = await GetTimeEntriesAsync();

        if (timeEntries == null || !timeEntries.Any())
        {
            Console.WriteLine("No time entries found or failed to fetch data.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(); 
            return;
        }

        Console.WriteLine("Processing data and generating HTML...");
        var activeTimeEntries = timeEntries.Where(e => e.DeletedOn == null).ToList();

        var employeeSummaries = activeTimeEntries
            .GroupBy(e => e.EmployeeName)
            .Select(g => new EmployeeSummary
            {
                Name = g.Key,
                TotalTimeWorked = g.Sum(e => e.TimeWorkedHours)
            })
            .OrderByDescending(es => es.TotalTimeWorked)
            .ToList();

        if (!employeeSummaries.Any())
        {
            Console.WriteLine("No active employee summaries to display after filtering. The HTML table will be empty.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(); 
            return;
        }

        string htmlContent = GenerateHtmlTable(employeeSummaries);

        string filePath = "EmployeeTimeReport.html";
        try
        {
            File.WriteAllText(filePath, htmlContent);
            string fullPath = Path.GetFullPath(filePath);
            Console.WriteLine($"HTML report generated successfully at: {fullPath}");
            Console.WriteLine("Please open this file in your web browser to view the table.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing HTML file: {ex.Message}");
            Console.WriteLine("Please ensure you have write permissions to the output directory.");
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(); 
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
            Console.WriteLine("Please check your internet connection or the API endpoint.");
            return null;
        }
        catch (JsonException e)
        {
            Console.WriteLine($"JSON Deserialization Error: {e.Message}");
            Console.WriteLine("The API response might not be in the expected JSON format.");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unexpected error occurred: {e.Message}");
            return null;
        }
    }

    private static string GenerateHtmlTable(List<EmployeeSummary> employeeSummaries)
    {
        var htmlBuilder = new System.Text.StringBuilder();
        htmlBuilder.AppendLine("<!DOCTYPE html>");
        htmlBuilder.AppendLine("<html lang=\"en\">");
        htmlBuilder.AppendLine("<head>");
        htmlBuilder.AppendLine("    <meta charset=\"UTF-8\">");
        htmlBuilder.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        htmlBuilder.AppendLine("    <title>Employee Time Report</title>");
        htmlBuilder.AppendLine("    <style>");
        htmlBuilder.AppendLine("        body { font-family: 'Inter', sans-serif; margin: 20px; background-color: #f4f7f6; color: #333; }");
        htmlBuilder.AppendLine("        h1 { color: #2c3e50; text-align: center; margin-bottom: 30px; }");
        htmlBuilder.AppendLine("        table { width: 80%; margin: 0 auto; border-collapse: collapse; box-shadow: 0 4px 8px rgba(0,0,0,0.1); border-radius: 8px; overflow: hidden; }");
        htmlBuilder.AppendLine("        th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #ddd; }");
        htmlBuilder.AppendLine("        th { background-color: #4CAF50; color: white; font-weight: bold; }");
        htmlBuilder.AppendLine("        tr:nth-child(even) { background-color: #f2f2f2; }");
        htmlBuilder.AppendLine("        tr:hover { background-color: #e9e9e9; }");
        htmlBuilder.AppendLine("        .low-hours { background-color: #ffdddd; color: #cc0000; font-weight: bold; }");
        htmlBuilder.AppendLine("        .low-hours:hover { background-color: #ffcccc; }");
        htmlBuilder.AppendLine("    </style>");
        htmlBuilder.AppendLine("</head>");
        htmlBuilder.AppendLine("<body>");
        htmlBuilder.AppendLine("    <h1>Employee Time Report</h1>");
        htmlBuilder.AppendLine("    <table>");
        htmlBuilder.AppendLine("        <thead>");
        htmlBuilder.AppendLine("            <tr>");
        htmlBuilder.AppendLine("                <th>Employee Name</th>");
        htmlBuilder.AppendLine("                <th>Total Time Worked (Hours)</th>");
        htmlBuilder.AppendLine("            </tr>");
        htmlBuilder.AppendLine("        </thead>");
        htmlBuilder.AppendLine("        <tbody>");

        foreach (var employee in employeeSummaries)
        {
            string rowClass = employee.TotalTimeWorked < 100 ? " class=\"low-hours\"" : "";
            htmlBuilder.AppendLine($"           <tr{rowClass}>");
            htmlBuilder.AppendLine($"                <td>{employee.Name}</td>");
            htmlBuilder.AppendLine($"                <td>{employee.TotalTimeWorked:F2}</td>");
            htmlBuilder.AppendLine("            </tr>");
        }

        htmlBuilder.AppendLine("        </tbody>");
        htmlBuilder.AppendLine("    </table>");
        htmlBuilder.AppendLine("</body>");
        htmlBuilder.AppendLine("</html>");

        return htmlBuilder.ToString();
    }
}
