#![cfg_attr(
    all(not(debug_assertions), target_os = "windows"),
    windows_subsystem = "windows"
)]

use tauri::{CustomMenuItem, Manager, SystemTray, SystemTrayEvent, WindowBuilder, WindowUrl};
use serde::{Deserialize, Serialize};
use reqwest::Client;
use std::sync::Arc;
use tokio::sync::Mutex;

#[derive(Serialize, Deserialize)]
struct ChatGPTRequest {
    prompt: String,
}

#[derive(Serialize, Deserialize)]
struct ChatGPTResponse {
    response: String,
}

#[tauri::command]
async fn ask_chatgpt(prompt: String, state: tauri::State<'_, AppState>) -> Result<String, String> {
    let client = state.client.lock().await;
    let api_key = &state.api_key;

    let request_body = ChatGPTRequest { prompt };

    let response = client
        .post("https://api.openai.com/v1/completions")
        .header("Authorization", format!("Bearer {}", api_key))
        .json(&request_body)
        .send()
        .await
        .map_err(|e| format!("Failed to send request: {}", e))?;

    if response.status().is_success() {
        let response_body: ChatGPTResponse = response
            .json()
            .await
            .map_err(|e| format!("Failed to parse response: {}", e))?;
        Ok(response_body.response)
    } else {
        Err(format!(
            "API returned an error: {}",
            response.status().as_u16()
        ))
    }
}

struct AppState {
    client: Arc<Mutex<Client>>,
    api_key: String,
}

fn main() {
    let api_key = std::env::var("CHATGPT_API_KEY").expect("CHATGPT_API_KEY must be set");

    let app_state = AppState {
        client: Arc::new(Mutex::new(Client::new())),
        api_key,
    };

    tauri::Builder::default()
        .manage(app_state)
        .setup(|app| {
            let main_window = app.get_window("main").unwrap();
            main_window.set_title("ChatGPT Desktop Application").unwrap();

            // Create the floating bubble window
            let _bubble_window = WindowBuilder::new(
                app,
                "floating_bubble",
                WindowUrl::App("index.html".into()),
            )
            .title("Floating Bubble")
            .inner_size(100.0, 100.0)
            .resizable(false)
            .decorations(false)
            .transparent(true)
            .always_on_top(true)
            .build()
            .unwrap();

            Ok(())
        })
        .invoke_handler(tauri::generate_handler![ask_chatgpt])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
