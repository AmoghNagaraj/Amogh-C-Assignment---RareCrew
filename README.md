# Amogh-C-Assignment---RareCrew

C# Assignment - RareCrew
This repository contains two C# console applications designed to visualize employee time entry data retrieved from a REST API endpoint.

Projects Included
HTML Table Visualization: Generates an HTML page displaying employee time data in a table, ordered by total time worked, with conditional styling.

Pie Chart Visualization: Generates a PNG image file of a pie chart, illustrating the percentage of total time worked by each employee.

1. HTML Table Visualization
This project fetches time entry data, processes it, and outputs an HTML file (EmployeeTimeReport.html) that can be viewed in a web browser.

Features
Retrieves data from https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries.

Calculates the total time worked for each employee (excluding deleted entries).

Orders employees by total time worked in descending order.

Generates a responsive HTML table showing employee names and their total time worked.

Highlights table rows in red for employees who worked less than 100 hours.

How to Run
Clone this repository or download the HtmlTableVisualization project files.

Open the project in Visual Studio or your preferred C# IDE.

Build the project.

Run the application.

Upon successful execution, a file named EmployeeTimeReport.html will be generated in your project's output directory (e.g., bin/Debug/net8.0/). The console output will provide the full path to this file. Open this HTML file in any web browser to view the table.

2. Pie Chart Visualization
This project fetches the same time entry data and generates a PNG image file (EmployeeTimePieChart.png) representing the distribution of total time worked among employees.

Features
Retrieves data from https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries.

Calculates the total time worked for each employee and their percentage of the overall total.

Generates a pie chart image with distinct colors for each employee's slice.

Includes a legend mapping colors to employee names, their percentages, and total hours.

Saves the chart as EmployeeTimePieChart.png.

How to Run
Clone this repository or download the PieChartVisualization project files.

Open the project in Visual Studio or your preferred C# IDE.

Install the System.Drawing.Common NuGet package. This is crucial for graphics operations in .NET Core/.NET 5+ and later versions (like .NET 8.0).

Using .NET CLI:
Navigate to the project directory in your terminal and run:

dotnet add package System.Drawing.Common

Using Visual Studio:
Right-click on the PieChartVisualization project in Solution Explorer -> "Manage NuGet Packages..." -> Browse tab -> Search for System.Drawing.Common -> Install.

Build the project.

Run the application.

Upon successful execution, a file named EmployeeTimePieChart.png will be generated in your project's output directory (e.g., bin/Debug/net8.0/). The console output will provide the full path to this file. You can open this PNG image with any image viewer.

API Endpoint
Both projects retrieve data from the following public API endpoint:

https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==

Requirements
.NET SDK (version 8.0 or compatible)

For Pie Chart: System.Drawing.Common NuGet package.

Contributing
Feel free to fork this repository, open issues, or submit pull requests.
