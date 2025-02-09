/**
 * ChatGPTIntegration.js
 * This module handles communication with the ChatGPT API.
 */

/**
 * Sends a query to the ChatGPT API and returns the response.
 * @param {string} prompt - The user's query to send to ChatGPT.
 * @returns {Promise<string>} - The response from ChatGPT or an error message.
 */
export async function sendQueryToChatGPT(prompt) {
  const apiUrl = "https://api.openai.com/v1/completions";
  const apiKey = process.env.CHATGPT_API_KEY; // Ensure the API key is set in the environment variables.

  if (!apiKey) {
    throw new Error("ChatGPT API key is missing. Please set the CHATGPT_API_KEY environment variable.");
  }

  const requestBody = {
    model: "text-davinci-003", // Specify the model to use (adjust as needed).
    prompt: prompt,
    max_tokens: 150, // Limit the response length.
    temperature: 0.7, // Adjust the creativity of the response.
  };

  try {
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${apiKey}`,
      },
      body: JSON.stringify(requestBody),
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(`ChatGPT API error: ${errorData.error.message}`);
    }

    const responseData = await response.json();
    return responseData.choices[0].text.trim(); // Return the generated text.
  } catch (error) {
    console.error("Error communicating with ChatGPT API:", error);
    throw new Error("Failed to fetch response from ChatGPT. Please try again.");
  }
}
