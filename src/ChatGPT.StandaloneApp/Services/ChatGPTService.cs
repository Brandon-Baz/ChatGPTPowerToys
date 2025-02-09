using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatGPT.StandaloneApp.Services
{
    public class ChatGPTService
    {
        private readonly HttpClient _httpClient;

        public ChatGPTService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
        }

        public async Task<string> GetResponseAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));
            }

            try
            {
                // Prepare the request payload
                var requestPayload = new
                {
                    model = "text-davinci-003",
                    prompt = query,
                    max_tokens = 150
                };

                var jsonPayload = JsonSerializer.Serialize(requestPayload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Add authorization header (replace YOUR_API_KEY with the actual API key)
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "YOUR_API_KEY");

                // Send the POST request
                var response = await _httpClient.PostAsync("completions", content);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Parse the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // Extract the generated text from the response
                if (responseJson.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    return choices[0].GetProperty("text").GetString();
                }

                throw new Exception("Unexpected response format from ChatGPT API.");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error communicating with ChatGPT API. Please check your network connection.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the ChatGPT response.", ex);
            }
        }
    }
}
