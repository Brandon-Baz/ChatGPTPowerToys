using System;
using System.Globalization;
using System.Web;
using Wox.Plugin.Common;

namespace ChatGPT.StandaloneApp.Services
{
    public class ChatGPTService
    {
        public void OpenChatGPT(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));
            }

            try
            {
                // Construct the URL with the PTquery parameter
                string encodedQuery = HttpUtility.UrlEncode(query);
                string url = $"https://chat.openai.com/?PTquery={encodedQuery}";

                // Open the browser with the constructed URL
                if (!Helper.OpenCommandInShell(BrowserInfo.Path, BrowserInfo.ArgumentsPattern, url))
                {
                    throw new Exception("Failed to open the browser with the specified URL.");
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow
                Console.Error.WriteLine($"Error launching browser: {ex.Message}");
                throw;
            }
        }
    }
}