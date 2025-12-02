using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace To_Do_Annotations.Tests
{
    public class ExtentManager
    {
        public static ExtentReports? Instance { get; private set; }

        public static ExtentReports CreateInstance()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string reportPath = Path.Combine(projectDirectory, "Reports", "index.html");

            var htmlReporter = new ExtentSparkReporter(reportPath);
            htmlReporter.Config.DocumentTitle = "Reporte de Pruebas Automatizadas - ToDo App";
            htmlReporter.Config.ReportName = "Resultados de Ejecución Selenium";
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Standard;

            Instance = new ExtentReports();
            Instance.AttachReporter(htmlReporter);
            Instance.AddSystemInfo("OS", Environment.OSVersion.ToString());
            Instance.AddSystemInfo("Browser", "Chrome");

            return Instance;
        }
    }
}