using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using To_Do_Annotations.Tests.Core;

namespace To_Do_Annotations.Tests
{
    [TestFixture]
    public class ToDoAppTests : BaseTest
    {
        [TestFixture]
        public class AppTests : BaseTest
        {
            private const string defaultPass = "123456";

            [Test]
            public void Test_Register_Flow()
            {
                string uniqueUser = "User_" + DateTime.Now.Ticks;
                test.Info($"Iniciando registro para usuario: {uniqueUser}");

                driver.Navigate().GoToUrl($"{BaseUrl}/Access/Register");

                Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Username"))).SendKeys(uniqueUser);
                driver.FindElement(By.Id("Password")).SendKeys(defaultPass);
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                Wait.Until(ExpectedConditions.UrlContains("/Access/Login"));
                Assert.IsTrue(driver.Url.Contains("/Access/Login"), "No se redirigió al login después del registro");

                TakeEvidence("Registro_Exitoso");
                test.Pass("Registro verificado");
            }

            [Test]
            public void Test_Login_Success_And_Failure()
            {
                test.Info("Probando login fallido");
                driver.Navigate().GoToUrl($"{BaseUrl}/Access/Login");

                var userInput = Wait.Until(ExpectedConditions.ElementIsVisible(By.Name("username")));
                userInput.SendKeys("wrong_user");
                driver.FindElement(By.Name("password")).SendKeys("WrongPass");
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                var errorAlert = Wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert.alert-danger")));

                TakeEvidence("Login_Fallo_Esperado");
                Assert.IsTrue(errorAlert.Displayed, "No apareció el mensaje de error.");

                test.Info("Probando login exitoso");
                userInput = driver.FindElement(By.Name("username"));
                userInput.Clear();
                userInput.SendKeys("joelthegoat");

                var passInput = driver.FindElement(By.Name("password"));
                passInput.Clear();
                passInput.SendKeys("1234");

                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                Wait.Until(ExpectedConditions.UrlContains(BaseUrl));

                TakeEvidence("Login_Exitoso_Dashboard");
                test.Pass("Login completado");
            }

            [Test]
            public void Test_Create_Task()
            {
                string user = "CreateUser_" + DateTime.Now.Ticks;
                RegisterAndLogin(user, defaultPass);
                test.Info("Creando tarea");

                Wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText("Add New Task"))).Click();
                Wait.Until(ExpectedConditions.UrlContains("/Tasks/Create"));

                var titleInput = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Title")));
                string uniqueTitle = "Task_" + Guid.NewGuid().ToString().Substring(0, 8);
                titleInput.SendKeys(uniqueTitle);
                driver.FindElement(By.Id("Description")).SendKeys("Description Create Test");

                new SelectElement(driver.FindElement(By.Id("Status"))).SelectByValue("0");

                var createdAt = driver.FindElement(By.Id("CreatedAt"));
                string isoDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = arguments[1];", createdAt, isoDate);

                driver.FindElement(By.XPath("//input[@value='Create']")).Click();

                Wait.Until(ExpectedConditions.UrlContains(BaseUrl));
                var taskRow = Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//td[contains(text(), '{uniqueTitle}')]")));

                TakeEvidence("Tarea_Creada_" + uniqueTitle);
                Assert.IsTrue(taskRow.Displayed);
            }

            [Test]
            public void Test_Edit_Task()
            {
                string user = "EditUser_" + DateTime.Now.Ticks;
                RegisterAndLogin(user, defaultPass);

                Wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText("Add New Task"))).Click();
                Wait.Until(ExpectedConditions.UrlContains("/Tasks/Create"));

                string uniqueTitle = "EditMe_" + Guid.NewGuid().ToString().Substring(0, 5);

                Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Title"))).SendKeys(uniqueTitle);
                driver.FindElement(By.Id("Description")).SendKeys("Original Desc");
                new SelectElement(driver.FindElement(By.Id("Status"))).SelectByValue("0");

                var createdAt = driver.FindElement(By.Id("CreatedAt"));
                string isoDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = arguments[1];", createdAt, isoDate);

                driver.FindElement(By.XPath("//input[@value='Create']")).Click();

                Wait.Until(d => !d.Url.Contains("Create"));
                Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//td[contains(text(), '{uniqueTitle}')]")));

                TakeEvidence("Antes_Editar_" + uniqueTitle);

                driver.FindElement(By.XPath($"//tr[td[contains(text(), '{uniqueTitle}')]]//a[contains(text(), 'Edit')]")).Click();
                Wait.Until(ExpectedConditions.UrlContains("/Tasks/Edit"));

                var editCreatedAt = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("CreatedAt")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = arguments[1];", editCreatedAt, isoDate);

                new SelectElement(driver.FindElement(By.Id("Status"))).SelectByValue("2");
                driver.FindElement(By.XPath("//input[@value='Save']")).Click();
                Wait.Until(d => !d.Url.Contains("Edit"));
                Wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("table")));

                bool isTaskHidden = Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath($"//td[contains(text(), '{uniqueTitle}')]")));

                TakeEvidence("Despues_Editar_Completado_" + uniqueTitle);

                Assert.IsTrue(isTaskHidden, "La tarea marcada como completada debería desaparecer de la lista de pendientes.");
            }

            [Test]
            public void Test_Delete_Task_Flow()
            {
                string user = "DelUser_" + DateTime.Now.Ticks;
                RegisterAndLogin(user, defaultPass);
                string titleToDelete = "DeleteMe_" + DateTime.Now.Ticks;

                Wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText("Add New Task"))).Click();
                Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Title"))).SendKeys(titleToDelete);

                new SelectElement(driver.FindElement(By.Id("Status"))).SelectByValue("0");
                var createdAt = driver.FindElement(By.Id("CreatedAt"));
                string isoDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = arguments[1];", createdAt, isoDate);

                driver.FindElement(By.XPath("//input[@value='Create']")).Click();

                Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//td[contains(text(), '{titleToDelete}')]")));

                driver.FindElement(By.XPath($"//tr[td[contains(text(), '{titleToDelete}')]]//a[contains(text(), 'Delete')]")).Click();
                Wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));
                driver.FindElement(By.XPath("//input[@type='submit' and @value='Delete']")).Click();

                Wait.Until(ExpectedConditions.UrlContains(BaseUrl));

                TakeEvidence("Tarea_Eliminada");

                bool isDeleted = Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath($"//td[contains(text(), '{titleToDelete}')]")));
                Assert.IsTrue(isDeleted);
            }

            private void RegisterAndLogin(string username, string password)
            {
                driver.Navigate().GoToUrl($"{BaseUrl}/Access/Register");
                Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Username"))).SendKeys(username);
                driver.FindElement(By.Id("Password")).SendKeys(password);
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();
                Wait.Until(ExpectedConditions.UrlContains("/Access/Login"));
                PerformLogin(username, password);
            }
        }
    }
}