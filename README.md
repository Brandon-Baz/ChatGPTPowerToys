# ChatGPT for PowerToys Run
<p align="center">
    <img alt="logo" src="./demo.gif">
</p>

### ChatGPT Desktop Application (Standalone Tauri App)
The ChatGPT Desktop Application is a standalone Tauri-based app that integrates ChatGPT functionalities with a persistent floating bubble UI for enhanced multitasking.

#### Features
- **Floating Bubble UI**: A small, always-on-top bubble that follows the active window. Clicking the bubble expands it into a modal panel for interacting with ChatGPT.
- **ChatGPT Integration**: Seamlessly send queries to ChatGPT and view responses directly within the app.
- **Non-intrusive Design**: The modal panel remains accessible without disrupting your workflow.

#### How to Build and Run
1. Clone this repository:
   ```bash
   git clone https://github.com/Brandon-Baz/ChatGPTPowerToys.git
   ```
2. Navigate to the `tauri-app` directory:
   ```bash
   cd ChatGPTPowerToys/tauri-app
   ```
3. Install dependencies:
   ```bash
   npm install
   ```
4. Build and run the application:
   ```bash
   npm run tauri dev
   ```
5. To build a production-ready binary:
   ```bash
   npm run tauri build
   ```

#### Interacting with the Floating Bubble
- **Bubble Behavior**: The floating bubble stays on top of other windows and adjusts its position to follow the active window.
- **Expanding the Bubble**: Click the bubble to open a modal panel where you can type queries and view ChatGPT responses.
- **Keyboard Shortcuts**: Use the "Enter" key to send queries quickly.

### How to install
If you simply want to install the plugin to get up and running quickly, I suggest downloading the [precompiled binaries](https://github.com/ferraridavide/ChatGPTPowerToys/releases) from the Release section and [installing the Tampermonkey user script](#using-tampermonkey-recommended)
The installation process is as follows:
1. Locate your PowerToys installation (eg. `C:\Program Files\PowerToys`)
1. Navigate to `\modules\launcher\Plugins`
1. Unpack the downloaded binaries

### Compiling plugin
1. Clone the PowerToys repository to your local disk using the command `git clone https://github.com/microsoft/PowerToys.git`
1. Navigate to the PowerToys directory using `cd PowerToys`
1. Initialize and update submodules with the command `git submodule update --init --recursive`
1. Fork the ChatGPTPowerToys repository on GitHub
1. Clone the fork of ChatGPTPowerToys into the local PowerToys repository by running `git clone https://github.com/ferraridavide/ChatGPTPowerToys.git` in the `PowerToys\src\modules\launcher\Plugins` directory
1. In Visual Studio, add the local clone of ChatGPTPowerToys as an existing project to the PowerToys's Plugins folder (`modules\launcher\Plugins`)
1. Compile

Unfortunately, ChatGPT does not provide a query string parameter for passing prompts. Therefore, we must utilize a browser extension to inject the prompt.

### Using Tampermonkey (recommended)
Tampermonkey is a popular browser extension that allows to inject custom user scripts in webpages, visit [tampermonkey.net](https://www.tampermonkey.net/) to install it.

Install this script
```javascript
// ==UserScript==
// @name         PowerToys Run ChatGPT Helper
// @version      0.2
// @description  https://github.com/ferraridavide/ChatGPTPowerToys
// @author       Davide Ferrari
// @match        https://chat.openai.com/?PTquery=*
// @icon         https://raw.githubusercontent.com/ferraridavide/ChatGPTPowerToys/master/src/PowerToys.ChatGPT.BrowserExtension/icons/icon128.png
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    console.log("PowerToys Run ChatGPT Helper script loaded");

    const searchParams = new URLSearchParams(window.location.search);
    const prompt = searchParams.get("PTquery");
    if (prompt) {
        const textArea = document.querySelector("form textarea");
        const submitButton = document.querySelector("form button");

        if (!textArea || !submitButton) {
            console.error("Cannot find required elements");
        }

        textArea.value = prompt;
        setTimeout(() => {
            textArea.value = prompt;
            submitButton.disabled = false;
            submitButton.click();
        }, 500);
    }
})();
```


<details>
  <summary>AIPRM browser extension compatability</summary>
  Thanks to @babico for providing this modified script!

 ```javascript
// ==UserScript==
// @name         PowerToys Run ChatGPT Helper
// @version      0.2
// @description  https://github.com/ferraridavide/ChatGPTPowerToys
// @author       Davide Ferrari
// @match        https://chat.openai.com/?PTquery=*
// @icon         https://raw.githubusercontent.com/ferraridavide/ChatGPTPowerToys/master/src/PowerToys.ChatGPT.BrowserExtension/icons/icon128.png
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    console.log("PowerToys Run ChatGPT Helper script loaded");

    const searchParams = new URLSearchParams(window.location.search);
    const prompt = searchParams.get("PTquery");

    if (prompt) {
        setTimeout(() => {
            const textArea = document.querySelector("form textarea");
            const submitButton = document.querySelectorAll("form button")[1]; // AIPRM add another button so change with the second button

            if (!textArea || !submitButton) {
                console.error("Cannot find required elements");
            }

            textArea.value = prompt;
            submitButton.disabled = false;
            submitButton.click();
        }, 500);
    }
})();
```
</details>

### Using the custom browser extension
1. Open your preferred browser and navigate to the settings or preferences menu.
1. Locate the option for "extensions" or "add-ons" and select it.
1. Enable developer mode in the extensions settings page.
1. In the extensions menu, look for a button or option labeled "Load Unpacked" or "Add Unpacked Extension."
1. Select the folder containing the extension and click "Open."