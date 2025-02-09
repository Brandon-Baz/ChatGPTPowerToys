using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ChatGPT.StandaloneApp.Services
{
    public class BrowserAutomation : IDisposable
    {
        private readonly ChromeDriver _driver;

        public BrowserAutomation()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // Run in headless mode
            options.AddArgument("--disable-gpu"); // Disable GPU for headless mode
            options.AddArgument("--no-sandbox"); // Bypass OS security model
            options.AddArgument("--disable-dev-shm-usage"); // Overcome limited resource problems
            options.AddArgument("--window-size=1920,1080"); // Set window size for consistent rendering

            _driver = new ChromeDriver(options);
        }

        public async Task<string> GetChatGPTResponseAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));
            }

            try
            {
                // Navigate to ChatGPT page
                _driver.Navigate().GoToUrl("https://chat.openai.com/");

                // Wait for the page to load and locate the input field
                var inputField = WaitForElement(By.CssSelector("form textarea"), TimeSpan.FromSeconds(10));
                if (inputField == null)
                {
                    throw new Exception("Failed to locate the input field on the ChatGPT page.");
                }

                // Enter the query into the input field
                inputField.SendKeys(query);

                // Locate and click the submit button
                var submitButton = WaitForElement(By.CssSelector("form button"), TimeSpan.FromSeconds(5));
                if (submitButton == null)
                {
                    throw new Exception("Failed to locate the submit button on the ChatGPT page.");
                }

                submitButton.Click();

                // Wait for the response to appear
                var responseElement = WaitForElement(By.CssSelector(".response-class"), TimeSpan.FromSeconds(15)); // Replace ".response-class" with the actual CSS selector for the response
                if (responseElement == null)
                {
                    throw new Exception("Failed to retrieve the response from ChatGPT.");
                }

                // Extract and return the response text
                return responseElement.Text;
            }
            catch (WebDriverException ex)
            {
                throw new Exception("An error occurred while interacting with the ChatGPT web interface.", ex);
            }
        }

        private IWebElement WaitForElement(By by, TimeSpan timeout)
        {
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, timeout);
            try
            {
                return wait.Until(driver => driver.FindElement(by));
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}
