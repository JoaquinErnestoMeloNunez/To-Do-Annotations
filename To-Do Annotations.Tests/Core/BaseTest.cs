using AventStack.ExtentReports;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace To_Do_Annotations.Tests.Core
{
    public class BaseTest
    {
        protected IWebDriver driver;
        protected ExtentReports extent;
        protected ExtentTest test;

        protected const string BaseUrl = "http://localhost:5118";

        public WebDriverWait Wait => new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            extent = ExtentManager.CreateInstance();
        }

        [SetUp]
        public void Setup()
        {
            test = extent.CreateTest(TestContext.CurrentContext.Test.Name);

            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                var errorMessage = TestContext.CurrentContext.Result.Message;
                var stacktrace = TestContext.CurrentContext.Result.StackTrace;

                string screenshotPath = CaptureScreenshot(driver, "Final_" + TestContext.CurrentContext.Test.Name);

                if (status == TestStatus.Failed)
                {
                    test.Fail("Test Fallido: " + errorMessage);
                    test.Fail(stacktrace);
                    if (!string.IsNullOrEmpty(screenshotPath))
                        test.AddScreenCaptureFromPath(screenshotPath, "Fallo Final");
                }
                else if (status == TestStatus.Passed)
                {
                    test.Pass("Test Exitoso");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al generar reporte/captura: " + ex.Message);
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                    driver.Dispose();
                    driver = null;
                }
            }
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            extent.Flush();
        }

        protected string CaptureScreenshot(IWebDriver driver, string screenShotName)
        {
            try
            {
                string safeName = string.Join("_", screenShotName.Split(Path.GetInvalidFileNameChars()));
                string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"{safeName}_{timeStamp}.png";

                string binDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Directory.GetParent(binDirectory).Parent.Parent.Parent.FullName;

                string screenshotFolder = Path.Combine(projectDirectory, "Reports", "Screenshots");

                if (!Directory.Exists(screenshotFolder))
                {
                    Directory.CreateDirectory(screenshotFolder);
                }

                string finalPath = Path.Combine(screenshotFolder, fileName);

                ITakesScreenshot ts = (ITakesScreenshot)driver;
                Screenshot screenshot = ts.GetScreenshot();
                screenshot.SaveAsFile(finalPath);

                return finalPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturando pantalla: {ex.Message}");
                return "";
            }
        }

        protected void TakeEvidence(string description)
        {
            string path = CaptureScreenshot(driver, description);
            if (!string.IsNullOrEmpty(path))
            {
                test.Info($"Evidencia: {description}", MediaEntityBuilder.CreateScreenCaptureFromPath(path).Build());
            }
        }

        protected void PerformLogin(string user, string pass)
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Access/Login");
            if (driver.Url.Contains(BaseUrl) && !driver.Url.Contains("Access/Login")) return;

            var userInput = Wait.Until(ExpectedConditions.ElementIsVisible(By.Name("username")));
            userInput.Clear();
            userInput.SendKeys(user);

            driver.FindElement(By.Name("password")).SendKeys(pass);
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            Wait.Until(ExpectedConditions.UrlContains(BaseUrl));
        }
    }
}