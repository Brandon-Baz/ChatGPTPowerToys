document.addEventListener("DOMContentLoaded", () => {
  const floatingBubble = document.getElementById("floating-bubble");
  const modalPanel = document.getElementById("modal-panel");
  const closeModalButton = document.getElementById("close-modal");
  const sendQueryButton = document.getElementById("send-query");
  const chatInput = document.getElementById("chat-input");
  const chatResponse = document.getElementById("chat-response");

  // Function to toggle the visibility of the modal panel
  const toggleModal = () => {
    if (modalPanel.classList.contains("hidden")) {
      modalPanel.classList.remove("hidden");
    } else {
      modalPanel.classList.add("hidden");
    }
  };

  // Event listener for the floating bubble click
  floatingBubble.addEventListener("click", () => {
    toggleModal();
  });

  // Event listener for the close button in the modal
  closeModalButton.addEventListener("click", () => {
    modalPanel.classList.add("hidden");
  });

  // Function to send a query to the backend
  const sendQuery = async () => {
    const prompt = chatInput.value.trim();
    if (!prompt) {
      chatResponse.textContent = "Please enter a query.";
      return;
    }

    chatResponse.textContent = "Loading...";

    try {
      const response = await window.__TAURI__.invoke("ask_chatgpt", { prompt });
      chatResponse.textContent = response;
    } catch (error) {
      chatResponse.textContent = `Error: ${error}`;
    }
  };

  // Event listener for the send button in the modal
  sendQueryButton.addEventListener("click", () => {
    sendQuery();
  });

  // Allow pressing "Enter" in the textarea to send the query
  chatInput.addEventListener("keydown", (event) => {
    if (event.key === "Enter" && !event.shiftKey) {
      event.preventDefault();
      sendQuery();
    }
  });

  // Ensure the floating bubble follows the active window
  const updateBubblePosition = () => {
    const activeWindow = document.activeElement;
    if (activeWindow) {
      const rect = activeWindow.getBoundingClientRect();
      floatingBubble.style.top = `${rect.top + window.scrollY + 10}px`;
      floatingBubble.style.left = `${rect.left + window.scrollX + 10}px`;
    }
  };

  // Periodically update the bubble's position
  setInterval(updateBubblePosition, 500);
});
