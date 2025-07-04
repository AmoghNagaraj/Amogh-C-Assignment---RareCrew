# Amogh-C#-Assignment---RareCrew

This repository contains two C# console applications designed to visualize employee time entry data retrieved from a REST API endpoint.

## 📁 Projects Included

- **HTML Table Visualization**: Generates an HTML page displaying employee time data in a table, ordered by total time worked, with conditional styling.
- **Pie Chart Visualization**: Generates a PNG pie chart illustrating the percentage of total time worked by each employee.

---

## 🧾 1. HTML Table Visualization

### Features

- Retrieves data from the [Time Entries API](https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries).
- Calculates total time worked by each employee (excluding deleted entries).
- Sorts employees by total time in descending order.
- Generates a responsive HTML table displaying names and time worked.
- Highlights rows in red for employees who worked less than **100 hours**.

### How to Run

1. Clone this repository or download the `HtmlTableVisualization` project files.
2. Open the project in Visual Studio or your preferred C# IDE.
3. Build and run the application.
4. Upon execution, the `EmployeeTimeReport.html` file will be generated in the output directory:


5. Open the HTML file in any web browser to view the visualization.

---

## 🥧 2. Pie Chart Visualization

### Features

- Retrieves data from the same [Time Entries API](https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries).
- Calculates each employee’s total time and their percentage of the overall time.
- Generates a pie chart image (`EmployeeTimePieChart.png`) with unique color slices per employee.
- Displays a legend mapping colors to employee names, percentages, and total hours.

### How to Run

1. Clone this repository or download the `PieChartVisualization` project files.
2. Open the project in Visual Studio or your preferred C# IDE.
3. Install the `System.Drawing.Common` NuGet package:

### Requirements
1. .NET SDK (version 8.0 or compatible)
2. For Pie Chart: System.Drawing.Common NuGet package.

#### Using .NET CLI
```bash
dotnet add package System.Drawing.Common
