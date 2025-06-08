// Required namespaces for collections, TMPro, and Unity UI
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class handles chat message logic and displays it in the UI
public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
    }
    // Max number of messages before old ones start getting removed
    [SerializeField] int maxMessages = 30;

    // List that holds all active chat messages in the system
    [SerializeField] List<Message> messageList = new List<Message>();

    // The parent UI object (usually a Vertical Layout Group) where messages are shown
    [SerializeField] GameObject chatPanel;

    // Prefab for each message object (should include a TextMeshProUGUI)
    [SerializeField] GameObject textObject;

    // ScrollRect component used to allow scrolling and auto-scroll to latest message
    [SerializeField] ScrollRect scrollRect;

    // The actual input field where the user types messages
    [SerializeField] TMP_InputField inputField;

    // Reference to the visible text inside the input field
    [SerializeField] TextMeshProUGUI inputFieldText;


    // Checks for Enter key and sends the message if input isn't empty
    public void RecieveMessageFromInputField(SpeakerProfile speaker)
    {
        if(speaker == null)
        {
            Debug.LogWarning("Speaker Profile is null please check the asset!");
            return;
        }
        // Make sure the input field and its text aren't null or empty
        if (inputField != null && !string.IsNullOrWhiteSpace(inputField.text))
        {
            // If the player hits Return/Enter
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                // Send the message to the chat
                SendMessageFromSpeaker(inputField.text, speaker);

                // Clear both the visual and internal input field texts
                inputFieldText.text = "";
                inputField.text = "";

                // Re-focus the input field so the player can type again right away
                inputField.ActivateInputField();
            }
        }
    }

    // Handles creating and displaying a new chat message
    public void SendMessageFromSpeaker(string text, SpeakerProfile speaker)
    {
        if (messageList.Count > maxMessages)
        {
            Destroy(messageList[0].TextMeshProComponent.gameObject);
            messageList.RemoveAt(0);
        }

        // Create new message and UI element
        Message newMessage = new Message
        {
            Text = $"{speaker.speakerName}: {text}", // 🟢 Unformatted for storage
            Speaker = speaker
        };

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
        newMessage.TextMeshProComponent = textComponent;

        // Apply formatted message for display
        string formattedText = FormatMessageWithSpeaker(speaker, text);
        textComponent.text = formattedText;

        messageList.Add(newMessage);

        AutoScroll();
    }

    // Ensures the ScrollRect always shows the latest message
    private void AutoScroll()
    {
        // Force layout to update so scrolling is calculated correctly
        Canvas.ForceUpdateCanvases();

        // Set scroll to bottom (0 = bottom, 1 = top)
        scrollRect.verticalNormalizedPosition = 0f;

        // Force layout update again to finalize scroll position
        Canvas.ForceUpdateCanvases();
    }

    private string FormatMessageWithSpeaker(SpeakerProfile speaker, string messageText)
    {
        string coloredName = $"<color=#{ColorUtility.ToHtmlStringRGB(speaker.nameColor)}>{speaker.speakerName}</color>";
        string coloredText = $"<color=#{ColorUtility.ToHtmlStringRGB(speaker.textColor)}>{messageText}</color>";
        return $"{coloredName}: {coloredText}";
    }
}
// Serializable class to store each message and its associated TMP UI element
[System.Serializable]
public class Message
{
    // The actual text content of the message
    public string Text;

    // The UI component used to display the message
    public TextMeshProUGUI TextMeshProComponent;
    public SpeakerProfile Speaker;

}
