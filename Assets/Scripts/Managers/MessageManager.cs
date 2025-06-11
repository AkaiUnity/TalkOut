// Required namespaces for collections, TMPro, and Unity UI
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// This class handles chat message logic and displays it in the UI
public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; }

    [SerializeField] AIChatManager aiChatManager;

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
    public List<Message> GetMessageList() { return messageList; }
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

    public string DefaultFileName = "/chatLogs.json";

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

    public void SendMessageFromSpeaker(string rawText, SpeakerProfile speaker)
    {
        if (messageList.Count >= maxMessages)
        {
            messageList.RemoveAt(0);
        }

        Message newMessage = new Message
        {
            RawText = rawText,
            Speaker = speaker
        };

        messageList.Add(newMessage);

        DisplayMessagesFromList();
        AutoScroll();
        SaveMessagesToJson(Application.persistentDataPath + DefaultFileName);

        // 🚀 If the speaker is YOU (player), trigger the AI response
        if (speaker.speakerName == "YOU")
        {
            if (aiChatManager != null)
            {
                // Decide which NPC speaker should respond
                SpeakerProfile npcSpeaker = aiChatManager.npcSpeaker;
                // Example: maybe always use a specific NPC, or pick based on last message, etc.
                // For now, if you have a default NPC, call:
                aiChatManager.GetAIResponse(npcSpeaker);
            }
            else
            {
                Debug.LogWarning("🧐 No AIChatManager found in the scene!");
            }
        }
    }
    // New method to build UI from the message list
    private void DisplayMessagesFromList()
    {
        foreach (Transform child in chatPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Message msg in messageList)
        {
            GameObject newTextObj = Instantiate(textObject, chatPanel.transform);
            TextMeshProUGUI textComponent = newTextObj.GetComponent<TextMeshProUGUI>();

            string formatted = FormatMessageWithSpeaker(msg.Speaker, msg.RawText);
            textComponent.text = formatted;

            msg.TextMeshProComponent = textComponent;
        }
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

        // If the speaker is not YOU, assume there’s narrative text to italicize
        if (speaker.speakerName != "YOU")
        {
            // Try to split the message into narrative and spoken parts
            // We’ll treat everything outside quotes as narrative, and inside quotes as spoken
            string formatted = "";
            int startQuote = messageText.IndexOf("\"");
            int endQuote = messageText.LastIndexOf("\"");

            if (startQuote != -1 && endQuote != -1 && endQuote > startQuote)
            {
                // Narrative before the quote
                string narrative = messageText.Substring(0, startQuote).Trim();
                // Spoken text inside quotes
                string spoken = messageText.Substring(startQuote, endQuote - startQuote + 1).Trim();
                // Narrative after the quote
                string narrativeAfter = messageText.Substring(endQuote + 1).Trim();

                // Italicize the narrative parts
                string coloredNarrative = $"<i><color=#{ColorUtility.ToHtmlStringRGB(speaker.textColor)}>{narrative}</color></i>";
                string coloredSpoken = $"<color=#{ColorUtility.ToHtmlStringRGB(speaker.textColor)}>{spoken}</color>";
                string coloredNarrativeAfter = $"<i><color=#{ColorUtility.ToHtmlStringRGB(speaker.textColor)}>{narrativeAfter}</color></i>";

                // Combine
                formatted = $"{coloredName}: {coloredNarrative} {coloredSpoken} {coloredNarrativeAfter}";
            }
            else
            {
                // No quotes? Just italicize the whole thing
                string coloredNarrative = $"<i><color=#{ColorUtility.ToHtmlStringRGB(speaker.textColor)}>{messageText}</color></i>";
                formatted = $"{coloredName}: {coloredNarrative}";
            }

            return formatted.Trim();
        }
        else
        {
            // Player text, no italics
            string coloredText = $"<color=#{ColorUtility.ToHtmlStringRGB(speaker.textColor)}>{messageText}</color>";
            return $"{coloredName}: {coloredText}";
        }
    }

    public void SaveMessagesToJson(string filePath)
    {
        if (messageList == null || messageList.Count == 0)
        {
            Debug.LogWarning("⚠️ No messages to save. The message list is null or empty.");
            return;
        }
        MessageSaveData saveData = new MessageSaveData();
        saveData.messages = new List<SimpleMessage>();
        
        foreach (Message msg in messageList)
        {
            saveData.messages.Add(new SimpleMessage
            {
                speakerName = msg.Speaker.speakerName,
                text = msg.RawText
            });
        }

        string json = JsonUtility.ToJson(saveData, true); // true = pretty print for easier reading

        File.WriteAllText(filePath, json);
        Debug.Log($"Saved messages to: {filePath}");
    }
}
// Serializable class to store each message and its associated TMP UI element

[System.Serializable]
public class Message
{
    public string RawText; // Only the message, no speaker or formatting
    public SpeakerProfile Speaker;
    public string Text => $"{Speaker.speakerName}: {RawText}"; // Derived property

    public TextMeshProUGUI TextMeshProComponent;
}
